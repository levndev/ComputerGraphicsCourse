#version 400 core
#define MAX_SPHERES 16
#define MAX_LIGHTS 16
#define RAY_DISPLACEMENT 0.001
#define PI 3.1415926535
#define HALF_PI (PI / 2.0)
#define N_IN 0.99
#define N_OUT 1.0
#define MATERIAL_TYPE_OPAQUE_DIFFUSE 0
#define MATERIAL_TYPE_REFLECTIVE 1
#define MATERIAL_TYPE_REFRACTIVE 2
#define MATERIAL_TYPE_REFLECTIVE_AND_REFRACTIVE 3
#define AIR_REFRACTION_INDEX 1
#define TREE_SIZE 256
//structs
struct Material {
    vec3 Ambient;
    vec3 Diffuse;
    vec3 Specular;
    float Shininess;
    float Reflection;
    float Refraction;
    float RefractionIndex;
};

struct Sphere {
    //vec3 Color;
    vec3 Position;
    float Radius;
    Material Material;
    vec3 Color;
    bool IgnoreLight;
};

struct CameraData {
    vec3 Position;
    vec2 ViewportSize;
    float FOV;
    vec3 Direction;
    vec3 Up;
    mat3 ViewToWorld;
};

struct Ray {
    vec3 Origin;
    vec3 Direction;
};

struct Hit {
    vec3 Position;
    vec3 Normal;
    vec3 Color;
    float Distance;
    Material Material;
};

struct Light {
    vec3 Position;
    vec3 Diffuse;
    vec3 Specular;
};

//in/outs and uniforms
in vec2 TexCoords;
out vec4 FragColor;

uniform CameraData Camera;

uniform Sphere Spheres[MAX_SPHERES];
uniform Light Lights[MAX_LIGHTS];
uniform int SphereCount;
uniform int LightCount;
uniform int Depth;
uniform float Time;
uniform vec3 AmbientLight;
uniform bool Debug;
uniform vec3 BackgroundColor;

vec3 ColorTree[TREE_SIZE];
Ray RayTree[TREE_SIZE];

//functions
int ipow(int base, int power) {
    int num = base;
    for (int i = 0; i < power - 1; i++) {
        num *= base;
    }
    return num;
}
float FresnelSchlick(float nIn, float nOut, vec3 direction, vec3 normal)
{
    float R0 = ((nOut - nIn) * (nOut - nIn)) / ((nOut + nIn) * (nOut + nIn));
    float fresnel = R0 + (1.0 - R0) * pow((1.0 - abs(dot(direction, normal))), 5.0);
    return fresnel;
}
//https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
bool RaySphereIntersection(Ray ray, Sphere sphere, out Hit oHit) {
    Hit hit;
    vec3 L = sphere.Position - ray.Origin;
    float radius2 = sphere.Radius * sphere.Radius;
    float tca = dot(L, ray.Direction); 
    float d2 = dot(L, L) - tca * tca; 
    if (d2 > radius2) return false; 
    float thc = sqrt(radius2 - d2); 
    float t0 = tca - thc; 
    float t1 = tca + thc; 
    if (t0 > t1) {
        float temp = t0;
        t0 = t1;
        t1 = temp;
    } 
    if (t0 < 0) { 
        t0 = t1;  //if t0 is negative, let's use t1 instead 
        if (t0 < 0) return false;  //both t0 and t1 are negative 
    } 
    float t = t0; 
    hit.Distance = t;
    //WTFFFFF
    //hit.Position = ray.Origin = ray.Direction * t;
    hit.Position = ray.Origin + ray.Direction * t;
    hit.Normal = normalize(hit.Position - sphere.Position);
    oHit = hit;
    return true;
}

bool LightVisible(Ray ray, Light light) {
    for (int i = 0; i < SphereCount; i++) {
        Hit hit;
        if (RaySphereIntersection(ray, Spheres[i], hit)) {
            if (!Spheres[i].IgnoreLight)
                return false;
            if (Spheres[i].Material.Refraction == 0) {
                return false;
            }
        }
    }
    return true;
}

vec3 GetLighting(Hit hit, Light light) {
    vec3 ambient = AmbientLight * hit.Material.Ambient;
    Ray shadowRay = Ray(hit.Position + hit.Normal * RAY_DISPLACEMENT, normalize(light.Position - hit.Position));
    
    //diffuse 
    vec3 norm = hit.Normal;
    vec3 lightDir = normalize(light.Position - hit.Position);
    float diff = dot(norm, lightDir);
    if (diff < 0 || !LightVisible(shadowRay, light)){
        return ambient;
    }
    vec3 diffuse = light.Diffuse * (max(diff, 0.0) * hit.Material.Diffuse);
    //specular
    vec3 viewDir = normalize(Camera.Position - hit.Position);
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(norm, halfwayDir), 0.0), hit.Material.Shininess);
    vec3 specular = light.Specular * (spec * hit.Material.Specular);
    vec3 result = ambient + diffuse + specular;
    return result;
}

bool CastRay(Ray ray, out Hit oHit) {
    #define MAX_DISTANCE 10000.0
    float minDistance = MAX_DISTANCE;
    for (int i = 0; i < SphereCount; i++) {
        Hit hit;
        if (RaySphereIntersection(ray, Spheres[i], hit)) {
            if (hit.Distance < minDistance) {
                minDistance = hit.Distance;
                hit.Material = Spheres[i].Material;
                hit.Color = vec3(0.0, 0.0, 0.0);
                if (Spheres[i].IgnoreLight) {
                    hit.Color = Spheres[i].Color;
                }
                else {
                    for (int j = 0; j < LightCount; j++) {
                        hit.Color += GetLighting(hit, Lights[j]);
                    }
                }
                oHit = hit;
            }
        }
    }
    return minDistance != MAX_DISTANCE;
}
vec3 TraceRay(Ray ray) {
    vec3 color = BackgroundColor;
    Hit hit;
    RayTree[0] = ray;
    for (int i = 0; i < Depth; i++) {
        int rowSize = ipow(2, i);
        for (int j = 0; j < rowSize; j++) {
            if (CastRay(RayTree[i + j], hit)) {
                if (hit.Material.Reflection == 0 && hit.Material.Refraction == 0) {
                    // Phong
                    ColorTree[i] = hit.Color;
                }
                if (hit.Material.Refraction > 0) {
                    // refract
                    float cosTheta = min(dot(-ray.Direction, hit.Normal), 1);
                    float sinTheta = sqrt(1 - cosTheta * cosTheta);
                    float refractionRatio;
                    if (dot(ray.Direction, hit.Normal) < 0) {
                        refractionRatio = 1.0 / hit.Material.RefractionIndex;
                    } else {
                        refractionRatio = hit.Material.RefractionIndex;
                        hit.Normal *= -1;
                    }
                    vec3 refraction;
                    if (sinTheta * refractionRatio > 1) {
                        refraction = reflect(ray.Direction, hit.Normal);
                    } else {
                        refraction = refract(ray.Direction, hit.Normal, refractionRatio);
                    }
                    ray = Ray(hit.Position + refraction * RAY_DISPLACEMENT, refraction);
                    continue;
                }
                if (hit.Material.Reflection > 0) {
                    // reflect
                    vec3 reflection = reflect(ray.Direction, hit.Normal);
                    ray = Ray(hit.Position + reflection * RAY_DISPLACEMENT, reflection);
                    continue;
                }
            }
            else {
                return BackgroundColor;
            }
        }
    }
    return color;
}

Ray GetRay(vec2 texcoords, CameraData camera)
{
    vec2 texDiff = 0.5 * vec2(2.0 * texcoords.x - 1.0, 2.0 * texcoords.y - 1.0);
    vec2 angleDiff = texDiff * vec2(camera.ViewportSize.x / camera.ViewportSize.y, 1.0) * tan(camera.FOV * 0.5);
    vec3 rayDirection = normalize(vec3(angleDiff, 1.0));
    return Ray(camera.Position, rayDirection * camera.ViewToWorld);
}

void main()
{
    Ray ray = GetRay(TexCoords, Camera);
    FragColor = vec4(TraceRay(ray), 1.0);
}   
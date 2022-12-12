#version 400 core
#define MAX_SPHERES 16
#define PI 3.1415926535
#define HALF_PI (PI / 2.0)
#define FAR_DISTANCE 1000000.0
#define N_IN 0.99
#define N_OUT 1.0
struct CameraData {
    vec3 Position;
    vec2 ViewportSize;
    float FOV;
    vec3 Direction;
    vec3 Up;
    mat3 ViewToWorld;
};
struct Material
{
    vec3 Emittance;
    vec3 Reflectance;
    float Roughness;
    float Opacity;
};
struct Sphere
{
    Material Material;
    vec3 Position;
    float Radius;
};
struct Ray {
    vec3 Origin;
    vec3 Direction;
};
struct Hit {
    vec3 Position;
    vec3 Normal;
    float Distance;
    Material Material;
};

out vec4 FragColor;
in vec2 TexCoords;
uniform int Samples;
uniform vec3 BackgroundColor;
uniform CameraData Camera;
uniform int Depth;
uniform float Time;
uniform vec3 AmbientLight;
uniform bool Debug;
uniform Sphere Spheres[MAX_SPHERES];
uniform int SphereCount;
float RandomNoise(vec2 co)
{
    co *= fract(Time * 12.343);
    return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

vec3 RandomHemispherePoint(vec2 rand)
{
    float cosTheta = sqrt(1.0 - rand.x);
    float sinTheta = sqrt(rand.x);
    float phi = 2.0 * PI * rand.y;
    return vec3(
        cos(phi) * sinTheta,
        sin(phi) * sinTheta,
        cosTheta
    );
}

vec3 NormalOrientedHemispherePoint(vec2 rand, vec3 n)
{
    vec3 v = RandomHemispherePoint(rand);
    return dot(v, n) < 0.0 ? -v : v;
}

float FresnelSchlick(float nIn, float nOut, vec3 direction, vec3 normal)
{
    float R0 = ((nOut - nIn) * (nOut - nIn)) / ((nOut + nIn) * (nOut + nIn));
    float fresnel = R0 + (1.0 - R0) * pow((1.0 - abs(dot(direction, normal))), 5.0);
    return fresnel;
}

vec3 IdealRefract(vec3 direction, vec3 normal, float nIn, float nOut)
{
    bool fromOutside = dot(normal, direction) < 0.0;
    float ratio = fromOutside ? nOut / nIn : nIn / nOut;

    vec3 refraction, reflection;

    refraction = fromOutside ? refract(direction, normal, ratio) : -refract(-direction, normal, ratio);
    reflection = reflect(direction, normal);

    return refraction == vec3(0.0) ? reflection : refraction;
}

bool IsRefracted(float rand, vec3 direction, vec3 normal, float opacity, float nIn, float nOut)
{
    float fresnel = FresnelSchlick(nIn, nOut, direction, normal);
    return opacity > rand && fresnel < rand;
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
bool CastRay(Ray ray, out Hit oHit) {
    #define MAX_DISTANCE 10000.0
    float minDistance = MAX_DISTANCE;
    for (int i = 0; i < SphereCount; i++) {
        Hit hit;
        if (RaySphereIntersection(ray, Spheres[i], hit)) {
            if (hit.Distance < minDistance) {
                minDistance = hit.Distance;
                hit.Material = Spheres[i].Material;
                oHit = hit;
            }
        }
    }
    return minDistance != MAX_DISTANCE;
}
vec3 TracePath(Ray ray, float seed) {
    vec3 L = vec3(0.0);
    vec3 F = vec3(1.0);
    for (int i = 0; i < Depth; i++)
    {
        Hit hit;
        if (CastRay(ray, hit))
        {
            vec3 newRayOrigin = hit.Position;
            vec3 normal = hit.Normal;
            vec2 rand = vec2(RandomNoise(seed * TexCoords.xy), seed * RandomNoise(TexCoords.yx));
            vec3 hemisphereDistributedDirection = NormalOrientedHemispherePoint(rand, normal);
            vec3 randomVec = vec3(
                RandomNoise(sin(seed * TexCoords.xy)),
                RandomNoise(cos(seed * TexCoords.xy)),
                RandomNoise(sin(seed * TexCoords.yx))
            );
            randomVec = normalize(2.0 * randomVec - 1.0);

            vec3 tangent = cross(randomVec, normal);
            vec3 bitangent = cross(normal, tangent);
            mat3 transform = mat3(tangent, bitangent, normal);

            vec3 newRayDirection = transform * hemisphereDistributedDirection;
            
            float refractRand = RandomNoise(cos(seed * TexCoords.yx));
            bool refracted = IsRefracted(refractRand, ray.Direction, normal, hit.Material.Opacity, N_IN, N_OUT);
            if (refracted)
            {
                vec3 idealRefraction = IdealRefract(ray.Direction, normal, N_IN, N_OUT);
                newRayDirection = normalize(mix(-newRayDirection, idealRefraction, hit.Material.Roughness));
                newRayOrigin += normal * (dot(newRayDirection, normal) < 0.0 ? -0.8 : 0.8);
            }
            else
            {
                vec3 idealReflection = reflect(ray.Direction, normal);
                newRayDirection = normalize(mix(newRayDirection, idealReflection, hit.Material.Roughness));
                newRayOrigin += normal * 0.8;
            }
            ray = Ray(newRayOrigin, newRayDirection);
            L += F * hit.Material.Emittance;
            F *= hit.Material.Reflectance;
        }
        else
        {
            F = vec3(0.0);
        }
    }
    return L;
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
    vec3 color = vec3(0);
    for (int i = 0; i < Samples; i++) {
        float seed = sin(float(i) * Time);
        color += TracePath(GetRay(TexCoords, Camera), seed);
    }
    FragColor = vec4(color / float(Samples), 1.0);
}   
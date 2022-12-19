#version 400 core
#define MAX_SPHERES 16
#define MAX_BOXES 16
#define PI 3.1415926535
#define HALF_PI (PI / 2.0)
#define FAR_DISTANCE 1000000.0
#define K_EPSILON 1e-8
#define MAX_MESH_SIZE 800
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
    float Smoothness;
    float Transparency;
    float RefractiveIndex;
};
struct Box
{
    Material Material;
    vec3 HalfSize;
    mat3 Rotation;
    vec3 Position;
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
struct PathTracingSettings {
    int Depth;
    float EnvironmentRefractiveIndex;
    bool UseSkyboxForLighting;
};

out vec4 FragColor;
in vec2 TexCoords;
uniform int Samples;
uniform CameraData Camera;
uniform float Time;
uniform bool Debug;
uniform Sphere Spheres[MAX_SPHERES];
uniform Box Boxes[MAX_BOXES];
uniform vec3 MeshVertices[MAX_MESH_SIZE];
uniform int MeshSize;
uniform vec3 MeshPosition;
uniform Material MeshMaterial;
uniform int SphereCount;
uniform int BoxCount;
uniform samplerCube Skybox;
uniform float SkyboxColorMultiplier;
uniform PathTracingSettings Settings;
//https://stackoverflow.com/a/17479300
// A single iteration of Bob Jenkins' One-At-A-Time hashing algorithm.
uint hash( uint x ) {
    x += ( x << 10u );
    x ^= ( x >>  6u );
    x += ( x <<  3u );
    x ^= ( x >> 11u );
    x += ( x << 15u );
    return x;
}
uint hash( uvec2 v ) { return hash( v.x ^ hash(v.y) ); }
// Construct a float with half-open range [0:1] using low 23 bits.
// All zeroes yields 0.0, all ones yields the next smallest representable value below 1.0.
float floatConstruct( uint m ) {
    const uint ieeeMantissa = 0x007FFFFFu; // binary32 mantissa bitmask
    const uint ieeeOne      = 0x3F800000u; // 1.0 in IEEE binary32

    m &= ieeeMantissa;                     // Keep only mantissa bits (fractional part)
    m |= ieeeOne;                          // Add fractional part to 1.0

    float  f = uintBitsToFloat( m );       // Range [1:2]
    return f - 1.0;                        // Range [0:1]
}
// Pseudo-random value in half-open range [0:1].
float random( vec2  v ) { return floatConstruct(hash(floatBitsToUint(v))); }

float RandomNoise(vec2 co)
{
    co *= fract(Time * 12.343);
    return random(co);
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

bool RayBoxIntersection(Ray ray, Box box, out Hit oHit)
{
    Hit hit;
    vec3 rd = box.Rotation * ray.Direction;
    vec3 ro = box.Rotation * (ray.Origin - box.Position);

    vec3 m = vec3(1.0) / rd;

    vec3 s = vec3((rd.x < 0.0) ? 1.0 : -1.0,
        (rd.y < 0.0) ? 1.0 : -1.0,
        (rd.z < 0.0) ? 1.0 : -1.0);
    vec3 t1 = m * (-ro + s * box.HalfSize);
    vec3 t2 = m * (-ro - s * box.HalfSize);

    float tN = max(max(t1.x, t1.y), t1.z);
    float tF = min(min(t2.x, t2.y), t2.z);

    if (tN > tF || tF < 0.0) return false;

    mat3 txi = transpose(box.Rotation);

    if (t1.x > t1.y && t1.x > t1.z)
        hit.Normal = txi[0] * s.x;
    else if (t1.y > t1.z)
        hit.Normal = txi[1] * s.y;
    else
        hit.Normal = txi[2] * s.z;
    hit.Distance = tN;
    hit.Position = ray.Origin + ray.Direction * tN;
    oHit = hit;
    return true;
}
//https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/ray-triangle-intersection-geometric-solution
bool RayTriangleIntersection(Ray ray, vec3 v0, vec3 v1, vec3 v2, out Hit oHit) 
{ 
    Hit hit;
    // compute plane's normal
    vec3 v0v1 = v1 - v0; 
    vec3 v0v2 = v2 - v0; 
    // no need to normalize
    vec3 N = cross(v0v1, v0v2);  //N 
    float area2 = length(N); 
 
    // Step 1: finding P
 
    // check if ray and plane are parallel.
    float NdotRayDirection = dot(N, ray.Direction); 
    if (abs(NdotRayDirection) < K_EPSILON)  //almost 0 
        return false;  //they are parallel so they don't intersect ! 
 
    // compute d parameter using equation 2
    float d = -dot(N, v0); 
 
    // compute t (equation 3)
    float t = -(dot(N, ray.Origin) + d) / NdotRayDirection; 
 
    // check if the triangle is in behind the ray
    if (t < 0) return false;  //the triangle is behind 
 
    // compute the intersection point using equation 1
    vec3 P = ray.Origin + t * ray.Direction; 
 
    // Step 2: inside-outside test
    vec3 C;  //vector perpendicular to triangle's plane 
 
    // edge 0
    vec3 edge0 = v1 - v0; 
    vec3 vp0 = P - v0; 
    C = cross(edge0, vp0); 
    if (dot(N, C) < 0) return false;  //P is on the right side 
 
    // edge 1
    vec3 edge1 = v2 - v1; 
    vec3 vp1 = P - v1; 
    C = cross(edge1, vp1);
    if (dot(N, C) < 0)  return false;  //P is on the right side 
 
    // edge 2
    vec3 edge2 = v0 - v2; 
    vec3 vp2 = P - v2; 
    C = cross(edge2, vp2);
    if (dot(N, C) < 0) return false;  //P is on the right side; 
    hit.Distance = t;
    hit.Position = P;
    hit.Normal = N;
    oHit = hit;
    return true;  //this ray hits the triangle 
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
    hit.Position = ray.Origin + ray.Direction * t;
    hit.Normal = normalize(hit.Position - sphere.Position);
    oHit = hit;
    return true;
}

bool CastRay(Ray ray, out Hit oHit) {
    #define MAX_DISTANCE 10000.0
    float minDistance = MAX_DISTANCE;
    Hit hit;
    for (int i = 0; i < SphereCount; i++) {
        if (RaySphereIntersection(ray, Spheres[i], hit)) {
            if (hit.Distance < minDistance) {
                minDistance = hit.Distance;
                hit.Material = Spheres[i].Material;
                oHit = hit;
            }
        }
    }
    for (int i = 0; i < BoxCount; i++) {
        if (RayBoxIntersection(ray, Boxes[i], hit)) {
            if (hit.Distance < minDistance) {
                minDistance = hit.Distance;
                hit.Material = Boxes[i].Material;
                oHit = hit;
            }
        }
    }
    // for (int i = 0; i < MeshCount; i++) {
    //     if (RayMeshIntersection(ray, Meshes[i], hit)) {
    //         if (hit.Distance < minDistance) {
    //             minDistance = hit.Distance;
    //             hit.Material = Meshes[i].Material;
    //             oHit = hit;
    //         }
    //     }
    // }
    Material def = Material(vec3(0), vec3(1), 0, 0, 1);
    for (int i = 0; i < MAX_MESH_SIZE; i += 3) {
        if (i == MeshSize)
            break;
        if (RayTriangleIntersection(ray, MeshVertices[i] + MeshPosition, MeshVertices[i + 1] + MeshPosition, MeshVertices[i + 2] + MeshPosition, hit)) {
            if (hit.Distance < minDistance) {
                minDistance = hit.Distance;
                hit.Material = MeshMaterial;
                oHit = hit;
            }
        }
    }
    return minDistance != MAX_DISTANCE;
}

vec3 TracePath(Ray ray, Hit hit, float seed) {
    vec3 L = vec3(0.0);
    vec3 F = vec3(1.0);
    for (int i = 0; i < Settings.Depth; i++)
    {
        if (i == 0 || CastRay(ray, hit))
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
            bool refracted = IsRefracted(refractRand, ray.Direction, normal, hit.Material.Transparency, hit.Material.RefractiveIndex, Settings.EnvironmentRefractiveIndex);
            if (refracted)
            {
                vec3 idealRefraction = IdealRefract(ray.Direction, normal, hit.Material.RefractiveIndex, Settings.EnvironmentRefractiveIndex);
                newRayDirection = normalize(mix(-newRayDirection, idealRefraction, hit.Material.Smoothness));
                newRayOrigin += normal * (dot(newRayDirection, normal) < 0.0 ? -0.8 : 0.8);
            }
            else
            {
                vec3 idealReflection = reflect(ray.Direction, normal);
                newRayDirection = normalize(mix(newRayDirection, idealReflection, hit.Material.Smoothness));
                newRayOrigin += normal * 0.8;
            }
            ray = Ray(newRayOrigin, newRayDirection);
            L += F * hit.Material.Emittance;
            F *= hit.Material.Reflectance;
        }
        else
        {
            if (Settings.UseSkyboxForLighting)
                L += F * texture(Skybox, ray.Direction).xyz * SkyboxColorMultiplier;
            else
                F = vec3(0.0);
            break;
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
    Ray firstRay = GetRay(TexCoords, Camera);
    vec3 color = vec3(0);
    Hit firstHit;
    if (CastRay(firstRay, firstHit)) {
        for (int i = 0; i < Samples; i++) {
            float seed = sin(float(i) + Time);
            color += TracePath(firstRay, firstHit, seed);
        }
        FragColor = vec4(color / float(Samples), 1.0);
    }
    else {
        FragColor = vec4(texture(Skybox, firstRay.Direction).xyz * SkyboxColorMultiplier, 1.0);
    }
}   
#version 330 core
struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
    sampler2D diffuseMap;
}; 
struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
in vec3 FragPos;  
in vec3 Normal;  
in vec2 TexCoords;
uniform Light light;
uniform Material material;
uniform vec3 viewPos;
uniform bool useTexture;
out vec4 FragColor;

void main()
{
    //ambient
    vec3 ambient = light.ambient * material.ambient;
    if (useTexture) {
        ambient = light.ambient * vec3(texture(material.diffuseMap, TexCoords));
    }
    //diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * (diff * material.diffuse);
    if (useTexture) {
        diffuse = light.diffuse * (diff * vec3(texture(material.diffuseMap, TexCoords)));
    }
    //specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(norm, halfwayDir), 0.0), material.shininess);
    vec3 specular = light.specular * (spec * material.specular);
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}   


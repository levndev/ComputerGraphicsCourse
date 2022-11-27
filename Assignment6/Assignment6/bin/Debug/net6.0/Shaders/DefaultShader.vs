#version 330 core
layout (location = 0) in vec3 aVertex;
layout (location = 1) in vec3 aNormal;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

out vec3 FragPos;  
out vec3 Normal;

void main()
{
    FragPos = vec3(vec4(aVertex, 1.0) * Model);
    Normal = aNormal * mat3(transpose(inverse(Model)));
    gl_Position = vec4(aVertex, 1.0) * Model * View * Projection;
}
#version 400 core

layout (location = 0) in vec2 vertex;
layout (location = 1) in vec2 texCoords;
out vec2 TexCoord;
void main()
{
    TexCoord = texCoords;
    gl_Position = vec4(vertex, 0.0, 1.0);
}

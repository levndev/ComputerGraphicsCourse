#version 330 core
layout (location = 0) in vec2 vertex;

uniform mat4 Model;
uniform mat4 Projection;

void main()
{
    gl_Position = vec4(vertex, 0.0, 1.0) * Model * Projection;
}
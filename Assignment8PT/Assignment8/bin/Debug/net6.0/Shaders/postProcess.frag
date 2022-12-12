#version 400 core
out vec4 FragColor;
in vec2 TexCoords;
uniform sampler2D ScreenTexture;
uniform int Samples;
void main()
{
    vec3 color = texture(ScreenTexture, TexCoords).rgb;
    color /= float(Samples);
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0 / 2.2));
    FragColor = vec4(color, 1.0);
}   
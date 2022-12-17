#version 400 core
out vec4 FragColor;
in vec2 TexCoords;
uniform sampler2D ScreenTexture;
uniform int Samples;
struct PostProcessingSettings {
    int KernelOffsetDivisor;
};
uniform PostProcessingSettings Settings;
vec3 Filter(vec3 Color) {
    // average over accumulated frames
    Color /= float(Samples);
    //gamma correction
    Color = Color / (Color + vec3(1.0));
    Color = pow(Color, vec3(1.0 / 2.2));
    return Color;
}

void main()
{
    float offset = 1.0 / float(Settings.KernelOffsetDivisor * 100);  
    vec2 offsets[9] = vec2[](
        vec2(-offset,  offset), // top-left
        vec2( 0.0f,    offset), // top-center
        vec2( offset,  offset), // top-right
        vec2(-offset,  0.0f),   // center-left
        vec2( 0.0f,    0.0f),   // center-center
        vec2( offset,  0.0f),   // center-right
        vec2(-offset, -offset), // bottom-left
        vec2( 0.0f,   -offset), // bottom-center
        vec2( offset, -offset)  // bottom-right    
    );
    float blur_divisor = 16;
    float kernel[9] = float[](
        1.0 / blur_divisor, 2.0 / blur_divisor, 1.0 / blur_divisor,
        2.0 / blur_divisor, 4.0 / blur_divisor, 2.0 / blur_divisor,
        1.0 / blur_divisor, 2.0 / blur_divisor, 1.0 / blur_divisor  
    );
    vec3 sampleTex[9];
    for(int i = 0; i < 9; i++)
    {
        sampleTex[i] = vec3(texture(ScreenTexture, TexCoords.st + offsets[i]));
        sampleTex[i] = Filter(sampleTex[i]);
    }
    vec3 col = vec3(0.0);
    for(int i = 0; i < 9; i++)
        col += sampleTex[i] * kernel[i];
    FragColor = vec4(col, 1.0);
}   
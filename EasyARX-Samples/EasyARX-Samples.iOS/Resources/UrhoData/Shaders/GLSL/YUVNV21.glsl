// Shader for YUVNV21 Camera Stream Most Android
#include "Uniforms.glsl"
#include "Samplers.glsl"
#include "Transform.glsl"
#include "ScreenPos.glsl"

varying highp vec2 vTexCoord;

void VS()
{
    mat4 modelMatrix = iModelMatrix;
    vec3 worldPos = GetWorldPos(modelMatrix);
    gl_Position = GetClipPos(worldPos);
    vTexCoord = iTexCoord;
}

void PS()
{
    vec2 cbcr = texture2D( sNormalMap, vTexCoord).ar - vec2(0.5, 0.5);
    vec3 ycbcr = vec3(texture2D( sDiffMap, vTexCoord).r, cbcr);
    vec3 rgb = mat3( 1.0,    1.0,   1.0,
                     0.0,   -0.344, 1.772,
                     1.402, -0.714, 0.0) * ycbcr;
    gl_FragColor = vec4(rgb, 1.0);
}

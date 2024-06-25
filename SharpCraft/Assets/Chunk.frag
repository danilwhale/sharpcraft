#version 330

out vec4 finalColor;

in vec2 fragTexCoord;
in vec4 fragColor;
in vec3 fragPosition;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

// fog uniforms
uniform vec3 viewPos;
uniform vec4 skyColor;
uniform float fogDensity;

// chunk appearing animation uniforms
uniform float chunkTime;
uniform float chunkAppearSpeed;

void main() {
    vec4 texel = texture(texture0, fragTexCoord);
    if (texel.a == 0) {
        discard;
    }
    
    finalColor = texel * colDiffuse * fragColor;
    
    // chunk appearing animation
    vec4 chunkColor = vec4(1.0, 1.0, 1.0, chunkTime < chunkAppearSpeed ? chunkTime / chunkAppearSpeed : 1.0);
    finalColor *= chunkColor;
    
    // fog
    float dist = length(viewPos - fragPosition);
    const vec4 fogColor = vec4(0.6, 0.6, 0.6, 1.0);
    float fogFactor = 1.0 / exp((dist * fogDensity) * (dist * fogDensity));
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    
    finalColor = mix(mix(fogColor, skyColor, 0.5), finalColor, fogFactor);
}
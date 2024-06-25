#version 330

#define CHUNK_MAX_TIME 0.3

in vec2 fragTexCoord;
in vec4 fragColor;
out vec4 finalColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform float chunkTime;

void main() {
    vec4 texel = texture(texture0, fragTexCoord);
    if (texel.a == 0) {
        discard;
    }
    
    vec4 chunkColor = vec4(1.0, 1.0, 1.0, chunkTime < CHUNK_MAX_TIME ? chunkTime / CHUNK_MAX_TIME : 1.0);
    
    finalColor = texel * colDiffuse * fragColor * chunkColor;
}
#version 330

in vec2 fragTexCoord;       // Fragment input attribute: texture coordinate
in vec4 fragColor;          // Fragment input attribute: color
out vec4 finalColor;        // Fragment output: color

uniform sampler2D texture0; // Fragment input texture (always required, could be a white pixel)
uniform vec4 colDiffuse;    // Fragment input color diffuse (multiplied by texture color)

void main() {
    vec4 texel = texture(texture0, fragTexCoord);
    if (texel.a == 0) {
        discard;
    }
    
    finalColor = texel * colDiffuse * fragColor;
}
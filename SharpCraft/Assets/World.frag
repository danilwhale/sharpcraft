#version 120

precision mediump float;

varying vec2 fragTexCoord;
varying vec4 fragColor;
varying vec3 fragPosition;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform bool isLit;

void main() {
    vec4 texelColor = texture2D(texture0, fragTexCoord);
    if (texelColor.a < 0.1) {
        discard;
    }
    
    vec4 finalColor = texelColor * colDiffuse * fragColor;
    if (!isLit) {
        finalColor *= vec4(0.6, 0.6, 0.6, 1.0);
    }
    
    vec4 ambient = isLit ? vec4(1.0, 1.0, 1.0, 1.0) : vec4(0.6, 0.6, 0.6, 1.0);
    vec4 fogColor = isLit ? vec4(0.996, 0.984, 0.98, 1.0) : vec4(0.055, 0.043, 0.039, 1);
    float fogDensity = isLit ? 0.001f : 0.01f;
    
    finalColor += texelColor * (ambient / 10.0);
    
    float dist = length(fragPosition);
    float fogFactor = 1.0 / exp((dist * fogDensity) * (dist * fogDensity));
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    
    gl_FragColor = mix(fogColor, finalColor, fogFactor);
}
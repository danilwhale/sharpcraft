#version 330

in vec3 vertPos;
in vec2 vertTexCoord;
in vec4 vertColor;

out vec2 fragTexCoord;
out vec4 fragColor;

uniform mat4 mvp;

void main() {
    gl_Position = mvp * vec4(vertPos, 1.0);
    fragTexCoord = vertTexCoord;
    fragColor = vertColor;
}
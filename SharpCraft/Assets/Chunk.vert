#version 120

attribute vec3 vertexPosition;   // Vertex input attribute: position
attribute vec2 vertexTexCoord;   // Vertex input attribute: texture coordinate
attribute float vertexLight;      // Vertex input attribute: light
varying vec3 fragPosition;
varying vec2 fragTexCoord;       // To-fragment attribute: texture coordinate
varying vec4 fragColor;          // To-fragment attribute: color

uniform mat4 mvp;           // Model-View-Projection matrix

void main() {
    vec4 position = mvp * vec4(vertexPosition, 1.0);

    fragPosition = position.xyz;
    fragTexCoord = vertexTexCoord;
    fragColor = vec4(vertexLight, vertexLight, vertexLight, 1.0);

    gl_Position = position;
}

#version 120

attribute vec3 vertexPosition;   // Vertex input attribute: position
attribute vec2 vertexTexCoord;   // Vertex input attribute: texture coordinate
attribute float vertexLight;      // Vertex input attribute: light
varying vec3 fragPosition;
varying vec2 fragTexCoord;       // To-fragment attribute: texture coordinate
varying vec4 fragColor;          // To-fragment attribute: color

uniform mat4 mvp;           // Model-View-Projection matrix
uniform mat4 matModel;

void main() {
    fragPosition = vec3(matModel * vec4(vertexPosition, 1.0));
    fragTexCoord = vertexTexCoord;
    fragColor = vec4(vertexLight, vertexLight, vertexLight, 1.0);

    gl_Position = mvp * vec4(vertexPosition, 1.0);
}

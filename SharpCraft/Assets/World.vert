#version 120

precision mediump float;

attribute vec3 vertexPosition;   // Vertex input attribute: position
attribute vec2 vertexTexCoord;   // Vertex input attribute: texture coordinate
attribute vec4 vertexColor;      // Vertex input attribute: color
varying vec3 fragPosition;
varying vec2 fragTexCoord;       // To-fragment attribute: texture coordinate
varying vec4 fragColor;          // To-fragment attribute: color

uniform mat4 mvp;           // Model-View-Projection matrix

void main() {
    vec4 position = mvp * vec4(vertexPosition, 1.0);
    
    fragPosition = position.xyz;
    fragTexCoord = vertexTexCoord;
    fragColor = vertexColor;
    
    gl_Position = position;
}

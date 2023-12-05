#version 330

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in float aTexIndex;

out vec2 TexCoord;
out float TexIndex;

uniform mat4 transform;

void main()
{
	TexCoord = aTexCoord;
	TexIndex = aTexIndex;
	gl_Position = vec4(aPosition, 1.0) * transform;
}
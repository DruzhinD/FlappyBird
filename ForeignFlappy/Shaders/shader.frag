#version 450 core

out vec4 outColor;

in vec2 TexCoord;
in float TexIndex;

uniform sampler2D textures[32];

void main()
{
	int index = int(TexIndex);
	outColor = texture(textures[index], TexCoord);
}
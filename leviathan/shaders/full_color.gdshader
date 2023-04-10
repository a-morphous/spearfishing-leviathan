shader_type canvas_item;

uniform bool active = false;
uniform vec4 flash_color: source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
	vec4 previous_color = texture(TEXTURE, UV);
	vec4 flash_color_real = vec4(flash_color.rgb, previous_color.a);
	vec4 new_color = previous_color;
	if (active == true)
	{
		new_color = flash_color_real;
	}
	COLOR = new_color;
}
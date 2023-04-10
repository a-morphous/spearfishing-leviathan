shader_type canvas_item;

uniform bool active = false;
uniform float active_brightness = 0.0;
uniform float active_contrast = 1.0;
uniform vec4 flash_color: source_color = vec4(1.0, 1.0, 1.0, 1.0);

vec3 brightnessContrast(vec3 value, float brightness, float contrast)
{
    return (value - 0.5) * contrast + 0.5 + brightness;
}

vec3 greyscale(in vec3 sample) {
	float grey = 0.21 * sample.r + 0.71 * sample.g + 0.07 * sample.b;
	return vec3(grey, grey, grey);
}

vec3 colorize(in vec3 greyscale, vec3 color) {
	return (greyscale * color);
}

void fragment() {
	vec4 previous_color = texture(TEXTURE, UV);
	vec3 flash_color_real = brightnessContrast(colorize(greyscale(previous_color.rgb), flash_color.rgb), active_brightness, active_contrast);
	vec4 flash_color_with_alpha = vec4(flash_color_real.rgb, previous_color.a);
	vec4 new_color = previous_color;
	if (active == true)
	{
		new_color = flash_color_with_alpha;
	}
	COLOR = new_color;
}
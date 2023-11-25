shader_type spatial;
render_mode unshaded;

// Lower values are smaller width.
uniform float outline_width : hint_range(0.0, 5, 0.1) = 0.5;

uniform vec4 base_color : source_color = vec4(1.0);
uniform bool enabled = false;

uniform float fresnel_factor: hint_range(0.0, 5, 0.1) = 1.0;
uniform float fresnel_step: hint_range(0.0, 5, 0.1) = 0.5;

float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((fresnel_factor - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), 1.0 / amount);
}

void fragment()
{
	if (enabled) {		
		float basic_fresnel = fresnel(outline_width, NORMAL, VIEW);
		basic_fresnel = step(fresnel_step, basic_fresnel); //0.5 magic number seems to give the most intuitive control
		ALBEDO = base_color.rgb * basic_fresnel;
		ALPHA = 0.0 + basic_fresnel;
	} else {
		ALPHA = 0.0;
	}
}
shader_type spatial;
render_mode unshaded, depth_draw_never, blend_mix;

uniform vec3 highlight_color: source_color;

void fragment() {
  ALBEDO = highlight_color;
  ALPHA = 0.5f;
}

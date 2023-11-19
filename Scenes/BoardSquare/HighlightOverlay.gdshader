shader_type spatial;
render_mode depth_draw_never, blend_mix;

uniform vec4 highlight_color: source_color;

void fragment() {
  ALBEDO = highlight_color.rgb;
  ALPHA = highlight_color.a;
}

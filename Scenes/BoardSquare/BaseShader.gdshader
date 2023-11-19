shader_type spatial;

uniform vec3 base_color: source_color;

void fragment() {
  ALBEDO = base_color;
}

shader_type spatial;
// render_mode unshaded;

uniform sampler2D noise;
uniform float pi = 3.1415926535;

vec2 calcUV(vec3 pos) {
	float u = 0.5 + atan(pos.z, pos.x) / (2.0 * pi);
	float v = 0.5 - asin(pos.y) / pi;
	return vec2(u, v);
}

vec3 calcNormal(vec3 pt, mat3 rot1, mat3 rot2) {
	vec3 pt1 = rot1 * pt;
	vec3 pt2 = rot2 * pt;
	pt1 += pt1 * texture(noise, calcUV(pt1)).x;
	pt2 += pt2 * texture(noise, calcUV(pt2)).x;
	pt += pt * texture(noise, calcUV(pt)).x;
	vec3 norm = normalize(cross(pt1 - pt, pt2 - pt));
	if (dot(norm, pt) < 0.0) {
		norm = -norm;
	}
	return norm;
}

void vertex() {
	float angle = 0.1;
	angle = 2.0 * pi / 128.0;
	mat3 rx = mat3(vec3(1, 0, 0), vec3(0, cos(angle), -sin(angle)), vec3(0, sin(angle), cos(angle)));
	mat3 ry = mat3(vec3(cos(angle), 0, sin(angle)), vec3(0, 1, 0), vec3(-sin(angle), 0, cos(angle)));
	mat3 rz = mat3(vec3(cos(angle), -sin(angle), 0), vec3(sin(angle), cos(angle), 0), vec3(0, 0, 1));
	
	UV = calcUV(VERTEX);
	NORMAL = calcNormal(VERTEX, rx, rz);
	VERTEX += VERTEX * texture(noise, calcUV(VERTEX)).x;
}

void fragment() {
	ALBEDO = vec3(0.2, 0.6, 0.04);
}

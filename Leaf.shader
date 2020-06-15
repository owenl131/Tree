shader_type spatial;
// render_mode unshaded;

uniform sampler2D noise;
uniform sampler2D normals;
uniform float pi = 3.1415926535;

vec2 calcUV(vec3 pos) {
	float u = 0.5 - atan(pos.z, -pos.x) / (2.0 * pi);
	float v = 0.5 - asin(pos.y) / pi;
	return vec2(u, v);
}

vec3 calcVertex(vec2 uv) {
	float u = uv.x;
	float v = uv.y;
	float y = sin((0.5 - v) * pi);
	float magOther = sqrt(1.0 - y*y);
	float z = -magOther * sin(u * 2.0 * pi);
	float x = -magOther * cos(u * 2.0 * pi);
	return vec3(x, y, z);
}

vec3 calcHeight(vec3 pt) {
	return pt + pt * texture(noise, calcUV(pt)).x;
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
	// NORMAL = calcNormal(VERTEX, rx, rz);
	// VERTEX = calcHeight(VERTEX);
	VERTEX = texture(noise, UV).xyz;
	VERTEX = VERTEX * 6.0 - 3.0;
	NORMAL = texture(normals, UV).xyz;
	NORMAL = NORMAL * 2.0 - 1.0;
	// VERTEX = calcVertex(UV);
}

void fragment() {
	ALBEDO = vec3(0.2, 0.6, 0.04);
	// ALBEDO = vec3(UV.x, UV.x, UV.x);
	// ALBEDO = vec3(UV.y, UV.y, UV.y);
}

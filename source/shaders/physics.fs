precision mediump float;
uniform sampler2D physicsData;
uniform vec2 bounds;
uniform vec3 targets[5];
uniform int targetCount;
const int POSITION_SLOT = 0;
const int VELOCITY_SLOT = 1;
vec4 texel(vec2 offset) {
  vec2 coord = (gl_FragCoord.xy + offset) / bounds;
  return texture2D(physicsData, coord);
}

vec3 nearestTarget(vec3 position) {
  vec3 best = targets[0];
  float bestDist = distance(position, targets[0]);
  if (targetCount > 1) {
    float d = distance(position, targets[1]);
    if (d < bestDist) { bestDist = d; best = targets[1]; }
  }
  if (targetCount > 2) {
    float d = distance(position, targets[2]);
    if (d < bestDist) { bestDist = d; best = targets[2]; }
  }
  if (targetCount > 3) {
    float d = distance(position, targets[3]);
    if (d < bestDist) { bestDist = d; best = targets[3]; }
  }
  if (targetCount > 4) {
    float d = distance(position, targets[4]);
    if (d < bestDist) { bestDist = d; best = targets[4]; }
  }
  return best;
}
void main() {
  int slot = int(mod(gl_FragCoord.x, 2.0));
  if (slot == POSITION_SLOT) {
    vec4 dataA = texel(vec2(0, 0));
    vec4 dataB = texel(vec2(1, 0));
    vec3 position = dataA.xyz;
    vec3 velocity = dataB.xyz;
    float phase = dataA.w;
    if (phase > 0.0) {
      position += velocity * 0.005;
      vec3 t = nearestTarget(position);
      if (length(t - position) < 0.035) phase = 0.0;
      else phase += 0.1;
    } else {
      position = vec3(-1);
    }
    gl_FragColor = vec4(position, phase);
  } else if (slot == VELOCITY_SLOT) {
    vec4 dataA = texel(vec2(-1, 0));
    vec4 dataB = texel(vec2(0, 0));
    vec3 position = dataA.xyz;
    vec3 velocity = dataB.xyz;
    float phase = dataA.w;
    if (phase > 0.0) {
      vec3 t = nearestTarget(position);
      vec3 delta = normalize(t - position);
      velocity += delta * 0.05;
      velocity *= 0.991;
    } else {
      velocity = vec3(0);
    }
    gl_FragColor = vec4(velocity, 1);
  }
}
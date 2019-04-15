// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

void AtsWavingGrassVert (inout appdata_full v) {
////////// start bending

	float4 _waveXSizeMove = float4(0.048, 0.06, 0.24, 0.096);
	float4 _waveZSizeMove = float4 (0.024, .08, 0.08, 0.2);
	float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
	float4 waves;
	waves = v.vertex.x * _waveXSizeMove;
	waves += v.vertex.z * _waveZSizeMove;
	_waveXSizeMove = float4(0.024, 0.04, -0.12, 0.096);
	_waveZSizeMove = float4 (0.006, .02, -0.02, 0.1);
	
	// Add in time to model them over time
	waves += _WaveAndDistance.x * waveSpeed;
	float4 s, c;
	waves = frac (waves);
	FastSinCos (waves, s,c);
	float waveAmount = v.color.a * _WaveAndDistance.z;

	// Faster winds move the grass more than slow winds 
	s *= normalize (waveSpeed);
	s = s * s;
	
	float lighting = dot (s, normalize (float4 (1,1,.4,.2))) * .7;
	s *= waveAmount;
	
	fixed3 waveColor = lerp (fixed3(0.5,0.5,0.5), _WavingTint.rgb, lighting);
	
	v.color.rgb = v.color.rgb * waveColor * 2;
	
	float3 waveMove = float3 (0,0,0);
	waveMove.x = dot (s, _waveXSizeMove);
	waveMove.z = dot (s, _waveZSizeMove);
	v.vertex.xz -= mul ((float3x3)unity_WorldToObject, waveMove).xz * 8;
			
////////// end bending
}
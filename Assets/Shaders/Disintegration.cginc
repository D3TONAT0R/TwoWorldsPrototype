half CalcScreenSpaceDisintegrationThreshold(sampler2D map, fixed4 map_st, float2 screenPos)
{
	float aspect = _ScreenParams.x / _ScreenParams.y;
	screenPos.x *= aspect;
	screenPos *= map_st.xy * 2.5;
	return tex2D(map, screenPos).a;
}

half CalcWorldSpaceDisintegrationThreshold(sampler2D map, fixed4 map_st, float3 worldPos)
{
	worldPos *= 1.0001;
	half x = tex2D(map, worldPos.yz * map_st.xy).a;
	half y = tex2D(map, worldPos.xz * map_st.xy).a;
	half z = tex2D(map, worldPos.xy * map_st.xy).a;
	return clamp((x+y+z)*0.42-0.12, 0.01, 0.99);
}
void SetAlbedo(Input IN, inout SurfaceOutputStandard o)
{
	// Albedo comes from a texture tinted by color
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}

void SetMetallicSmoothness(Input IN, inout SurfaceOutputStandard o)
{
	// Metallic and smoothness come from slider variables
    fixed2 mg = tex2D(_MetallicGlossMap, IN.uv_MainTex).ra;
    o.Metallic = mg.x * _Metallic;
    o.Smoothness = mg.y * _Glossiness;
}

void SetNormal(Input IN, inout SurfaceOutputStandard o)
{
    o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
}

void SetOcclusion(Input IN, inout SurfaceOutputStandard o)
{
    o.Occlusion = lerp(1, tex2D(_OcclusionMap, IN.uv_MainTex).r, _OcclusionStrength);
}

void SetEmission(Input IN, inout SurfaceOutputStandard o)
{
    o.Emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
}

//Includes everything
void SetCommonOutput(Input IN, inout SurfaceOutputStandard o)
{
    SetAlbedo(IN, o);
    SetMetallicSmoothness(IN, o);
    SetNormal(IN, o);
    SetOcclusion(IN, o);
    SetEmission(IN, o);
}

//Includes everything but emission
void SetCommonOutputNoEmission(Input IN, inout SurfaceOutputStandard o)
{
    SetAlbedo(IN, o);
    SetMetallicSmoothness(IN, o);
    SetNormal(IN, o);
    SetOcclusion(IN, o);
}
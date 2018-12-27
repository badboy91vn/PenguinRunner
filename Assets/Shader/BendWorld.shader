Shader "HNL/BendWorld"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Curvatrue ("Curvatrue", Float) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		
		CGPROGRAM
			#pragma surface surf Lambert vertex:vert addshadow

			uniform sampler2D _MainTex;
			uniform float _Curvatrue;

			struct Input
			{
				float2 uv_MainTex;
			};

			void vert(inout appdata_full v)
			{
				float4 worldSpace = mul(unity_ObjectToWorld, v.vertex);
				worldSpace.xyz -= _WorldSpaceCameraPos.xyz;
				worldSpace = float4( 0.0f, -_Curvatrue * (worldSpace.z * worldSpace.z), 0.0f, 0.0f);

				v.vertex += mul(unity_ObjectToWorld, worldSpace);
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				half4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rbg;
				o.Alpha = c.a;
			}
		ENDCG
    }
	
	FallBack "Mobile/Diffuse"
}

Shader "Unlit/WaterFX" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Height ("Height", Float) = 1
		_Color1 ("BaseColor", Vector) = (1,1,1,1)
		_Color2 ("HeightColor", Vector) = (1,1,1,1)
		_Color3 ("VelosityColor", Vector) = (1,1,1,1)
		_ColumnsX ("Columns (X)", Float) = 1
		_RowsY ("Rows (Y)", Float) = 1
		_AnimSpeed ("Frames Per Seconds", Float) = 10
		_AtlasInfo ("X Position, Y Position, Width, Height", Vector) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}
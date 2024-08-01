Shader "HyperShaderWithOutline" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.4
		_Diffuse ("_Diffuse", Vector) = (1,1,1,1)
		_SelfShadowColor ("_SelfShadowColor", Vector) = (1,1,1,1)
		_SpecularColor ("_SpecularColor", Vector) = (1,1,1,1)
		_Glossiness ("_Glossiness", Float) = 0.5
		_Falloff ("_Falloff", Range(0, 1)) = 0
		_FalloffPower ("_Falloff Power", Float) = 1
		_FalloffColor ("Falloff Color", Vector) = (1,1,1,1)
		_Alpha ("Alpha", Float) = 1
		_Red ("Red", Range(0, 1)) = 0
		_OutlineColor ("_OutlineColor", Vector) = (1,1,1,1)
		_OutlineThickness ("_OutlineThickness", Float) = 0.05
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
	Fallback "VertexLit"
}
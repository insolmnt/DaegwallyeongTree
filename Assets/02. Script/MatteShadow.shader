Shader "Custom/MatteShadow" {
	Properties {
	}
	
	SubShader {
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf 

		ENDCG
	} 
	FallBack "Transparent/Cutout/VertexLit"
}

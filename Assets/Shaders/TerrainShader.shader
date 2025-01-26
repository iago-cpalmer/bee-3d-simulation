Shader "MyShaders/TerrainShader" {

	Properties{
		[MainTexture] _AlbedoTexture("Color", 2D) = "white" {}
	}

		SubShader{
			Tags { "RenderPipline" = "Universal Pipeline"}

			Pass {
				Name "ForwardLit"
				Tags{"LightMode" = "UniversalForward"}
				Lighting On
				Cull Back
				ZTest LEqual
				ZWrite On

				HLSLPROGRAM
					#pragma vertex Vertex
					//#pragma require geometry
					//#pragma geometry Geometry
					#pragma fragment Fragment
					

					#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
					#pragma multi_compile _ LIGHTMAP_ON
					#pragma multi_compile _ DYNAMICLIGHTMAP_ON
					#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
					#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
					#pragma multi_compile _ SHADOWS_SHADOWMASK
					#pragma multi_compile _ DIRLIGHTMAP_COMBINED
					#pragma multi_compile_fog

					#include "TerrainShader.hlsl"
				ENDHLSL

			}


			Pass {
				Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

		// -------------------------------------
		// Render State Commands
		ZWrite On
		ColorMask 0
		Cull Back
		HLSLPROGRAM
		#pragma target 2.0

		// -------------------------------------
		// Shader Stages
		#pragma vertex DepthOnlyVertex
		#pragma fragment DepthOnlyFragment

			
			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			#include "DepthCast.hlsl"
			ENDHLSL
		}

		Pass
		{
			Name "DepthNormals"
			Tags
			{
				"LightMode" = "DepthNormals"
			}

		// -------------------------------------
		// Render State Commands
		ZWrite On
		Cull Back

		HLSLPROGRAM
		#pragma target 2.0

		// -------------------------------------
		// Shader Stages
		#pragma vertex DepthNormalsVertex
		#pragma fragment DepthNormalsFragment

		#include "DepthNormals.hlsl"
		ENDHLSL

		}
		}
		FallBack "Diffuse"
}
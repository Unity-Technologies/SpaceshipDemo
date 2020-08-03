Shader "Hologram"
{
	Properties
	{
				[NoScaleOffset] Texture_A7830126("Texture", 2D) = "white" {}
				Vector1_59D367DF("Speed", Float) = 1
				Vector1_9E53E34E("Brightness", Range(0, 1)) = 0.2
		
	}
	SubShader
{
	Pass
	{
		Lighting Off
		Blend One Zero

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		HLSLEND

		HLSLPROGRAM
		#include "ShaderGraphLibrary/UnityCustomRenderTexture.hlsl"
		#pragma vertex CustomRenderTextureVertexShader
		#pragma fragment frag
		#pragma target 3.0

		float4 SRGBToLinear( float4 c ) { return c; }
		float3 SRGBToLinear( float3 c ) { return c; }

		struct SurfaceInputs
		{
			// update input values
			float3 localTexcoord;
			float3 globalTexcoord;
			uint primitiveID;
			float3 direction;
		};

		SurfaceInputs ConvertV2FToSurfaceInputs( v2f_customrendertexture IN )
		{
			SurfaceInputs o;
			
			o.localTexcoord = IN.localTexcoord;
			o.globalTexcoord = IN.globalTexcoord;
			o.primitiveID = IN.primitiveID;
			o.direction = IN.direction;

			return o;
		}

    						TEXTURE2D(Texture_A7830126); SAMPLER(samplerTexture_A7830126);
						float Vector1_59D367DF;
						float Vector1_9E53E34E;
						SAMPLER(SamplerState_Linear_Clamp_sampler);
				
				
				        void Unity_Modulo_float(float A, float B, out float Out)
				        {
				            Out = fmod(A, B);
				        }
				
				        void Unity_Divide_float(float A, float B, out float Out)
				        {
				            Out = A / B;
				        }
				
				        void Unity_Lerp_float(float A, float B, float T, out float Out)
				        {
				            Out = lerp(A, B, T);
				        }
				
				        void Unity_Multiply_float (float3 A, float3 B, out float3 Out)
				        {
				            Out = A * B;
				        }
				
				        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
				        {
				            Out = A + B;
				        }
				
				        void Unity_Add_float(float A, float B, out float Out)
				        {
				            Out = A + B;
				        }
				
				        void Unity_Absolute_float(float In, out float Out)
				        {
				            Out = abs(In);
				        }
				
				        void Unity_Multiply_float (float A, float B, out float Out)
				        {
				            Out = A * B;
				        }
				
				        void Unity_Saturate_float(float In, out float Out)
				        {
				            Out = saturate(In);
				        }
				
				        void Unity_OneMinus_float(float In, out float Out)
				        {
				            Out = 1 - In;
				        }
				
				        void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
				        {
				            //rotation matrix
				            UV -= Center;
				            float s = sin(Rotation);
				            float c = cos(Rotation);
				
				            //center rotation matrix
				            float2x2 rMatrix = float2x2(c, -s, s, c);
				            rMatrix *= 0.5;
				            rMatrix += 0.5;
				            rMatrix = rMatrix*2 - 1;
				
				            //multiply the UVs by the rotation matrix
				            UV.xy = mul(UV.xy, rMatrix);
				            UV += Center;
				
				            Out = UV;
				        }
				
				        void Unity_Multiply_float (float2 A, float2 B, out float2 Out)
				        {
				            Out = A * B;
				        }
				
				        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
				        {
				            Out = A + B;
				        }
				
				        void Unity_Power_float(float A, float B, out float Out)
				        {
				            Out = pow(A, B);
				        }
				
						struct SurfaceDescription{
							float4 Color;
						};
				
						SurfaceDescription PopulateSurfaceData(SurfaceInputs IN) {
							SurfaceDescription surface = (SurfaceDescription)0;
							float3 _CustomTextureUpdateData_5D9CFE11_data = IN.localTexcoord;
							float _Property_37780923_Out = Vector1_59D367DF;
							float _Modulo_6ED8ED3C_Out;
							Unity_Modulo_float(_Time.y, _Property_37780923_Out, _Modulo_6ED8ED3C_Out);
							float _Divide_F4C03485_Out;
							Unity_Divide_float(_Modulo_6ED8ED3C_Out, _Property_37780923_Out, _Divide_F4C03485_Out);
							float _Lerp_181A3043_Out;
							Unity_Lerp_float(2, 1, _Divide_F4C03485_Out, _Lerp_181A3043_Out);
							float3 _Multiply_B979F1C0_Out;
							Unity_Multiply_float(_CustomTextureUpdateData_5D9CFE11_data, (_Lerp_181A3043_Out.xxx), _Multiply_B979F1C0_Out);
							
							float _Lerp_1F8280A4_Out;
							Unity_Lerp_float(-0.5, 0, _Divide_F4C03485_Out, _Lerp_1F8280A4_Out);
							float3 _Add_94E96FC9_Out;
							Unity_Add_float3(_Multiply_B979F1C0_Out, (_Lerp_1F8280A4_Out.xxx), _Add_94E96FC9_Out);
							float4 _SampleTexture2D_94E71B54_RGBA = SAMPLE_TEXTURE2D(Texture_A7830126, SamplerState_Linear_Clamp_sampler, (_Add_94E96FC9_Out.xy));
							float _SampleTexture2D_94E71B54_R = _SampleTexture2D_94E71B54_RGBA.r;
							float _SampleTexture2D_94E71B54_G = _SampleTexture2D_94E71B54_RGBA.g;
							float _SampleTexture2D_94E71B54_B = _SampleTexture2D_94E71B54_RGBA.b;
							float _SampleTexture2D_94E71B54_A = _SampleTexture2D_94E71B54_RGBA.a;
							float _Add_BC479BBC_Out;
							Unity_Add_float(_Divide_F4C03485_Out, -0.5, _Add_BC479BBC_Out);
							float _Absolute_DFE47D9A_Out;
							Unity_Absolute_float(_Add_BC479BBC_Out, _Absolute_DFE47D9A_Out);
							float _Multiply_D877F5A4_Out;
							Unity_Multiply_float(_Absolute_DFE47D9A_Out, 2, _Multiply_D877F5A4_Out);
							
							float _Saturate_6D4AC1F8_Out;
							Unity_Saturate_float(_Multiply_D877F5A4_Out, _Saturate_6D4AC1F8_Out);
							float _OneMinus_17F7CA1D_Out;
							Unity_OneMinus_float(_Saturate_6D4AC1F8_Out, _OneMinus_17F7CA1D_Out);
							float _Multiply_874D00CF_Out;
							Unity_Multiply_float(_SampleTexture2D_94E71B54_R, _OneMinus_17F7CA1D_Out, _Multiply_874D00CF_Out);
							
							float3 _CustomTextureUpdateData_72D97F40_data = IN.localTexcoord;
							float2 _Rotate_F182630C_Out;
							Unity_Rotate_Radians_float((_CustomTextureUpdateData_72D97F40_data.xy), float2 (0.5,0.5), 2, _Rotate_F182630C_Out);
							float _Multiply_9C01B902_Out;
							Unity_Multiply_float(_Property_37780923_Out, 0.5, _Multiply_9C01B902_Out);
							
							float _Add_38BB8851_Out;
							Unity_Add_float(_Time.y, _Multiply_9C01B902_Out, _Add_38BB8851_Out);
							float _Modulo_A8F7E78B_Out;
							Unity_Modulo_float(_Add_38BB8851_Out, _Property_37780923_Out, _Modulo_A8F7E78B_Out);
							float _Divide_8AFE3E78_Out;
							Unity_Divide_float(_Modulo_A8F7E78B_Out, _Property_37780923_Out, _Divide_8AFE3E78_Out);
							float _Lerp_B00BD9B1_Out;
							Unity_Lerp_float(2, 1, _Divide_8AFE3E78_Out, _Lerp_B00BD9B1_Out);
							float2 _Multiply_4ED78CBA_Out;
							Unity_Multiply_float(_Rotate_F182630C_Out, (_Lerp_B00BD9B1_Out.xx), _Multiply_4ED78CBA_Out);
							
							float _Lerp_4C971ACF_Out;
							Unity_Lerp_float(-0.5, 0, _Divide_8AFE3E78_Out, _Lerp_4C971ACF_Out);
							float2 _Add_D3231A86_Out;
							Unity_Add_float2(_Multiply_4ED78CBA_Out, (_Lerp_4C971ACF_Out.xx), _Add_D3231A86_Out);
							float4 _SampleTexture2D_9F2549F2_RGBA = SAMPLE_TEXTURE2D(Texture_A7830126, SamplerState_Linear_Clamp_sampler, _Add_D3231A86_Out);
							float _SampleTexture2D_9F2549F2_R = _SampleTexture2D_9F2549F2_RGBA.r;
							float _SampleTexture2D_9F2549F2_G = _SampleTexture2D_9F2549F2_RGBA.g;
							float _SampleTexture2D_9F2549F2_B = _SampleTexture2D_9F2549F2_RGBA.b;
							float _SampleTexture2D_9F2549F2_A = _SampleTexture2D_9F2549F2_RGBA.a;
							float _Add_FAD1AE08_Out;
							Unity_Add_float(_Divide_8AFE3E78_Out, -0.5, _Add_FAD1AE08_Out);
							float _Absolute_114085C8_Out;
							Unity_Absolute_float(_Add_FAD1AE08_Out, _Absolute_114085C8_Out);
							float _Multiply_220B2686_Out;
							Unity_Multiply_float(_Absolute_114085C8_Out, 2, _Multiply_220B2686_Out);
							
							float _Saturate_A8EC61E8_Out;
							Unity_Saturate_float(_Multiply_220B2686_Out, _Saturate_A8EC61E8_Out);
							float _OneMinus_CF548D0B_Out;
							Unity_OneMinus_float(_Saturate_A8EC61E8_Out, _OneMinus_CF548D0B_Out);
							float _Multiply_26270506_Out;
							Unity_Multiply_float(_SampleTexture2D_9F2549F2_R, _OneMinus_CF548D0B_Out, _Multiply_26270506_Out);
							
							float _Add_68A45C88_Out;
							Unity_Add_float(_Multiply_874D00CF_Out, _Multiply_26270506_Out, _Add_68A45C88_Out);
							float _Property_5404FC99_Out = Vector1_9E53E34E;
							float _Lerp_2E1D7AA4_Out;
							Unity_Lerp_float(_Add_68A45C88_Out, 1, _Property_5404FC99_Out, _Lerp_2E1D7AA4_Out);
							float _Power_92E0BCD5_Out;
							Unity_Power_float(_Lerp_2E1D7AA4_Out, 0.3, _Power_92E0BCD5_Out);
							surface.Color = (_Power_92E0BCD5_Out.xxxx);
							return surface;
						}
				
	

		float4 frag(v2f_customrendertexture IN) : COLOR
		{
			SurfaceInputs surfaceInput = ConvertV2FToSurfaceInputs(IN);

			SurfaceDescription surf = PopulateSurfaceData(surfaceInput);

			return surf.Color;
		}
		ENDHLSL
	}
}

	FallBack "Hidden/InternalErrorShader"
}

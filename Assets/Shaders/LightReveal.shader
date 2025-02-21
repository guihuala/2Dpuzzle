Shader "Custom/LightReveal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Opacity ("Overall Opacity", Range(0, 1)) = 1
        _Color ("Tint Color", Color) = (1,1,1,1)
        _LightColor ("Light Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 positionOS : TEXCOORD2;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Opacity;
                float4 _Color;
                float4 _LightColor;
                float4 _LightPositions[8];
                float _LightIntensities[8];
                float _LightRanges[8];
                int _LightCount;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS.xyz;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                // 应用基础颜色和精灵颜色
                color *= _Color * IN.color;
                
                // 计算所有光源的累积影响
                float totalLight = 0;
                float3 finalLightColor = float3(0, 0, 0);
                
                for (int i = 0; i < _LightCount; i++)
                {
                    float2 lightPos = _LightPositions[i].xy;
                    float distance = length(IN.positionOS.xy - lightPos);
                    float range = _LightRanges[i];
                    float intensity = _LightIntensities[i];
                    
                    // 计算光照衰减
                    float attenuation = saturate(1 - distance / range);
                    attenuation = smoothstep(0, 1, attenuation);
                    
                    totalLight += attenuation * intensity;
                    finalLightColor += _LightColor.rgb * attenuation * intensity;
                }
                
                // 限制总光照强度并应用平滑过渡
                totalLight = saturate(totalLight);
                totalLight = smoothstep(0, 1, totalLight);
                finalLightColor = saturate(finalLightColor);
                
                // 应用光照颜色到RGB通道
                color.rgb *= (1 + finalLightColor);
                
                // 应用总体不透明度
                color.a *= totalLight * _Opacity;
                
                return color;
            }
            ENDHLSL
        }
    }
} 
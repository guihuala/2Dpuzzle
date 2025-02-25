Shader "Custom/LightReveal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Opacity ("Overall Opacity", Range(0, 1)) = 1
        _Color ("Tint Color", Color) = (1,1,1,1)
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _SpotlightFalloff ("Spotlight Falloff", Range(1, 10)) = 2 // 控制聚光灯边缘过渡
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
                float _SpotlightFalloff;
                
                // 光源基本信息
                float4 _LightPositions[8];
                float _LightIntensities[8];
                float _LightRanges[8];
                int _LightCount;
                
                // 聚光灯特定信息
                int _LightTypes[8]; // 0=点光源, 1=聚光灯
                float4 _LightDirections[8]; // 聚光灯方向
                float _LightInnerAngles[8]; // 内角 (弧度)
                float _LightOuterAngles[8]; // 外角 (弧度)
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

            // 计算点光源亮度
            float CalculatePointLight(float2 position, float2 lightPos, float range, float intensity)
            {
                float distance = length(position - lightPos);
                float attenuation = saturate(1 - distance / range);
                attenuation = smoothstep(0, 1, attenuation);
                return attenuation * intensity;
            }

            // 计算聚光灯亮度
            float CalculateSpotlight(float2 position, float2 lightPos, float2 lightDir, 
                                    float innerAngle, float outerAngle, float range, float intensity)
            {
                float distance = length(position - lightPos);
                
                // 基础距离衰减
                float distanceAttenuation = saturate(1 - distance / range);
                
                // 如果距离衰减为0，提前返回
                if (distanceAttenuation <= 0)
                    return 0;
                
                // 计算当前位置到光源的方向
                float2 toFragment = normalize(position - lightPos);
                
                // 计算点积 (越接近1，表示越接近光照中心)
                float cosAngle = dot(lightDir, toFragment);
                
                // 将内外角的余弦值计算出来
                float cosOuter = cos(outerAngle * 0.5);
                float cosInner = cos(innerAngle * 0.5);
                
                // 计算角度衰减
                float angleAttenuation = smoothstep(cosOuter, cosInner, cosAngle);
                
                // 应用非线性衰减使边缘更加锐利
                angleAttenuation = pow(angleAttenuation, _SpotlightFalloff);
                
                // 综合距离衰减和角度衰减
                return distanceAttenuation * angleAttenuation * intensity;
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
                    float range = _LightRanges[i];
                    float intensity = _LightIntensities[i];
                    float lightContribution = 0;
                    
                    // 根据光源类型选择不同的计算方法
                    if (_LightTypes[i] == 0) // 点光源
                    {
                        lightContribution = CalculatePointLight(IN.positionOS.xy, lightPos, range, intensity);
                    }
                    else if (_LightTypes[i] == 1) // 聚光灯
                    {
                        float2 lightDir = _LightDirections[i].xy;
                        float innerAngle = _LightInnerAngles[i];
                        float outerAngle = _LightOuterAngles[i];
                        
                        lightContribution = CalculateSpotlight(IN.positionOS.xy, lightPos, lightDir, 
                                                            innerAngle, outerAngle, range, intensity);
                    }
                    
                    totalLight += lightContribution;
                    finalLightColor += _LightColor.rgb * lightContribution;
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
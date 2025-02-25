Shader "Custom/LightReveal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Opacity ("Overall Opacity", Range(0, 1)) = 1
        _Color ("Tint Color", Color) = (1,1,1,1)
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _SpotlightFalloff ("Spotlight Falloff", Range(1, 10)) = 2 // 控制聚光灯边缘过渡
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.7 // 阴影强度
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
                float _ShadowIntensity;
                
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
                
                // 阴影投射器信息
                float4 _ShadowCasterPositions[16];
                int _ShadowCasterCount;
                float4 _ShadowCasterCorners[64]; // 16个阴影投射器 * 每个4个角点
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

            // 检查点是否在多边形阴影中
            bool IsInPolygonShadow(float2 position, float2 lightPos, float4 corners[4])
            {
                // 计算光源到当前片段的方向和距离
                float2 lightToPos = normalize(position - lightPos);
                float distToPos = length(position - lightPos);
                
                // 首先检查光源是否在多边形内部
                bool lightInsidePolygon = true; // 假设光源在内部，然后尝试证明它在外部
                for (int i = 0; i < 4; i++)
                {
                    float2 v1 = corners[i].xy;
                    float2 v2 = corners[(i + 1) % 4].xy;
                    float2 edge = v2 - v1;
                    float2 normal = float2(-edge.y, edge.x); // 边的法线
                    normal = normalize(normal);
                    
                    // 如果光源在任何一条边的外侧，则光源不在多边形内部
                    if (dot(lightPos - v1, normal) > 0)
                    {
                        lightInsidePolygon = false;
                        break;
                    }
                }
                
                // 如果光源在多边形内部，表示当前点在阴影中
                if (lightInsidePolygon)
                {
                    return true;
                }
                
                // 检查光线是否与多边形的任意边相交
                const float EPSILON = 0.0001; // 数值精度容差
                
                for (int i = 0; i < 4; i++)
                {
                    float2 v1 = corners[i].xy;
                    float2 v2 = corners[(i + 1) % 4].xy;
                    
                    // 计算光源到顶点的向量和距离
                    float2 lightToV1 = v1 - lightPos;
                    float distLightToV1 = length(lightToV1);
                    
                    float2 lightToV2 = v2 - lightPos;
                    float distLightToV2 = length(lightToV2);
                    
                    // 如果两个顶点都在光源后面或比当前片段更远，跳过这条边
                    if ((distLightToV1 > distToPos && distLightToV2 > distToPos))
                        continue;
                    
                    // 计算光线与边的交点
                    float2 edge = v2 - v1;
                    float2 normal = float2(-edge.y, edge.x); // 边的法线
                    normal = normalize(normal);
                    
                    // 如果光线与法线几乎平行，则不相交
                    float d = dot(lightToPos, normal);
                    if (abs(d) < EPSILON)
                        continue;
                    
                    // 计算交点参数
                    float2 v1ToLightPos = v1 - lightPos; // 修正向量方向
                    float t1 = dot(v1ToLightPos, normal) / d;
                    
                    // 如果交点在光线的正方向上且在片段之前
                    if (t1 > 0 && t1 < distToPos)
                    {
                        // 计算交点
                        float2 intersection = lightPos + t1 * lightToPos;
                        
                        // 检查交点是否在边上
                        float2 v1ToIntersection = intersection - v1;
                        float dotProduct = dot(v1ToIntersection, edge);
                        float edgeLengthSq = dot(edge, edge);
                        
                        // 使用容差进行边界检查
                        if (dotProduct >= -EPSILON && dotProduct <= edgeLengthSq + EPSILON)
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }

            // 计算阴影衰减
            float CalculateShadowAttenuation(float2 position, float2 lightPos)
            {
                float shadowAttenuation = 1.0;
                
                // 检查所有阴影投射器
                for (int i = 0; i < _ShadowCasterCount; i++)
                {
                    // 使用精确形状计算阴影
                    float4 corners[4];
                    for (int j = 0; j < 4; j++)
                    {
                        corners[j] = _ShadowCasterCorners[i * 4 + j];
                    }
                    
                    bool inShadow = IsInPolygonShadow(position, lightPos, corners);
                    
                    if (inShadow)
                    {
                        // 应用阴影强度
                        shadowAttenuation *= (1.0 - _ShadowIntensity);
                    }
                }
                
                return shadowAttenuation;
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
                    
                    // 应用阴影衰减
                    if (_ShadowCasterCount > 0)
                    {
                        float shadowAttenuation = CalculateShadowAttenuation(IN.positionOS.xy, lightPos);
                        lightContribution *= shadowAttenuation;
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
Shader "Custom/LightReveal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 1
        _BlurAmount ("Blur Amount", Range(0, 1)) = 0.1
        _Color ("Tint", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_LIGHTS 4

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _Opacity;
            float _BlurAmount;
            fixed4 _Color;

            // 光源数据
            float4 _LightPositions[MAX_LIGHTS];
            float _LightIntensities[MAX_LIGHTS];
            float _LightRanges[MAX_LIGHTS];
            int _LightCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                return o;
            }

            float CalculateLightInfluence(float2 worldPos, float2 lightPos, float range, float intensity)
            {
                float dist = length(worldPos - lightPos);
                float normalizedDist = saturate(dist / range);
                return lerp(intensity, 0, normalizedDist);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样周围9个点进行模糊
                float2 blur = _MainTex_TexelSize.xy * _BlurAmount * 30;
                
                fixed4 col = tex2D(_MainTex, i.uv) * 0.2;
                col += tex2D(_MainTex, i.uv + float2(-blur.x, -blur.y)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(-blur.x, 0)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(-blur.x, blur.y)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(0, -blur.y)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(0, blur.y)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(blur.x, -blur.y)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(blur.x, 0)) * 0.1;
                col += tex2D(_MainTex, i.uv + float2(blur.x, blur.y)) * 0.1;

                col *= _Color;

                // 计算所有光源的影响
                float totalLight = 0;
                for (int idx = 0; idx < _LightCount; idx++)
                {
                    float2 lightPos = _LightPositions[idx].xy;
                    float range = _LightRanges[idx];
                    float intensity = _LightIntensities[idx];
                    
                    float lightInfluence = CalculateLightInfluence(i.worldPos.xy, lightPos, range, intensity);
                    totalLight = max(totalLight, lightInfluence);
                }

                // 应用光照和边缘虚化
                float edgeFade = smoothstep(1.0, 1.0 - _BlurAmount, length(i.uv - 0.5) * 2);
                col.a *= totalLight * edgeFade * _Opacity;

                return col;
            }
            ENDCG
        }
    }
} 
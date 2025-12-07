Shader "Universal Render Pipeline/2D/OldTVNoise_ScreenSpace"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)

        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.4   // 노이즈 강도
        _NoiseScale ("Noise Scale", Range(1,3000)) = 800        // 도트 크기 (화면 기준)
        _NoiseSpeed ("Noise Speed", Range(0,50)) = 10           // 패턴 갱신 속도
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "UniversalMaterialType"="SpriteUnlit"
            "CanUseSpriteAtlas"="True"
        }

        ZWrite Off
        Cull Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Forward"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Tint;
                float  _NoiseIntensity;
                float  _NoiseScale;
                float  _NoiseSpeed;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float2 screenUV    : TEXCOORD1;   // 화면 기준 좌표
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float4 pos = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionHCS = pos;
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color       = IN.color * _Tint;

                // NDC(-1~1) -> 0~1 스크린 UV
                float2 ndc = pos.xy / pos.w;
                OUT.screenUV = ndc * 0.5f + 0.5f;

                return OUT;
            }

            // 간단한 2D 해시 함수 (의사 랜덤)
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // 스프라이트 기본 색
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                // 🔹 화면 기준 도트 노이즈 좌표
                float2 noisePos = IN.screenUV * _NoiseScale;

                // 도트 그리드
                float2 grid1 = floor(noisePos);
                float2 grid2 = floor(noisePos * 0.73);

                // 시간은 랜덤 시드로만 사용 (스크롤 X)
                float t = _Time.y * _NoiseSpeed;

                float n1 = hash21(grid1 + float2(t, t * 7.123));
                float n2 = hash21(grid2 - float2(t * 3.1, t * 1.7));

                float nMix = (n1 + n2 * 0.7) / 1.7; // 대략 0~1
                float noise = (nMix - 0.5) * 2.0;   // -1~1

                col.rgb += noise * _NoiseIntensity;
                col.rgb = saturate(col.rgb);
                return col;
            }
            ENDHLSL
        }
    }
}

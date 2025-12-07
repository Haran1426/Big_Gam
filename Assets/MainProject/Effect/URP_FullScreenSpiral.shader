Shader "Custom/URP_FullScreenSpiral_Switch"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Blit Texture", 2D) = "white" {}
        
        // [ON/OFF 스위치] 0 = 끔, 1 = 켬
        [Toggle] _IsActive ("Is Active", Float) = 1.0
        
        _TwistStrength ("Twist Strength", Float) = 10.0
        _Speed ("Rotation Speed", Float) = 3.0
        _CenterX ("Center X (0-1)", Float) = 0.5
        _CenterY ("Center Y (0-1)", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "FullScreenSpiralPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl" 

            struct Attributes
            {
                uint vertexID : SV_VertexID;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            CBUFFER_START(UnityPerMaterial)
                float _IsActive; // 스위치 변수
                float _TwistStrength;
                float _Speed;
                float _CenterX;
                float _CenterY;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);
                output.positionCS = pos;
                output.uv = uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                // [최적화] 스위치가 꺼져(0) 있으면 복잡한 계산 없이 원본 화면만 반환
                if (_IsActive == 0.0)
                {
                    return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv);
                }

                // --- 나선형 왜곡(Distortion) 로직 ---
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 center = float2(_CenterX, _CenterY);

                // 1. 중심점 기준 이동
                float2 dc = uv - center;
                dc.x *= aspect; 

                // 2. 극좌표계 변환 (소용돌이 효과의 핵심)
                float dist = length(dc);
                float angle = atan2(dc.y, dc.x); 

                // 3. 거리에 따른 각도 비틀기 + 시간 회전
                angle += dist * _TwistStrength - _Time.y * _Speed;

                // 4. 다시 좌표 복구
                float2 twistedUV;
                sincos(angle, twistedUV.y, twistedUV.x);
                twistedUV *= dist;
                twistedUV.x /= aspect;
                float2 finalUV = twistedUV + center;
                
                // -------------------------------

                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, finalUV);
                return col;
            }
            ENDHLSL
        }
    }
}
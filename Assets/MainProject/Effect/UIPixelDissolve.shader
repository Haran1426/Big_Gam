Shader "UI/PixelDissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveColor ("Edge Color", Color) = (1, 0.2, 0, 1) // 부서지는 경계선 색
        _Direction ("Wind Direction", Vector) = (1, 1, 0, 0) // 날리는 방향
        
        // UI Masking support
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 noiseUV  : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            fixed4 _Color;
            float _DissolveAmount;
            fixed4 _DissolveColor;
            float4 _Direction;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                
                // Dissolve 진행 시 바람에 날리는 효과 (Vertex 이동)
                float moveFactor = _DissolveAmount * _DissolveAmount * 20.0; // 흩날리는 거리
                v.vertex.xy += _Direction.xy * moveFactor * (sin(v.vertex.x * 10) + 1);

                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.noiseUV = TRANSFORM_TEX(v.texcoord, _NoiseTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                fixed4 noise = tex2D(_NoiseTex, IN.noiseUV);

                // 노이즈 값을 기준으로 잘라내기
                float cut = noise.r - _DissolveAmount;

                if (cut < 0)
                {
                    discard; // 픽셀 제거 (모래처럼 사라짐)
                }

                // 경계선에 색상 입히기 (타오르는 느낌)
                if (cut < 0.1 && _DissolveAmount > 0)
                {
                    color += _DissolveColor;
                }

                // 알파값 페이드
                color.a *= (1.0 - _DissolveAmount);

                return color;
            }
            ENDCG
        }
    }
}
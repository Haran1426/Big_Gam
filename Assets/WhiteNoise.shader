Shader "Custom/OldTVNoise2D"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)

        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.4   // 노이즈 강도
        _NoiseScale ("Noise Scale", Range(1,500)) = 80          // 노이즈 입자 크기
        _NoiseSpeed ("Noise Speed", Range(0,10)) = 3            // 노이즈 움직임 속도
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;

            float _NoiseIntensity;
            float _NoiseScale;
            float _NoiseSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos    : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Tint;
                return o;
            }

            // 간단한 2D 해시 함수 (의사 랜덤)
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 노이즈용 좌표 (x는 UV, y는 시간 기반으로 변함)
                float2 noiseUV = float2(i.uv.x * _NoiseScale, _Time.y * _NoiseSpeed);

                // 0~1 랜덤 값
                float n = hash21(noiseUV);

                // -1 ~ 1 범위로 변환
                float noise = (n - 0.5) * 2.0;

                // 밝기에 노이즈 적용
                col.rgb += noise * _NoiseIntensity;

                // 색상 범위 유지
                col.rgb = saturate(col.rgb);

                return col;
            }

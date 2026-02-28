Shader "Custom/SwirlFunnel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength ("Swirl Strength", Float) = 5
        _Speed ("Rotation Speed", Float) = 1
        _Radius ("Effect Radius", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Strength;
            float _Speed;
            float _Radius;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 uv = i.uv - center;

                float dist = length(uv);
                
                // Ограничение радиуса эффекта
                float mask = saturate(1 - dist / _Radius);

                // Угол закрутки
                float angle = _Strength * mask * dist + _Time.y * _Speed;

                float s = sin(angle);
                float c = cos(angle);

                float2 rotatedUV;
                rotatedUV.x = uv.x * c - uv.y * s;
                rotatedUV.y = uv.x * s + uv.y * c;

                rotatedUV += center;

                return tex2D(_MainTex, rotatedUV);
            }
            ENDCG
        }
    }
}
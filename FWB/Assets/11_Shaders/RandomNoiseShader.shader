Shader "Custom/RandomNoiseShader"
{
     Properties
    {
        _Transparency ("Transparency", Range(0,1)) = 0.2
        _UseRandomColor ("Use Random Color", Float) = 0.0
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        _CustomTime ("Custom Time", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Transparency;
            float _UseRandomColor;
            fixed4 _Color;
            float _CustomTime;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float random(float2 p)
            {
                return frac(sin(dot(p.xy + (_CustomTime), float2(12.9898, 78.233))) * 43758.5453); //float2 내부값은 시드, 뒤의 값은 스케일링 인자
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noiseValue = random(i.uv);
                fixed4 noiseColor;

                if (_UseRandomColor > 0.5)
                {
                    noiseColor = fixed4(random(i.uv + float2(0.1, 0.2)), // R
                                        random(i.uv + float2(0.3, 0.4)), // G
                                        random(i.uv + float2(0.5, 0.6)), // B
                                        1.0); // A (투명도는 따로 적용)
                }
                else
                {
                    noiseColor = fixed4(noiseValue, noiseValue, noiseValue, 1.0);
                }
                noiseColor.a *= _Transparency;
                return noiseColor * _Color;
            }
            ENDCG
        }
    }
}

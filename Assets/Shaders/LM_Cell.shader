Shader "Tactical/LM_Cell"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0.2, 0.2, 0.3, 1)
        _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 1)

        [HideInInspector]_Highlighted("Highlighted", Float) = 0
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

            float4 _MainColor;
            float4 _HighlightColor;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Highlighted)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(_HighlightColor, _MainColor, _Highlighted);
            }
            ENDCG
        }
    }
}

Shader "Skybox/Gradient"
{
    Properties
    {
        _TopColor    ("Top Color",    Color) = (0.02, 0.02, 0.02, 1)
        _HorizonColor("Horizon Color",Color) = (1.0,  0.35, 0.0,  1)
        _BottomColor ("Bottom Color", Color) = (0.05, 0.02, 0.0,  1)

        _HorizonHeight   ("Horizon Height",    Range(-1, 1)) = 0.0
        _HorizonSharpness("Horizon Sharpness", Range(0.1, 10)) = 2.5
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TopColor;
            fixed4 _HorizonColor;
            fixed4 _BottomColor;
            float  _HorizonHeight;
            float  _HorizonSharpness;

            struct appdata
            {
                float4 vertex   : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.worldDir = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Normalised vertical position: -1 (bottom) to +1 (top).
                float t = normalize(i.worldDir).y - _HorizonHeight;

                // Upper half: horizon → top
                float upperBlend = pow(saturate( t), _HorizonSharpness);
                // Lower half: horizon → bottom
                float lowerBlend = pow(saturate(-t), _HorizonSharpness);

                fixed4 col = lerp(_HorizonColor, _TopColor,    upperBlend);
                col        = lerp(col,           _BottomColor, lowerBlend);

                return col;
            }
            ENDCG
        }
    }

    Fallback Off
}

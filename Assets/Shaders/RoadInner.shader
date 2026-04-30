Shader "Custom/RoadInner"
{
    Properties
    {
        _Color              ("Color",           Color)              = (1,1,1,1)
        _MainTex            ("Albedo (RGB)",    2D)                 = "white" {}
        _Glossiness         ("Smoothness",      Range(0,1))         = 0.5
        _GlossMapScale      ("Smoothness Scale",Range(0,1))         = 1.0
        _Metallic           ("Metallic",        Range(0,1))         = 0.0
        [Normal] _BumpMap   ("Normal Map",      2D)                 = "bump" {}
        _BumpScale          ("Normal Scale",    Float)              = 1.0
        _OcclusionMap       ("Occlusion",       2D)                 = "white" {}
        _OcclusionStrength  ("Occlusion Strength", Range(0,1))      = 1.0
        _MetallicGlossMap   ("Metallic Map",    2D)                 = "white" {}
        _ParallaxMap        ("Height Map",      2D)                 = "black" {}
        _Parallax           ("Height Scale",    Range(0,0.08))      = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }
        LOD 200

        // Pull depth toward camera — eliminates z-fighting with coplanar feather mesh
        Offset -1, -1

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _OcclusionMap;
        sampler2D _MetallicGlossMap;
        sampler2D _ParallaxMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        fixed4  _Color;
        half    _Glossiness;
        half    _GlossMapScale;
        half    _Metallic;
        float   _BumpScale;
        half    _OcclusionStrength;
        float   _Parallax;

        /// Apply parallax, sample all maps and fill the PBR output.
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;

            half h = tex2D(_ParallaxMap, uv).r;
            uv += ParallaxOffset(h, _Parallax, IN.viewDir);

            fixed4 c = tex2D(_MainTex, uv) * _Color;
            o.Albedo = c.rgb;

            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);

            fixed4 mg = tex2D(_MetallicGlossMap, uv);
            o.Metallic   = mg.r * _Metallic;
            o.Smoothness = mg.a * _Glossiness * _GlossMapScale;

            half occ = tex2D(_OcclusionMap, uv).g;
            o.Occlusion  = LerpOneTo(occ, _OcclusionStrength);

            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Standard"
}

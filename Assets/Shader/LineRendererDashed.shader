Shader "Custom/LineRendererDashed"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,0)
        _DashSize ("Dash Size", Float) = 0.5
        _DashSpeed ("Dash Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainColor;
            float4 _BackgroundColor;
            float _DashSize;
            float _DashSpeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y; // Use _Time.y for shader animation
                float dashPattern = fmod(i.uv.x * _DashSize + time * _DashSpeed, 1.0);
                float alpha = step(0.5, dashPattern); // Creates the dashed pattern
                return lerp(_MainColor, _BackgroundColor, alpha);
            }
            ENDCG
        }
    }
}

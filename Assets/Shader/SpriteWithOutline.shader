Shader "Custom/SpriteWithOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Float) = 0.1
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment OutlineSpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;

            fixed4 OutlineSpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

                // Outline calculation
                float2 texDdx = ddx(IN.texcoord);
                float2 texDdy = ddy(IN.texcoord);

                float2 uvOffset = float2(_OutlineWidth, _OutlineWidth);
                fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + float2(0, uvOffset.y));
                fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - float2(0, uvOffset.y));
                fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + float2(uvOffset.x, 0));
                fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - float2(uvOffset.x, 0));

                fixed outline = max(max(pixelUp.a, pixelDown.a), max(pixelRight.a, pixelLeft.a)) - c.a;
                c.rgb = lerp(c.rgb, _OutlineColor.rgb, outline * _OutlineColor.a);
                c.rgb *= c.a;

                return c;
            }
            ENDCG
        }
    }
}

Shader "UI/ClippedUnlitModel"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
    }

    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Cull Off
            Lighting Off
            Offset -1, -1
            Fog { Mode Off }
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _PanelOffsetAndSharpness;
            float _PanelSizeX, _PanelSizeY;

            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 posInPanel : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;

                float2 clipSpace =  o.vertex.xy / o.vertex.w;

                // Normalize clip space
                o.posInPanel = (clipSpace.xy + 1) * 0.5;

                // Adjust for panel offset
                o.posInPanel.x  -= _PanelOffsetAndSharpness.x;
                o.posInPanel.y  -= _PanelOffsetAndSharpness.y;

                // Adjust for panel size
                o.posInPanel.x  *= (1 / _PanelSizeX);
                o.posInPanel.y  *= (1 / _PanelSizeY);

                // Transform back to clip space
                o.posInPanel *= 2;
                o.posInPanel -= 1;

                return o;
            }

            half4 frag (v2f IN) : COLOR
            {
                // Softness factor
                float2 factor = (float2(1.0, 1.0) - abs(IN.posInPanel)) * _PanelOffsetAndSharpness.zw;

                // Sample the texture
                half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                col.a *= clamp( min(factor.x, factor.y), 0.0, 1.0);

                return col;
            }
            ENDCG
        }
    }
}

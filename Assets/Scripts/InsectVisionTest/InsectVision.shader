Shader "Custom/InsectVision"
{
    Properties
    {
        // Visible properties in Inspector
        _MainTex ("Base Texture", 2D) = "white" {}
        _HexSize ("Hexagon Density", Range(1,100)) = 30
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            sampler2D _MainTex;
            float _HexSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Hexagonal grid calculation
                float2 hexUV = i.uv * _HexSize;
                float2 hexIndex = floor(hexUV);
                float2 hexFrac = frac(hexUV);

                // Sample texture at hexagon center
                float2 centerUV = (hexIndex + 0.5) / _HexSize;
                fixed4 col = tex2D(_MainTex, centerUV);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
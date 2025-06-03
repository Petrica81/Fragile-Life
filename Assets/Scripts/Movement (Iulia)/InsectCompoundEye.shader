Shader "Custom/InsectVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HexSize ("Hexagon Size", Range(1,50)) = 20
        _MotionSensitivity ("Motion Sensitivity", Range(0,1)) = 0.3
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            float _HexSize;
            float _MotionSensitivity;

            v2f vert (appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag (v2f i) : SV_Target
            {
                // Hexagonal pixelation
                float2 hexUV = i.uv * _HexSize;
                float2 hexIndex = floor(hexUV);
                float2 hexCenter = (hexIndex + 0.5) / _HexSize;
                fixed4 col = tex2D(_MainTex, hexCenter);

                // Motion detection
                float2 motionUV = i.uv * 15.0;
                float motion = sin(_Time.y * 8.0 + motionUV.x + motionUV.y);
                col.rgb += motion * _MotionSensitivity * float3(1,0.3,0);

                // UV spectrum
                float uvResponse = dot(col.rgb, float3(0.2, 0.7, 0.1));
                col.rgb = lerp(col.rgb, col.rgb * uvResponse * 2.0, 0.6);

                // Edge darkening
                float2 centerVec = i.uv - 0.5;
                float edgeDist = length(centerVec) * 2.0;
                col.rgb *= 1.0 - smoothstep(0.8, 1.2, edgeDist);

                return col;
            }
            ENDCG
        }
    }
}
Shader "Custom/Sobel"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float Convolve(float3x3 kernel, float3x3 pixels, float denom, float offset)
            {
                float res = 0.0;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        res += kernel[2 - x][2 - y] * pixels[x][y];
                    }
                }

                return res;
            }

            float3x3 GetData(int channel, sampler2D tex, float2 uv, float4 size)
            {
                float3x3 mat;
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        mat[x + 1][y + 1] = tex2D(tex, uv + float2(x * size.x, y * size.y))[channel];
                    }
                }
                return mat;
            }

            fixed4 frag(v2f i) : SV_Target
            {

            float3x3 SobelKernelFilterXDir = float3x3 (
                -1.0, 0, 1.0,
                -2.0, 0, 2.0,
                -1.0, 0, 1.0);


            float3x3 SobelKernelFilterYDir = float3x3 (
                -1.0, -2.0, -1.0,
                0, 0, 0,
                1.0, 2.0, 1.0);

                float3x3 matr = GetData(0, _MainTex, i.uv, _MainTex_TexelSize);
                float3x3 matg = GetData(1, _MainTex, i.uv, _MainTex_TexelSize);
                float3x3 matb = GetData(2, _MainTex, i.uv, _MainTex_TexelSize);

                // kernel
                float4 ColXDir = float4(
                    Convolve(SobelKernelFilterXDir, matr, 1.0, 0.0),
                    Convolve(SobelKernelFilterXDir, matg, 1.0, 0.0),
                    Convolve(SobelKernelFilterXDir, matb, 1.0, 0.0),
                    1.0);

                float4 ColYDir = float4(
                    Convolve(SobelKernelFilterYDir, matr, 1.0, 0.0),
                    Convolve(SobelKernelFilterYDir, matg, 1.0, 0.0),
                    Convolve(SobelKernelFilterYDir, matb, 1.0, 0.0),
                    1.0);


                float4 gl_FragColor = float4(
                    sqrt(ColXDir.r * ColXDir.r + ColYDir.r * ColYDir.r),
                    sqrt(ColXDir.g * ColXDir.g + ColYDir.g * ColYDir.g),
                    sqrt(ColXDir.b * ColXDir.b + ColYDir.b * ColYDir.b),
                    1.0
                    );
 /*               "Cropp" Image*/
                if (i.uv.x < 0.2f || i.uv.x > 0.8f) {
                    gl_FragColor = float4(0, 0, 0, 1);
                }

                if (i.uv.y < 0.2f || i.uv.y > 0.8f) {
                    gl_FragColor = float4(0, 0, 0, 1);
                }
                    return gl_FragColor;
                }
                ENDCG
            }
    }
}
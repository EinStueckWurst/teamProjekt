Shader "Custom/GaussianFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4x4 _KernelGaussianFilter;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float Convolve(float4x4 kernel, float4x4 pixels, float denom, float offset)
            {
                float res = 0.0;
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        res += kernel[3-x][3-y] * pixels[x][y];
                    }
                }

                return res;
            }

            float4x4 GetData(int channel, sampler2D tex, float2 uv, float4 size)
            {
                float4x4 mat;
                for (int y = -1; y < 3; y++)
                {
                    for (int x = -1; x < 3; x++)
                    {
                        mat[x + 1][y + 1] = tex2D(tex, uv + float2(x * size.x, y * size.y))[channel];
                    }
                }
                return mat;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //Get PixelData of Size 4x4 for convolution       
                float4x4 matr = GetData(0, _MainTex, i.uv, _MainTex_TexelSize);           
                float4x4 matg = GetData(1, _MainTex, i.uv, _MainTex_TexelSize);            
                float4x4 matb = GetData(2, _MainTex, i.uv, _MainTex_TexelSize);         

                //Convolve with GaussanKernel
                float4 gl_FragColor = float4(   
                    Convolve(_KernelGaussianFilter,matr,1.0,0.0),
                    Convolve(_KernelGaussianFilter,matg,1.0,0.0),
                    Convolve(_KernelGaussianFilter,matb,1.0,0.0),
                    1.0);

        
            return gl_FragColor;
            }
            ENDCG
        }
    }
}

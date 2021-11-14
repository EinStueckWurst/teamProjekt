Shader "Custom/ShowDetectedCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterA("_CenterA", float) = 0
        _CenterB("_CenterB", float) = 0
        _Radius("_Radius", float) = 1
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
                float4 scrPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.scrPos = ComputeScreenPos(v.vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Radius;
            float _CenterA;
            float _CenterB;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                    
                //Draw current fragment if the center is the calculated hough-transform-circle
                for (int theta = 0; theta <= 360; theta++) {
                    int x = ((i.uv.x * _MainTex_TexelSize.z) + _Radius * cos(theta));
                    int y = ((i.uv.y * _MainTex_TexelSize.w) + _Radius * sin(theta));

                    if ((int)_CenterA == x && (int)_CenterB == y) {
                        return float4(1, 0, 0, 1);
                    }
                }
                return col;
            }
            ENDCG
        }
    }
}

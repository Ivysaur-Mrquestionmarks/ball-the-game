Shader "Unlit/wave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Color("Color", Color) = (1,0,0,1)
        _Speed("Speed", Float) = 1.0
        _Amplitud("Amplitud", Float) = 1.0
    }
    SubShader
    {
        Tags {  "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float  _Amplitud, _Speed;
            float4 _Color;

           // v2f vert (appdata v, appdata_full vertexData)
            v2f vert(appdata v)
            {
                float3 p = v.vertex.xyz;
                //float k = (UNITY_PI / _Magnitud);
                //float f = k * (p.x + _Time.y * _Speed);
                //p.y = sin((2*UNITY_PI / _Magnitud) * (p.x + _Time.y * _Speed))* _Amplitud;
                p.y = sin(p.x + _Time.y * _Speed) * _Amplitud;
                v.vertex.xyz = p;
               // float3 tangent = normalize(float3(1, k * _Amplitud * cos(f), 0));
               // V.normal = float3(-tangent.y, tangent.x, 0);
                

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv)*_Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

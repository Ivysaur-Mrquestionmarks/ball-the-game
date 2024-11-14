Shader "Unlit/rainbowWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Spread("Spread", Range (0.5, 10.0)) = 1
        _Speed("Speed", Range(-10.0, 10.0)) = 1
        _Offset("Time Offset", Range(0, 7.0)) = 0
        _Saturation("Saturation", Range(0, 10.0)) = 0.5
        _Luminosity("Luminosity", Range(0, 10.0)) = 0.5
        _Transparency("Transparency", Range(0, 1.0)) = 1.0
        _Amplitud("Amplitud", Float) = 1.0
        _Speed2("Speed", Float) = 1.0
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
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Shared/ShaderTools.cginc"

            /*  struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

          struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };*/

            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };

            struct fragmentinput {
                float4 position : SV_POSITION;
                float4 texcoord0 : TEXCOORD0;
                fixed3 localPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _Speed;
            half _Spread;
            half _Offset;
            fixed _Saturation;
            fixed _Luminosity;
            fixed _Transparency;

            float  _Amplitud, _Speed2;

            fragmentinput vert (vertexInput v)
            {
                float3 p = v.vertex.xyz;
                p.y = sin(p.x + _Time.y * _Speed) * _Amplitud;
                v.vertex.xyz = p;
                fragmentinput o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.texcoord0;
                o.localPosition = v.vertex.xyz + fixed3(0.5,0.5,0.5);
                return o;
            }

            fixed4 frag(fragmentinput i) : SV_Target
            {
                fixed2 lPos = i.localPosition / _Spread;
                half time_ = _Time.y * _Speed2 / _Spread;
                half offsetTime = time_ + _Offset;
                fixed hue = (-lPos.x) / 2.0 + time_;
                //hue = clamp(0.0,hue,1.0);
                while (hue < 0.0) hue += 1.0;
                while (hue > 1.0) hue -= 1.0;
                fixed4 hsl = fixed4(hue,_Saturation,_Luminosity, _Transparency);
                return HSLtoRGB(hsl);
                //return (hsl);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

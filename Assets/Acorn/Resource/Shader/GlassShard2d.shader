Shader "Custom/2DGlassShard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(1.0, 3.0)) = 1.5
        _EdgeGlow ("Edge Glow", Range(0.0, 1.0)) = 0.3
        _EdgeColor ("Edge Color", Color) = (0.8,0.9,1.0,1)
        _Refraction ("Refraction", Range(1.0, 1.5)) = 1.2
    }
    
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            float _Brightness;
            float _EdgeGlow;
            fixed4 _EdgeColor;
            float _Refraction;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // 采样纹理
                fixed4 texColor = tex2D(_MainTex, IN.uv);
                
                // 应用折射效果 - 提亮颜色
                texColor.rgb *= _Refraction;
                
                // 应用亮度
                texColor.rgb *= _Brightness;
                
                // 边缘发光效果
                float edge = (1.0 - saturate(length(IN.uv - float2(0.5, 0.5)) * 2.0));
                fixed4 edgeColor = _EdgeColor * _EdgeGlow * edge * edge * 2.0;
                
                // 组合颜色
                fixed4 finalColor = texColor * IN.color + edgeColor;
                
                // 保持原始透明度
                finalColor.a = texColor.a * IN.color.a;
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Sprites/Default"
}
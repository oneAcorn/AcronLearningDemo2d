Shader "Custom/GlassShard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Refraction ("Refraction", Range(1.0, 1.5)) = 1.2
        _Reflection ("Reflection", Range(0.0, 1.0)) = 0.3
        _Specular ("Specular", Range(0.0, 10.0)) = 5.0
        _Gloss ("Gloss", Range(0.0, 1.0)) = 0.5
        _Fresnel ("Fresnel", Range(0.0, 5.0)) = 2.0
        _EdgeThickness ("Edge Thickness", Range(0.0, 0.1)) = 0.02
        _EdgeColor ("Edge Color", Color) = (0.8,0.9,1.0,1)
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
        // 第一个Pass：渲染玻璃内部
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Refraction;
            float _Reflection;
            float _Specular;
            float _Gloss;
            float _Fresnel;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 采样纹理
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // 折射效果 - 提亮颜色
                col.rgb *= _Refraction;
                
                // 计算光照
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                
                // 漫反射
                float diff = max(0.0, dot(worldNormal, lightDir));
                float3 diffuse = _LightColor0.rgb * diff;
                
                // 镜面反射
                float3 halfDir = normalize(lightDir + i.viewDir);
                float spec = pow(max(0.0, dot(worldNormal, halfDir)), _Specular);
                float3 specular = _LightColor0.rgb * spec * _Gloss;
                
                // 菲涅尔反射
                float fresnel = pow(1.0 - max(0.0, dot(worldNormal, i.viewDir)), _Fresnel);
                float3 reflection = fresnel * _Reflection;
                
                // 组合所有光照效果
                col.rgb = col.rgb * (diffuse + reflection) + specular;
                
                // 确保颜色值不超过1
                col.rgb = min(col.rgb, fixed3(1,1,1));
                
                return col;
            }
            ENDCG
        }
        
        // 第二个Pass：渲染玻璃边缘
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            float _EdgeThickness;
            fixed4 _EdgeColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                // 沿法线方向膨胀顶点，创建边缘效果
                float3 normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                float4 pos = v.vertex + float4(normal * _EdgeThickness, 0);
                o.vertex = UnityObjectToClipPos(pos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return _EdgeColor;
            }
            ENDCG
        }
    }
    
    FallBack "Transparent/Diffuse"
}
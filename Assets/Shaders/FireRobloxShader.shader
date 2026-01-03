Shader "Custom/RobloxFire"
{
    Properties
    {
        _MainTex ("Fire Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Range(0, 5)) = 2
        _Color ("Tint Color", Color) = (1,0.8,0.3,1)
        _Brightness ("Brightness", Range(1, 5)) = 2.5
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }
        
        Blend SrcAlpha One
        ZWrite Off
        Cull Off
        
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScrollSpeed;
            float4 _Color;
            float _Brightness;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // Billboard - always face camera
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 center = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - center);
                float3 up = float3(0, 1, 0);
                float3 right = normalize(cross(up, viewDir));
                up = cross(viewDir, right);
                
                float3 localPos = v.vertex.xyz;
                worldPos = center + right * localPos.x + up * localPos.y;
                
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                
                // Scroll UV upward
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.y += _Time.y * _ScrollSpeed;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color * _Brightness;
                return col;
            }
            ENDCG
        }
    }
}
Shader "Custom/ToonFireParticle"
{
    Properties
    {
        _MainTex ("Fire Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(0.5, 3)) = 1.5
        _Steps ("Color Steps", Range(2, 8)) = 3
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        
        Blend SrcAlpha One  // Additive blending for fire glow
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
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _Brightness;
            float _Steps;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Posterize/quantize colors for toon effect
                texColor.rgb = floor(texColor.rgb * _Steps) / _Steps;
                
                // Apply particle system color and tint
                fixed4 col = texColor * i.color * _TintColor;
                
                // Boost brightness
                col.rgb *= _Brightness;
                
                // Keep alpha for transparency
                col.a = texColor.a * i.color.a;
                
                return col;
            }
            ENDCG
        }
    }
}
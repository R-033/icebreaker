Shader "Custom/XZGrid"
{
    Properties
    {
        _GridStep ("Grid size", Float) = 10
        _GridWidth ("Grid width", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
       
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade
 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
 
 
        struct Input
        {
            float3 worldPos;
        };

        float _GridStep;
        float _GridWidth;
       
        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = float4(0,0,0,0);
           
            // grid overlay
            float2 pos = IN.worldPos.xz / _GridStep;
            float2 f  = abs(frac(pos));
            float2 df = fwidth(pos) * _GridWidth;
            float2 g = smoothstep(-df ,df , f);
            float grid = 1.0 - saturate(g.x * g.y);
            c.rgb = lerp(float3(0,0,0), float3(1,1,1), grid);
            c.a = grid;
            float dist = distance(float2(IN.worldPos.x, IN.worldPos.z), float2(_WorldSpaceCameraPos.x, _WorldSpaceCameraPos.z));
            dist = dist - 1;
            float f2 = 75 - 1;
            float p = dist / f2;
            float cl = lerp(0, 1, p);
            cl = min(1, cl);
            cl = max(0, cl);
            o.Albedo = c.rgb;
            o.Alpha = c.a - cl;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
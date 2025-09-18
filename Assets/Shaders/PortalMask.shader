Shader "Portals/PortalMask"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        //LOD 100

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "Mask"

            Stencil
            {
                Ref 1
                Pass replace
            }

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                //#pragma multi_compile_fog

                //#include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    //float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    //float2 uv : TEXCOORD0;
                    //UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                    float4 screenPos : TEXCOORD0;
                };

                //sampler2D _MainTex;
                //float4 _MainTex_ST;

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    //UNITY_TRANSFER_FOG(o,o.vertex);
                    o.screenPos = ComputeScreenPos(o.vertex);
                    return o;
                }

                uniform sampler2D _MainTex;

                float4 frag (v2f i) : SV_Target
                {
                    // sample the texture
                    //fixed4 col = tex2D(_MainTex, i.uv);
                    // apply fog
                    //UNITY_APPLY_FOG(i.fogCoord, col);
                    float2 uv = i.screenPos.xy / i.screenPos.w;
                    float4 col = tex2D(_MainTex, uv);
                    return col;
                }
            ENDHLSL
        }
    }
}

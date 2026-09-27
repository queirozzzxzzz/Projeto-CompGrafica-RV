Shader "Hidden/SonarEdge"
{
    Properties
    {
        _GreyColor ("Cor do Corpo (cinza)", Color) = (0.3, 0.3, 0.3, 1)
        _EdgeColor ("Cor da Aresta (branco)", Color) = (1, 1, 1, 1)
        _NormalThreshold ("Sensibilidade de Normal", Range(0.01, 1)) = 0.4
        _DepthThreshold ("Sensibilidade de Profundidade", Range(0.001, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "SonarEdgePass"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

            half4 _GreyColor;
            half4 _EdgeColor;
            float _NormalThreshold;
            float _DepthThreshold;

            // Enviados pelo SonarController.cs a cada frame:
            float3 _SonarOrigin;   // posição mundial do pulso
            float  _SonarRadius;   // alcance do pulso
            float  _SonarAmount;   // 0-1, quanto o pulso ainda está "vivo"

            float3 GetWorldPos(float2 uv, float rawDepth)
            {
                return ComputeWorldSpacePosition(uv, rawDepth, UNITY_MATRIX_I_VP);
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                half4 sceneColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // Distância fixa entre amostras vizinhas — arestas sempre nítidas,
                // sem borrão progressivo.
                float2 texel = _BlitTexture_TexelSize.xy;

                float rawDepth = SampleSceneDepth(uv);
                float3 normalC = SampleSceneNormals(uv);

                float3 normalR = SampleSceneNormals(uv + float2(texel.x, 0));
                float3 normalU = SampleSceneNormals(uv + float2(0, texel.y));

                float rawDepthR = SampleSceneDepth(uv + float2(texel.x, 0));
                float rawDepthU = SampleSceneDepth(uv + float2(0, texel.y));

                float normalDiff = distance(normalC, normalR) + distance(normalC, normalU);

                float depthC = LinearEyeDepth(rawDepth, _ZBufferParams);
                float depthR = LinearEyeDepth(rawDepthR, _ZBufferParams);
                float depthU = LinearEyeDepth(rawDepthU, _ZBufferParams);
                float depthDiff = abs(depthC - depthR) + abs(depthC - depthU);

                bool isEdge = (normalDiff > _NormalThreshold) || (depthDiff > _DepthThreshold * depthC);

                // Máscara de alcance: só revela o que está perto da origem do pulso.
                float3 worldPos = GetWorldPos(uv, rawDepth);
                float dist = distance(worldPos, _SonarOrigin);
                float mask = 1.0 - saturate(dist / max(_SonarRadius, 0.001));
                mask *= _SonarAmount;

                half4 result = sceneColor;
                result.rgb = lerp(result.rgb, _GreyColor.rgb, mask * 0.5);

                if (isEdge)
                    result.rgb = lerp(result.rgb, _EdgeColor.rgb, mask);

                return result;
            }
            ENDHLSL
        }
    }
}

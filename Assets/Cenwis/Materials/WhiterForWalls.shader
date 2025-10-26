Shader "Custom/TwoColorSplitYURP"
{
    Properties
    {
        _ColorA ("Color A (Bottom)", Color) = (1, 1, 1, 1) // Bia�y
        _ColorB ("Color B (Top)", Color) = (1, 0.937, 0.62, 1) // FFEF9E
        _HeightMin ("Height Min (Bottom Y)", Float) = 0.0 // Dolny limit osi Y
        _HeightMax ("Height Max (Top Y)", Float) = 1.0 // G�rny limit osi Y
        _SplitPercentage ("Split Percentage", Range(0, 1)) = 0.2 // Podzia� (20%)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Atrybuty wej�ciowe wierzcho�k�w
            struct Attributes
            {
                float4 positionOS : POSITION; // Pozycja w przestrzeni obiektu
            };

            // Dane przesy�ane do fragment shadera
            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // Pozycja w przestrzeni klipu
                float3 worldPos : TEXCOORD0;     // Pozycja w przestrzeni �wiata
            };

            // Parametry shadera
            float4 _ColorA;           // Pierwszy kolor (bia�y)
            float4 _ColorB;           // Drugi kolor (��ty)
            float _HeightMin;         // Dolna granica wysoko�ci
            float _HeightMax;         // G�rna granica wysoko�ci
            float _SplitPercentage;   // Procentowy podzia� kolor�w

            // Funkcja wierzcho�kowa
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Przekszta�cenie pozycji do przestrzeni klipu
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                // Przekszta�cenie pozycji do przestrzeni �wiata
                OUT.worldPos = TransformObjectToWorld(IN.positionOS).xyz;
                return OUT;
            }

            // Funkcja fragmentowa
            half4 frag(Varyings IN) : SV_Target
            {
                // Normalizacja wysoko�ci w osi Y na zakres 0-1
                float yNormalized = (IN.worldPos.y - _HeightMin) / (_HeightMax - _HeightMin);

                // Granica procentowa (np. 20% wysoko�ci)
                float splitThreshold = _SplitPercentage;

                // Wyb�r koloru na podstawie procentowego podzia�u
                float4 finalColor = (yNormalized <= splitThreshold) ? _ColorA : _ColorB;

                return finalColor;
            }
            ENDHLSL
        }
    }
}

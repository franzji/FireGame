using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGeneratorUtils {
    public static float[,] generateTerrainDataHeightmap(int resolution, float baseScaleFactor,
        int octaves, float octaveAmplitudeFactor, float octaveScaleFactor, float seed)
    {
        float[,] terrainHeights = new float[resolution, resolution];

        for (int x = 0; x < resolution; x++)
            for (int z = 0; z < resolution; z++)
            {
                float currAmplitude = 1;
                float currScale = 1;
                float noiseValue = 0;

                for (int octave = 0; octave < octaves; octave++)
                {
                    noiseValue += calculateTerrainHeight(x, z, resolution,
                        currAmplitude, baseScaleFactor * currScale, seed) * currAmplitude;
                    currAmplitude *= octaveAmplitudeFactor;
                    currScale *= octaveScaleFactor;
                }
                terrainHeights[x, z] = noiseValue;
            }

        normalizeArray(terrainHeights);
        return terrainHeights;
    }

    private static float calculateTerrainHeight(float x, float z, int resolution,
        float amplitude, float scale, float seed)
    {
        float xCoord = x * scale / (int)resolution + seed;
        float zCoord = z * scale / (int)resolution + seed;

        return Mathf.PerlinNoise(xCoord, zCoord);
    }

    private static void normalizeArray(float[,] ary)
    {
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        //Get the min and max values.
        for (int x = 0; x < ary.GetLength(0); x++)
            for (int y = 0; y < ary.GetLength(1); y++)
            {
                float currVal = ary[x, y];
                if (currVal < minValue)
                    minValue = currVal;
                if (currVal > maxValue)
                    maxValue = currVal;
            }

        //Normalize the array.
        for (int x = 0; x < ary.GetLength(0); x++)
            for (int y = 0; y < ary.GetLength(1); y++)
                ary[x, y] = Mathf.InverseLerp(minValue, maxValue, ary[x, y]);
    }

    public static float[,,] generateTerrainDataAlphaMap(TerrainData terrainData,
        float highFactor, float lowFactor, float normalFactor)
    {
        float[,,] alphaMap = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 4];
        // For each point on the alphamap...
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float[] alphaWeights = new float[4];

                float xNorm = x * 1.0f / (terrainData.alphamapWidth - 1);
                float yNorm = y * 1.0f / (terrainData.alphamapHeight - 1);

                float height = terrainData.GetInterpolatedHeight(yNorm, xNorm) / terrainData.size.y;
                Vector3 normalVector = terrainData.GetInterpolatedNormal(yNorm, xNorm);

                //Default texture
                alphaWeights[0] = .5f;

                //More weight at higher altitude
                if (height > highFactor)
                    alphaWeights[1] = (height - highFactor) * 3f / (1 - highFactor);

                //More weight at lower altitude
                alphaWeights[2] = lowFactor / Mathf.Pow(height, 2);

                //More weight based on height and the terrain normal z value
                alphaWeights[3] = normalFactor * height * normalVector.z;

                float alphaWeightsSum = alphaWeights[0] + alphaWeights[1] + alphaWeights[2];
                //Normalize weights and set alphaMap values
                for (int index = 0; index < alphaWeights.Length; index++)
                {
                    alphaWeights[index] /= alphaWeightsSum;
                    alphaMap[x, y, index] = alphaWeights[index];
                }
            }
        }
        return alphaMap;
    }
}

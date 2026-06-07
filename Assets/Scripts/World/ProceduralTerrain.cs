using UnityEngine;

public static class ProceduralTerrain
{
    public static void GenerateTerrain(WorldConfig config = null)
    {
        if (Terrain.activeTerrain != null) return;

        GameObject terrainGO = new GameObject("Procedural Terrain");
        Terrain terrain = terrainGO.AddComponent<Terrain>();
        TerrainCollider collider = terrainGO.AddComponent<TerrainCollider>();
        TerrainData terrainData = new TerrainData();

        int resolution = config != null ? config.TerrainResolution : 513;
        float maxHeight = config != null ? config.TerrainMaxHeight : 90f;
        Vector3 worldSize = config != null ? config.WorldSize : new Vector3(500, 300, 500);

        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(worldSize.x, maxHeight, worldSize.z);
        terrainGO.transform.position = new Vector3(-worldSize.x * 0.5f, 0f, -worldSize.z * 0.5f);

        float[,] heights = GeneratePerlinHeights(terrainData, config);
        terrainData.SetHeights(0, 0, heights);
        terrain.terrainData = terrainData;
        collider.terrainData = terrainData;

        Debug.Log("Terrain generated");
    }

    private static float[,] GeneratePerlinHeights(TerrainData td, WorldConfig config)
    {
        int width = td.heightmapResolution;
        int height = td.heightmapResolution;
        float[,] heights = new float[width, height];
        float baseFrequency = config != null ? config.BaseNoiseFrequency : 0.006f;
        int octaves = Mathf.Clamp(config != null ? config.NoiseOctaves : 4, 1, 8);
        float heightScale = Mathf.Clamp01(config != null ? config.TerrainHeightScale : 0.55f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float amplitude = 0.65f;
                float frequency = baseFrequency;
                float value = 0f;
                float totalAmplitude = 0f;

                for (int octave = 0; octave < octaves; octave++)
                {
                    value += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
                    totalAmplitude += amplitude;
                    amplitude *= 0.45f;
                    frequency *= 1.85f;
                }

                float normalized = totalAmplitude > 0f ? value / totalAmplitude : 0f;
                normalized = Mathf.SmoothStep(0f, 1f, normalized) * heightScale;
                heights[x, z] = Mathf.Clamp01(normalized);
            }
        }

        int smoothIterations = Mathf.Clamp(config != null ? config.TerrainSmoothIterations : 3, 0, 8);
        for (int i = 0; i < smoothIterations; i++)
            heights = SmoothHeights(heights);

        return heights;
    }

    private static float[,] SmoothHeights(float[,] source)
    {
        int width = source.GetLength(0);
        int height = source.GetLength(1);
        float[,] result = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float sum = 0f;
                int samples = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    int sx = Mathf.Clamp(x + dx, 0, width - 1);
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        int sz = Mathf.Clamp(z + dz, 0, height - 1);
                        sum += source[sx, sz];
                        samples++;
                    }
                }

                result[x, z] = sum / samples;
            }
        }

        return result;
    }
}

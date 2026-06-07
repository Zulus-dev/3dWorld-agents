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
        float maxHeight = config != null ? config.TerrainMaxHeight : 150f;
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
        float baseFrequency = config != null ? config.BaseNoiseFrequency : 0.01f;
        int octaves = Mathf.Clamp(config != null ? config.NoiseOctaves : 4, 1, 8);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float amplitude = 0.6f;
                float frequency = baseFrequency;
                float value = 0f;
                float totalAmplitude = 0f;

                for (int octave = 0; octave < octaves; octave++)
                {
                    value += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
                    totalAmplitude += amplitude;
                    amplitude *= 0.5f;
                    frequency *= 2f;
                }

                heights[x, z] = Mathf.Clamp01(value / totalAmplitude);
            }
        }
        return heights;
    }
}

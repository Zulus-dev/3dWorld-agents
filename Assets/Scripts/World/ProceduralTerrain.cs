using UnityEngine;

public static class ProceduralTerrain
{
    public static void GenerateTerrain()
    {
        GameObject terrainGO = new GameObject("Terrain");
        Terrain terrain = terrainGO.AddComponent<Terrain>();
        TerrainData terrainData = new TerrainData();
        terrain.terrainData = terrainData;

        terrainData.heightmapResolution = 513;
        terrainData.size = new Vector3(500, 150, 500);

        float[,] heights = GeneratePerlinHeights(terrainData);
        terrainData.SetHeights(0, 0, heights);

        // Добавить текстуры, деревья и т.д. по желанию
        Debug.Log("Terrain generated");
    }

    private static float[,] GeneratePerlinHeights(TerrainData td)
    {
        int width = td.heightmapResolution;
        int height = td.heightmapResolution;
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float nx = x * 0.01f;
                float nz = z * 0.01f;
                heights[x, z] = Mathf.PerlinNoise(nx, nz) * 0.6f +
                               Mathf.PerlinNoise(nx * 2, nz * 2) * 0.3f +
                               Mathf.PerlinNoise(nx * 4, nz * 4) * 0.1f;
            }
        }
        return heights;
    }
}
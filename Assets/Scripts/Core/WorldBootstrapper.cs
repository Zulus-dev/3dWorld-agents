using UnityEngine;

public static class WorldBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureWorldRoot()
    {
        if (Object.FindObjectOfType<WorldManager>() != null) return;

        GameObject root = new GameObject("ExternalWorldRoot");
        WorldManager manager = root.AddComponent<WorldManager>();
        manager.config = Resources.Load<WorldConfig>("WorldConfig");
        root.AddComponent<ResourceSpawner>();
    }
}

using UnityEditor;
using UnityEngine;

/// <summary>
/// One-shot editor tool that places SM_Rock_1 through SM_Rock_5 prefabs
/// around the outer edge zone, matching the coordinate convention of the
/// existing Cacti and Trees root objects (parent scale 0.333, children in
/// design-space units ~3x world units).
/// Run via Tools > CS4700 > Place Edge Rocks, then delete this script.
/// </summary>
public static class EdgeRockPlacer
{
    // ── Prefab paths ──────────────────────────────────────────────────────────
    private static readonly string[] RockPrefabPaths =
    {
        "Assets/GameReady3D/Stylized_Rocks_and Cactus/Built-in/Prefabs/SM_Rock_1.prefab",
        "Assets/GameReady3D/Stylized_Rocks_and Cactus/Built-in/Prefabs/SM_Rock_2.prefab",
        "Assets/GameReady3D/Stylized_Rocks_and Cactus/Built-in/Prefabs/SM_Rock_3.prefab",
        "Assets/GameReady3D/Stylized_Rocks_and Cactus/Built-in/Prefabs/SM_Rock_4.prefab",
        "Assets/GameReady3D/Stylized_Rocks_and Cactus/Built-in/Prefabs/SM_Rock_5.prefab",
    };

    // ── Coordinate convention (matches /Cacti and /Trees roots) ──────────────
    // Parent localScale = 0.333 → design-space unit = world unit / 0.333 ≈ 3×
    private const float ParentScale = 0.333f;

    // ── World-space boundary (matches PlayerBoundary) ─────────────────────────
    private static readonly Vector2 WorldCenter    = new Vector2(-8.43f, -11.44f); // X, Z
    private const float WorldHalfExtent            = 120f;   // hard boundary
    private const float WorldTownClearance         = 70f;    // inner exclusion radius (keep town open)
    private const float WorldGroundHalfExtent      = 457f;   // visual ground edge (275 * 0.333 * 5 ≈ visible limit)

    // ── Placement settings ────────────────────────────────────────────────────
    private const int   TotalRocks     = 150;
    private const int   Seed           = 42;
    private const float ScaleMin       = 3f;
    private const float ScaleMax       = 7f;

    // ── Cluster settings ──────────────────────────────────────────────────────
    private const int   ClusterCount   = 15;
    private const int   ClusterSize    = 3;    // rocks per cluster
    private const float ClusterSpread  = 50f;  // design-space spread within a cluster

    [MenuItem("Tools/CS4700/Place Edge Rocks")]
    public static void Place()
    {
        // Load prefabs
        var prefabs = new GameObject[RockPrefabPaths.Length];
        for (int i = 0; i < RockPrefabPaths.Length; i++)
        {
            prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(RockPrefabPaths[i]);
            if (prefabs[i] == null)
            {
                Debug.LogError($"[EdgeRockPlacer] Prefab not found: {RockPrefabPaths[i]}");
                return;
            }
        }

        // Create or reuse /Rocks root with same scale convention as /Cacti
        var rootGO = GameObject.Find("Rocks") ?? new GameObject("Rocks");
        Undo.RegisterCreatedObjectUndo(rootGO, "Place Edge Rocks");

        // Clear existing children
        for (int i = rootGO.transform.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(rootGO.transform.GetChild(i).gameObject);

        rootGO.transform.localScale    = new Vector3(ParentScale, ParentScale, ParentScale);
        rootGO.transform.localPosition = Vector3.zero;
        rootGO.transform.localRotation = Quaternion.identity;

        // Convert boundary values to design-space (divide by ParentScale)
        float dsCenter_X       = WorldCenter.x    / ParentScale;
        float dsCenter_Z       = WorldCenter.y    / ParentScale;
        float dsInnerRadius    = WorldTownClearance / ParentScale;
        float dsOuterRadius    = WorldGroundHalfExtent / ParentScale;
        float dsBoundaryInner  = (WorldHalfExtent - 10f) / ParentScale; // edge zone starts just inside boundary
        float dsBoundaryOuter  = WorldGroundHalfExtent   / ParentScale;

        var rng = new System.Random(Seed);
        int  placed  = 0;
        int  rockIdx = 0;

        // ── Clustered rocks ───────────────────────────────────────────────────
        for (int c = 0; c < ClusterCount && placed < TotalRocks; c++)
        {
            Vector2 origin = RandomEdgePoint(rng, dsCenter_X, dsCenter_Z,
                                             dsBoundaryInner, dsBoundaryOuter, dsInnerRadius);

            for (int k = 0; k < ClusterSize && placed < TotalRocks; k++)
            {
                float ox = (float)(rng.NextDouble() * 2 - 1) * ClusterSpread;
                float oz = (float)(rng.NextDouble() * 2 - 1) * ClusterSpread;
                float dx = origin.x + ox;
                float dz = origin.y + oz;

                if (!InEdgeZone(dx, dz, dsCenter_X, dsCenter_Z, dsBoundaryInner, dsBoundaryOuter, dsInnerRadius))
                    continue;

                PlaceRock(prefabs, rng, rootGO, ref rockIdx, ref placed, dx, dz);
            }
        }

        // ── Scattered rocks ───────────────────────────────────────────────────
        int attempts = 0;
        while (placed < TotalRocks && attempts < TotalRocks * 10)
        {
            attempts++;
            Vector2 pt = RandomEdgePoint(rng, dsCenter_X, dsCenter_Z,
                                         dsBoundaryInner, dsBoundaryOuter, dsInnerRadius);
            PlaceRock(prefabs, rng, rootGO, ref rockIdx, ref placed, pt.x, pt.y);
        }

        EditorUtility.SetDirty(rootGO);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log($"[EdgeRockPlacer] Placed {placed} rocks under /Rocks.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void PlaceRock(
        GameObject[] prefabs, System.Random rng,
        GameObject root, ref int rockIdx, ref int placed,
        float dx, float dz)
    {
        var prefab   = prefabs[rng.Next(prefabs.Length)];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(instance, "Place Edge Rock");

        float scale = ScaleMin + (float)rng.NextDouble() * (ScaleMax - ScaleMin);
        float rotY  = (float)rng.NextDouble() * 360f;

        instance.transform.SetParent(root.transform, false);
        instance.transform.localPosition = new Vector3(dx, 0f, dz);
        instance.transform.localRotation = Quaternion.Euler(0f, rotY, 0f);
        instance.transform.localScale    = new Vector3(scale, scale, scale);
        instance.name = $"Rock_{++rockIdx}";

        placed++;
    }

    /// <summary>Returns a random design-space point within the edge zone.</summary>
    private static Vector2 RandomEdgePoint(
        System.Random rng,
        float cx, float cz,
        float innerR, float outerR, float townR)
    {
        for (int i = 0; i < 50; i++)
        {
            float angle = (float)(rng.NextDouble() * Mathf.PI * 2);
            float dist  = innerR + (float)rng.NextDouble() * (outerR - innerR);
            float x     = cx + Mathf.Cos(angle) * dist;
            float z     = cz + Mathf.Sin(angle) * dist;

            if (InEdgeZone(x, z, cx, cz, innerR, outerR, townR))
                return new Vector2(x, z);
        }
        return new Vector2(cx + innerR, cz);
    }

    private static bool InEdgeZone(
        float x, float z,
        float cx, float cz,
        float innerR, float outerR, float townR)
    {
        float dx   = x - cx;
        float dz   = z - cz;
        float dist = Mathf.Sqrt(dx * dx + dz * dz);
        return dist >= townR && dist >= innerR && dist <= outerR;
    }
}

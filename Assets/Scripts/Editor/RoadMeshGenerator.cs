using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tool that generates organic, terrain-flush village road meshes from Catmull-Rom splines.
/// Roads use procedural strip meshes with per-vertex edge noise for irregular, natural-looking borders.
/// </summary>
[InitializeOnLoad]
public static class RoadMeshGenerator
{
    // ── Geometry constants ────────────────────────────────────────────────────
    private const string RoadsRootName   = "VillageRoads";
    private const float  GroundY         = 0.06f;   // small flush offset above terrain (prevents z-fighting)
    private const int    SplineSteps     = 50;       // samples per spline segment
    private const float  MainRoadWidth   = 13f;
    private const float  BranchWidth     = 7f;
    private const float  FeatherMult     = 1.65f;    // feather strip is wider than core road
    private const float  NoiseAmp        = 2.2f;     // lateral edge noise amplitude (world units)
    private const float  WidthVar        = 0.14f;    // +/- fraction width variation along road length
    private const float  TileLength      = 9f;       // UV tiling repeat length (world units)

    // ── Material paths ────────────────────────────────────────────────────────
    private const string MainMatPath    = "Assets/Materials/Roads/DirtRoadMain.mat";
    private const string FeatherMatPath = "Assets/Materials/Roads/DirtPathFeather.mat";
    private const string MeshFolder     = "Assets/Materials/Roads/Meshes";

    // ── Auto-build on domain reload if VillageRoads is empty ─────────────────
    private static bool _pendingBuild = false;

    static RoadMeshGenerator()
    {
        _pendingBuild = true;
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if (!_pendingBuild) return;
        _pendingBuild = false;
        EditorApplication.update -= OnEditorUpdate;
        AutoBuildIfNeeded();
    }

    private static void AutoBuildIfNeeded()
    {
        var root = GameObject.Find(RoadsRootName);
        if (root != null && root.transform.childCount > 0) return;  // already built
        Build();
    }

    // ── Menu item ─────────────────────────────────────────────────────────────
    [MenuItem("Tools/Village Roads/Rebuild Road System")]
    public static void Build()
    {
        // Load materials
        var roadMat    = AssetDatabase.LoadAssetAtPath<Material>(MainMatPath);
        var featherMat = AssetDatabase.LoadAssetAtPath<Material>(FeatherMatPath);
        if (roadMat == null || featherMat == null)
        {
            Debug.LogError("[RoadMeshGenerator] Road materials not found. " +
                           $"Expected at:\n  {MainMatPath}\n  {FeatherMatPath}");
            return;
        }

        // Ensure mesh save folder exists
        if (!AssetDatabase.IsValidFolder(MeshFolder))
            AssetDatabase.CreateFolder("Assets/Materials/Roads", "Meshes");

        // Destroy old root
        var oldRoot = GameObject.Find(RoadsRootName);
        if (oldRoot != null)
            Undo.DestroyObjectImmediate(oldRoot);

        // New root
        var root = new GameObject(RoadsRootName);
        Undo.RegisterCreatedObjectUndo(root, "Rebuild Village Roads");

        // ── Spline definitions ──────────────────────────────────────────────
        // All control points are in world-space XZ; Y is overridden to GroundY.
        // House positions (rounded for clarity):
        //   Basic_House      (-5,   0,  -82)
        //   Basic_House.001  (121,  0,  141)
        //   Basic_House.002  (-611, 0, -160)
        //   BasicHouse2.001  (-149, 0,  211)
        //   BasicHouse2      (-724, 0,  194)
        //   SpawnPoint       (0,    0,   -2)

        // Main road: spine through village from south to far west
        var mainSpline = MakePoints(
            new Vector3(  15f, 0f, -120f),   // south entry
            new Vector3(   8f, 0f,  -50f),   // near Basic_House
            new Vector3( -18f, 0f,   10f),   // past spawn
            new Vector3( -55f, 0f,   80f),   // mid bend
            new Vector3(-120f, 0f,  155f),   // approach BasicHouse2.001
            new Vector3(-200f, 0f,  215f),   // past BasicHouse2.001
            new Vector3(-330f, 0f,  220f),   // continue west
            new Vector3(-480f, 0f,  210f),   // long stretch
            new Vector3(-620f, 0f,  198f),   // near BasicHouse2 cluster
            new Vector3(-730f, 0f,  188f),   // near BasicHouse2
            new Vector3(-800f, 0f,  175f)    // road terminus
        );

        // Branch A: detour south to Basic_House
        var branchA = MakePoints(
            new Vector3(  -6f, 0f,  -28f),   // junction on main road
            new Vector3(  -5f, 0f,  -55f),   // curve down
            new Vector3(  -5f, 0f,  -82f)    // Basic_House door
        );

        // Branch B: eastward swing to Basic_House.001
        var branchB = MakePoints(
            new Vector3( -38f, 0f,   68f),   // junction on main road
            new Vector3(  25f, 0f,  100f),   // sweep east
            new Vector3(  90f, 0f,  130f),   // approach
            new Vector3( 121f, 0f,  141f)    // Basic_House.001 door
        );

        // Branch C: long southern arc to Basic_House.002
        var branchC = MakePoints(
            new Vector3(-310f, 0f,  218f),   // junction
            new Vector3(-400f, 0f,  120f),   // arc south
            new Vector3(-490f, 0f,   10f),   // continuing south
            new Vector3(-565f, 0f, -100f),   // approach
            new Vector3(-611f, 0f, -160f)    // Basic_House.002 door
        );

        // ── Build segments ──────────────────────────────────────────────────
        var mainGO = BuildSegment("MainRoad", mainSpline, MainRoadWidth, roadMat, featherMat);
        mainGO.transform.SetParent(root.transform, true);

        var branchGroup = new GameObject("Branches");
        branchGroup.transform.SetParent(root.transform, true);

        var branchDefs = new (string name, Vector3[] pts, float width)[]
        {
            ("Branch_BasicHouse",    branchA, BranchWidth),
            ("Branch_BasicHouse001", branchB, BranchWidth),
            ("Branch_BasicHouse002", branchC, BranchWidth),
        };

        foreach (var b in branchDefs)
        {
            var bGO = BuildSegment(b.name, b.pts, b.width, roadMat, featherMat);
            bGO.transform.SetParent(branchGroup.transform, true);
        }

        EditorUtility.SetDirty(root);
        AssetDatabase.SaveAssets();
        Debug.Log("[RoadMeshGenerator] Village road system built successfully.");
    }

    // ── Segment builder ───────────────────────────────────────────────────────

    /// <summary>
    /// Builds a road segment: samples a Catmull-Rom spline, generates a core strip mesh
    /// and a wider feather strip mesh, saves both as assets, and returns the parent GameObject.
    /// </summary>
    private static GameObject BuildSegment(
        string segName,
        Vector3[] controlPts,
        float baseWidth,
        Material roadMat,
        Material featherMat)
    {
        var parent = new GameObject(segName);
        var spine  = SampleCatmullRom(controlPts, SplineSteps);

        // Core road mesh
        var coreMesh = BuildStripMesh(spine, baseWidth, isFeather: false, seed: 0f);
        SaveMesh(ref coreMesh, $"{segName}_Core");
        AttachMesh(parent.transform, $"{segName}_Core", coreMesh, roadMat);

        // Feather blend mesh (wider, used with semi-transparent material)
        var featherMesh = BuildStripMesh(spine, baseWidth * FeatherMult, isFeather: true, seed: 100f);
        SaveMesh(ref featherMesh, $"{segName}_Feather");
        AttachMesh(parent.transform, $"{segName}_Feather", featherMesh, featherMat);

        return parent;
    }

    // ── Mesh generation ───────────────────────────────────────────────────────

    /// <summary>
    /// Generates a road strip mesh along the given spine with organic irregular edges.
    /// Edge noise creates the worn, uneven look of a natural dirt path.
    /// </summary>
    private static Mesh BuildStripMesh(
        List<Vector3> spine,
        float width,
        bool isFeather,
        float seed)
    {
        int count = spine.Count;
        var verts  = new Vector3[count * 2];
        var uvs    = new Vector2[count * 2];
        var tris   = new int[(count - 1) * 6];

        // Pre-compute cumulative lengths for UV tiling
        var lens = new float[count];
        lens[0] = 0f;
        for (int i = 1; i < count; i++)
            lens[i] = lens[i - 1] + Vector3.Distance(spine[i], spine[i - 1]);

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / Mathf.Max(1, count - 1);

            // Tangent along spline
            Vector3 tangent;
            if (i == 0)
                tangent = (spine[1] - spine[0]).normalized;
            else if (i == count - 1)
                tangent = (spine[count - 1] - spine[count - 2]).normalized;
            else
                tangent = (spine[i + 1] - spine[i - 1]).normalized;

            var right  = Vector3.Cross(Vector3.up, tangent).normalized;
            var center = new Vector3(spine[i].x, GroundY, spine[i].z);

            // Width variation: sinusoidal modulation + Perlin noise for worn patches
            float widthMod = 1f
                + Mathf.Sin(t * Mathf.PI * 4.1f + seed) * WidthVar
                + (Mathf.PerlinNoise(t * 6.5f, seed * 0.3f) - 0.5f) * WidthVar;
            float halfW = (width * widthMod) * 0.5f;

            // Independent per-side noise for irregular edge shape
            float noiseL = NoiseAmp * (Mathf.PerlinNoise(t * 9.1f + seed, 0.33f + seed * 0.1f) - 0.5f) * 2f;
            float noiseR = NoiseAmp * (Mathf.PerlinNoise(t * 9.1f + seed, 1.71f + seed * 0.1f) - 0.5f) * 2f;

            var leftPt  = new Vector3(
                (center - right * (halfW + noiseL)).x, GroundY,
                (center - right * (halfW + noiseL)).z);
            var rightPt = new Vector3(
                (center + right * (halfW + noiseR)).x, GroundY,
                (center + right * (halfW + noiseR)).z);

            int vi = i * 2;
            verts[vi]     = leftPt;
            verts[vi + 1] = rightPt;

            float v = lens[i] / TileLength;
            uvs[vi]     = new Vector2(0f, v);
            uvs[vi + 1] = new Vector2(1f, v);
        }

        // Triangle winding
        for (int i = 0; i < count - 1; i++)
        {
            int bl = i * 2, br = i * 2 + 1;
            int tl = (i + 1) * 2, tr = (i + 1) * 2 + 1;
            int ti = i * 6;
            tris[ti]     = bl; tris[ti + 1] = tl; tris[ti + 2] = br;
            tris[ti + 3] = br; tris[ti + 4] = tl; tris[ti + 5] = tr;
        }

        var mesh = new Mesh { name = "RoadStrip" };
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ── Catmull-Rom spline ────────────────────────────────────────────────────

    private static Vector3[] MakePoints(params Vector3[] pts) => pts;

    private static List<Vector3> SampleCatmullRom(Vector3[] pts, int stepsPerSeg)
    {
        var all = new List<Vector3>(pts);
        all.Insert(0, pts[0]);
        all.Add(pts[pts.Length - 1]);

        var result = new List<Vector3>();
        for (int i = 1; i < all.Count - 2; i++)
        {
            for (int s = 0; s < stepsPerSeg; s++)
            {
                float t = (float)s / stepsPerSeg;
                result.Add(CatmullRom(all[i - 1], all[i], all[i + 1], all[i + 2], t));
            }
        }
        result.Add(all[all.Count - 2]);
        return result;
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t, t3 = t2 * t;
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Saves the mesh as a project asset so it survives domain reloads and play-mode.
    /// Updates the ref to point to the saved asset copy.
    /// </summary>
    private static void SaveMesh(ref Mesh mesh, string assetName)
    {
        string path     = $"{MeshFolder}/{assetName}.asset";
        var    existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing != null)
        {
            existing.Clear();
            existing.SetVertices(mesh.vertices);
            existing.SetUVs(0, new List<Vector2>(mesh.uv));
            existing.SetTriangles(mesh.triangles, 0);
            existing.RecalculateNormals();
            existing.RecalculateBounds();
            EditorUtility.SetDirty(existing);
            mesh = existing;
        }
        else
        {
            AssetDatabase.CreateAsset(mesh, path);
        }
    }

    /// <summary>Creates a child GameObject with MeshFilter and MeshRenderer.</summary>
    private static GameObject AttachMesh(Transform parent, string goName, Mesh mesh, Material mat)
    {
        var go  = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var mf  = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        var mr  = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial    = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows    = true;
        return go;
    }
}

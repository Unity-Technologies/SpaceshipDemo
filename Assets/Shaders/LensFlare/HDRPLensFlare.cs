using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class HDRPLensFlare : MonoBehaviour
{
    [SerializeField, HideInInspector]
    MeshRenderer m_MeshRenderer;
    [SerializeField, HideInInspector]
    MeshFilter m_MeshFilter;
    [SerializeField]
    Light m_Light;

    [Header("Global Settings")]
    public float OcclusionRadius = 1.0f;
    public float NearFadeStartDistance = 1.0f;
    public float NearFadeEndDistance = 3.0f;
    public float FarFadeStartDistance = 10.0f;
    public float FarFadeEndDistance = 50.0f;

    [Header("Flare Element Settings")]
    [SerializeField]
    public List<FlareSettings> Flares;

    void Awake()
    {
        if (m_MeshFilter == null)
            m_MeshFilter = GetComponent<MeshFilter>();
        if (m_MeshRenderer == null)
            m_MeshRenderer = GetComponent<MeshRenderer>();

        m_Light = GetComponent<Light>();

        m_MeshFilter.hideFlags = HideFlags.None;
        m_MeshRenderer.hideFlags = HideFlags.None;

        if (Flares == null)
            Flares = new List<FlareSettings>();

        m_MeshFilter.mesh = InitMesh();
    }

    void OnEnable()
    {
        UpdateGeometry();
    }


    // Use this for initialization
    void Start ()
    {
        m_Light = GetComponent<Light>();
    }

    void OnValidate()
    {
        UpdateGeometry();
        UpdateMaterials();
    }

    // Update is called once per frame
    void Update ()
    {
        // Lazy!
        UpdateVaryingAttributes();
    }

    Mesh InitMesh()
    {
        Mesh m = new Mesh();
        m.MarkDynamic();
        return m;
    }

    void UpdateMaterials()
    {
        Material[] mats = new Material[Flares.Count];

        int i = 0;
        foreach(FlareSettings f in Flares)
        {
            mats[i] = f.Material;
            i++;
        }
        m_MeshRenderer.sharedMaterials = mats;
    }

    void UpdateGeometry()
    {
        Mesh m = m_MeshFilter.sharedMesh;

        // Positions
        List<Vector3> vertices = new List<Vector3>();
        foreach (FlareSettings s in Flares)
        {
            vertices.Add(new Vector3(-1, -1, 0));
            vertices.Add(new Vector3(1, -1, 0));
            vertices.Add(new Vector3(1, 1, 0));
            vertices.Add(new Vector3(-1, 1, 0));
        }
        m.SetVertices(vertices);

        // UVs
        List<Vector2> uvs = new List<Vector2>();
        foreach (FlareSettings s in Flares)
        {
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));
        }
        m.SetUVs(0, uvs);

        // Variable Data
        m.SetColors(GetLensFlareColor());
        m.SetUVs(1, GetLensFlareData());
        m.SetUVs(2, GetWorldPositionAndRadius());
        m.SetUVs(3, GetDistanceFadeData());

        m.subMeshCount = Flares.Count;

        // Tris
        for (int i = 0; i < Flares.Count; i++)
        {
            int[] tris = new int[6];
            tris[0] = (i * 4) + 0;
            tris[1] = (i * 4) + 1;
            tris[2] = (i * 4) + 2;
            tris[3] = (i * 4) + 2;
            tris[4] = (i * 4) + 3;
            tris[5] = (i * 4) + 0;
            m.SetTriangles(tris, i);
        }

        Bounds b = m.bounds;
        b.extents = new Vector3(OcclusionRadius, OcclusionRadius, OcclusionRadius);
        m.bounds = b;
        m.UploadMeshData(false);
    }

    void UpdateVaryingAttributes()
    {
        Mesh m = m_MeshFilter.sharedMesh;

        m.SetColors(GetLensFlareColor());
        m.SetUVs(1, GetLensFlareData());
        m.SetUVs(2, GetWorldPositionAndRadius());
        m.SetUVs(3, GetDistanceFadeData());

        Bounds b = m.bounds;
        b.extents = new Vector3(OcclusionRadius, OcclusionRadius, OcclusionRadius);
        m.bounds = b;
        m.name = "LensFlare (" + gameObject.name + ")";
    }

    List<Color> GetLensFlareColor()
    {
        List<Color> colors = new List<Color>();
        foreach (FlareSettings s in Flares)
        {
            Color c = (s.MultiplyByLightColor && m_Light != null)? s.Color * m_Light.color * m_Light.intensity : s.Color;

            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
        }
        return colors;
    }

    List<Vector4> GetLensFlareData()
    {
        List<Vector4> lfData = new List<Vector4>();

        foreach(FlareSettings s in Flares)
        {
            Vector4 data = new Vector4(s.RayPosition, s.AutoRotate? -1 : Mathf.Abs(s.Rotation), s.Size.x, s.Size.y);
            lfData.Add(data); lfData.Add(data); lfData.Add(data); lfData.Add(data);
        }
        return lfData;
    }
    List<Vector4> GetDistanceFadeData()
    {
        List<Vector4> fadeData = new List<Vector4>();

        foreach (FlareSettings s in Flares)
        {
            Vector4 data = new Vector4(NearFadeStartDistance,NearFadeEndDistance, FarFadeStartDistance, FarFadeEndDistance);
            fadeData.Add(data); fadeData.Add(data); fadeData.Add(data); fadeData.Add(data);
        }
        return fadeData;
    }
    

    List<Vector4> GetWorldPositionAndRadius()
    {
        List<Vector4> worldPos = new List<Vector4>();
        Vector3 pos = transform.position;
        Vector4 value = new Vector4(pos.x,pos.y,pos.z, OcclusionRadius);
        foreach (FlareSettings s in Flares)
        {
            worldPos.Add(value); worldPos.Add(value); worldPos.Add(value); worldPos.Add(value);
        }

        return worldPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, OcclusionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, OcclusionRadius);
    }

    [System.Serializable]
    public class FlareSettings
    {
        public float RayPosition;
        public Material Material;
        [ColorUsage(true,true)]
        public Color Color;
        public bool MultiplyByLightColor;
        public Vector2 Size;
        public float Rotation;
        public bool AutoRotate;
        
        public FlareSettings()
        {
            RayPosition = 0.0f;
            Color = Color.white;
            MultiplyByLightColor = true;
            Size = new Vector2(0.3f, 0.3f);
            Rotation = 0.0f;
            AutoRotate = false;
        }
    }
}

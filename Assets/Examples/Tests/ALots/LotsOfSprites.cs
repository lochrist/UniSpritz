using UnityEngine;

public class LotsOfSprites : MonoBehaviour
{
    [SerializeField]
    private Material material;

    [SerializeField]
    private float minScale = 0.15f;

    [SerializeField]
    private float maxScale = 0.2f;

    [SerializeField]
    private int count;

    private Mesh mesh;

    // Matrix here is a compressed transform information
    // xy is the position, z is rotation, w is the scale
    private ComputeBuffer transformBuffer;

    // uvBuffer contains float4 values in which xy is the uv dimension and zw is the texture offset
    private ComputeBuffer uvBuffer;
    private ComputeBuffer colorBuffer;

    private uint[] args;

    private ComputeBuffer argsBuffer;

    Vector4[] m_Transforms;
    Vector4[] m_Uvs;
    Vector4[] m_Colors;

    private void Awake()
    {
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = -1;

        this.mesh = CreateQuad();

        UpdateBuffers();        
    }

    private static readonly Bounds BOUNDS = new Bounds(Vector2.zero, Vector3.one);

    private void UpdateBuffers()
    {
        // Prepare values
        const float maxRotation = Mathf.PI * 2;
        if (m_Transforms == null || m_Transforms.Length < this.count)
        {
            m_Transforms = new Vector4[this.count];
            m_Uvs = new Vector4[this.count];
            m_Colors = new Vector4[this.count];

            this.transformBuffer = new ComputeBuffer(this.count, 16);
            this.uvBuffer = new ComputeBuffer(this.count, 16);
            this.colorBuffer = new ComputeBuffer(this.count, 16);

            this.args = new uint[] {
                6, (uint)this.count, 0, 0, 0
            };
            this.argsBuffer = new ComputeBuffer(1, this.args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

            for (int i = 0; i < this.count; ++i)
            {
                // transform
                float x = UnityEngine.Random.Range(-8f, 8f);
                float y = UnityEngine.Random.Range(-4.0f, 4.0f);
                // float rotation = UnityEngine.Random.Range(0, maxRotation);
                float rotation = 0;
                // float scale = UnityEngine.Random.Range(0.5f, 1);
                float scale = 1;
                m_Transforms[i] = new Vector4(x, y, rotation, scale);

                // UV
                float u = UnityEngine.Random.Range(0, 8) * 0.125f;
                float v = UnityEngine.Random.Range(0, 8) * 0.125f;
                m_Uvs[i] = new Vector4(0.125f, 0.125f, u, v);

                // color
                float r = UnityEngine.Random.Range(0f, 1.0f);
                float g = UnityEngine.Random.Range(0f, 1.0f);
                float b = UnityEngine.Random.Range(0f, 1.0f);
                // m_Colors[i] = new Vector4(r, g, b, 1.0f);
                m_Colors[i] = new Vector4(1f, 1f, 1f, 1.0f);
            }

            this.uvBuffer.SetData(m_Uvs);
            int uvBufferId = Shader.PropertyToID("uvBuffer");
            this.material.SetBuffer(uvBufferId, this.uvBuffer);

            this.colorBuffer.SetData(m_Colors);
            int colorsBufferId = Shader.PropertyToID("colorsBuffer");
            this.material.SetBuffer(colorsBufferId, this.colorBuffer);
        }
        /*
        for (int i = 0; i < this.count; ++i)
        {
            // transform
            float x = UnityEngine.Random.Range(-8f, 8f);
            float y = UnityEngine.Random.Range(-4.0f, 4.0f);
            float rotation = UnityEngine.Random.Range(0, maxRotation);
            float scale = UnityEngine.Random.Range(this.minScale, this.maxScale);
            m_Transforms[i] = new Vector4(x, y, rotation, scale);
        }
        */
        this.transformBuffer.SetData(m_Transforms);
        
        /*
        var array = this.transformBuffer.BeginWrite<Vector4>(0, m_Transforms.Length);
        for (int i = 0; i < this.count; ++i)
        {
            // transform
            float x = UnityEngine.Random.Range(-8f, 8f);
            float y = UnityEngine.Random.Range(-4.0f, 4.0f);
            float rotation = UnityEngine.Random.Range(0, maxRotation);
            float scale = UnityEngine.Random.Range(this.minScale, this.maxScale);
            m_Transforms[i] = new Vector4(x, y, rotation, scale);
            // array[i] = new Vector4(x, y, rotation, scale);
        }
                
        array.CopyFrom(m_Transforms);
        this.transformBuffer.EndWrite<Vector4>(m_Transforms.Length);
        */

        int matrixBufferId = Shader.PropertyToID("transformBuffer");
        this.material.SetBuffer(matrixBufferId, this.transformBuffer);

        this.argsBuffer.SetData(this.args);
    }

    private void Update()
    {
        // UpdateBuffers();        
        Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, BOUNDS, this.argsBuffer);
    }

    private void OnDisable()
    {
        if (transformBuffer != null)
        {
            transformBuffer.Release();
            transformBuffer = null;
        }

        if (uvBuffer != null)
        {
            uvBuffer.Release();
            uvBuffer = null;
        }

        if (colorBuffer != null)
        {
            colorBuffer.Release();
            colorBuffer = null;
        }

        if (argsBuffer != null)
        {
            argsBuffer.Release();
            argsBuffer = null;
        }
    }

    private static Mesh CreateQuad()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);
        vertices[3] = new Vector3(1, 1, 0);
        mesh.vertices = vertices;

        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        mesh.uv = uv;

        return mesh;
    }
}
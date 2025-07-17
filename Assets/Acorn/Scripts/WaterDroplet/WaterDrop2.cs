using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class WaterDrop2 : MonoBehaviour
{
    // -- Inspector Fields --
    [Header("Droplet Properties")]
    [SerializeField] private int resolution = 20; // How many vertices make up the droplet's edge
    [SerializeField] private float initialRadius = 0.5f;
    [SerializeField] private float teardropFactor = 1.5f;

    // -- Private Components & State --
    private MeshFilter meshFilter;
    private Rigidbody2D rb;
    private PolygonCollider2D polyCollider;
    private Mesh mesh;

    private Vector3[] vertices;
    private Vector3[] originalVertices;
    private Vector3 teardropBezier;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody2D>();
        polyCollider = GetComponent<PolygonCollider2D>();

        mesh = new Mesh();
        mesh.name = "WaterDrop";
        meshFilter.mesh = mesh;
    }


    // Start is called before the first frame update
    void Start()
    {
        GenerateSphereMesh();
        Deform2Teardrop();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateSphereMesh()
    {
        vertices = new Vector3[resolution + 1]; // +1 for the center vertex
        // var points = new List<Vector2>(); // For the collider

        // Center vertex
        vertices[0] = Vector3.zero;

        // Edge vertices
        float angleStep = 360f / resolution;
        for (int i = 0; i < resolution; i++)
        {
            float angleRad = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angleRad);
            float y = Mathf.Sin(angleRad);
            vertices[i + 1] = new Vector3(x, y, 0) * initialRadius;
            // points.Add(vertices[i + 1]);
        }

        originalVertices = (Vector3[])vertices.Clone();
    }

    private void CalcTeardropBezier()
    {
        
    }

    private void Deform2Teardrop()
    {
        var points = new List<Vector2>(); // For the collider
        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 dir = (vertices[i] - vertices[0]).normalized;
            Debug.Log($"v[{i}] dir:{dir},({vertices[i]})");
            // 顶部顶点拉伸
            if (vertices[i].y < 0)
            {
                float stretchFactor = 1 + teardropFactor * Mathf.Abs(vertices[i].y);
                vertices[i] = vertices[0] + dir * (Vector3.Distance(originalVertices[i], Vector3.zero) * stretchFactor);
            }
            // 底部顶点保持圆形
            else
            {
                vertices[i] = originalVertices[i];
            }
            points.Add(vertices[i]);
        }

        UpdateMeshAndCollider(points);
    }

    /// <summary>
    /// Updates the mesh vertices, triangles, and recalculates normals.
    /// Also updates the polygon collider.
    /// </summary>
    private void UpdateMeshAndCollider(List<Vector2> colliderPoints)
    {
        // Triangles
        int[] triangles = new int[resolution * 3];
        for (int i = 0; i < resolution; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0; // Center vertex
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = (i == resolution - 1) ? 1 : i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        UpdateCollider(colliderPoints);
    }

    /// <summary>
    /// Updates the PolygonCollider2D path to match the mesh outline.
    /// </summary>
    private void UpdateCollider(List<Vector2> points)
    {
        polyCollider.SetPath(0, points);
    }
}

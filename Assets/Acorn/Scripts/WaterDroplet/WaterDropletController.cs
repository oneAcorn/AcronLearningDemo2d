using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class WaterDropletController : MonoBehaviour
{
    // -- Inspector Fields --
    [Header("Droplet Properties")]
    [SerializeField] private int resolution = 20; // How many vertices make up the droplet's edge
    [SerializeField] private float initialRadius = 0.5f;
    [SerializeField] private float teardropFactor = 1.5f; // How pronounced the teardrop tail is

    [Header("Animation Timings")]
    [SerializeField] private float growthDuration = 0.5f;
    [SerializeField] private float spreadDuration = 0.3f;

    [Header("Physics Effects")]
    [SerializeField] private float stretchFactor = 0.1f; // How much it stretches with speed

    // -- Private Components & State --
    private MeshFilter meshFilter;
    private Rigidbody2D rb;
    private PolygonCollider2D polyCollider;
    private Mesh mesh;

    private Vector3[] vertices;
    private bool hasCollided = false;

    void Awake()
    {
        // Get references to all required components
        meshFilter = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody2D>();
        polyCollider = GetComponent<PolygonCollider2D>();

        // Create a new mesh object
        mesh = new Mesh();
        mesh.name = "WaterDroplet_Mesh";
        meshFilter.mesh = mesh;
    }

    void Start()
    {
        GenerateInitialShape();
        StartCoroutine(GrowDroplet());
    }

    void FixedUpdate()
    {
        // Only deform the droplet shape while it's falling
        if (!hasCollided && rb.velocity.y < -0.1f)
        {
            AnimateFallingShape();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            rb.gravityScale = 0; // Stop gravity
            rb.velocity = Vector2.zero; // Stop movement
            StartCoroutine(SpreadOnSurface(collision));
        }
    }
    
    /// <summary>
    /// Generates the initial teardrop mesh.
    /// </summary>
    private void GenerateInitialShape()
    {
        vertices = new Vector3[resolution + 1]; // +1 for the center vertex
        var points = new List<Vector2>(); // For the collider

        // Center vertex
        vertices[0] = Vector3.zero;

        // Edge vertices
        float angleStep = 360f / resolution;
        for (int i = 0; i < resolution; i++)
        {
            float angleRad = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Sin(angleRad);
            float y = Mathf.Cos(angleRad);

            // Create the teardrop shape by modifying the y-position
            float teardropY = y;
            if (y > 0)
            {
                teardropY = Mathf.Pow(y, 0.5f); // Pulls the top half upwards
            }
            else
            {
                teardropY *= teardropFactor; // Stretches the bottom half downwards
            }
            
            vertices[i + 1] = new Vector3(x, teardropY, 0) * initialRadius;
            points.Add(vertices[i + 1]);
        }

        UpdateMeshAndCollider(points);
    }
    
    /// <summary>
    /// Animates the droplet growing to its initial size.
    /// </summary>
    private IEnumerator GrowDroplet()
    {
        float time = 0;
        transform.localScale = Vector3.zero;

        while (time < growthDuration)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(0, 1, time / growthDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        //将BodyType修改为Dynamic
        rb.isKinematic = false; // Allow physics to take over after growing
    }

    /// <summary>
    /// Stretches the mesh based on falling speed.
    /// </summary>
    private void AnimateFallingShape()
    {
        Vector3[] currentVertices = mesh.vertices;
        var points = new List<Vector2>();

        // We only modify the edge vertices (skip the center one at index 0)
        for (int i = 1; i < currentVertices.Length; i++)
        {
            // Stretch more the faster it falls (velocity is negative)
            float stretch = 1.0f - (rb.velocity.y * stretchFactor * 0.1f);
            
            // Apply stretch, but keep width somewhat constant
            currentVertices[i].y *= stretch;
            currentVertices[i].x /= Mathf.Sqrt(stretch); // Conserves volume slightly

            points.Add(currentVertices[i]);
        }

        mesh.vertices = currentVertices;
        UpdateCollider(points);
    }

    /// <summary>
    /// Animates the droplet spreading out on a surface after impact.
    /// </summary>
    private IEnumerator SpreadOnSurface(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector3[] currentVertices = mesh.vertices;
        Vector3[] targetVertices = new Vector3[currentVertices.Length];
        
        // Calculate the flattened target shape
        for(int i = 0; i < currentVertices.Length; i++)
        {
            // Project vertex onto a line perpendicular to the collision normal
            Vector3 flattenedVertex = Vector3.ProjectOnPlane(currentVertices[i], normal);
            targetVertices[i] = flattenedVertex * 1.5f; // Spread out a bit wider
        }

        float time = 0;
        while (time < spreadDuration)
        {
            var points = new List<Vector2>();
            for (int i = 0; i < vertices.Length; i++)
            {
                // Interpolate from current shape to target flattened shape
                vertices[i] = Vector3.Lerp(currentVertices[i], targetVertices[i], time / spreadDuration);
                if (i > 0) points.Add(vertices[i]); // Don't add center vertex to collider
            }
            UpdateMeshAndCollider(points);
            time += Time.deltaTime;
            yield return null;
        }

        // Optional: Destroy or pool the object after it has spread
        // Destroy(gameObject, 2f); 
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
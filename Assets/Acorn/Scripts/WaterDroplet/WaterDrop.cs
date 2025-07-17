using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody2D))]
public class WaterDrop : MonoBehaviour
{
    [Header("Parameters")]
    public float spawnDuration = 0.5f;
    public float minSize = 0.2f;
    public float maxSize = 0.5f;
    public float spreadSpeed = 2f;
    public float maxSpread = 2f;
    public float destroyDelay = 1f;

    private Mesh mesh;
    private Rigidbody2D rb;
    private PolygonCollider2D polyCollider;
    private float currentSize;
    private bool isSplashing = false;
    private float splashProgress = 0f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.drag = 0.1f;
        rb.angularDrag = 0.05f;
        
        polyCollider = gameObject.AddComponent<PolygonCollider2D>();
        polyCollider.isTrigger = false;
        
        CreateDropletMesh();
        StartCoroutine(SpawnAnimation());
    }

    IEnumerator SpawnAnimation()
    {
        float timer = 0;
        currentSize = minSize;
        
        while (timer < spawnDuration)
        {
            timer += Time.deltaTime;
            currentSize = Mathf.Lerp(minSize, maxSize, timer/spawnDuration);
            UpdateDropletShape();
            yield return null;
        }
    }
    
    void CreateDropletMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Water Drop";
        UpdateDropletShape();
        
        // 设置材质（在Inspector中分配半透明材质）
        // GetComponent<MeshRenderer>().material = Resources.Load<Material>("WaterMaterial");
    }

    // 水滴基本形状（上圆下尖）
    void UpdateDropletShape()
    {
        int segments = 16;
        Vector3[] vertices = new Vector3[segments];
        int[] triangles = new int[(segments - 2) * 3];
        
        // 创建水滴形状
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.PI * i / (segments - 2);
            float radius = currentSize;
            
            // 下半部分收缩形成尖端
            if (angle > Mathf.PI)
            {
                radius *= 1.0f - (angle - Mathf.PI) / Mathf.PI * 0.7f;
            }
            
            vertices[i] = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius * 1.2f,  // 垂直拉伸
                0
            );
        }
        
        // 创建三角形
        for (int i = 0; i < segments - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        // 更新碰撞体
        Vector2[] colliderPoints = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            colliderPoints[i] = vertices[i];
        }
        polyCollider.SetPath(0, colliderPoints);
    }

    void FixedUpdate()
    {
        if (!isSplashing)
        {
            // 下落过程中轻微变形（受空气阻力影响）
            float velocityFactor = Mathf.Clamp01(rb.velocity.magnitude / 5f);
            StretchDrop(1f + velocityFactor * 0.3f, 1f - velocityFactor * 0.2f);
        }
        else if (splashProgress < 1f)
        {
            // 碰撞后摊开效果
            splashProgress += Time.fixedDeltaTime * spreadSpeed;
            StretchDrop(
                Mathf.Lerp(1f, maxSpread, splashProgress),
                Mathf.Lerp(1f, 0.2f, splashProgress)
            );
        }
    }

    void StretchDrop(float xScale, float yScale)
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(
                vertices[i].x * xScale,
                vertices[i].y * yScale,
                0
            );
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        
        // 更新碰撞体
        Vector2[] colliderPoints = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colliderPoints[i] = vertices[i];
        }
        polyCollider.SetPath(0, colliderPoints);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isSplashing)
        {
            isSplashing = true;
            rb.simulated = false; // 停止物理模拟
            Invoke("DestroyDrop", destroyDelay);
        }
    }

    void DestroyDrop()
    {
        Destroy(gameObject);
    }
}
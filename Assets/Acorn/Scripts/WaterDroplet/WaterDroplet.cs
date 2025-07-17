using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class WaterDroplet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Mesh mesh;
    private Vector3[] originalVertices;
    private PolygonCollider2D polyCollider;

    [Header("Deformation Settings")]
    public float maxDeformation = 0.5f; // 最大变形量
    public float deformationFactor = 0.1f; // 变形系数
    public float restoreSpeed = 5f; // 形状恢复速度
    public float surfaceTension = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        polyCollider = GetComponent<PolygonCollider2D>();
        CreateSphereMesh();
        rb.gravityScale = 1f; // 启用重力
    }

    void Update()
    {
        DeformBasedOnVelocity();
        ApplySurfaceTension();
        RestoreShape();
        mesh.RecalculateNormals();
        UpdateCollider();
    }

    void DeformBasedOnVelocity()
    {
        Vector3[] vertices = mesh.vertices;
        float velocityMagnitude = rb.velocity.magnitude;

        // 只在下落时变形 (velocity.y < 0)
        if (rb.velocity.y < 0)
        {
            float deformation = Mathf.Clamp(
                velocityMagnitude * deformationFactor,
                0,
                maxDeformation
            );

            for (int i = 1; i < vertices.Length; i++)
            {
                Vector3 dir = (vertices[i] - vertices[0]).normalized;

                // 底部顶点拉伸 (y < 0)
                if (vertices[i].y < 0)
                {
                    float stretchFactor = 1 + deformation * Mathf.Abs(vertices[i].y);
                    vertices[i] = vertices[0] + dir * (Vector3.Distance(originalVertices[i], Vector3.zero) * stretchFactor);
                }
                // 顶部顶点保持圆形
                else
                {
                    vertices[i] = originalVertices[i];
                }
            }
        }

        mesh.vertices = vertices;
    }

    void ApplySurfaceTension()
    {
        Vector3[] vertices = mesh.vertices;

        // 模拟表面张力 - 使顶点靠近原始位置
        for (int i = 1; i < vertices.Length; i++)
        {
            // 计算相邻顶点
            int prev = (i == 1) ? vertices.Length - 1 : i - 1;
            int next = (i == vertices.Length - 1) ? 1 : i + 1;

            // 计算理想位置（相邻顶点的中点）
            Vector3 targetPos = (vertices[prev] + vertices[next]) / 2f;

            // 向理想位置移动
            vertices[i] = Vector3.Lerp(
                vertices[i],
                targetPos,
                surfaceTension * Time.deltaTime
            );
        }

        mesh.vertices = vertices;
    }

    void UpdateCollider()
    {
        if (polyCollider == null) return;

        Vector3[] vertices = mesh.vertices;
        int vertexCount = vertices.Length - 1; // 忽略中心点

        // 创建碰撞体路径
        Vector2[] path = new Vector2[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            path[i] = new Vector2(vertices[i + 1].x, vertices[i + 1].y);
        }

        polyCollider.SetPath(0, path);
    }


    void RestoreShape()
    {
        Vector3[] vertices = mesh.vertices;

        // 逐渐恢复原始形状
        for (int i = 1; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.Lerp(
                vertices[i],
                originalVertices[i],
                restoreSpeed * Time.deltaTime
            );
        }

        mesh.vertices = vertices;
    }

    void CreateSphereMesh(int segments = 20)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // 生成顶点
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        // 中心点
        vertices[0] = Vector3.zero;

        // 周边点
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle) * 0.5f,
                Mathf.Sin(angle) * 0.5f,
                0
            );
        }

        // 生成三角形
        for (int i = 0; i < segments; i++)
        {
            int index = i * 3;
            triangles[index] = 0;
            triangles[index + 1] = i + 1;
            triangles[index + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        originalVertices = (Vector3[])vertices.Clone();
    }
}

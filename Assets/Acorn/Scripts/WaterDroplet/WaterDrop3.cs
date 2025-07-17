using System.Collections;
using System.Collections.Generic;
using Acorn.Tools;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WaterDrop3 : MonoBehaviour, IPoolable
{
    [SerializeField] private float growthDuration = 0.5f;
    [SerializeField] private GameObject splashEffectPrefab;
    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayers;
    private Rigidbody2D rb;
    private ObjectPool pool;
    private float growthProgress;
    private bool isGrowing = true;
    private Vector3 spawnPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isGrowing)
        {
            GrowDroplet();
        }
    }

    private void GrowDroplet()
    {
        growthProgress += Time.deltaTime / growthDuration;

        if (growthProgress >= 1f)
        {
            // 生长完成
            growthProgress = 1f;
            isGrowing = false;
            rb.isKinematic = false;
            // rb.gravityScale = fallGravity;
        }

        // 计算当前缩放
        float currentScale = Mathf.Lerp(0, 1, growthProgress);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);

        // 位置修正：保持顶部位置不变
        float verticalOffset = currentScale * 0.5f; // 根据实际精灵高度调整
        transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y - verticalOffset,
            spawnPosition.z
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"OnCollisionEnter2D {collision.gameObject}");
        // 检查碰撞层
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            CreateSplashEffect();
            ReturnToPool();
        }
    }

    private void CreateSplashEffect()
    {
        if (splashEffectPrefab)
        {
            Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void OnGetFromPool()
    {
        ResetState();

        spawnPosition = transform.position;
        growthProgress = 0f;
        isGrowing = true;
        transform.localScale = Vector3.zero;

        GetComponent<Collider2D>().enabled = true;
    }

    public void OnReturnToPool()
    {
        ResetState();
        GetComponent<Collider2D>().enabled = false;
    }

    private void ResetState()
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
    public void SetPool(ObjectPool poolReference)
    {
        pool = poolReference;
    }
}

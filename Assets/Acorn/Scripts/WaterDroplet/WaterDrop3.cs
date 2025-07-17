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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(GrowDroplet());
    }


    private IEnumerator GrowDroplet()
    {
        float time = 0;
        transform.localScale = Vector3.zero;
        Vector3 startPosition = transform.position;
        while (time < growthDuration)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(0, 1, time / growthDuration);
            // 位置修正：保持顶部Y轴不变
            Vector3 newPos = startPosition;
            newPos.y -= transform.localScale.y * 0.5f; // 向下偏移半个高度
            transform.position = newPos;
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
        //将BodyType修改为Dynamic
        rb.isKinematic = false;
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

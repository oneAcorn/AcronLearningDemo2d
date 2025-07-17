using System.Collections;
using System.Collections.Generic;
using Acorn.Tools;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class WaterDropSpawner : MonoBehaviour
{
    [SerializeField] private float spawnDuration = 20;
    [SerializeField] private float spawnInterval = 0.9f;
    private ObjectPool dropPool;

    private const int INITIAL_POOL_SIZE = 10;

    private void Awake()
    {
        dropPool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        StartCoroutine(SpawnDrops());
    }

    IEnumerator SpawnDrops()
    {
        float time = 0f;
        while (time < spawnDuration)
        {
            GameObject drop = dropPool.GetFromPool(transform.position, transform.rotation);
            drop.GetComponent<WaterDrop3>()?.SetPool(dropPool);
            time += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        
        // 绘制生成区域
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
    }
}

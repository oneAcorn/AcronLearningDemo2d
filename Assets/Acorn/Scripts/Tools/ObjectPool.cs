using System.Collections.Generic;
using Acorn.Tools;
using UnityEngine;

namespace Acorn.Tools
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private bool expandable = true;

        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeObjects = new List<GameObject>();

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialSize; i++)
            {
                CreatePooledObject();
            }
        }

        private GameObject CreatePooledObject()
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }

        public GameObject GetFromPool(Vector3 position, Quaternion rotation)
        {
            GameObject obj;

            if (pool.Count == 0)
            {
                if (expandable)
                {
                    obj = CreatePooledObject();
                }
                else
                {
                    Debug.LogWarning("Object pool exhausted and not expandable");
                    return null;
                }
            }
            else
            {
                obj = pool.Dequeue();
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            // 调用池化对象接口
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnGetFromPool();

            activeObjects.Add(obj);
            return obj;
        }

        public void ReturnToPool(GameObject obj)
        {
            if (!activeObjects.Contains(obj))
            {
                Debug.LogWarning("Trying to return object not from this pool: " + obj.name);
                return;
            }

            activeObjects.Remove(obj);
            pool.Enqueue(obj);

            // 调用池化对象接口
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnReturnToPool();

            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }

        public void ReturnAllToPool()
        {
            while (activeObjects.Count > 0)
            {
                ReturnToPool(activeObjects[0]);
            }
        }
    }
}
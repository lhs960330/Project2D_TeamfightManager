using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

// È­»ì ¸®Á¨
public class arrowSpawn : ObjectPool
{

    Transform target;
    [SerializeField] ArrowAttack prefab1;
    public void SetEnemy(Transform enemy)
    {
        target = enemy;
    }
    public Transform GetEnemy()
    {
        return target;
    }
    private void Awake()
    {     
        CreatePool(prefab, size, capacity);
    }

    private void Start()
    {
    }
    public void CreatePool(ArrowAttack prefab, int size, int capacity)
    {
        this.prefab = prefab;
        this.size = size;
        this.capacity = capacity;

        objectPool = new Stack<PooledObject>(capacity);
        for (int i = 0; i < size; i++)
        {
            ArrowAttack instance = Instantiate(prefab);
            instance.gameObject.SetActive(false);
            instance.Pool = this; 
            instance.transform.parent = transform;
            objectPool.Push(instance);
        }
    }

    public override PooledObject GetPool(Vector3 position, Quaternion rotation)
    {
        if (objectPool.Count > 0)
        {
            PooledObject instance = objectPool.Pop();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            PooledObject instance = Instantiate(prefab);
            instance.Pool = this;
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            return instance;
        }
    }

    public override void ReturnPool(PooledObject instance)
    {
        if (objectPool.Count < capacity)
        {
            instance.gameObject.SetActive(false);
            instance.transform.parent = transform;
            objectPool.Push(instance);
        }
        else
        {
            Destroy(instance.gameObject);
        }
    }
}

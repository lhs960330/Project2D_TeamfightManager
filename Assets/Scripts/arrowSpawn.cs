using UnityEngine;


// ȭ�� ����
public class arrowSpawn : ObjectPool
{
    private void Start()
    {
        Debug.Log($"{prefab.name}�� {size}�� ����");
        CreatePool(prefab, size, capacity);
    }
    
    
    
}

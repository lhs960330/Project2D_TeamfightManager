using UnityEngine;


// 화살 리젠
public class arrowSpawn : ObjectPool
{
    private void Start()
    {
        Debug.Log($"{prefab.name}을 {size}개 생성");
        CreatePool(prefab, size, capacity);
    }
    
    
    
}

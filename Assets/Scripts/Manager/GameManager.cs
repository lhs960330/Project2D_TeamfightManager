using UnityEngine;

// ���ӿ� �ʿ���ϴ� ��ɵ� �߰�
public class GameManager : Singleton<GameManager>
{
    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}

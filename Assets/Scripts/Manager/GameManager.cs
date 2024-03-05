using UnityEngine;

// 게임에 필요로하는 기능들 추가
public class GameManager : Singleton<GameManager>
{
    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}

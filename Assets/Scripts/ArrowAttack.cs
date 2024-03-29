using UnityEngine;

// 화살의 방향 및 발사
public class ArrowAttack : PooledObject
{
    LongChampionController controller;
    // 스폰위치 가져옴
    // 각도
    float angle;
    // 화살 방향
    Vector2 arrow;
    Vector2 arrowEnd;
    // 적 방향
    Vector2 enemy;
    // 화살 속도
    float speed;

    private void Start()
    {
        controller = gameObject.GetComponentInParent<LongChampionController>();
        speed = 20;
        // 이 스크립트를 가지고있는 친구에 위치 방향
        arrow = transform.position;
        // 적찾기 위치 방향
        if (controller != null)
            enemy = controller.EnemyPos.position;
    }

    private void Update()
    {
        if (enemy == null)
            return;
        arrowEnd = transform.position;
        arrow = transform.position;
        // 계속확인 (벡터라서 계속확인해서 화살 방향을 정해줌)
        if (controller.EnemyPos != null)
            enemy = controller.EnemyPos.position;
        else
        {
            controller = gameObject.GetComponentInParent<LongChampionController>();
        }

        // 잘은 모르지만 적과 화살의 방향의 각도를  삼각함수를 통해 각도를 정해줌
        angle = Mathf.Atan2(enemy.y - arrow.y, enemy.x - arrow.x) * Mathf.Rad2Deg;
        // 화살의 회전값을 적회전값으로 보내줌
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.position = Vector2.Lerp(arrow, enemy, speed * Time.deltaTime);
        if(arrowEnd == enemy)
        {
            Release();
        }
    }
}

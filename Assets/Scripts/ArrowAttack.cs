using System.Collections;
using System.Collections.Generic;
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

        enemy = controller.EnemyPos.position;
    }

    private void Update()
    {
        // 계속확인 (벡터라서 계속확인해서 화살 방향을 정해줌)
        arrow = transform.position;
        enemy = controller.EnemyPos.position;
        // 잘은 모르지만 적과 화살의 방향의 각도를  삼각함수를 통해 각도를 정해줌
        angle = Mathf.Atan2(enemy.y - arrow.y, enemy.x - arrow.x) * Mathf.Rad2Deg;
        // 화살의 회전값을 적회전값으로 보내줌
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.position = Vector2.Lerp(arrow, enemy, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Release();
    }
}

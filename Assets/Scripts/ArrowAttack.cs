using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ȭ���� ���� �� �߻�
public class ArrowAttack : PooledObject
{
    LongChampionController controller;
    // ������ġ ������
    // ����
    float angle;
    // ȭ�� ����
    Vector2 arrow;
    // �� ����
    Vector2 enemy;
    // ȭ�� �ӵ�
    float speed;

    private void Start()
    {
        controller = gameObject.GetComponentInParent<LongChampionController>();
        speed = 20;
        // �� ��ũ��Ʈ�� �������ִ� ģ���� ��ġ ����
        arrow = transform.position;
        // ��ã�� ��ġ ����

        enemy = controller.EnemyPos.position;
    }

    private void Update()
    {
        // ���Ȯ�� (���Ͷ� ���Ȯ���ؼ� ȭ�� ������ ������)
        arrow = transform.position;
        enemy = controller.EnemyPos.position;
        // ���� ������ ���� ȭ���� ������ ������  �ﰢ�Լ��� ���� ������ ������
        angle = Mathf.Atan2(enemy.y - arrow.y, enemy.x - arrow.x) * Mathf.Rad2Deg;
        // ȭ���� ȸ������ ��ȸ�������� ������
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.position = Vector2.Lerp(arrow, enemy, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Release();
    }
}

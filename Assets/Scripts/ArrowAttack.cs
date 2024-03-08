using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ȭ���� ���� �� �߻�
public class ArrowAttack : PooledObject
{
    // ������ġ ������
    arrowSpawn arrowSpawn;
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
        speed = 20;
        // ȭ�� ��ġ�� ���ӸŴ������� ���Ÿ����� 0��° ģ���� ArrowPoint���� ������Ŵ
        arrowSpawn = Manager.Game.longChampion[0].ArrowPoint;
        // �� ��ũ��Ʈ�� �������ִ� ģ���� ��ġ ����
        arrow = transform.position;
        // ��ã�� ��ġ ����

        enemy = Manager.Game.shortChampion[0].gameObject.transform.position;
    }

    private void Update()
    {
        // ���Ȯ�� (���Ͷ� ���Ȯ���ؼ� ȭ�� ������ ������)
        arrow = transform.position;
        enemy = Manager.Game.shortChampion[0].gameObject.transform.position;
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

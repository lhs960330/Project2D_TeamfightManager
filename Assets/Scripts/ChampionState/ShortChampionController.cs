using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShortChampionController : MonoBehaviour
{ 
    // ������ ��Ƶ�
    public List<ChampionData> Enemy = new List<ChampionData>();
    // ���µ� ��Ƶ�
    public enum State { Idle, Find, Move, Attack, Die }

    // ���µ� ������ ���¸ӽ��� ������
    private StateMachine<State> stateMachine = new StateMachine<State>();

    // �� è�Ǿ� ������ ��� Ŭ������ ������ 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }
    ChampionData targetEnemy;
    public ChampionData TargetEnemy { get { return targetEnemy; } set { targetEnemy = value; } }
    // ���� ��ġ�� �����´�.
    Transform enemyPos;
    public Transform EnemyPos { get { return enemyPos; } set { enemyPos = value; } }


    private void Start()
    {
        // �� ���µ��� ���¸ӽſ� ����
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        // ù ���¸� ������
        stateMachine.Start(State.Find);
        Manager.Game.ChampionDataProduce(data);
    }

    private void Update()
    {
        // ���°� ���Ҷ����� Ȯ������
        stateMachine.Update();
    }
    public void Attack()
    {
        // ���� �ڷ�ƾ�� ����ϱ� ���� ����
        if (attackRouine == null)
        {
            attackRouine = StartCoroutine(AttackRoutine());
        }
    }

    public void StopAttack()
    {
        // ������ ���߰� ����
        if (attackRouine != null)
            StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (EnemyPos)
        {
            // ������°� �����϶��� �ڷ�ƾ�� ���
            if (stateMachine.CheckState(State.Attack))
            {
                data.animator.Play("Attack");
                yield return new WaitForSeconds(data.attackTime);
            }
            else
            {
                yield return null;
            }
        }
    }
    public void HitDamag()
    {
        targetEnemy.hp -= data.damage;
    }
    private class ChampionState : BaseState<State>
    {

        protected ShortChampionController controller;
        protected Vector3 dir;

        // ���⼭ �ʱ�ȭ
        public ChampionState(ShortChampionController owner)
        {
            this.controller = owner;
        }
    }
    // �������ִ� ����
    private class IdleState : ChampionState
    {
        public IdleState(ShortChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Idle");
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            // ���� ã�� ���·� ��ȯ���ش�.
            controller.stateMachine.ChangeState(State.Find);
        }

    }
    private class FindState : ChampionState
    {
        public FindState(ShortChampionController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // �޴����� �ִ� �����͸� �����ͼ� �� ����
            foreach (ChampionData a in Manager.Game.championDatas)
            {
                if (controller.data.Team != a.Team)
                {
                    if (controller.Enemy.Contains(a))
                        continue;
                    controller.Enemy.Add(a);
                }
            }

            if (controller.data.Team == 0 && Manager.Game.countBuleteam == 0)
            {// A���̸� ���� ���� 0��
                controller.stateMachine.ChangeState(State.Idle);
            }
            else if (controller.data.Team == 1 && Manager.Game.countRedteam == 0)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            else
            {
                if (controller.Enemy[0] == null)
                {
                    controller.Enemy.RemoveAt(0);
                    return;
                }
                controller.EnemyPos = controller.Enemy[0].transform;
                controller.targetEnemy = controller.Enemy[0];
            }
            // ����� �� ã��
            foreach (ChampionData a in controller.Enemy)
            {
                if (controller.EnemyPos == null)
                    continue;
                if (a == null)
                {
                    controller.Enemy.Remove(a);
                    return;
                }

                // ó�� ������ ģ���� ������ �� ��
                if (Vector3.Distance(controller.transform.position, controller.EnemyPos.position) >= Vector3.Distance(controller.transform.position, a.gameObject.GetComponent<Transform>().position))
                {
                    // ���� ����� �� ����
                    controller.EnemyPos = a.transform;
                    controller.targetEnemy = a;
                }
            }
        }


        public override void Transition()
        {
            // ��������
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            // ���� ��ġ�� ������ �̵����·� ��ȯ �� �̿ܿ��� �������ִ� ����
            if (controller.TargetEnemy == null)
            {
                controller.stateMachine.ChangeState(State.Idle);

            }
            else
            {
                controller.stateMachine.ChangeState(State.Move);
            }
        }
    }
    private class MoveState : ChampionState
    {
        public MoveState(ShortChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            dir = (controller.enemyPos.position - controller.transform.position).normalized;
        }

        public override void Update()
        {
            // ���� �� ���̿� x�� 0���� ������ ������ ���� ũ�� �����ʺ����Ѵ�.
            if (dir.x < 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

            }

            else if (dir.x > 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            // �������� �̵�
            if (controller.EnemyPos != null)
                controller.transform.Translate(dir * controller.data.speed * Time.deltaTime);
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
                return;
            }
            // ���� �������(�׾�����)
            if (controller.targetEnemy == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
                return;
            }
            // ��Ÿ��ȿ� �������� ��������
            if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
        public override void Exit()
        {
            if (controller.targetEnemy == null)
            {
                controller.Enemy.RemoveAt(0);
            }
        }
    }
    private class AttackState : ChampionState
    {
        public AttackState(ShortChampionController owner) : base(owner)
        {
        }

        // ������ �׼�
        public override void Enter()
        {
            // ���� �ڷ�ƾ ����
            controller.Attack();
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            else if (controller.EnemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            // ���������� �ȋ�����
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
        public override void Exit()
        {
            if (controller.Enemy == null)
            {
                controller.StopAttack();
                controller.Enemy.Remove(controller.targetEnemy);
            }
        }
    }

    // ������
    private class DieState : ChampionState
    {
        bool isDie = true;
        public DieState(ShortChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            // ���δ�.
            if (isDie)
            {
                if (controller.data.Team == 0)
                {
                    Manager.Game.SetBuleScore();
                }
                else
                {
                    Manager.Game.SetRedScore();
                }
                controller.StopAttack();
                isDie = false;
                controller.data.animator.Play("Die");
                Manager.Game.RemoveChampion(this.controller.data);

                Destroy(controller.gameObject, 1f);
            }

        }

    }
}

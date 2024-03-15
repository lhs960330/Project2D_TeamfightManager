using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LongChampionController : MonoBehaviour
{
    // ������ ��Ƶ�
    public List<ChampionData> Enemy = new List<ChampionData>();
    // ���µ� ��Ƶ�
    public enum State { Idle, Find, Move, Attack, Avoid, Die }

    // ���µ� ������ ���¸ӽ��� ������
    private StateMachine<State> stateMachine = new StateMachine<State>();
    public StateMachine<State> StateMachine { get { return stateMachine; } }

    // �� è�Ǿ� ������ ��� Ŭ������ ������ 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } }
    [SerializeField] LayerMask wallLayer;

    // ���ο� ����Ʈ�� ������
    [SerializeField] arrowSpawn arrowPoint;
    public arrowSpawn ArrowPoint { get { return arrowPoint; } set { arrowPoint = value; } }
    ChampionData targetEnemy;
    public ChampionData TargetEnemy { get { return targetEnemy; } set { targetEnemy = value; } }
    // ���� ��ġ�� �����´�.
    Transform enemyPos;
    public Transform EnemyPos { get { return enemyPos; } set { enemyPos = value; } }

    private bool isAttack;
    public bool IsAttack { get { return isAttack; } }
    private void Start()
    {
        // �� ���µ��� ���¸ӽſ� ����
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Avoid, new AvoidState(this));
        stateMachine.AddState(State.Die, new DieState(this));
        // ù ���¸� ������
        stateMachine.Start(State.Find);

    }

    private void Update()
    {
        // ���°� ���Ҷ����� Ȯ������
        stateMachine.Update();
    }

    private void OnTriggerEnter(Collider other)
    {

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
        {
            StopCoroutine(attackRouine);
        }
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (EnemyPos)
        {
            if (stateMachine.CheckState(State.Attack))
            {
                isAttack = false;
                // ���� �ִϸ��̼��� ��������
                data.animator.Play("Attack");
                if (arrowPoint != null)
                {
                    PooledObject pooledObject = arrowPoint.GetPool(arrowPoint.transform.position, arrowPoint.transform.rotation);
                }
                yield return new WaitForSeconds(data.attackTime);
                isAttack = true;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return null;
            }
        }

    }
    public void HitDamag()
    {
        enemyPos.GetComponent<ChampionData>().hp -= data.damage;
    }

    private class ChampionState : BaseState<State>
    {

        protected LongChampionController controller;


        protected Vector3 dir;

        protected Vector3 startPos;

        // ���ο����ΰ� ���� ��� Ȱ ������ ���� ��������� ������
        protected Vector3 arrowPos;
        protected bool isOK;
        // ���⼭ �ʱ�ȭ
        public ChampionState(LongChampionController owner)
        {
            this.controller = owner;
        }

    }
    // �������ִ� ����
    private class IdleState : ChampionState
    {
        public IdleState(LongChampionController owner) : base(owner)
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
        public FindState(LongChampionController owner) : base(owner)
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
                controller.EnemyPos = controller.Enemy[0].transform;
                controller.targetEnemy = controller.Enemy[0];
            }
            // �̷��� ������ ��� ���
            if (controller.EnemyPos == null)
                controller.stateMachine.ChangeState(State.Idle);
            // ����� �� ã��
            foreach (ChampionData a in controller.Enemy)
            {
                if (controller.EnemyPos == null)
                    continue;
                if (a == null)
                {
                    controller.Enemy.Remove(a);
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
        public MoveState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            // ����� ģ���ѿ� �Ÿ� ���
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
            controller.transform.Translate(dir * controller.data.speed * Time.deltaTime);

        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            else if (controller.enemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
                            }
            // ��Ÿ��ȿ� �������� ��������
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
        public override void Exit()
        {
            if (controller.Enemy == null)
            {
                controller.StopAttack();
            }
            if (controller.targetEnemy == null)
            {
                controller.Enemy.Remove(controller.targetEnemy);
            }
        }
    }
    private class AttackState : ChampionState
    {
        public AttackState(LongChampionController owner) : base(owner)
        {

        }

        // ������ �׼�
        public override void Enter()
        {
            // ������Ʈ�� flipx�� ���¸� Ȯ��
            isOK = controller.gameObject.GetComponent<SpriteRenderer>().flipX;

            arrowPos = controller.ArrowPoint.transform.localPosition;
            // ������Ʈ�� flipx�� ���� ȭ���� ������ point�� �ٲ���
            if (isOK == true)
            {
                if (controller.arrowPoint.transform.localPosition.x > 0)
                    controller.arrowPoint.transform.localPosition = -arrowPos;
                else
                    controller.arrowPoint.transform.localPosition = arrowPos;
            }
            else if (isOK == false)
            {
                if (controller.arrowPoint.transform.localPosition.x < 0)
                    controller.arrowPoint.transform.localPosition = -arrowPos;
                else
                    controller.arrowPoint.transform.localPosition = arrowPos;
            }
            // �ڷ�ƾ�� ��������� ȭ���� ������ ���ư�
            controller.Attack();
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            if (controller.enemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            // ����� �� �Ÿ��� ȸ�ǰŸ��ȿ� ������ �Ѵ붧���� ���� �ð���ŭ ��ٷȴٰ� ���� ȸ�ǻ��·� �ٲ���
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= 3f && controller.IsAttack)
            {
                controller.stateMachine.ChangeState(State.Avoid);
            }

        }
        public override void Exit()
        {
            if (controller.Enemy == null)
            {
                controller.StopAttack();
            }
            if (controller.targetEnemy == null)
            {
                controller.Enemy.Remove(controller.targetEnemy);
            }
        }
    }
    //ȸ�ǰ���
    private class AvoidState : ChampionState
    {
        bool curnt;
        RaycastHit2D hit;
        Vector3 hitDir;
        public AvoidState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {

            controller.data.animator.Play("Move");
            startPos = controller.transform.position;
            dir = (controller.enemyPos.position - startPos).normalized;
            hit = Physics2D.Raycast(startPos, -dir, 3f, controller.wallLayer);
            if (hit)
            {
                hitDir = ((Vector3)hit.point - controller.transform.position).normalized;
            }
        }
        public override void Update()
        {
            //�ݴ�� �̵� (�󸶳�?)
            if (hit)
            {
                Vector3 perpendicular = new Vector3(hitDir.y, -hitDir.x, hitDir.z);
                perpendicular.Normalize();
                controller.transform.Translate(perpendicular * controller.data.speed * Time.deltaTime);
                if (perpendicular.x < 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

                }
                else if (perpendicular.x > 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
            else
            {
                controller.transform.Translate(-dir * controller.data.speed * Time.deltaTime);
                if (-dir.x < 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

                }
                else if (-dir.x > 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
            // ���� �� ���̿� x�� 0���� ������ ������ ���� ũ�� �����ʺ����Ѵ�.
            // ���⼱ �� �ݴ������� ���ߵǴ� -���ش�.

            curnt = controller.gameObject.GetComponent<SpriteRenderer>().flipX;
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
            else if (Vector3.Distance(controller.transform.position, startPos) >= 2)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
        public override void Exit()
        {
            // ������ �����Ŀ� ���� ���� ������ �ݴ������ ����ش�
            if (curnt == true)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
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
        public DieState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
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

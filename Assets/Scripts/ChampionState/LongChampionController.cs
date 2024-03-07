using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class LongChampionController : MonoBehaviour
{
    // ���µ� ��Ƶ�
    public enum State { Idle, Find, Move, Attack, Avoid, Die }

    // ���µ� ������ ���¸ӽ��� ������
    private StateMachine<State> stateMachine = new StateMachine<State>();

    // �� è�Ǿ� ������ ��� Ŭ������ ������ 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }

    // ���濡 ���ӿ�����Ʈ�� ������
    [SerializeField] GameObject enemy;
    public GameObject Enemy { get { return enemy; } set { enemy = value; } }



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
    public void Attack()
    {
        // ���� �ڷ�ƾ�� ����ϱ� ���� ����
        attackRouine = StartCoroutine(AttackRoutine());
    }
    public void StopAttack()
    {
        // ������ ���߰� ����
        StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (Enemy.gameObject)
        {
            // ���� �ִϸ��̼��� ��������
            data.animator.Play("Attack");
            // ���õɋ����� ü���� ����
            Enemy.GetComponent<ChampionData>().hp -= data.damage;
            // �� ���ø��� �ð��� ��
            yield return new WaitForSeconds(data.attackTime);

        }
    }
    private class ChampionState : BaseState<State>
    {

        protected LongChampionController controller;
        // ���� ��ġ�� �����´�.
        protected Transform enemyPos;

        protected Vector3 dir;

        protected Vector3 startPos;
        // ���⼭ �ʱ�ȭ
        public ChampionState(LongChampionController owner)
        {
            this.controller = owner;
            enemyPos = owner.Enemy.transform;
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
            // ���� ������ ��ġ ���� ���ӿ�����Ʈ�� ��ġ�� �������� ������ ������ �ִ� ���·� ����.
            if (controller.Enemy == true)
            {
                enemyPos = controller.Enemy.transform;
            }
            else
            {
                controller.stateMachine.ChangeState(State.Idle);
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
            if (enemyPos != null)
            {
                controller.stateMachine.ChangeState(State.Move);
            }
            else
            {
                controller.stateMachine.ChangeState(State.Idle);
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
            dir = (enemyPos.position - controller.transform.position).normalized;

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

            // ��Ÿ��ȿ� �������� ��������
            if (Vector3.Distance(enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
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
            controller.data.avoidcool = 0;
            controller.Attack();
        }
        public override void Update()
        {
            controller.data.avoidcool += Time.deltaTime;
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            // ����� �� �Ÿ��� ȸ�ǰŸ��ȿ� �������� ȸ�ǻ��·� �ٲ���
            if (Vector3.Distance(enemyPos.position, controller.transform.position) <= controller.data.range && controller.data.avoidcool > controller.data.attackTime)
            {
                controller.StopAttack();
                controller.stateMachine.ChangeState(State.Avoid);
            }
            // ���������� �ȋ�����
            if (controller.Enemy == true && Vector3.Distance(enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                controller.StopAttack();
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }
    //ȸ�ǰ���
    private class AvoidState : ChampionState
    {
        bool curnt;
        public AvoidState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            startPos = controller.transform.position;
            dir = (enemyPos.position - controller.transform.position).normalized;
        }
        public override void Update()
        {
            //�ݴ�� �̵� (�󸶳�?)
            controller.transform.Translate(-dir * controller.data.speed * Time.deltaTime);

            // ���� �� ���̿� x�� 0���� ������ ������ ���� ũ�� �����ʺ����Ѵ�.
            // ���⼱ �� �ݴ������� ���ߵǴ� -���ش�.
            if (-dir.x < 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

            }
            else if (-dir.x > 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            curnt = controller.gameObject.GetComponent<SpriteRenderer>().flipX;
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
        }

        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }

            if (Vector3.Distance(controller.transform.position, startPos) >= 2 && controller.Enemy != null)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
            else if (controller.Enemy == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }
    // ������
    private class DieState : ChampionState
    {
        public DieState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            // ���δ�.
            controller.data.animator.Play("Die");
            Destroy(controller.gameObject, 1f);
        }

    }


}

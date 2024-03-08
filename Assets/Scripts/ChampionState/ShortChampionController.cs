using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class ShortChampionController : MonoBehaviour
{
    // ���µ� ��Ƶ�
    public enum State { Idle, Find, Move, Attack, Die }

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
        if(attackRouine !=  null)
        StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (Enemy.gameObject)
        {
            
            // ���õɋ����� ü���� ����
            Enemy.GetComponent<ChampionData>().hp -= data.damage;
            // �� ���ø��� �ð��� ��
            yield return new WaitForSeconds(data.attackTime);

        }
    }
    private class ChampionState : BaseState<State>
    {

        protected ShortChampionController controller;
        // ���� ��ġ�� �����´�.
        protected Transform enemyPos;

        protected Vector3 dir;

        // ���⼭ �ʱ�ȭ
        public ChampionState(ShortChampionController owner)
        {
            this.controller = owner;
            enemyPos = owner.Enemy.transform;
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
        public MoveState(ShortChampionController owner) : base(owner)
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
        public AttackState(ShortChampionController owner) : base(owner)
        {
        }

        // ������ �׼�
        public override void Enter()
        {
            // ���� �ִϸ��̼��� ��������
            // �ִϸ��̼� �̺�Ʈ�� ���� ���� �ڷ�ƾ ����
            controller.data.animator.Play("Attack");
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {

                controller.stateMachine.ChangeState(State.Die);
            }

            // ���������� �ȋ�����
            if (controller.Enemy == true && Vector3.Distance(enemyPos.position, controller.transform.position) >= controller.data.range)
            {

                controller.stateMachine.ChangeState(State.Idle);
            }
        }
        public override void Exit()
        {
            controller.StopAttack();
        }
    }

    // ������
    private class DieState : ChampionState
    {
        public DieState(ShortChampionController owner) : base(owner)
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

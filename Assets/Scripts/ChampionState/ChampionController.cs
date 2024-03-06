using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class ChampionController : MonoBehaviour
{
    public enum State { Idle, Find, Move, Attack, Die }

    private StateMachine<State> stateMachine = new StateMachine<State>();

    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }

    [SerializeField] GameObject enemy;
    public GameObject Enemy { get { return enemy; } set { enemy = value; } }


    private void Start()
    {
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Die, new DieState(this));


        stateMachine.Start(State.Find);
    }

    private void Update()
    {
        stateMachine.Update();
    }
    public void Attack()
    {
        attackRouine = StartCoroutine(AttackRoutine());

    }
    public void StopAttack()
    {
        StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (Enemy.gameObject)
        {
            data.animator.Play("Attack");

            Debug.Log("�̳� ������.");
            Enemy.GetComponent<ChampionData>().hp -= data.damage;
            yield return new WaitForSeconds(data.attackTime);

        }
    }
    private class ChampionState : BaseState<State>
    {

        protected ChampionController controller;
        // ���� ��ġ�� �����´�.
        protected Transform enemyPos;

        protected Vector3 dir;

        // ���⼭ �ʱ�ȭ
        public ChampionState(ChampionController owner)
        {
            this.controller = owner;
            enemyPos = owner.Enemy.transform;
        }
    }
    // �������ִ� ����
    private class IdleState : ChampionState
    {
        public IdleState(ChampionController owner) : base(owner)
        {
        }
        public override void Update()
        {

            if (controller.data.hp >= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
        }
        public override void Transition()
        {
            // ���� ã�� ���·� ��ȯ���ش�.
            controller.stateMachine.ChangeState(State.Find);
        }
    }
    private class FindState : ChampionState
    {
        public FindState(ChampionController owner) : base(owner)
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

        public override void Update()
        {
            // ��������
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
        }
        public override void Transition()
        {
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
        public MoveState(ChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
        }

        public override void Update()
        {

            // �������� �̵�
            dir = (enemyPos.position - controller.transform.position).normalized;
            controller.transform.Translate(dir * controller.data.speed * Time.deltaTime);
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
        }
        public override void Transition()
        {
            // ��Ÿ��ȿ� �������� ��������
            if (Vector3.Distance(enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
    }
    private class AttackState : ChampionState
    {
        public AttackState(ChampionController owner) : base(owner)
        {
        }

        // ������ �׼�
        public override void Enter()
        {

            Debug.Log("������.");
            controller.Attack();
        }
        public override void Update()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }

            // ���������� �ȋ�����
            if (controller.Enemy == true && Vector3.Distance(enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                Debug.Log("������");
                controller.StopAttack();
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }

    // ������
    private class DieState : ChampionState
    {
        public DieState(ChampionController owner) : base(owner)
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

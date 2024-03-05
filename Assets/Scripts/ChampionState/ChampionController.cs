using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionController : MonoBehaviour
{
    public enum State { Find, Move, Attack, Hit, Die }

    private int hp;
    public int HP {  get { return hp; } set { hp = value; } }

    private bool red;
    public bool Red { get { return red; } set { red = value; } }

    private int damage;
    public int Damage { get { return damage; } set { damage = value; } }

    private int range;
    public int Range { get { return range; } set { range = value; } }

    private StateMachine<State> stateMachine = new StateMachine<State>();

    private void Start()
    {
        stateMachine.AddState(State.Find, new FindState());
        stateMachine.AddState(State.Move, new MoveState());
        stateMachine.AddState(State.Attack, new AttackState());
        stateMachine.AddState(State.Hit, new HitState());
        stateMachine.AddState(State.Die, new DieState());

        stateMachine.Start(State.Find);
    }

    private void Update()
    {
        stateMachine.Update();
    }
}

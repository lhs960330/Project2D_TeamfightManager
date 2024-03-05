using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionManager : Singleton<ChampionManager>
{
    int hp;
    
    public enum State { Idle, Move, Attack, Die}
}

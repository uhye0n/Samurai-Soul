using UnityEngine;
using System.Collections;


public abstract class BossState
{
    protected Boss boss;
    protected float stateTimer;

    public BossState(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

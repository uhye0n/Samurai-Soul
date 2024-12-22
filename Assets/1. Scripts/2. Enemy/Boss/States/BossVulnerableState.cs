using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class BossVulnerableState : BossState
{
    public BossVulnerableState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.SetInvulnerable(false);
        stateTimer = boss.vulnerablePhaseTime;
        
        // 전투 위치로 이동
        boss.transform.position = boss.battlePosition;
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        
        if (stateTimer <= 0)
        {
            boss.SetState(new BossInvulnerableState(boss));
        }
    }
}

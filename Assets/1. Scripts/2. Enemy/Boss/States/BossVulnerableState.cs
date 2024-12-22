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
        
        // 전투 위치로 이동 시 충돌 비활성화
        boss.SetCollisionEnabled(false);
        boss.transform.position = boss.battlePosition;
        // 이동 완료 후 충돌 활성화
        boss.SetCollisionEnabled(true);
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

using System.Collections.Generic;
using UnityEngine;

public class PlayerDraw : PlayerBehaviour
{
    private Coroutine attackCoroutine;
    private float currentTime = 0f;
    private float attackTime = 0.3f;

    public PlayerDraw(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetBool("inDraw", true);
        attackCoroutine = player.StartCoroutine(Function.DelayedAction(1f, "None",
                () => {},
                () => {playerCombat.isAttacking = false;},
                () => {
                    playerCombat.isAttacking = true; 
                    playerCombat.isAttackMoving = true;
                    playerCombat.Attack(1f);  // 여기에 공격 추가
                }));
    }

    public override void CheckInput()
    {
        base.CheckInput();
    }

    public override void CheckState()
    {
        base.CheckState();
        if (!playerCombat.isAttacking)
        {
            Function.SetBehaviour(player, new PlayerBattle(player));
        }
    }

    public override void Perform()
    {
        base.Perform();

        if (playerCombat.isAttacking && playerCombat.isAttackMoving && currentTime < attackTime)
        {
            playerCombat.AttackMove(5f);
            currentTime += Time.deltaTime;
        }
        else if (playerCombat.isAttacking && playerCombat.isAttackMoving && currentTime >= attackTime)
        {
            playerCombat.isAttackMoving = false;
            currentTime = 0f;
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.an.SetBool("inDraw", false);

        // 공격이 적중했을 때만 콤보 증가
        if (playerCombat.IsAttackHit())
        {
            playerCombat.RegisterHit();
            Debug.Log("Attack Hit - Combo Increased");
        }
        else
        {
            playerCombat.ResetCombo();
            Debug.Log("Attack Missed - Combo Reset");
        }
    }
}

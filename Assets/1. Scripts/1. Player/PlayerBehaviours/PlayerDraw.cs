using System.Collections.Generic;
using UnityEngine;

public class PlayerDraw : PlayerBehaviour
{
    private Coroutine attackCoroutine;
    private bool isAttacking = false;
    private bool isAttackMoving = false;
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
                () => {isAttacking = false;},
                () => {isAttacking = true; isAttackMoving = true;}));
    }

    public override void CheckInput()
    {
        base.CheckInput();
    }

    public override void CheckState()
    {
        base.CheckState();
        if (!isAttacking)
        {
            Function.SetBehaviour(player, new PlayerBattle(player));
        }
    }

    public override void Perform()
    {
        base.Perform();

        if (isAttacking && isAttackMoving && currentTime < attackTime)
        {
            playerCombat.AttackMove(12.5f);
            currentTime += Time.deltaTime;
        }
        else if (isAttacking && isAttackMoving && currentTime >= attackTime)
        {
            isAttackMoving = false;
            currentTime = 0f;
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.an.SetBool("inDraw", false);
        playerCombat.wasAttacking = true;
        playerCombat.comboStack += 1;
    }
}

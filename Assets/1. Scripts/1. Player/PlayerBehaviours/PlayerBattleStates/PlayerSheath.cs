using System.Collections.Generic;
using UnityEngine;

public class PlayerSheath : PlayerBehaviour
{
    private int lowerBodyLayerIndex = 1;
    private bool canAttack = true;
    private bool attackDone = true;
    private Coroutine readyCoroutine;

    public PlayerSheath(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();

        if (playerCombat.comboStack % 3 == 0 && playerCombat.comboStack > 0)
        {
            Function.SetBehaviour(player, new PlayerCommandReady(player));
        }

        player.an.SetBool("inSheath", true);
        player.an.SetLayerWeight(lowerBodyLayerIndex, 0.75f);
        readyCoroutine = player.StartCoroutine(Function.DelayedAction(0.5f, "None",
                () => {},
                () => {canAttack = true;},
                () => {canAttack = false; attackDone = false;}));
    }

    public override void CheckInput()
    {
        base.CheckInput();
    }

    public override void CheckState()
    {
        base.CheckState();

        player.an.SetFloat("MoveSpeed", playerMove.moveVector.magnitude);

        if (playerCombat.detectedEnemies.Count <= 0)
        {
            Function.SetBehaviour(player, new PlayerIdle(player));
        }

        if (playerCombat.detectedEnemies.Count > 0)
        {
            playerCombat.currentTarget = playerCombat.GetClosestEnemy();
            playerCombat.LookAtTarget(playerCombat.currentTarget);
            
            if (playerCombat.AttackCheck() && canAttack && !attackDone)
            {
                attackDone = true;
                Function.SetBehaviour(player, new PlayerDraw(player));
            }
        }
    }

    public override void Perform()
    {
        base.Perform();
        playerMove.GroundMove(playerStats.walkSpeed);
        playerCombat.UpdateMovementAnimation();
    }

    public override void Exit()
    {
        base.Exit();
        player.an.SetBool("inSheath", false);
        player.an.SetLayerWeight(lowerBodyLayerIndex, 0);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerReady : PlayerBehaviour
{
    private int lowerBodyLayerIndex = 1;

    public PlayerReady(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetBool("isReady", true);
        player.an.SetLayerWeight(lowerBodyLayerIndex, 0.75f);
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
            
            if (playerCombat.AttackCheck())
            {
                Function.SetBehaviour(player, new PlayerAttack(player));
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
        player.an.SetBool("isReady", false);
        player.an.SetLayerWeight(lowerBodyLayerIndex, 0);
    }
}

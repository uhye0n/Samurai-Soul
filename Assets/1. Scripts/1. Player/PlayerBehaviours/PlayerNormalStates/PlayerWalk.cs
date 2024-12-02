using UnityEngine;

public class PlayerWalk : PlayerBehaviour
{
    public PlayerWalk(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void CheckInput()
    {
        base.CheckInput();
    }

    public override void CheckState()
    {
        base.CheckState();
        
        player.an.SetFloat("MoveSpeed", player.playerMove.moveVector.magnitude);

        if (playerCombat.detectedEnemies.Count > 0)
        {
            Function.SetBehaviour(player, new PlayerBattle(player));
        }
        else if (playerMove.horizontalAxis == 0 && playerMove.verticalAxis == 0)
        {
            Function.SetBehaviour(player, new PlayerIdle(player));
        }
    }

    public override void Perform()
    {
        base.Perform();
        playerMove.Rotation();
        playerMove.GroundMove(playerStats.runSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

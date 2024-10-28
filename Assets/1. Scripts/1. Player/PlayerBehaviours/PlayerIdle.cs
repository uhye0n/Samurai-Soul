using UnityEngine;

public class PlayerIdle : PlayerBehaviour
{
    public PlayerIdle(Player player) : base(player)
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
        
        player.an.SetFloat("MoveSpeed", 0f);

        if (playerCombat.detectedEnemies.Count > 0)
        {
            Function.SetBehaviour(player, new PlayerBattle(player));
        }
        else if (playerMove.horizontalAxis != 0 || playerMove.verticalAxis != 0)
        {
            Function.SetBehaviour(player, new PlayerWalk(player));
        }
    }

    public override void Perform()
    {
        base.Perform();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

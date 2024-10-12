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
        player.an.SetFloat("MoveSpeed", 0f);

        base.CheckState();

        if (playerMove.horizontalAxis != 0 || playerMove.verticalAxis != 0)
        {
            Function.SetBehaviour(player, new PlayerWalk(player));
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Function.SetBehaviour(player, new PlayerCombat(player));
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

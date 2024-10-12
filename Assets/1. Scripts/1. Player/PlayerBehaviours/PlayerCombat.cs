using UnityEngine;

public class PlayerCombat : PlayerBehaviour
{
    public PlayerCombat(Player player) : base(player)
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
        player.an.SetFloat("MoveSpeed", player.playerMove.moveVector.magnitude);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Function.SetBehaviour(player, new PlayerIdle(player));
        }
    }

    public override void Perform()
    {
        base.Perform();
        playerMove.GroundMove();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

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

        playerMove.horizontalAxis = Input.GetAxisRaw("Horizontal");
        playerMove.verticalAxis = Input.GetAxisRaw("Vertical");
    }

    public override void CheckState()
    {
        base.CheckState();
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

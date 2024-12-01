using System.Collections.Generic;
using UnityEngine;

public class PlayerCommandReady : PlayerBehaviour
{
    private Coroutine commandReadyCoroutine;
    private bool canCommand;

    public PlayerCommandReady(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetBool("isReady", true);
        commandReadyCoroutine = player.StartCoroutine(Function.DelayedAction(3f, "None",
                () => {},
                () => {canCommand = false;},
                () => {canCommand = true;}));
    }

    public override void CheckInput()
    {
        base.CheckInput();
    }

    public override void CheckState()
    {
        base.CheckState();
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

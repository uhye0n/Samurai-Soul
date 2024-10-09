using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour
{
    protected Player player;
    protected readonly string playerAnimation;
    protected PlayerInput playerInput;
    protected PlayerMove playerMove;
    protected PlayerStats playerStats;
    protected PlayerCombat playerCombat;


    public PlayerBehaviour(Player player)
    {
        this.player = player;
        playerAnimation = GetType().Name;
        playerInput = player.playerInput;
        playerMove = player.playerMove;
        playerStats = player.playerStats;
        playerCombat = player.playerCombat;
    }

    public virtual void Enter()
    {

    }

    public virtual void CheckInput()
    {
        playerInput.TouchInput();
    }

    public virtual void CheckState()
    {

    }

    public virtual void Perform()
    {

    }

    public virtual void Exit()
    {

    }
}

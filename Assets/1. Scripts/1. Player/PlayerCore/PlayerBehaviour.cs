using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour
{
    protected Player player;
    protected PlayerInput playerInput;
    protected PlayerMove playerMove;
    protected PlayerStats playerStats;
    protected PlayerCombat playerCombat;


    public PlayerBehaviour(Player player)
    {
        this.player = player;
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
        playerCombat.DetectEnemies();
    }

    public virtual void Perform()
    {

    }

    public virtual void Exit()
    {

    }
}

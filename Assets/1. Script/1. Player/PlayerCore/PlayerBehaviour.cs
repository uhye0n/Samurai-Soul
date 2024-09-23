using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour
{
    protected Player player;
    protected PlayerMove playerMove;
    protected PlayerStats playerStats;
    protected PlayerCombat playerCombat;

    public PlayerBehaviour(Player player)
    {
        this.player = player;
        playerMove = player.playerMove;
        playerStats = player.playerStats;
        playerCombat = player.playerCombat;
    }

    public virtual void Enter()
    {

    }

    public virtual void CheckInput()
    {

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

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {

    }
}

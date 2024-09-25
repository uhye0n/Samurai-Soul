using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour
{
    protected Player player;
    protected readonly string playerAnimation;
    protected PlayerMove playerMove;
    protected PlayerStats playerStats;
    protected PlayerCombat playerCombat;


    public PlayerBehaviour(Player player)
    {
        this.player = player;
        playerAnimation = GetType().Name;
        playerMove = player.playerMove;
        playerStats = player.playerStats;
        playerCombat = player.playerCombat;
    }

    public virtual void Enter()
    {
        player.an.SetBool(playerAnimation, true);
    }

        public virtual void CheckInput()
        {
            playerMove.horizontalAxis = Input.GetAxisRaw("Horizontal");
            playerMove.verticalAxis = Input.GetAxisRaw("Vertical");
        }

    public virtual void CheckState()
    {

    }

    public virtual void Perform()
    {

    }

    public virtual void Exit()
    {
        player.an.SetBool(playerAnimation, false);
    }
}

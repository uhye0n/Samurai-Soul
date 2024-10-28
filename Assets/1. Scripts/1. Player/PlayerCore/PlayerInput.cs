using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private Player player;

    public void Initialize(Player player)
    {
        this.player = player;
    }
    
    public void TouchInput()
    {
        player.playerMove.horizontalAxis = player.variableJoystick.Horizontal;
        player.playerMove.verticalAxis = player.variableJoystick.Vertical;
    }
}

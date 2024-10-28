using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private Player player;

    public float runSpeed = 3.5f;
    public float walkSpeed = 2f;
    
    public void Initialize(Player player)
    {
        this.player = player;
    }
}

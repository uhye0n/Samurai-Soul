using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    private Player player;

    public float speed = 5f;
    public float horizontalAxis;
    public float verticalAxis;
    public Vector3 moveVector;

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void GroundMove()
    {   
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
        player.transform.position += speed * Time.deltaTime * moveVector;
    }

    public void Rotation()
    {
        player.transform.LookAt(player.transform.position + moveVector);
    }
}

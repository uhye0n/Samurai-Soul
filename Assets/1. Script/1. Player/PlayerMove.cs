using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
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

    public void CheckInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
    }

    public void GroundMove()
    {   
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
        transform.position += speed * Time.deltaTime * moveVector;
    }
}

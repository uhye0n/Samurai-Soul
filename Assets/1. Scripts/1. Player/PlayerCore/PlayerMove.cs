using System.Collections;
using UnityEngine;

public class PlayerMove
{
    private Player player;

    public float horizontalAxis;
    public float verticalAxis;
    public Vector3 moveVector;

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void GroundMove(float speed)
    {
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
        player.transform.position += speed * Time.deltaTime * moveVector;
    }

    public void Rotation()
    {
        if (moveVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVector);
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, 1000 * Time.deltaTime);
        }
    }
}


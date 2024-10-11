using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private Player player;

    // private Vector2 touchStartPos;
    // private Vector2 touchCurrentPos;

    public void Initialize(Player player)
    {
        this.player = player;
    }
    
    public void TouchInput()
    {
        player.playerMove.horizontalAxis = player.variableJoystick.Horizontal;
        player.playerMove.verticalAxis = player.variableJoystick.Vertical;
        Debug.Log($"Horizontal: {player.playerMove.horizontalAxis}, Vertical: {player.playerMove.verticalAxis}");
    }

    // public void TouchInput()
    // {
    //     // 테스트용 마우스 입력 처리
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         touchStartPos = Input.mousePosition;
    //     }
    //     else if (Input.GetMouseButton(0))
    //     {
    //         touchCurrentPos = Input.mousePosition;
    //         Vector2 moveDirection = (touchCurrentPos - touchStartPos).normalized;
    //         player.playerMove.horizontalAxis = moveDirection.x;
    //         player.playerMove.verticalAxis = moveDirection.y;
    //     }
    //     else if (Input.GetMouseButtonUp(0))
    //     {
    //         player.playerMove.horizontalAxis = 0;
    //         player.playerMove.verticalAxis = 0;
    //     }

    //     // 실제 터치 입력 처리
    //     if (Input.touchCount > 0)
    //     {
    //         Touch touch = Input.GetTouch(0);
    //         if (touch.phase == TouchPhase.Began)
    //         {
    //             touchStartPos = touch.position;
    //         }
    //         else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
    //         {
    //             touchCurrentPos = touch.position;
    //             Vector2 moveDirection = (touchCurrentPos - touchStartPos).normalized;
    //             player.playerMove.horizontalAxis = moveDirection.x;
    //             player.playerMove.verticalAxis = moveDirection.y;
    //         }
    //         else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
    //         {
    //             player.playerMove.horizontalAxis = 0;
    //             player.playerMove.verticalAxis = 0;
    //         }
    //     }
    // }

}

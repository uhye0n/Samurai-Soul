using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private Player player;
    private List<Vector2> inputPattern = new List<Vector2>();
    private float patternRecordInterval = 0.1f; // 0.1초마다 위치 기록
    private float lastRecordTime = 0f;
    private bool isCommandMode = false;
    private Vector2 lastTouchPosition;

    public void Initialize(Player player)
    {
        this.player = player;
    }
    
    public List<Vector2> GetInputPattern() => inputPattern;
    public void ClearInputPattern() => inputPattern.Clear();
    
    public void SetCommandMode(bool value)
    {
        isCommandMode = value;
        ClearInputPattern();
    }
    
    public void TouchInput()
    {
        if (!isCommandMode)
        {
            // 일반 조이스틱 입력
            player.playerMove.horizontalAxis = player.variableJoystick.Horizontal;
            player.playerMove.verticalAxis = player.variableJoystick.Vertical;
        }
        else
        {
            // 터치 또는 마우스 드래그 입력
#if UNITY_EDITOR
            // 에디터에서 마우스 입력 사용
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 touchPos = Input.mousePosition;
                StartTouch(touchPos);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 touchPos = Input.mousePosition;
                MoveTouch(touchPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndTouch();
            }
#else
            // 실제 기기에서 터치 입력 사용
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = touch.position;

                if (touch.phase == TouchPhase.Began)
                {
                    StartTouch(touchPos);
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    MoveTouch(touchPos);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    EndTouch();
                }
            }
#endif
        }
    }

    private void StartTouch(Vector2 touchPos)
    {
        lastTouchPosition = touchPos;
        ClearInputPattern();
        (player.playerBehaviour as PlayerCommandReady)?.UpdateLine(touchPos);
    }

    private void MoveTouch(Vector2 touchPos)
    {
        if (Vector2.Distance(touchPos, lastTouchPosition) > 10f)
        {
            Vector2 direction = (touchPos - lastTouchPosition).normalized;
            inputPattern.Add(direction);
            lastTouchPosition = touchPos;
            (player.playerBehaviour as PlayerCommandReady)?.UpdateLine(touchPos);
        }
    }

    private void EndTouch()
    {
        inputPattern.Add(Vector2.zero); // 패턴 종료 표시
    }
}

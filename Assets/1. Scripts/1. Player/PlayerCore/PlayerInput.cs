using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private Player player;
    private List<Vector2> inputPattern = new List<Vector2>();
    private bool isCommandMode = false;
    private Vector2 lastJoystickInput;
    private float minMoveDistance = 0.3f; // 조이스틱 입력 최소 거리
    private float inputDelay = 0.1f; // 입력 기록 간격
    private float lastInputTime = 0f;
    private Vector2 patternStartPosition;
    private float patternCompleteThreshold = 0.3f; // 시작 위치와의 거리 체크용 임계값

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
        if (value)
        {
            patternStartPosition = new Vector2(player.variableJoystick.Horizontal, player.variableJoystick.Vertical);
        }
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
            // 패턴 인식을 위한 조이스틱 입력 기록
            Vector2 currentInput = new Vector2(player.variableJoystick.Horizontal, player.variableJoystick.Vertical);
            
            if (currentInput.magnitude > minMoveDistance && Time.time - lastInputTime > inputDelay)
            {
                inputPattern.Add(currentInput);
                lastInputTime = Time.time;
                lastJoystickInput = currentInput;

                // 시작 위치로 돌아왔는지 체크
                if (inputPattern.Count > 3 && Vector2.Distance(currentInput, patternStartPosition) < patternCompleteThreshold)
                {
                    inputPattern.Add(Vector2.zero); // 패턴 완료 표시
                }
            }
            else if (currentInput.magnitude < 0.1f && lastJoystickInput.magnitude > 0.1f)
            {
                // 조이스틱을 놓았을 때 패턴 종료 표시
                inputPattern.Add(Vector2.zero);
            }
        }
    }
}

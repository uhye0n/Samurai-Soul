using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private Player player;
    private List<Vector2> inputPattern = new List<Vector2>();
    private bool isCommandMode = false;
    private bool hasStartedRecording = false;  // 패턴 기록 시작 여부
    private Vector2 lastJoystickInput;
    private float minMoveDistance = 0.3f; // 조이스틱 입력 최소 거리
    private float inputDelay = 0.1f; // 입력 기록 간격
    private float lastInputTime = 0f;
    private Vector2 patternStartPosition;
    private float patternCompleteThreshold = 0.3f; // 시작 위치와의 거리 체크용 임계값
    private const int MIN_PATTERN_POINTS = 8;  // 최소 패턴 포인트 수
    private const int MAX_PATTERN_POINTS = 30; // 최대 패턴 포인트 수
    private float returnThreshold = 0.4f;      // 시작점 복귀 판정 거리
    private bool checkingReturn = false;       // 시작점 복귀 체크 시작 여부

    public void Initialize(Player player)
    {
        this.player = player;
    }
    
    public List<Vector2> GetInputPattern() => inputPattern;
    
    public void ClearInputPattern()
    {
        inputPattern.Clear();
        hasStartedRecording = false;  // 패턴 기록 상태 초기화
    }
    
    public void SetCommandMode(bool value)
    {
        isCommandMode = value;
        ClearInputPattern();
        if (value)
        {
            hasStartedRecording = false;  // 커맨드 모드 시작 시 기록 상태 초기화
        }
    }
    
    public void TouchInput()
    {
        if (!isCommandMode)
        {
            // 일반 조이스틱 입력
            player.playerMove.horizontalAxis = player.variableJoystick.Horizontal;
            player.playerMove.verticalAxis = player.variableJoystick.Vertical;
            return;
        }

        // 현재 조이스틱 입력 가져오기
        Vector2 currentInput = new Vector2(player.variableJoystick.Horizontal, player.variableJoystick.Vertical);

        // 패턴 기록 시작
        if (!hasStartedRecording && currentInput.magnitude > minMoveDistance)
        {
            hasStartedRecording = true;
            checkingReturn = false;
            patternStartPosition = currentInput;
            inputPattern.Add(currentInput);
            lastInputTime = Time.time;
            lastJoystickInput = currentInput;
            Debug.Log("패턴 기록 시작");
            return;
        }

        // 패턴 기록 중
        if (hasStartedRecording)
        {
            if (currentInput.magnitude > minMoveDistance && Time.time - lastInputTime > inputDelay)
            {
                // 최대 입력 수 체크
                if (inputPattern.Count >= MAX_PATTERN_POINTS)
                {
                    Debug.Log($"패턴 실패: 최대 입력 수 초과 ({MAX_PATTERN_POINTS})");
                    player.playerCombat.comboStack = 0;
                    ClearInputPattern();
                    return;
                }

                inputPattern.Add(currentInput);
                lastInputTime = Time.time;
                lastJoystickInput = currentInput;

                // 최소 입력 수 도달 시 복귀 체크 시작
                if (!checkingReturn && inputPattern.Count >= MIN_PATTERN_POINTS)
                {
                    checkingReturn = true;
                    Debug.Log("시작점 복귀 체크 시작");
                }

                // 복귀 체크가 활성화된 경우에만 시작점 복귀 확인
                if (checkingReturn)
                {
                    float distanceToStart = Vector2.Distance(currentInput, patternStartPosition);
                    if (distanceToStart < returnThreshold)
                    {
                        inputPattern.Add(Vector2.zero); // 패턴 완료 표시
                        Debug.Log($"패턴 완료: 포인트 수={inputPattern.Count}, 시작점과의 거리={distanceToStart:F2}");
                        return;
                    }
                }
            }
            else if (currentInput.magnitude < 0.1f && lastJoystickInput.magnitude > 0.1f)
            {
                // 조이스틱을 놓았을 때 패턴 실패로 처리
                Debug.Log($"패턴 실패: 조이스틱 해제 (입력 수: {inputPattern.Count})");
                player.playerCombat.comboStack = 0;
                ClearInputPattern();
            }
        }
    }
}

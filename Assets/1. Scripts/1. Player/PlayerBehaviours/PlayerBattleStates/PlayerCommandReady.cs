using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCommandReady : PlayerSkill
{
    private Coroutine commandReadyCoroutine;
    private bool canCommand;
    private List<Vector3> linePositions = new List<Vector3>();
    private List<Vector2> drawPoints = new List<Vector2>();
    private LineRenderer lineRenderer; // LineRenderer 컴포넌트
    private readonly Color lineColor = new Color(0f, 0.5f, 1f, 0.8f); // 선 색상
    private const int LINE_THICKNESS = 50; // 선 두께를 증가시킵니다.
    private const float FADE_SPEED = 0.5f; // 선이 사라지는 속도

    public PlayerCommandReady(Player player) : base(player)
    {
        InitializeLineRenderer();
    }

    private void InitializeLineRenderer()
    {
        // LineRenderer 설정
        GameObject lineObj = new GameObject("DrawLine");
        lineObj.transform.SetParent(player.drawCanvas.transform, false); // 부모를 DrawCanvas로 설정
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f; // 선의 시작 굵기
        lineRenderer.endWidth = 0.1f;   // 선의 끝 굵기
        lineRenderer.material = new Material(Shader.Find("UI/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.useWorldSpace = false; // UI 캔버스 좌표계 사용
    }

    public void UpdateLine(Vector2 screenPos)
    {
        Vector3 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            player.drawCanvas.transform as RectTransform,
            screenPos,
            player.drawCanvas.worldCamera,
            out Vector2 localPoint);

        localPos = new Vector3(localPoint.x, localPoint.y, 0f);

        linePositions.Add(localPos);
        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetBool("inCommand", true);
        player.variableJoystick.gameObject.SetActive(false);
        player.playerInput.SetCommandMode(true);
        canCommand = true;  // 시작시 즉시 패턴 인식 가능하도록 설정
        linePositions.Clear();
        lineRenderer.enabled = true;
        drawPoints.Clear();
        if (player.drawCanvas != null)
        {
            player.drawCanvas.sortingOrder = 5;  // 임시로 드로잉 캔버스를 최상단으로
        }
    }

    public override void CheckInput()
    {
        base.CheckInput();
        
        if (!canCommand) return;
        
        List<Vector2> pattern = player.playerInput.GetInputPattern();
        
        if (pattern.Count == 0) return;
        
        // 터치가 끝났을 때 패턴 검사 (마지막 입력이 Vector2.zero)
        if (pattern.Count > 5 && pattern[pattern.Count - 1] == Vector2.zero)
        {
            // 마지막 zero 입력 제거
            pattern.RemoveAt(pattern.Count - 1);
            
            if (IsCircularPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerHorizontalSlash(player));
            }
            else if (IsTrianglePattern(pattern))
            {
                // 첫 입력의 y값으로 방향 판단
                bool isUpsideDown = pattern[0].y < 0;
                if (isUpsideDown)
                    Function.SetBehaviour(player, new PlayerThrust(player));
                else
                    Function.SetBehaviour(player, new PlayerThrustSlash(player));
            }
            else if (IsDiagonalSlashPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerDiagonalSlash(player));
            }
            else if (IsHorizontalDiagonalSlashPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerHorizontalDiagonalSlash(player));
            }
            else
            {
                // 패턴 인식 실패시 초기화
                Function.SetBehaviour(player, new PlayerBattle(player));
            }
            
            player.playerInput.ClearInputPattern();
        }
    }

    public override void CheckState()
    {
        base.CheckState();
        if (!canCommand)
        {
            Function.SetBehaviour(player, new PlayerBattle(player));
        }
    }

    public override void Perform()
    {
        base.Perform();
    }

    public override void Exit()
    {
        base.Exit();
        player.an.SetBool("inCommand", false);
        player.variableJoystick.gameObject.SetActive(true);
        player.playerInput.SetCommandMode(false);
        if (commandReadyCoroutine != null)
            player.StopCoroutine(commandReadyCoroutine);
        linePositions.Clear();
        lineRenderer.enabled = false;
        if (player.drawCanvas != null)
        {
            player.drawCanvas.sortingOrder = 1;  // 원래 정렬 순서로 복구
        }
    }

    private bool IsDiagonalSlashPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 6) return false;
        
        Vector2 startDirection = pattern[0];
        Vector2 endDirection = pattern[pattern.Count - 1];
        float totalAngle = Vector2.SignedAngle(startDirection, endDirection);
        
        return Mathf.Abs(totalAngle) >= 150f && Mathf.Abs(totalAngle) <= 210f;
    }

    private bool IsHorizontalDiagonalSlashPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 8) return false;
        
        float totalAngle = 0f;
        for (int i = 1; i < pattern.Count; i++)
        {
            Vector2 v1 = pattern[i - 1];
            Vector2 v2 = pattern[i];
            if (v1.magnitude > 0.1f && v2.magnitude > 0.1f)
            {
                totalAngle += Vector2.SignedAngle(v1, v2);
            }
        }
        
        return Mathf.Abs(totalAngle) >= 330f;
    }
}

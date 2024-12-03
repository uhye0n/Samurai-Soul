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

        if (pattern.Count > 5 && pattern[pattern.Count - 1] == Vector2.zero)
        {
            pattern.RemoveAt(pattern.Count - 1);
            bool patternRecognized = true;

            if (IsCircularPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerHorizontalSlash(player));
            }
            else if (IsDiagonalSlashPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerDiagonalSlash(player));
            }
            else if (IsTrianglePattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerThrust(player));
            }
            else if (IsHorizontalDiagonalSlashPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerHorizontalDiagonalSlash(player));
            }
            else if (IsThrustSlashPattern(pattern))
            {
                Function.SetBehaviour(player, new PlayerThrustSlash(player));
            }
            else
            {
                patternRecognized = false;
                Function.SetBehaviour(player, new PlayerBattle(player));
            }

            if (patternRecognized)
            {
                canCommand = false;
                player.playerInput.SetCommandMode(false);
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
        // 상태 전환 시 조이스틱 입력이 바로 적용되도록 설정
        player.variableJoystick.gameObject.SetActive(true);
    }
}

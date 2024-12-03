using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCommandReady : PlayerSkill
{
    private Coroutine commandReadyCoroutine;
    private bool canCommand;
    private List<Vector3> linePositions = new List<Vector3>();
    private List<Vector2> drawPoints = new List<Vector2>();
    private Texture2D drawTexture;
    private bool isDrawing;
    private readonly Color lineColor = new Color(0f, 0.5f, 1f, 0.8f); // 선 색상
    private const int LINE_THICKNESS = 3; // 선 두께
    private const float FADE_SPEED = 0.5f; // 선이 사라지는 속도

    public PlayerCommandReady(Player player) : base(player)
    {
        InitializeDrawing();
    }

    private void InitializeDrawing()
    {
        drawTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Bilinear;
        player.drawImage.texture = drawTexture;
        ClearDrawing();
    }

    private void ClearDrawing()
    {
        Color clearColor = new Color(0, 0, 0, 0); // 투명한 색상
        Color[] pixels = new Color[drawTexture.width * drawTexture.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clearColor;
        drawTexture.SetPixels(pixels);
        drawTexture.Apply();
    }

    public void UpdateLine(Vector2 screenPos)
    {
        if (!isDrawing)
        {
            isDrawing = true;
            ClearDrawing();
        }

        if (drawPoints.Count > 0)
        {
            Vector2 lastPos = drawPoints[drawPoints.Count - 1];
            DrawLine(lastPos, screenPos, new Color(0f, 0.5f, 1f, 1f)); // 선 색상 (알파값 1)
        }
        drawPoints.Add(screenPos);
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(start, end));
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 pixel = Vector2.Lerp(start, end, t);
            DrawPixel((int)pixel.x, (int)pixel.y, color);
        }
        drawTexture.Apply();
    }

    private void DrawPixel(int x, int y, Color color)
    {
        // 원형 브러쉬로 픽셀 그리기
        for (int i = -LINE_THICKNESS; i <= LINE_THICKNESS; i++)
        {
            for (int j = -LINE_THICKNESS; j <= LINE_THICKNESS; j++)
            {
                int dx = x + i;
                int dy = y + j;
                if (dx >= 0 && dx < drawTexture.width && dy >= 0 && dy < drawTexture.height)
                {
                    float distance = Mathf.Sqrt(i * i + j * j);
                    if (distance <= LINE_THICKNESS)
                    {
                        // 발광 효과를 위해 투명도 조절
                        float alpha = 1.0f - (distance / LINE_THICKNESS);
                        Color pixelColor = Color.white; // 흰색으로 설정
                        pixelColor.a = alpha;
                        drawTexture.SetPixel(dx, dy, pixelColor);
                    }
                }
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetBool("inCommand", true);
        player.variableJoystick.gameObject.SetActive(false);
        player.playerInput.SetCommandMode(true);
        canCommand = true;  // 시작시 즉시 패턴 인식 가능하도록 설정
        linePositions.Clear();
        player.drawImage.gameObject.SetActive(true);
        ClearDrawing();
        drawPoints.Clear();
        isDrawing = false;
        player.drawImage.color = new Color(1, 1, 1, 1); // 알파값을 1로 변경
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
                // 패턴 인식 실패시 ��기화
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
        player.drawImage.gameObject.SetActive(false);
        player.StartCoroutine(FadeOutLine());
        if (player.drawCanvas != null)
        {
            player.drawCanvas.sortingOrder = 1;  // 원래 정렬 순서로 복구
        }
    }

    private IEnumerator<object> FadeOutLine()
    {
        float alpha = 1f;
        while (alpha > 0)
        {
            alpha -= FADE_SPEED * Time.deltaTime;
            player.drawImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        player.drawImage.gameObject.SetActive(false);
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

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

    public PlayerCommandReady(Player player) : base(player)
    {
        InitializeDrawing();
    }

    private void InitializeDrawing()
    {
        drawTexture = new Texture2D(Screen.width, Screen.height);
        drawTexture.filterMode = FilterMode.Bilinear;
        ClearDrawing();
        player.drawImage.texture = drawTexture;
    }

    private void ClearDrawing()
    {
        Color[] pixels = new Color[Screen.width * Screen.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        drawTexture.SetPixels(pixels);
        drawTexture.Apply();
    }

    public void UpdateLine(Vector2 screenPos)
    {
        if (drawPoints.Count > 0)
        {
            Vector2 lastPos = drawPoints[drawPoints.Count - 1];
            DrawLine(lastPos, screenPos, new Color(0f, 0.5f, 1f, 1f));
        }
        drawPoints.Add(screenPos);
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        int steps = (int)Vector2.Distance(start, end);
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)steps;
            Vector2 pixel = Vector2.Lerp(start, end, t);
            DrawPixel((int)pixel.x, (int)pixel.y, color);
        }
        drawTexture.Apply();
    }

    private void DrawPixel(int x, int y, Color color)
    {
        for(int i=-2; i<=2; i++)
            for(int j=-2; j<=2; j++)
                if(x+i >= 0 && x+i < drawTexture.width && y+j >= 0 && y+j < drawTexture.height)
                    drawTexture.SetPixel(x+i, y+j, color);
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

using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : PlayerBehaviour
{
    public PlayerSkill(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void CheckInput()
    {
        base.CheckInput();
    }

    public override void CheckState()
    {
        base.CheckState();
    }

    public override void Perform()
    {
        base.Perform();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected virtual bool IsCircularPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 12) return false;

        float totalAngle = 0f;
        Vector2 center = Vector2.zero;
        
        // 중심점 계산
        foreach (Vector2 point in pattern)
            center += point;
        center /= pattern.Count;

        // 각도 변화 계산
        for (int i = 1; i < pattern.Count; i++)
        {
            Vector2 v1 = (pattern[i - 1] - center).normalized;
            Vector2 v2 = (pattern[i] - center).normalized;
            float angle = Vector2.SignedAngle(v1, v2);
            totalAngle += angle;
        }

        return Mathf.Abs(totalAngle) >= 330f;
    }

    protected virtual bool IsDiagonalSlashPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 8) return false;

        // 시작점에서 위로 이동
        bool startsUpward = pattern[3].y > pattern[0].y;
        
        // 180도 회전 확인
        float totalAngle = 0f;
        for (int i = 4; i < pattern.Count - 1; i++)
        {
            Vector2 v1 = (pattern[i] - pattern[i-1]).normalized;
            Vector2 v2 = (pattern[i+1] - pattern[i]).normalized;
            totalAngle += Vector2.SignedAngle(v1, v2);
        }

        return startsUpward && Mathf.Abs(totalAngle) >= 150f && Mathf.Abs(totalAngle) <= 210f;
    }

    protected virtual bool IsTrianglePattern(List<Vector2> pattern)
    {
        if (pattern.Count < 8) return false;

        int corners = 0;
        float angleThreshold = 60f;
        float totalAngle = 0f;

        for (int i = 1; i < pattern.Count - 1; i++)
        {
            Vector2 v1 = (pattern[i] - pattern[i-1]).normalized;
            Vector2 v2 = (pattern[i+1] - pattern[i]).normalized;
            float angle = Vector2.Angle(v1, v2);
            
            if (angle > angleThreshold)
            {
                corners++;
                totalAngle += angle;
            }
        }

        return corners == 3 && totalAngle >= 240f;
    }

    protected virtual bool IsHorizontalDiagonalSlashPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 12) return false;

        // 시작점에서 위로 이동
        bool startsUpward = pattern[3].y > pattern[0].y;
        
        // 360도 회전 후 아래로 이동
        float totalAngle = 0f;
        for (int i = 4; i < pattern.Count - 4; i++)
        {
            Vector2 v1 = (pattern[i] - pattern[i-1]).normalized;
            Vector2 v2 = (pattern[i+1] - pattern[i]).normalized;
            totalAngle += Vector2.SignedAngle(v1, v2);
        }

        bool endsDownward = pattern[pattern.Count-1].y < pattern[pattern.Count-4].y;

        return startsUpward && Mathf.Abs(totalAngle) >= 330f && endsDownward;
    }

    protected virtual bool IsThrustSlashPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 12) return false;

        // 시작점에서 위로 이동
        bool startsUpward = pattern[3].y > pattern[0].y;
        
        // 중간에 삼각형 패턴
        bool hasTriangle = false;
        int midPoint = pattern.Count / 2;
        List<Vector2> middlePattern = pattern.GetRange(midPoint - 4, 8);
        hasTriangle = IsTrianglePattern(middlePattern);

        // 마지막 아래로 이동
        bool endsDownward = pattern[pattern.Count-1].y < pattern[pattern.Count-4].y;

        return startsUpward && hasTriangle && endsDownward;
    }
}

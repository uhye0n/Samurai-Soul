using System.Collections.Generic;
using System.Linq; // LINQ 추가
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

    // 직선 움직임 감지
    protected bool IsLinearMovement(List<Vector2> points, float tolerance = 0.1f)
    {
        if (points.Count < 2) return true;
        
        Vector2 direction = (points[points.Count - 1] - points[0]).normalized;
        
        for (int i = 1; i < points.Count; i++)
        {
            Vector2 currentDirection = (points[i] - points[i-1]).normalized;
            if (Vector2.Distance(direction, currentDirection) > tolerance)
                return false;
        }
        return true;
    }

    // 회전 움직임 감지
    protected float GetRotationAngle(List<Vector2> points, Vector2 center)
    {
        float totalAngle = 0f;
        for (int i = 1; i < points.Count; i++)
        {
            Vector2 v1 = (points[i - 1] - center).normalized;
            Vector2 v2 = (points[i] - center).normalized;
            totalAngle += Vector2.SignedAngle(v1, v2);
        }
        return totalAngle;
    }

    protected virtual bool IsCircularPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 5) return false;

        // 중심점 계산
        Vector2 center = pattern.Aggregate(Vector2.zero, (acc, p) => acc + p) / pattern.Count;

        // 시작점과 끝점이 가까운지 확인
        float endDistance = Vector2.Distance(pattern[pattern.Count - 1], pattern[0]);
        bool returnsToStart = endDistance < Screen.height * 0.3f;

        // 회전 방향 확인
        float totalAngle = 0f;
        Vector2 prevPoint = pattern[0];
        Vector2 prevDirection = Vector2.zero;

        for (int i = 1; i < pattern.Count; i++)
        {
            Vector2 currentPoint = pattern[i];
            Vector2 currentDirection = (currentPoint - prevPoint).normalized;

            if (prevDirection != Vector2.zero)
            {
                float angle = Vector2.SignedAngle(prevDirection, currentDirection);
                totalAngle += angle;
            }

            prevPoint = currentPoint;
            prevDirection = currentDirection;
        }

        // 디버그 정보
        Debug.Log($"Circle Check - Total Angle: {totalAngle}, End Distance: {endDistance}");

        // 회전각이 일정 이상이고 시작점으로 돌아왔는지 확인
        return Mathf.Abs(totalAngle) >= 180f && returnsToStart;
    }

    protected virtual bool IsTrianglePattern(List<Vector2> pattern)
    {
        if (pattern.Count < 8) return false;

        // 시작점이 하단에 있는지 확인
        float bottomThreshold = Screen.height * 0.3f;
        if (pattern[0].y > bottomThreshold) return false;

        // 급격한 방향 전환 횟수 체크
        int sharpTurns = 0;
        float angleThreshold = 45f; // 좀 더 관대한 각도
        
        for (int i = 3; i < pattern.Count - 3; i++)
        {
            Vector2 v1 = (pattern[i] - pattern[i-3]).normalized;
            Vector2 v2 = (pattern[i+3] - pattern[i]).normalized;
            float angle = Vector2.Angle(v1, v2);
            
            if (angle > angleThreshold)
            {
                sharpTurns++;
                i += 3; // 같은 코너를 중복 감지하지 않도록
            }
        }

        // 대략적인 삼각형 모양 확인 (3번의 방향 전환)
        bool hasThreeTurns = sharpTurns >= 2 && sharpTurns <= 4;

        // 위쪽 꼭지점이 존재하는지 확인
        float maxY = pattern.Max(p => p.y);
        bool hasTopVertex = pattern.Any(p => p.y > (pattern[0].y + maxY) / 2);

        // 시작점으로 돌아왔는지 확인
        float endDistance = Vector2.Distance(pattern[pattern.Count - 1], pattern[0]);
        bool returnsToStart = endDistance < Screen.height * 0.2f;

        return hasThreeTurns && hasTopVertex && returnsToStart;
    }
}

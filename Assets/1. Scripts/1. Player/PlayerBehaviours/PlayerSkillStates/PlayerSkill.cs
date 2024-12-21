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

        // 적의 방향 확인
        Vector3 enemyDirection = Vector3.zero;
        if (player.playerCombat.currentTarget != null)
        {
            enemyDirection = (player.playerCombat.currentTarget.position - player.transform.position).normalized;
        }

        // 시작점이 적의 반대 방향인지 확인
        Vector2 firstInput = pattern[0].normalized;
        Vector3 firstInputWorld = new Vector3(firstInput.x, 0, firstInput.y);
        bool isValidStart = enemyDirection == Vector3.zero || Vector3.Angle(-enemyDirection, firstInputWorld) < 45f;

        if (!isValidStart) return false;

        // 나머지 원형 패턴 체크 로직
        Vector2 center = pattern.Aggregate(Vector2.zero, (acc, p) => acc + p) / pattern.Count;
        float endDistance = Vector2.Distance(pattern[pattern.Count - 1], pattern[0]);
        bool returnsToStart = endDistance < Screen.height * 0.3f;

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

        return Mathf.Abs(totalAngle) >= 180f && returnsToStart;
    }

    protected virtual bool IsTrianglePattern(List<Vector2> pattern)
    {
        if (pattern.Count < 6) return false;

        // 적의 방향 확인
        Vector3 enemyDirection = Vector3.zero;
        if (player.playerCombat.currentTarget != null)
        {
            enemyDirection = (player.playerCombat.currentTarget.position - player.transform.position).normalized;
        }

        // 시작점이 적의 반대 방향인지 확인
        Vector2 firstInput = pattern[0].normalized;
        Vector3 firstInputWorld = new Vector3(firstInput.x, 0, firstInput.y);
        bool isValidStart = enemyDirection == Vector3.zero || Vector3.Angle(-enemyDirection, firstInputWorld) < 60f;

        if (!isValidStart) return false;

        // 방향 전환 체크
        int sharpTurns = 0;
        float angleThreshold = 30f;
        
        for (int i = 2; i < pattern.Count - 2; i++)
        {
            Vector2 v1 = (pattern[i] - pattern[i-2]).normalized;
            Vector2 v2 = (pattern[i+2] - pattern[i]).normalized;
            float angle = Vector2.Angle(v1, v2);
            
            if (angle > angleThreshold)
            {
                sharpTurns++;
                i += 2;
            }
        }

        // 기본 조건 체크
        float endDistance = Vector2.Distance(pattern[pattern.Count - 1], pattern[0]);
        bool returnsToStart = endDistance < Screen.height * 0.3f;
        
        Debug.Log($"Sharp Turns: {sharpTurns}, Returns to Start: {returnsToStart}");

        // sharpTurns가 2 이상이면 삼각형으로 인식
        return sharpTurns >= 2 && returnsToStart;
    }
}

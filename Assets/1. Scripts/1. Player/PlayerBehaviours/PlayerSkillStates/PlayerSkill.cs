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
        if (pattern.Count < 8) return false;

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

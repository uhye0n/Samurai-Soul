using System.Collections.Generic;
using System.Linq; // LINQ 추가
using UnityEngine;

public class PlayerSkill : PlayerBehaviour
{
    protected bool isSkillInvincible = false;

    public PlayerSkill(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        // 스킬 시작 시 무적 상태 설정
        isSkillInvincible = true;
        player.playerStats.SetInvincible(true);
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
        // 스킬 종료 시 무적 해제
        if (isSkillInvincible)
        {
            isSkillInvincible = false;
            player.playerStats.SetInvincible(false);
        }
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
    private float CalculateRotationAngle(List<Vector2> points, Vector2 center)
    {
        if (points.Count < 2) return 0f;
        
        float totalAngle = 0f;
        Vector2 prevVector = (points[0] - center).normalized;

        for (int i = 1; i < points.Count; i++)
        {
            Vector2 currentVector = (points[i] - center).normalized;
            float angle = Vector2.SignedAngle(prevVector, currentVector);
            totalAngle += angle;
            prevVector = currentVector;
        }

        return totalAngle;
    }

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

        // 패턴의 중심점 계산
        Vector2 center = pattern.Aggregate(Vector2.zero, (acc, p) => acc + p) / pattern.Count;
        
        // 중심점으로부터의 평균 거리와 표준편차 계산
        float avgRadius = pattern.Average(p => Vector2.Distance(p, center));
        
        // 반지름 변화를 표준편차로 계산
        float variance = pattern.Average(p => Mathf.Pow(Vector2.Distance(p, center) - avgRadius, 2));
        float standardDeviation = Mathf.Sqrt(variance);
        float normalizedDeviation = standardDeviation / avgRadius;  // 반지름 대비 표준편차
        
        // 원형 패턴의 조건 수정
        bool isRadiusConsistent = normalizedDeviation < 0.7f;  // 70%까지 허용
        float totalAngle = CalculateRotationAngle(pattern, center);
        bool hasFullRotation = Mathf.Abs(totalAngle) >= 270f;

        bool isValid = isRadiusConsistent && hasFullRotation;

        // 디버그 정보 개선
        Debug.Log($"원형 패턴 검사: {(isValid ? "성공" : "실패")}\n" +
                 $"시작점: ({pattern[0].x:F2}, {pattern[0].y:F2})\n" +
                 $"현재점: ({pattern[pattern.Count-1].x:F2}, {pattern[pattern.Count-1].y:F2})\n" +
                 $"중심점: ({center.x:F2}, {center.y:F2})\n" +
                 $"평균 반지름: {avgRadius:F2}\n" +
                 $"반지름 편차: {normalizedDeviation*100:F1}% (임계값: 70%)\n" +
                 $"회전 각도: {totalAngle:F1}° (임계값: 270°)\n" +
                 $"포인트 수: {pattern.Count}\n" +
                 (isValid ? "패턴 인식 성공" : $"실패 원인: {(isRadiusConsistent ? "" : "반지름 불일치, ")}{(hasFullRotation ? "" : "회전 각도 부족")}"));

        if (!isValid) playerCombat.comboStack = 0;

        return isValid;
    }

    protected virtual bool IsTrianglePattern(List<Vector2> pattern)
    {
        if (pattern.Count < 4) return false;

        List<Vector2> vertices = new List<Vector2>();
        float angleThreshold = 60f; // 꼭짓점 판정을 위한 각도 임계값
        
        // 꼭짓점 찾기
        for (int i = 1; i < pattern.Count - 1; i++)
        {
            Vector2 prev = (pattern[i] - pattern[i - 1]).normalized;
            Vector2 next = (pattern[i + 1] - pattern[i]).normalized;
            float angle = Vector2.Angle(prev, next);
            
            if (angle > angleThreshold)
            {
                vertices.Add(pattern[i]);
            }
        }

        // 시작점과 끝점이 가까운지 확인
        float endDistance = Vector2.Distance(pattern[0], pattern[pattern.Count - 1]);
        bool isClosedShape = endDistance < Screen.height * 0.2f;

        // 역삼각형 조건: 정확히 2개의 꼭짓점과 닫힌 형태
        bool isValid = vertices.Count == 2 && isClosedShape;

        if (!isValid)
        {
            Debug.Log($"역삼각형 패턴 실패: 꼭짓점 수={vertices.Count}, 닫힘={isClosedShape}");
            playerCombat.comboStack = 0; // 실패시 콤보 초기화
        }

        return isValid;
    }
}

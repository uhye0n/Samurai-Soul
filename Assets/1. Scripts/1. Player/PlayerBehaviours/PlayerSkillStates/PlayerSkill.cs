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

    protected bool IsCircularPattern(List<Vector2> pattern)
    {
        if (pattern.Count < 8) return false;

        float angleSum = 0f;

        for (int i = 1; i < pattern.Count; i++)
        {
            Vector2 v1 = pattern[i - 1];
            Vector2 v2 = pattern[i];
            angleSum += Vector2.SignedAngle(v1, v2);
        }

        return Mathf.Abs(angleSum) >= 300f; // 300도 이상이면 원형 패턴으로 인정
    }

    protected bool IsTrianglePattern(List<Vector2> pattern)
    {
        if (pattern.Count < 6) return false;

        int cornerCount = 0;
        float angleThreshold = 60f;

        for (int i = 1; i < pattern.Count - 1; i++)
        {
            Vector2 v1 = pattern[i] - pattern[i - 1];
            Vector2 v2 = pattern[i + 1] - pattern[i];
            float angle = Vector2.Angle(v1, v2);

            if (angle > angleThreshold)
            {
                cornerCount++;
            }
        }

        return cornerCount >= 2; // 최소 두 개의 코너가 있을 때 삼각형으로 판단
    }
}

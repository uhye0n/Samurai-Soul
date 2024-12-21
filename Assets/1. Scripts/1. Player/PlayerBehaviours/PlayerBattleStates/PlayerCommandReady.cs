using System.Collections.Generic;
using UnityEngine;

public class PlayerCommandReady : PlayerSkill
{
    private bool canCommand;
    private float patternTimeout = 2.0f; // 패턴 입력 제한 시간
    private float patternStartTime;

    public PlayerCommandReady(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetBool("inCommand", true);
        player.playerInput.SetCommandMode(true);
        canCommand = true;
        patternStartTime = Time.time;
    }

    public override void CheckInput()
    {
        base.CheckInput();

        if (!canCommand) return;

        if (playerCombat.detectedEnemies.Count <= 0)
        {
            Function.SetBehaviour(player, new PlayerIdle(player));
        }

        // 제한 시간 체크
        if (playerCombat.detectedEnemies.Count > 0 && Time.time - patternStartTime > patternTimeout)
        {
            Function.SetBehaviour(player, new PlayerSheath(player));
            return;
        }

        List<Vector2> pattern = player.playerInput.GetInputPattern();

        if (pattern.Count == 0) return;

        // 패턴이 완료되었을 때 (마지막 입력이 Vector2.zero)
        if (pattern.Count > 3 && pattern[pattern.Count - 1] == Vector2.zero)
        {
            pattern.RemoveAt(pattern.Count - 1); // 완료 표시 제거
            bool patternRecognized = true;

            if (IsTrianglePattern(pattern))
            {
                Debug.Log("Triangle Pattern Detected");
                Function.SetBehaviour(player, new PlayerThrust(player));
            }
            else if (IsCircularPattern(pattern))
            {
                Debug.Log("Circular Pattern Detected");
                Function.SetBehaviour(player, new PlayerHorizontalSlash(player));
            }
            else
            {
                Debug.Log("No Pattern Recognized");
                patternRecognized = false;
                Function.SetBehaviour(player, new PlayerSheath(player));
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
        if (!canCommand && playerCombat.detectedEnemies.Count <= 0)
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
        player.playerInput.SetCommandMode(false);
        player.variableJoystick.gameObject.SetActive(true);
    }
}

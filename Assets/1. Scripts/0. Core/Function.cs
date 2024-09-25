using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function
{
// 플레이어 행동 지정
    public static void SetBehaviour(Player player, PlayerBehaviour newBehaviour)
    {
        player.playerBehaviour?.Exit();
        player.playerBehaviour = newBehaviour;
        player.playerBehaviour.Enter();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function
{
    public static void SetBehaviour(Player player, PlayerBehaviour newBehaviour)
    {
        player.playerBehaviour?.Exit();
        player.playerBehaviour = newBehaviour;
        player.playerBehaviour.Enter();
    }

    public static IEnumerator DelayedAction(float time, string key, Action timeIn, Action timeOut, Action timeOn)
    {
        timeOn?.Invoke();

        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            if (Input.GetButtonDown(key))
            {
                timeIn?.Invoke();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        timeOut?.Invoke();
    }
}

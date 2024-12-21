using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHorizontalSlash : PlayerSkill
{
    private int lowerBodyLayerIndex = 1;

    public PlayerHorizontalSlash(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetTrigger("HorizontalSlash");
        player.an.SetLayerWeight(lowerBodyLayerIndex, 0);
        player.StartCoroutine(DelayedEffect());
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
        player.an.SetLayerWeight(lowerBodyLayerIndex, 0);
        playerCombat.alreadyAttacked = true;
    }

    private IEnumerator DelayedEffect()
    {
        yield return new WaitForSeconds(0.2f);
        Transform enemy = player.playerCombat.GetClosestEnemy();
        if (player.slashEffectPrefab != null)
        {
            GameObject effect = null;
            if (enemy != null)
            {
                Vector3 direction = (enemy.position - player.transform.position).normalized;
                direction.y = 0f;
                Quaternion effectRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(180f, 0f, 0f);
                effect = Object.Instantiate(player.slashEffectPrefab, player.transform.position, effectRotation);
            }
            else
            {
                effect = Object.Instantiate(player.slashEffectPrefab, player.transform.position, Quaternion.identity);
            }
            
            Function.SetBehaviour(player, new PlayerSheath(player));
        }
    }
}

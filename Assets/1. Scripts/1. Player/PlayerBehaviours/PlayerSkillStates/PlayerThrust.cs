using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrust : PlayerSkill
{
    private int lowerBodyLayerIndex = 1;

    public PlayerThrust(Player player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.an.SetTrigger("Thrust");
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
        yield return new WaitForSeconds(0.25f);
        Transform enemy = player.playerCombat.GetClosestEnemy();
        if (player.thrustEffectPrefab != null)
        {
            GameObject effect = null;
            Vector3 spawnPosition = player.transform.position + player.transform.forward * 7f;
            
            if (enemy != null)
            {
                Vector3 direction = (enemy.position - player.transform.position).normalized;
                direction.y = 0f;
                Quaternion effectRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(180f, 0f, 0f);
                effect = Object.Instantiate(player.thrustEffectPrefab, spawnPosition, effectRotation);
            }
            else
            {
                effect = Object.Instantiate(player.thrustEffectPrefab, spawnPosition, player.transform.rotation);
            }
            
            if (effect != null)
            {
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    yield return new WaitForSeconds(ps.main.duration);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            
            Function.SetBehaviour(player, new PlayerSheath(player));
        }
    }
}

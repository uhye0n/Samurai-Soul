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
        
        if (player.thrustEffectPrefab != null)
        {
            // 이펙트 생성 위치 계산
            Vector3 spawnPosition = player.transform.position + player.transform.forward * 7f;
            GameObject effect = null;
            
            // 콜라이더 범위 내의 모든 적 검출
            Collider[] hitColliders = Physics.OverlapBox(
                player.transform.position + player.transform.forward * 3f, // 박스 중심점
                new Vector3(2f, 1f, 4f), // 박스 크기의 절반
                player.transform.rotation, // 박스 회전
                player.playerCombat.enemyLayer // 적 레이어
            );

            // 이펙트 생성
            if (hitColliders.Length > 0)
            {
                Vector3 direction = player.transform.forward;
                Quaternion effectRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(180f, 0f, 0f);
                effect = Object.Instantiate(player.thrustEffectPrefab, spawnPosition, effectRotation);

                // 범위 내의 모든 적에게 데미지
                foreach (Collider col in hitColliders)
                {
                    var damageable = col.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(player.playerCombat.attackDamage);
                        Debug.Log($"Hit enemy: {col.gameObject.name}");
                    }
                }
            }
            else
            {
                effect = Object.Instantiate(player.thrustEffectPrefab, spawnPosition, player.transform.rotation);
            }
            
            // 이펙트 자동 제거
            if (effect != null)
            {
                GameObject.Destroy(effect, 2f); // UnityEngine.GameObject.Destroy 사용
            }
            
            yield return new WaitForSeconds(0.75f);
            // 스킬 종료 전 무적 해제
            isSkillInvincible = false;
            player.playerStats.SetInvincible(false);
            Function.SetBehaviour(player, new PlayerSheath(player));
        }
    }

    // 디버그용 기즈모 (에디터에서만 표시)
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                player.transform.position + player.transform.forward * 3f,
                player.transform.rotation,
                Vector3.one
            );
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(4f, 2f, 8f));
        }
    }
}

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
        
        if (player.slashEffectPrefab != null)
        {
            GameObject effect = null;

            // 반원형 범위 내의 적 검출
            Vector3 playerPosition = player.transform.position;
            float radius = 4f; // 반원의 반지름
            float angle = 180f; // 반원의 각도
            
            Collider[] hitColliders = Physics.OverlapSphere(playerPosition, radius, player.playerCombat.enemyLayer);
            List<Collider> validTargets = new List<Collider>();

            foreach (Collider col in hitColliders)
            {
                Vector3 directionToTarget = (col.transform.position - playerPosition).normalized;
                float angleToTarget = Vector3.Angle(player.transform.forward, directionToTarget);
                
                // 전방 180도 내에 있는 적만 선택
                if (angleToTarget <= angle * 0.5f)
                {
                    validTargets.Add(col);
                }
            }

            // 이펙트 생성 및 데미지 처리
            if (validTargets.Count > 0)
            {
                Vector3 direction = player.transform.forward;
                Quaternion effectRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(180f, 0f, 0f);
                effect = Object.Instantiate(player.slashEffectPrefab, playerPosition, effectRotation);

                // 범위 내 모든 적에게 데미지
                foreach (Collider target in validTargets)
                {
                    var damageable = target.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(player.playerCombat.attackDamage);
                        Debug.Log($"Slash hit: {target.gameObject.name}");
                    }
                }
            }
            else
            {
                effect = Object.Instantiate(player.slashEffectPrefab, playerPosition, player.transform.rotation);
            }

            if (effect != null)
            {
                GameObject.Destroy(effect, 2f);
            }
            
            yield return new WaitForSeconds(0.8f);
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
            // 반원 형태의 기즈모 그리기
            Vector3 center = player.transform.position;
            float radius = 4f;
            
            Gizmos.color = Color.red;
            int segments = 32;
            float deltaAngle = 180f / segments;
            
            for (int i = 0; i <= segments; i++)
            {
                float angle = -90f + deltaAngle * i;
                Vector3 direction = Quaternion.Euler(0, angle, 0) * player.transform.forward;
                Vector3 point = center + direction * radius;
                
                if (i > 0)
                {
                    Vector3 prevDirection = Quaternion.Euler(0, -90f + deltaAngle * (i-1), 0) * player.transform.forward;
                    Vector3 prevPoint = center + prevDirection * radius;
                    Gizmos.DrawLine(prevPoint, point);
                }
            }
            
            // 반원의 양 끝점을 중심점과 연결
            Vector3 leftPoint = center + (Quaternion.Euler(0, -90f, 0) * player.transform.forward) * radius;
            Vector3 rightPoint = center + (Quaternion.Euler(0, 90f, 0) * player.transform.forward) * radius;
            Gizmos.DrawLine(center, leftPoint);
            Gizmos.DrawLine(center, rightPoint);
        }
    }
}

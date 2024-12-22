using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat
{
    private Player player;

    public float detectionRadius = 5f;
    public LayerMask enemyLayer = LayerMask.GetMask("Enemy");
    public Transform currentTarget;
    public List<Transform> detectedEnemies = new List<Transform>();
    public int currentTargetIndex = 0;
    public bool alreadyAttacked = false;
    public int attackDamage = 1;
    public int skillDamage = 2;
    public float attackRange = 3f;
    public bool isAttacking = false;
    public bool isAttackMoving = false;

    public delegate void ComboChangedDelegate(int comboCount);
    public event ComboChangedDelegate onComboChanged;

    private int _comboStack = 0;
    public int comboStack
    {
        get { return _comboStack; }
        set
        {
            _comboStack = value;
            onComboChanged?.Invoke(_comboStack);
        }
    }

    private float lastHitTime = 0f;
    private float comboResetTime = 3f; // 2.5초 동안 타격이 없으면 콤보 초기화
    private float comboCheckInterval = 0.5f;  // 콤보 체크 간격
    private float lastComboCheckTime = 0f;    // 마지막 콤보 체크 시간

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void RemoveEnemy(Transform enemy)
    {
        if (currentTarget == enemy)
        {
            currentTarget = null;
        }
        detectedEnemies.Remove(enemy);
        
        // 적 탐지 상태 업데이트
        player.an.SetBool("EnemyDetected", detectedEnemies.Count > 0);
        
        // 타겟이 없어졌을 때 가장 가까운 적을 새로운 타겟으로 설정
        if (currentTarget == null && detectedEnemies.Count > 0)
        {
            currentTarget = GetClosestEnemy();
        }
    }

    public void DetectEnemies()
    {
        detectedEnemies.Clear();
        Collider[] enemiesInRange = Physics.OverlapSphere(player.transform.position, detectionRadius, enemyLayer);

        foreach (Collider enemy in enemiesInRange)
        {
            // 죽은 적은 감지하지 않음
            var enemyRabbit = enemy.GetComponent<EnemyRabbit>();
            if (enemyRabbit != null && !enemyRabbit.isDead)
            {
                detectedEnemies.Add(enemy.transform);
            }
        }

        player.an.SetBool("EnemyDetected", detectedEnemies.Count > 0);
    }

    public Transform GetClosestEnemy()
    {
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform enemy in detectedEnemies)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
                currentTargetIndex = detectedEnemies.IndexOf(enemy);
            }
        }

        return closestEnemy;
    }

    public void LookAtTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 direction = (target.position - player.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            
            player.transform.rotation = lookRotation;
        }
    }

    public void UpdateMovementAnimation()
    {
        if (currentTarget == null) return;

        float moveX = player.playerMove.horizontalAxis;
        float moveY = player.playerMove.verticalAxis;

        Vector3 inputDirection = new Vector3(moveX, 0, moveY);
        Vector3 localMovement = player.transform.InverseTransformDirection(inputDirection);

        player.an.SetFloat("MoveX", localMovement.x);
        player.an.SetFloat("MoveY", localMovement.z);
    }

    public bool ReadyCheck()
    {
        if (currentTarget == null) return false;

        Vector3 targetDirection = (currentTarget.position - player.transform.position).normalized;

        Vector3 playerMovementDirection = new Vector3(player.playerMove.horizontalAxis, 0, player.playerMove.verticalAxis).normalized;

        float angle = Vector3.Angle(targetDirection, playerMovementDirection);

        if (angle > 120f && playerMovementDirection.magnitude > 0.1f)
        {
            return true;
        }

        return false;
    }

    public bool AttackCheck()
    {
        if (currentTarget == null) return false;

        Vector3 targetDirection = (currentTarget.position - player.transform.position).normalized;

        Vector3 playerMovementDirection = new Vector3(player.playerMove.horizontalAxis, 0, player.playerMove.verticalAxis).normalized;

        float angle = Vector3.Angle(targetDirection, playerMovementDirection);

        if (angle < 30f && playerMovementDirection.magnitude > 0.1f)
        {
            return true;
        }

        return false;
    }

    public void AttackMove(float speed)
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(player.transform.position, currentTarget.position);

        if (distanceToTarget > 1.5f)
        {
            Vector3 targetDirection = (currentTarget.position - player.transform.position).normalized;
            player.transform.position += speed * Time.deltaTime * targetDirection;
        }
    }

    public void Attack(float power)
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(player.transform.position, currentTarget.position);
        
        if (distanceToTarget <= attackRange)
        {
            GameObject targetObject = currentTarget.gameObject;
            if (targetObject.CompareTag("Enemy"))
            {
                var damageable = targetObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
        alreadyAttacked = true;
    }

    public void ComboCheck()
    {
        if (Time.time - lastComboCheckTime < comboCheckInterval)
            return;

        lastComboCheckTime = Time.time;
        DetectEnemies();

        // 콤보 상태 업데이트
        bool comboOver3 = comboStack >= 3;
        player.an.SetBool("ComboOver3", comboOver3);

        // 적이 없거나 마지막 타격 후 시간 초과 시 콤보 리셋
        if (comboStack > 0 && 
            (detectedEnemies.Count == 0 || Time.time - lastHitTime > comboResetTime))
        {
            ResetCombo();
        }
    }

    public void RegisterHit()
    {
        lastHitTime = Time.time;
        lastComboCheckTime = Time.time;
        DetectEnemies();
        comboStack++;
        Debug.Log($"RegisterHit - New Combo Count: {comboStack}");
    }

    public void ResetCombo()
    {
        if (_comboStack > 0)  // 콤보가 있을 때만 리셋 로그 출력
        {
            Debug.Log($"Combo Reset: Last combo was {_comboStack}");
            comboStack = 0;
        }
    }
}

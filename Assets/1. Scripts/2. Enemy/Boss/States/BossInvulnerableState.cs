using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossInvulnerableState : BossState
{
    private float patternDuration = 5f;    // 각 패턴의 지속 시간
    private float patternDelay = 1f;       // 패턴 간 딜레이
    private float currentPatternTimer;
    private float nextPatternTime = 0f;  // 변수 위치 이동
    private int[] selectedPatterns;        // 이번 페이즈에서 사용할 패턴들
    private int currentPatternIndex = 0;
    private bool isExecutingPattern = false;
    private Vector3 mapCenter;
    private float mapSize = 20f;
    private bool hasMovedToSafePosition = false;
    private float moveSpeed = 10f;
    private float initialDelay = 3f;  // 리스폰 후 대기 시간
    private float moveDelay = 2f;     // 이동 후 패턴 시작까지 대기 시간
    private bool hasStartedMoving = false;
    private bool isInitialDelay = true;

    public BossInvulnerableState(Boss boss) : base(boss) 
    {
        mapCenter = Vector3.zero;  // 맵 중앙 위치 설정
        // 랜덤하게 2개의 패턴 선택
        selectedPatterns = new int[2];
        System.Array.Copy(ShufflePatterns(), 0, selectedPatterns, 0, 2);
    }

    private int[] ShufflePatterns()
    {
        int[] patterns = new int[] { 0, 1, 2, 3 }; // Cross, Circle, Spiral, Random
        for (int i = patterns.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = patterns[i];
            patterns[i] = patterns[j];
            patterns[j] = temp;
        }
        return patterns;
    }

    public override void Enter()
    {
        boss.SetInvulnerable(true);
        stateTimer = boss.invulnerablePhaseTime;
        mapCenter = Vector3.zero;
        hasMovedToSafePosition = false;
        hasStartedMoving = false;
        isInitialDelay = true;
        isExecutingPattern = false;
        Debug.Log("Boss entering invulnerable state");
    }

    public override void Update()
    {
        if (isInitialDelay)
        {
            initialDelay -= Time.deltaTime;
            if (initialDelay <= 0)
            {
                isInitialDelay = false;
                boss.SetCollisionEnabled(false);
                hasStartedMoving = true;
                Debug.Log("Initial delay finished, starting movement");
            }
            return;
        }

        if (!hasMovedToSafePosition && hasStartedMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, boss.safePosition, step);

            if (Vector3.Distance(boss.transform.position, boss.safePosition) < 0.1f)
            {
                hasMovedToSafePosition = true;
                boss.SetCollisionEnabled(true);
                Debug.Log("Boss reached safe position, waiting before pattern start");
            }
            return;
        }

        if (hasMovedToSafePosition && moveDelay > 0)
        {
            moveDelay -= Time.deltaTime;
            return;
        }

        // 기존의 패턴 실행 로직
        stateTimer -= Time.deltaTime;
        
        if (!isExecutingPattern && hasMovedToSafePosition)
        {
            if (currentPatternIndex < selectedPatterns.Length)
            {
                isExecutingPattern = true;
                currentPatternTimer = patternDuration;
                nextPatternTime = Time.time + patternDuration;
                StartPattern(selectedPatterns[currentPatternIndex]);
                Debug.Log($"Starting pattern {currentPatternIndex}");
            }
        }
        else
        {
            currentPatternTimer -= Time.deltaTime;
            if (currentPatternTimer <= 0)
            {
                isExecutingPattern = false;
                currentPatternIndex++;
                if (currentPatternIndex < selectedPatterns.Length)
                {
                    // 코루틴 대신 딜레이 타이머 사용
                    nextPatternTime = Time.time + patternDelay;
                }
            }
        }

        if (stateTimer <= 0)
        {
            boss.SetState(new BossVulnerableState(boss));
        }
    }

    // 코루틴 제거하고 타이머 로직으로 변경
    private void DelayNextPattern()
    {
        nextPatternTime = Time.time + patternDelay;
        isExecutingPattern = false;
    }

    private void StartPattern(int patternIndex)
    {
        if (!isExecutingPattern) return;  // 실행 중이 아닐 때만 패턴 실행

        switch (patternIndex)
        {
            case 0: CrossPattern(); break;
            case 1: CirclePattern(); break;
            case 2: SpiralPattern(); break;
            case 3: RandomPattern(); break;
        }
    }

    private void CrossPattern()
    {
        // 3개의 랜덤한 위치에 십자가 생성
        for (int cross = 0; cross < 3; cross++)
        {
            // 랜덤 중심점 생성 (-mapSize/3 ~ mapSize/3 사이)
            Vector3 crossCenter = new Vector3(
                Random.Range(-mapSize/3, mapSize/3),
                0,
                Random.Range(-mapSize/3, mapSize/3)
            );

            // 수평 라인 (더 길게)
            for (float i = -mapSize; i <= mapSize; i += 2f)
            {
                Vector3 horizontalPos = crossCenter + new Vector3(i, 0, 0);
                // 맵 범위 내에서만 생성
                if (Mathf.Abs(horizontalPos.x) <= mapSize/2)
                {
                    boss.SpawnTileAttack(horizontalPos);
                }
            }

            // 수직 라인 (중앙 포인트 제외)
            for (float i = -mapSize; i <= mapSize; i += 2f)
            {
                if (Mathf.Abs(i) > 0.1f)  // 중앙 근처는 건너뛰기
                {
                    Vector3 verticalPos = crossCenter + new Vector3(0, 0, i);
                    // 맵 범위 내에서만 생성
                    if (Mathf.Abs(verticalPos.z) <= mapSize/2)
                    {
                        boss.SpawnTileAttack(verticalPos);
                    }
                }
            }
        }
    }

    private void CirclePattern()
    {
        Debug.Log($"Spawning circle pattern at center: {mapCenter}");
        int segments = 12;
        float[] radii = new float[] { 6f, 8f, 10f }; // 3개의 동심원 반지름

        foreach (float radius in radii)
        {
            for (int i = 0; i < segments; i++)
            {
                float angle = i * (360f / segments) * Mathf.Deg2Rad;
                Vector3 spawnPos = mapCenter + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                boss.SpawnTileAttack(spawnPos);
            }
            // 각 원 사이에 약간의 딜레이
            // boss.StartCoroutine(new WaitForSeconds(0.2f));
        }
    }

    private void SpiralPattern()
    {
        float angle = 0f;
        float radius = 3f; // 시작 반경
        float angleStep = 25f; // 각도 간격 증가
        float radiusStep = 0.4f; // 반경 증가량 조정

        // 총 회전 수를 줄이고 간격을 늘림
        for (int i = 0; i < 30; i++)
        {
            Vector3 spawnPos = mapCenter + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            boss.SpawnTileAttack(spawnPos);

            angle += angleStep;
            radius += radiusStep;
        }
    }

    private void RandomPattern()
    {
        int attackCount = 30;  // 공격 횟수 30회로 증가
        float delay = 0.15f;   // 딜레이는 약간 감소
        boss.StartCoroutine(SpawnRandomAttacks(attackCount, delay));
    }

    private IEnumerator SpawnRandomAttacks(int count, float delay)
    {
        float safeRadius = 2f; // 플레이어 주변 안전 반경

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos;
            do
            {
                randomPos = mapCenter + new Vector3(
                    Random.Range(-mapSize/2, mapSize/2),
                    0,
                    Random.Range(-mapSize/2, mapSize/2)
                );
            } 
            // 플레이어 위치로부터 최소 안전거리 확보
            while (Vector3.Distance(randomPos, GameObject.FindGameObjectWithTag("Player").transform.position) < safeRadius);

            boss.SpawnTileAttack(randomPos);
            yield return new WaitForSeconds(delay);
        }
    }

    private Vector3 GetNextAttackPosition()
    {
        // 공격할 타일의 위치 계산 로직
        // 예시로 랜덤 위치 반환
        return new Vector3(
            Random.Range(-10f, 10f),
            0,
            Random.Range(-10f, 10f)
        );
    }
}

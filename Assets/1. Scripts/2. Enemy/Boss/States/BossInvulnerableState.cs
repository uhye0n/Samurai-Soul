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
        isExecutingPattern = false;
        boss.SetCollisionEnabled(false); // 콜라이더 비활성화 대신 충돌 비활성화
        Debug.Log("Boss entering invulnerable state");
    }

    public override void Update()
    {
        if (!hasMovedToSafePosition)
        {
            // 보스를 안전 위치로 이동
            float step = moveSpeed * Time.deltaTime;
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, boss.safePosition, step);

            // 목표 위치에 도달했는지 확인
            if (Vector3.Distance(boss.transform.position, boss.safePosition) < 0.1f)
            {
                hasMovedToSafePosition = true;
                boss.SetCollisionEnabled(true); // 콜라이더 활성화 대신 충돌 활성화
                Debug.Log("Boss reached safe position");
            }
            return;
        }

        stateTimer -= Time.deltaTime;
        
        // 안전 위치 도달 후 패턴 시작
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
        // 수평 라인
        for (float i = -mapSize/2; i <= mapSize/2; i += 2f)
        {
            Vector3 horizontalPos = new Vector3(i, 0, mapCenter.z);
            boss.SpawnTileAttack(horizontalPos);
        }

        // 수직 라인 (중앙 포인트 제외)
        for (float i = -mapSize/2; i <= mapSize/2; i += 2f)
        {
            // 중앙 포인트가 아닐 때만 생성
            if (Mathf.Abs(i) > 0.1f)  // 중앙 근처는 건너뛰기
            {
                Vector3 verticalPos = new Vector3(mapCenter.x, 0, i);
                boss.SpawnTileAttack(verticalPos);
            }
        }
    }

    private void CirclePattern()
    {
        Debug.Log($"Spawning circle pattern at center: {mapCenter}");
        int segments = 12;
        float radius = 8f;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (360f / segments) * Mathf.Deg2Rad;
            // mapCenter를 기준으로 월드 좌표 생성
            Vector3 spawnPos = mapCenter + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            boss.SpawnTileAttack(spawnPos);
        }
    }

    private void SpiralPattern()
    {
        // 코루틴을 Boss 클래스를 통해 실행
        boss.StartSpiralPattern((pos) => boss.SpawnTileAttack(pos));
    }

    private void RandomPattern()
    {
        int attackCount = 15;  // 증가된 공격 횟수
        float delay = 0.2f;    // 공격 간 딜레이
        boss.StartCoroutine(SpawnRandomAttacks(attackCount, delay));
    }

    private IEnumerator SpawnRandomAttacks(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = mapCenter + new Vector3(
                Random.Range(-mapSize/2, mapSize/2),
                0,
                Random.Range(-mapSize/2, mapSize/2)
            );
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

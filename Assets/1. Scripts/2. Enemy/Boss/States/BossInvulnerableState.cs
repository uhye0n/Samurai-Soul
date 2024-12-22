using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossInvulnerableState : BossState
{
    private float nextAttackTime;
    private float attackInterval = 1.5f;
    private int currentPattern = 0;
    private Vector3 mapCenter;
    private float mapSize = 20f;
    private float patternDuration = 5f;    // 각 패턴의 지속 시간
    private float warningDuration = 4f;    // 경고 지속 시간
    private float patternDelay = 1f;       // 패턴 간 딜레이
    private float currentPatternTimer;
    private int[] selectedPatterns;        // 이번 페이즈에서 사용할 패턴들
    private int currentPatternIndex = 0;
    private bool isExecutingPattern = false;

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
        
        // 맵 중앙을 기준으로 패턴 생성
        mapCenter = Vector3.zero;
        
        boss.transform.position = boss.safePosition;
        Debug.Log($"Boss entered invulnerable state at position: {boss.transform.position}");
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        
        if (!isExecutingPattern)
        {
            if (currentPatternIndex < selectedPatterns.Length)
            {
                StartPattern(selectedPatterns[currentPatternIndex]);
                isExecutingPattern = true;
                currentPatternTimer = patternDuration;
            }
        }
        else
        {
            currentPatternTimer -= Time.deltaTime;
            if (currentPatternTimer <= 0)
            {
                isExecutingPattern = false;
                currentPatternIndex++;
                // 패턴 간 딜레이 추가
                if (currentPatternIndex < selectedPatterns.Length)
                {
                    boss.StartCoroutine(WaitForNextPattern());
                }
            }
        }

        if (stateTimer <= 0)
        {
            boss.SetState(new BossVulnerableState(boss));
        }
    }

    private IEnumerator WaitForNextPattern()
    {
        yield return new WaitForSeconds(patternDelay);
        isExecutingPattern = false;
    }

    private void StartPattern(int patternIndex)
    {
        switch (patternIndex)
        {
            case 0: CrossPattern(); break;
            case 1: CirclePattern(); break;
            case 2: SpiralPattern(); break;
            case 3: RandomPattern(); break;
        }
    }

    private void PerformTileAttack()
    {
        // 패턴 순환
        currentPattern = (currentPattern + 1) % 4;
        
        switch (currentPattern)
        {
            case 0:
                CrossPattern();
                break;
            case 1:
                CirclePattern();
                break;
            case 2:
                SpiralPattern();
                break;
            case 3:
                RandomPattern();
                break;
        }
    }

    private void CrossPattern()
    {
        // 월드 좌표 기준으로 타일 생성
        for (float i = -mapSize/2; i <= mapSize/2; i += 2f)
        {
            Vector3 horizontalPos = new Vector3(i, 0, mapCenter.z);
            Vector3 verticalPos = new Vector3(mapCenter.x, 0, i);
            boss.SpawnTileAttack(horizontalPos);
            boss.SpawnTileAttack(verticalPos);
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
            // 상대 좌표로 타일 생성
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            boss.SpawnTileAttack(pos);
        }
    }

    private void SpiralPattern()
    {
        // 코루틴을 Boss 클래스를 통해 실행
        boss.StartSpiralPattern((pos) => boss.SpawnTileAttack(pos));
    }

    private void RandomPattern()
    {
        // 랜덤 위치에 여러 개의 타일 생성
        int attackCount = 8;
        for (int i = 0; i < attackCount; i++)
        {
            // 상대 좌표로 타일 생성
            Vector3 randomPos = new Vector3(
                Random.Range(-mapSize/2, mapSize/2),
                0,
                Random.Range(-mapSize/2, mapSize/2)
            );
            boss.SpawnTileAttack(randomPos);
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

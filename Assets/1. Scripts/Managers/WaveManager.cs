using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;          // 웨이브 이름
        public int enemyCount;           // 이 웨이브의 적 수
        public List<Transform> spawnPoints = new List<Transform>();  // 스폰 포인트 목록
        [HideInInspector] public bool isCompleted;  // 웨이브 완료 여부
    }

    [Header("Wave Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private float waveCooldown = 5f; // 웨이브 사이 쿨타임
    
    [Header("Current Status")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int remainingEnemies = 0;
    [SerializeField] private float nextWaveTime = 0f; // 다음 웨이브 시작 시간

    private List<GameObject> activeEnemies = new List<GameObject>();

    // 이벤트 선언을 클래스 상단으로 이동
    public delegate void WaveChangedDelegate(int waveNumber, string waveName);
    public delegate void EnemyCountChangedDelegate(int remaining, int total);
    
    public event WaveChangedDelegate onWaveChanged;
    public event EnemyCountChangedDelegate onEnemyCountChanged;

    private void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("모든 웨이브 클리어!");
            return;
        }

        Wave currentWave = waves[currentWaveIndex];
        remainingEnemies = currentWave.enemyCount;

        // 웨이브 시작 시 이벤트 호출
        onWaveChanged?.Invoke(currentWaveIndex + 1, currentWave.waveName);
        onEnemyCountChanged?.Invoke(remainingEnemies, currentWave.enemyCount);

        // 스폰 포인트들을 순회하면서 적 생성
        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            // 스폰 포인트 순환
            Transform spawnPoint = currentWave.spawnPoints[i % currentWave.spawnPoints.Count];
            SpawnEnemy(spawnPoint.position);
        }

        Debug.Log($"Wave {currentWave.waveName} 시작! 적 {remainingEnemies}마리 생성");
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        activeEnemies.Add(enemy);

        var rabbitComponent = enemy.GetComponent<EnemyRabbit>();
        if (rabbitComponent != null)
        {
            rabbitComponent.OnDeath += () => 
            {
                HandleEnemyDeath(enemy);
            };
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        remainingEnemies--;

        // 적 수 변경 이벤트 호출
        onEnemyCountChanged?.Invoke(remainingEnemies, waves[currentWaveIndex].enemyCount);

        if (remainingEnemies <= 0)
        {
            waves[currentWaveIndex].isCompleted = true;
            currentWaveIndex++;
            Debug.Log($"Wave {currentWaveIndex} 클리어! {waveCooldown}초 후 다음 웨이브 시작");
            nextWaveTime = Time.time + waveCooldown;
            StartCoroutine(StartNextWaveWithDelay());
        }
    }

    private IEnumerator StartNextWaveWithDelay()
    {
        yield return new WaitForSeconds(waveCooldown);
        StartNextWave();
    }

#if UNITY_EDITOR
    // 에디터에서 스폰 포인트 시각화
    private void OnDrawGizmos()
    {
        foreach (var wave in waves)
        {
            foreach (var spawnPoint in wave.spawnPoints)
            {
                if (spawnPoint != null)
                {
                    // 완료된 웨이브는 회색, 현재 웨이브는 빨간색, 남은 웨이브는 노란색
                    Gizmos.color = wave.isCompleted ? Color.gray : 
                                 (waves.IndexOf(wave) == currentWaveIndex ? Color.red : Color.yellow);
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                    
                    // 스폰 포인트 번호 표시
                    UnityEditor.Handles.Label(spawnPoint.position + Vector3.up, 
                        $"Wave {waves.IndexOf(wave) + 1}\nPoint {wave.spawnPoints.IndexOf(spawnPoint) + 1}");
                }
            }
        }
        
        // 남은 쿨타임 표시 추가
        if (Application.isPlaying && Time.time < nextWaveTime)
        {
            float remainingTime = nextWaveTime - Time.time;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2,
                $"Next Wave in: {remainingTime:F1}s");
        }
    }
#endif
}

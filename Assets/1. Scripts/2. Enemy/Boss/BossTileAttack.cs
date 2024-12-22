using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class BossTileAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float warningDuration = 1f;
    public float damageDelay = 0.5f;
    public int damage = 1;
    public float tileSize = 2f;  // 타일 한 변의 길이

    [Header("Visual Settings")]
    public Material warningMaterial;  // 경고 상태 머티리얼
    public GameObject rockEffectPrefab; // 바위 이펙트 프리팹
    private MeshRenderer meshRenderer;
    private GameObject currentRockEffect;
    
    [Header("Effect Settings")]
    public float warningHeight = 0.1f;  // 경고 상태일 때 타일 높이
    public float damageHeight = 0.3f;   // 데미지 상태일 때 타일 높이
    public float fadeSpeed = 2f;        // 페이드 속도

    private BoxCollider boxCollider;  // BoxCollider 참조 추가
    private Transform tileVisual;  // 타일 시각 효과용 Transform 추가
    private Vector3 initialPosition;  // 초기 생성 위치 저장용

    private void Awake()
    {
        // 자식 큐브의 Transform과 MeshRenderer 가져오기
        tileVisual = transform.GetChild(0);
        meshRenderer = tileVisual.GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();

        // 초기 위치 정확히 저장
        initialPosition = new Vector3(
            transform.position.x,
            0f,  // y값은 0으로 초기화
            transform.position.z
        );
        
        // 타일과 콜라이더 크기 설정
        SetTileSize(tileSize);
        
        // 경고 타일의 위치 설정
        transform.position = initialPosition + new Vector3(0, 0.05f, 0);
    }

    private void SetTileSize(float size)
    {
        // 부모 오브젝트의 크기는 1로 유지
        transform.localScale = Vector3.one;
        
        // 자식 큐브의 크기 설정
        if (tileVisual != null)
        {
            tileVisual.localScale = new Vector3(size, 0.1f, size);
        }
        
        // 콜라이더 크기 설정 (높이는 damageHeight 사용)
        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(size, damageHeight, size);
            boxCollider.center = new Vector3(0, damageHeight/2, 0);
        }
    }

    private void Start()
    {
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // 경고 상태
        yield return StartCoroutine(ShowWarning());
        
        // 데미지 상태
        yield return StartCoroutine(ActivateDamage());
        
        // 페이드 아웃 및 제거
        yield return StartCoroutine(FadeOut());
        
        Destroy(gameObject);
    }

    private IEnumerator ShowWarning()
    {
        meshRenderer.material = warningMaterial;
        Color warningColor = new Color(1f, 1f, 0f, 0.2f); // 노란색 초기 알파값 0
        float elapsedTime = 0f;
        
        // 타일이 서서히 나타나는 효과
        while (elapsedTime < warningDuration)
        {
            float alpha = Mathf.Lerp(0, 0.2f, elapsedTime / warningDuration); // 0.5f에서 0.2f로 수정
            warningColor.a = alpha;
            warningMaterial.color = warningColor;
            
            // 자식 큐브의 높이만 조절
            tileVisual.localScale = new Vector3(
                tileSize,
                Mathf.Lerp(0.1f, warningHeight, elapsedTime / warningDuration),
                tileSize
            );
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ActivateDamage()
    {
        meshRenderer.enabled = false;

        // 바위 이펙트 생성 위치를 현재 타일의 위치로 설정
        if (rockEffectPrefab != null)
        {
            Vector3 rockSpawnPosition = transform.position;
            rockSpawnPosition.y = 0f;  // y값만 0으로 설정
            
            currentRockEffect = Instantiate(rockEffectPrefab, rockSpawnPosition, Quaternion.identity);
            currentRockEffect.transform.localScale = new Vector3(tileSize, damageHeight, tileSize);
            
            Debug.Log($"Rock spawned at: {rockSpawnPosition}, Transform position: {transform.position}");
        }
        else
        {
            Debug.LogError("Rock effect prefab is missing!");
        }

        // 데미지 판정도 같은 위치 사용
        Collider[] hits = Physics.OverlapBox(
            transform.position + new Vector3(0, damageHeight/2, 0),
            new Vector3(tileSize/2, damageHeight/2, tileSize/2),
            transform.rotation,
            LayerMask.GetMask("Player")
        );

        foreach (var hit in hits)
        {
            var player = hit.GetComponent<Player>();
            if (player != null)
                player.playerStats.TakeDamage(damage);
        }

        yield return new WaitForSeconds(damageDelay);
    }

    private IEnumerator FadeOut()
    {
        if (currentRockEffect != null)
        {
            // 바위 이펙트 페이드아웃 처리
            float elapsedTime = 0f;
            float fadeDuration = 0.5f;
            Vector3 originalScale = currentRockEffect.transform.localScale;
            
            while (elapsedTime < fadeDuration)
            {
                float t = elapsedTime / fadeDuration;
                currentRockEffect.transform.localScale = Vector3.Lerp(
                    originalScale, 
                    new Vector3(originalScale.x, 0.1f, originalScale.z), 
                    t
                );
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            Destroy(currentRockEffect);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // 실제 공격 영역 시각화
        Vector3 position = Application.isPlaying ? transform.position : initialPosition;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(position + new Vector3(0, warningHeight/2, 0), 
            new Vector3(tileSize, warningHeight, tileSize));
            
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(position + new Vector3(0, damageHeight/2, 0), 
            new Vector3(tileSize, damageHeight, tileSize));
    }
}

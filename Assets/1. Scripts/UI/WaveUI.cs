using UnityEngine;
using TMPro;
using System.Collections;

public class WaveUI : MonoBehaviour
{
    [Header("Wave Announcement")]
    [SerializeField] private GameObject waveAnnouncement;
    [SerializeField] private TMP_Text waveAnnouncementText;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;
    
    private WaveManager waveManager;
    private Coroutine announceCoroutine;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        // CanvasGroup 컴포넌트 가져오기 (없으면 추가)
        canvasGroup = waveAnnouncement.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = waveAnnouncement.AddComponent<CanvasGroup>();

        // 초기 상태 설정
        waveAnnouncement.SetActive(false);
        canvasGroup.alpha = 0f;
        
        // WaveManager 연결
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.onWaveChanged += HandleWaveChanged;
        }
        else
        {
            Debug.LogError("WaveManager not found!");
        }
    }

    private void HandleWaveChanged(int waveNumber, string waveName)
    {
        if (announceCoroutine != null)
            StopCoroutine(announceCoroutine);
            
        announceCoroutine = StartCoroutine(ShowWaveAnnouncement(waveName));
    }

    private IEnumerator ShowWaveAnnouncement(string waveName)
    {
        waveAnnouncement.SetActive(true);
        waveAnnouncementText.text = $"{waveName}";

        // 페이드 인
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = elapsed / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 표시 지속
        yield return new WaitForSeconds(displayDuration);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        waveAnnouncement.SetActive(false);
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.onWaveChanged -= HandleWaveChanged;
        }
    }
}

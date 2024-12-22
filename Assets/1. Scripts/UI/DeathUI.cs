using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private float textPulseSpeed = 1f;
    [SerializeField] private float sceneTransitionDelay = 3f;
    [SerializeField] private float fadeInDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        deathPanel.SetActive(false);
        canvasGroup = deathPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = deathPanel.AddComponent<CanvasGroup>();
    }

    public void ShowDeathScreen()
    {
        deathPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        Time.timeScale = 0.5f;
        StartCoroutine(FadeInDeathScreen());
    }

    private IEnumerator FadeInDeathScreen()
    {
        float elapsed = 0;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            yield return null;
        }

        StartCoroutine(PulseTextEffect());
        StartCoroutine(TransitionToGameOver());
    }

    private IEnumerator PulseTextEffect()
    {
        while (deathPanel.activeSelf)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * textPulseSpeed) + 1) / 2;
            deathText.color = new Color(1f, 0f, 0f, 0.5f + (pulse * 0.5f));
            yield return null;
        }
    }

    private IEnumerator TransitionToGameOver()
    {
        yield return new WaitForSecondsRealtime(sceneTransitionDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}

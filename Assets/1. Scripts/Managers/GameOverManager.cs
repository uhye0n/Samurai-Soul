using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private float textPulseSpeed = 1f;

    private void Start()
    {
        InitializeUI();
        StartCoroutine(PulseTextEffect());
    }

    private void InitializeUI()
    {
        retryButton.onClick.AddListener(RetryGame);
        menuButton.onClick.AddListener(ReturnToMenu);
    }

    private System.Collections.IEnumerator PulseTextEffect()
    {
        while (true)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * textPulseSpeed) + 1) / 2;
            gameOverText.color = new Color(1f, 0f, 0f, 0.5f + (pulse * 0.5f));
            yield return null;
        }
    }

    private void RetryGame()
    {
        SceneManager.LoadScene(1);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

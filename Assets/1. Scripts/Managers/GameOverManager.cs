using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        retryButton.onClick.AddListener(RetryGame);
        menuButton.onClick.AddListener(ReturnToMenu);
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

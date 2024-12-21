using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   // GameScene으로 전환
    }

    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }
}

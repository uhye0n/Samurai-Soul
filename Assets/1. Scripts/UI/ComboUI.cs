using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private Animator comboAnimator; // 선택사항: UI 애니메이션용
    [SerializeField] private float fadeDuration = 0.5f; // 페이드아웃 시간
    private Player player;
    private CanvasGroup canvasGroup; // 페이드아웃을 위한 CanvasGroup

    private void Start()
    {
        player = FindObjectOfType<Player>();
        player.playerCombat.onComboChanged += UpdateComboUI;
        comboText.gameObject.SetActive(false);
        canvasGroup = comboText.gameObject.AddComponent<CanvasGroup>();
    }

    private void UpdateComboUI(int comboCount)
    {
        if (comboCount > 0)
        {
            StopAllCoroutines(); // 진행 중인 페이드아웃 중단
            canvasGroup.alpha = 1f;
            comboText.gameObject.SetActive(true);
            comboText.text = $"COMBO {comboCount}";
            if (comboAnimator != null)
                comboAnimator.SetTrigger("ComboUpdate");
        }
        else
        {
            // 콤보가 0이 되면 페이드아웃 시작
            StartCoroutine(FadeOutCombo());
        }
    }

    private IEnumerator FadeOutCombo()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        comboText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (player != null && player.playerCombat != null)
            player.playerCombat.onComboChanged -= UpdateComboUI;
    }
}

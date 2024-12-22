using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private Animator comboAnimator; // 선택사항: UI 애니메이션용
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        player.playerCombat.onComboChanged += UpdateComboUI;
        comboText.gameObject.SetActive(false);
    }

    private void UpdateComboUI(int comboCount)
    {
        if (comboCount > 0)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"COMBO {comboCount}";
            if (comboAnimator != null)
                comboAnimator.SetTrigger("ComboUpdate");
        }
        else
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (player != null && player.playerCombat != null)
            player.playerCombat.onComboChanged -= UpdateComboUI;
    }
}

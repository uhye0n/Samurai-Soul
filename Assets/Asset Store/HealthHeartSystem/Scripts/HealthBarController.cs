/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    private GameObject[] heartContainers;
    private Image[] heartFills;

    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    
    [SerializeField] private float healthUpdateDelay = 1f; // 체력 업데이트 지연 시간
    [SerializeField] private float healthUpdateSpeed = 2f; // 체력 감소 속도
    private float targetHealth;
    private float currentDisplayHealth;
    private Coroutine healthUpdateCoroutine;

    private void Start()
    {
        heartContainers = new GameObject[(int)PlayerStats.Instance.MaxTotalHealth];
        heartFills = new Image[(int)PlayerStats.Instance.MaxTotalHealth];

        PlayerStats.Instance.onHealthChangedCallback += UpdateHeartsHUD;
        InstantiateHeartContainers();
        currentDisplayHealth = PlayerStats.Instance.Health;
        UpdateHeartsHUD();
    }

    public void UpdateHeartsHUD()
    {
        SetHeartContainers();
        targetHealth = PlayerStats.Instance.Health;

        if (healthUpdateCoroutine != null)
            StopCoroutine(healthUpdateCoroutine);
        
        healthUpdateCoroutine = StartCoroutine(SmoothHealthUpdate());
    }

    private IEnumerator SmoothHealthUpdate()
    {
        // 체력이 감소할 때만 딜레이 적용
        if (targetHealth < currentDisplayHealth)
            yield return new WaitForSeconds(healthUpdateDelay);

        while (Mathf.Abs(currentDisplayHealth - targetHealth) > 0.01f)
        {
            currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealth, 
                Time.deltaTime * healthUpdateSpeed);
            UpdateHeartFills();
            yield return null;
        }

        currentDisplayHealth = targetHealth;
        UpdateHeartFills();
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerStats.Instance.MaxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void UpdateHeartFills()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < Mathf.Floor(currentDisplayHealth))
            {
                heartFills[i].fillAmount = 1;
            }
            else if (i == Mathf.Floor(currentDisplayHealth))
            {
                heartFills[i].fillAmount = currentDisplayHealth % 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }

    void InstantiateHeartContainers()
    {
        for (int i = 0; i < PlayerStats.Instance.MaxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.onHealthChangedCallback -= UpdateHeartsHUD;
    }
}

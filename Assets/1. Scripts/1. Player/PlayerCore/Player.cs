using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 컴포넌트 호출
    public Rigidbody rb;
    public CapsuleCollider cc;
    public Animator an;

    [Header("UI References")]
    public Canvas drawCanvas;
    public RawImage _drawImage;

    // 플레이어 클래스 호출
    public VariableJoystick variableJoystick;
    public PlayerInput playerInput;
    public PlayerMove playerMove;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerBehaviour playerBehaviour;

    // MonoBehaviour 상속 메서드
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        an = GetComponent<Animator>();

        playerInput = new PlayerInput();
        playerInput.Initialize(this);

        playerMove = new PlayerMove();
        playerMove.Initialize(this);

        playerCombat = new PlayerCombat();
        playerCombat.Initialize(this);

        playerStats = new PlayerStats();
        playerStats.Initialize(this);

        // Canvas 정렬 순서 설정
        if (drawCanvas != null)
        {
            drawCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            drawCanvas.pixelPerfect = false;
            drawCanvas.overrideSorting = true;
            drawCanvas.sortingOrder = 1;

            // RawImage 설정
            if (_drawImage != null)
            {
                _drawImage.rectTransform.anchorMin = Vector2.zero;
                _drawImage.rectTransform.anchorMax = Vector2.one;
                _drawImage.rectTransform.offsetMin = Vector2.zero;
                _drawImage.rectTransform.offsetMax = Vector2.zero;
                _drawImage.color = new Color(1, 1, 1, 0);
                _drawImage.raycastTarget = false;

                // 발광 효과를 위한 Material 적용
                Material glowMaterial = new Material(Shader.Find("UI/GlowEffect"));
                glowMaterial.SetColor("_GlowColor", Color.white);
                glowMaterial.SetFloat("_GlowIntensity", 1.0f);
                _drawImage.material = glowMaterial;
            }
        }
    }

    public void Start()
    {
        Function.SetBehaviour(this, new PlayerIdle(this));
    }

    public void Update()
    {
        playerBehaviour?.CheckInput();
        playerBehaviour?.CheckState();
    }

    public void FixedUpdate()
    {
        playerBehaviour?.Perform();
    }
}

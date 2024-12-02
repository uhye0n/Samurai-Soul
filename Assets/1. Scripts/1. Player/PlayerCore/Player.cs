using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 추가

public class Player : MonoBehaviour
{
    // 컴포넌트 호출
    public Rigidbody rb;
    public CapsuleCollider cc;
    public Animator an;
    public Canvas drawCanvas;  // 추가
    public RawImage drawImage;  // 추가

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

        // UI 컴포넌트 초기화
        GameObject canvasObj = new GameObject("DrawCanvas");
        drawCanvas = canvasObj.AddComponent<Canvas>();
        drawCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        drawCanvas.sortingOrder = 100;

        GameObject imageObj = new GameObject("DrawImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        drawImage = imageObj.AddComponent<RawImage>();
        drawImage.rectTransform.anchorMin = Vector2.zero;
        drawImage.rectTransform.anchorMax = Vector2.one;
        drawImage.rectTransform.sizeDelta = Vector2.zero;
        drawImage.color = new Color(1, 1, 1, 0); // 투명 배경
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

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

    [Header("Skill Prefabs")]
    public GameObject slashEffectPrefab;
    public GameObject thrustEffectPrefab;

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

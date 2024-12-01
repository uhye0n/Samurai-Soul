using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
// 컴포넌트 호출
    public Rigidbody rb;
    public CapsuleCollider cc;
    public Animator an;

// 플레이어 클래스 호출
    public PlayerInput playerInput;
    public PlayerMove playerMove;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerBehaviour playerBehaviour;
    public PlayerCommands playerCommands;
    public VariableJoystick variableJoystick;

// MonoBehaviour 상속 메서드
    void Awake()
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

        playerCommands = new PlayerCommands();
        playerCommands.Initialize(this);
    }

    void Start()
    {
        Function.SetBehaviour(this, new PlayerIdle(this));
    }

    void Update()
    {
        playerBehaviour?.CheckInput();
        playerBehaviour?.CheckState();
    }

    void FixedUpdate()
    {
        playerBehaviour?.Perform();
    }
}

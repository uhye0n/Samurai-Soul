using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
// 컴포넌트 호출
    public Rigidbody rb;
    public MeshCollider mc;
    public Animator an;

// 플레이어 클래스 호출
    public PlayerMove playerMove;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerBehaviour playerBehaviour;

// MonoBehaviour 상속 메서드
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mc = GetComponent<MeshCollider>();
        an = GetComponent<Animator>();

        playerMove = new PlayerMove();
        playerMove.Initialize(this);
    }

    void Start()
    {
        Function.SetBehaviour(this, new PlayerIdle(this));
    }

    void Update()
    {
        playerMove.Rotation();
        playerBehaviour?.CheckInput();
        playerBehaviour?.CheckState();
    }

    void FixedUpdate()
    {
        playerBehaviour?.Perform();
    }
}

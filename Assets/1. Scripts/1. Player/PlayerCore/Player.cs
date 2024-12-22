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

    [Header("Mesh Renderers")]
    [SerializeField] private SkinnedMeshRenderer[] skinnedMeshRenderers;
    [SerializeField] private MeshRenderer[] meshRenderers;
    
    // 모든 렌더러를 한번에 접근하기 위한 인터페이스
    public Renderer[] AllRenderers
    {
        get
        {
            Renderer[] allRenderers = new Renderer[skinnedMeshRenderers.Length + meshRenderers.Length];
            skinnedMeshRenderers.CopyTo(allRenderers, 0);
            meshRenderers.CopyTo(allRenderers, skinnedMeshRenderers.Length);
            return allRenderers;
        }
    }

    // 플레이어 클래스 호출 - public으로 모두 변경
    public VariableJoystick variableJoystick;
    public PlayerInput playerInput;
    public PlayerMove playerMove;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerBehaviour playerBehaviour;

    public bool isDead;  // 사망 상태 플래그 추가

    // MonoBehaviour 상속 메서드
    private void Awake()  // void 제거
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

        isDead = false;  // 초기화

        if ((skinnedMeshRenderers == null || skinnedMeshRenderers.Length == 0) &&
            (meshRenderers == null || meshRenderers.Length == 0))
        {
            Debug.LogWarning("No Renderers assigned to Player!");
        }

        Debug.Log("Player initialized with stats");
    }

    public void Start()
    {
        Function.SetBehaviour(this, new PlayerIdle(this));
    }

    public void Update()
    {
        if (isDead) return;  // isDead 사용

        // playerCombat.ComboCheck();  // 콤보 체크를 먼저 수행
        playerBehaviour?.CheckInput();
        playerBehaviour?.CheckState();
    }

    public void FixedUpdate()
    {
        if (isDead) return;  // isDead 사용
        
        playerBehaviour?.Perform();
    }
}

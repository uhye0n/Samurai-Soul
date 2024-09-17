using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public MeshCollider mc;

    public PlayerMove playerMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mc = GetComponent<MeshCollider>();
        playerMove = gameObject.AddComponent<PlayerMove>();
        playerMove.Initialize(this);
    }

    void Start()
    {

    }

    void Update()
    {
        playerMove.CheckInput();
    }

    void FixedUpdate()
    {
        playerMove.GroundMove();
    }
}

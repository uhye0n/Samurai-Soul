using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    public Animator an;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void TakeDamage()
    {
        an.SetTrigger("TakeDamage");
    }
}

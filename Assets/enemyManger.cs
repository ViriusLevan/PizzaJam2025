using System;
using UnityEngine;
using UnityEngine.Events;

public class enemyManger : MonoBehaviour
{
    public float health = 100;

    public bool isAttacking;

    public float dmg;
    public bool kb;
    public UnityEvent<float> damage;
    public float p;
}

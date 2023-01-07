using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 10f;
    protected float groundDrag = 5f;
    protected float jumpForce = 12f;
    protected float jumpCoolDown = 0.25f;
    protected float airMultiplier = 0.4f;
    protected int health = 100;

    [Header("Sensing")]

    [SerializeField] protected float detectionRange = 1f;

}

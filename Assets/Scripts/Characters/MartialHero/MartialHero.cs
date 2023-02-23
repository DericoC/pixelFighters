using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartialHero : CharacterScript
{
    [Header("Movement")]
    [SerializeField] float speed = 1.5f;
    [SerializeField] float jumpForce = 3.5f;

    [Header("Audio")]
    [SerializeField] AudioClip swordAttack;
    [SerializeField] AudioClip swordAttackEnemy;
    [SerializeField] AudioClip swordAttackTwo;
    [SerializeField] AudioClip swordAttackEnemyTwo;
    [SerializeField] public bool isPlayerOne = false;

    [Header("Attack")]
    [SerializeField] public float damageMultiplier = 1.15f;

    void Start()
    {
        Starter(speed, jumpForce, swordAttack, swordAttackEnemy, swordAttackTwo, swordAttackEnemyTwo, isPlayerOne, transform.name.Replace("(Clone)", ""));
    }

    void Update()
    {
        Updater();
    }
}

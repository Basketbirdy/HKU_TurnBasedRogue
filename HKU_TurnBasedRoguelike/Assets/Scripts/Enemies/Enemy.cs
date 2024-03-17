using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "EnemyObject")]
public class Enemy : ScriptableObject
{
    [Header("Description")]
    public string enemyName;
    public string description;

    [Header("Enemy data")]
    [Header("Movement")]
    public int movementRange = 1;
    public float movementSpeed = 8;
    [Header("Combat")]
    public int attackRange = 1;
    public bool needsCharge = false;
    public float damage = 1f;
    public float damageMultiplier = 1f;
    [Space]
    public float maxHp = 100f;
    public float baseDefense = 5f;
    public float defenseMultiplier = 1f;
}

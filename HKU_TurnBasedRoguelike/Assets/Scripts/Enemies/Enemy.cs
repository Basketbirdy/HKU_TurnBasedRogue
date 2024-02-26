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
    [Header("Combat")]
    public float minDamage = 60;
    public float maxDamage = 120;
    public float damageMultiplier = 1f;
    [Space]
    public float maxHp = 100f;
    public float baseDefense = 5f;
    public float defenseMultiplier = 1f;
}

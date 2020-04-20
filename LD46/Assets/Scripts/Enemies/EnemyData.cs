using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public Sprite sprite;
    public int health;
    public int movementRange;
    public int attackRange;
    public int damage;
    public AttackType attackType;

    public enum AttackType
    {
        Melee,
        Ranged
    }
}

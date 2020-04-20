using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hut : MonoBehaviour
{
    const int SPAWN_AFTER_TURNS_ABSORBED = 2;

    [SerializeField] private Sprite damagedSprite;
    [SerializeField] private Sprite absorbedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int health;
    public Vector2Int gridPosition;

    private int turnsAbsorbed;
    private EnemyData enemyData;

    public void Initialize(Vector2Int gridPos)
    {
        this.gridPosition = gridPos;
    }

    private void Awake()
    {
        turnsAbsorbed = 0;
        health = 1;
    }

    public void NewTurn()
    {
        if (enemyData != null) turnsAbsorbed++;

        if (turnsAbsorbed >= SPAWN_AFTER_TURNS_ABSORBED)
        {
            SpawnVirus();
        }
    }

    private void SpawnVirus()
    {
        World.instance.DamageObjectAtPoint(gridPosition, 999);
        World.instance.enemyHandler.SpawnUnitAtPoint(gridPosition, enemyData);
    }

    public bool Absorb(EnemyData data)
    {
        if (enemyData != null) return false; // Already absorbed
        this.enemyData = data;
        turnsAbsorbed = 0;
        spriteRenderer.sprite = absorbedSprite;
        return true;
    }

    // Returns true if dead
    public bool Damage(int damage)
    {
        if (health <= 0) return false;
        this.health -= damage;
        if (health <= 0 || enemyData != null)
        {
            health = 0;
            Invoke("DestroyUnit", Unit.ATTACK_DURATION - 0.2f);
            return true;
        }
        if (enemyData != null) spriteRenderer.sprite = damagedSprite;
        return false;
    }

    public void DestroyUnit()
    {
        World.instance.CheckVictory();
        Destroy(gameObject);
    }

    public bool IsAbsorbed()
    {
        if (enemyData != null) return true;
        return false;
    }
}

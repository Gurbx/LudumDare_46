using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audio;
    [SerializeField] private Enemy enemyPrefab;

    private List<Enemy> enemies;

    private void Start()
    {
        WorldInput input = World.instance.input;

        // Get spawn points
        List<Vector2Int> enemySpawnPoints = World.instance.GetAllObjectPoints(World.WorldObject.Enemy);

        // Create enemeis
        enemies = new List<Enemy>();
        CreateEnemiesBasedOnData(input.enemies, enemySpawnPoints.Count);

        // Place enemies at right positions
        for (int i = 0; i < enemySpawnPoints.Count; i++)
        {
            enemies[i].UpdatePosition(enemySpawnPoints[i]);
        }
    }

    private void CreateEnemiesBasedOnData(List<WorldInput.EnemyTypes> enemyTypes, int amount)
    {
        Dictionary<EnemyData, int> selectedAmountOfEach = new Dictionary<EnemyData, int>();

        int nrAdded = 0;
        while (nrAdded < amount)
        {
            int index = Random.Range(0, enemyTypes.Count - 1);
            WorldInput.EnemyTypes type = enemyTypes[index];

            int nr = 0;
            if (selectedAmountOfEach.TryGetValue(type.data, out nr))
            {
                if (nr < type.max)
                {
                    selectedAmountOfEach[type.data]++;
                    nrAdded++;
                }
            }
            else
            {
                selectedAmountOfEach.Add(type.data, 1);
                nrAdded++;
            }
        }

        //Create enemies
        foreach(KeyValuePair<EnemyData, int> pair in selectedAmountOfEach)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                EnemyData data = pair.Key;
                var enemy = Instantiate(enemyPrefab, transform);
                enemy.InitializeUnit(data.health, data.sprite, data.attackRange, data.movementRange, data.damage, data.attackType);
                enemy.data = data;
                enemies.Add(enemy);
            }
        }
    }

    // Returns turn duration
    public float DoEnemyActions()
    {
        audio.Play();

        float duration = GetAttackSessionDuration() + 1f;
        StartCoroutine(DoEnemyAttacks());

        Invoke("EnemyMoveActions", duration);

        duration += Unit.MOVE_DURATION + 1f;  //Every move happens at the same time

        return duration;
    }

    // ---- -------

    IEnumerator DoEnemyAttacks()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].HasPlannedAttack())
            {
                enemies[i].DoPlannedAttack();
                yield return new WaitForSeconds(Unit.ATTACK_DURATION + 0.25f);
            }
        }
    }

    private float GetAttackSessionDuration()
    {
        float duration = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].HasPlannedAttack()) duration += Unit.ATTACK_DURATION+0.25f;
        }
        return duration;
    }


    private void EnemyMoveActions()
    {
        foreach (var enemy in enemies)
        {
            enemy.PerformMoveAction();
            enemy.PlanAttack();
            enemy.DisplayPlannedAction();
        }
    }

    public void DisplayPlannedActions()
    {
        foreach (var enemy in enemies)
        {
            enemy.DisplayPlannedAction();
        }
    }

    public bool DamageEnemyAtPoint(Vector2Int point, int damage)
    {

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].gridPositioin == point)
            {
                if (enemies[i].DamageUnit(damage))
                {
                    enemies.Remove(enemies[i]);
                    return true;
                }
            }

        }
        return false;
    }

    public void SpawnUnitAtPoint(Vector2Int point, EnemyData data)
    {
        var enemy = Instantiate(enemyPrefab, transform);
        enemy.InitializeUnit(data.health, data.sprite, data.attackRange, data.movementRange, data.damage, data.attackType);
        enemy.UpdatePosition(point);
        enemy.data = data;
        enemies.Add(enemy);
        World.instance.MarkWorldPosition(point.x, point.y, World.WorldObject.Enemy);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [SerializeField] private AudioSource audioSoruce;
    private int attackRange;
    private int moveRange;
    private int damage;
    private EnemyData.AttackType attackType;

    private UnitAction plannedAction;
    private bool hasPlannedAttack;


    private void Awake()
    {
        Init();
        hasPlannedAttack = false;
    }

    public void InitializeUnit(int health, Sprite sprite, int attackRange, int moveRange, int damage, EnemyData.AttackType attackType)
    {
        base.InitializeUnit(health, sprite);
        this.attackType = attackType;
        this.attackRange = attackRange;
        this.moveRange = moveRange;
        this.damage = damage;
    }

    public void PerformMoveAction()
    {
        Vector2Int closestTarget = World.instance.GetClosestObjectFromPosition(World.WorldObject.Friend, gridPositioin, true);
        // No target exsist if vector is null
        if (closestTarget.x < 0 && closestTarget.y < 0) return;

        UnitAction action = new UnitAction();
        action.type = UnitAction.ActionType.Move;

        Vector2Int direction = closestTarget - gridPositioin;
        action.direction = GetRandomClampedDirection(direction, moveRange);

        DoAction(action);
    }

    public void PlanAttack()
    {
        hasPlannedAttack = true;

        // chance to target cell or friendly unit
        World.WorldObject target = (Random.Range(0, 100) < GameData.instance.attackCellChance) ? World.WorldObject.Hut : World.WorldObject.Friend;

        Vector2Int closestTarget = World.instance.GetClosestObjectFromPosition(target, gridPositioin, true);

        // No target exsist if vector is null
        if (closestTarget.x < 0 && closestTarget.y < 0)
        {
            hasPlannedAttack = false;
            return;
        }

        plannedAction = new UnitAction();
        Vector2Int directionToTarget = closestTarget - gridPositioin;
        plannedAction.type = (attackType == EnemyData.AttackType.Melee) ? UnitAction.ActionType.MeleeAttack : UnitAction.ActionType.RangedAttack;
        plannedAction.damage = damage;
        plannedAction.direction = GetRandomClampedDirection(directionToTarget, attackRange);
    }

    public void DisplayPlannedAction()
    {
        if (hasPlannedAttack) DisplayAction(plannedAction);
    }

    public void DoPlannedAttack()
    {
        if (hasPlannedAttack)
        {
            DoAction(plannedAction);
            audioSoruce.Play();
        }
        hasPlannedAttack = false;
        HideActionMarkers();
    }

    private Vector2Int GetRandomClampedDirection(Vector2Int direction, int rangeClamp)
    {
        Vector2Int clampedDirection;
        if (direction.x == 0)
        {
            clampedDirection = new Vector2Int(0, Mathf.Clamp(direction.y, -rangeClamp, rangeClamp));
        }
        else if (direction.y == 0)
        {
            clampedDirection = new Vector2Int(Mathf.Clamp(direction.x, -rangeClamp, rangeClamp), 0);
        }
        else // Pick random direction and clamp it to movement range
        {
            clampedDirection = (Random.Range(0, 100) < 50) ? new Vector2Int(Mathf.Clamp(direction.y, -rangeClamp, rangeClamp), 0) : new Vector2Int(0, Mathf.Clamp(direction.y, -rangeClamp, rangeClamp));
        }
        return clampedDirection;
    }

    public bool HasPlannedAttack()
    {
        return hasPlannedAttack;
    }
}

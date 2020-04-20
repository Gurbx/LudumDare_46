using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private GameObject deathEffectPrefab;
    public const float ATTACK_DURATION = 0.25f;
    public const float MOVE_DURATION = 1f;

    [SerializeField] private GameObject actionMarkerPrefab;
    [SerializeField] private Projectile projectilePrefab;

    public World.WorldObject unitType;
    public Vector2Int gridPositioin;

    private List<GameObject> actionMarkers;

    //[SerializeField] private int maxHealth;
    private int health;
    public EnemyData data;

    [System.Serializable]
    public struct UnitAction
    {
        public ActionType type;
        public Vector2Int direction;
        public int damage;

        public enum ActionType
        {
            Move,
            MeleeAttack,
            RangedAttack
        }
    }

    void Awake()
    {
        Init();
    }

    protected void Init()
    {
        actionMarkers = new List<GameObject>();
        transform.position = GetUnitWorldPosition();
    }

    public void InitializeUnit(int health, Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        this.health = health;
    }

    public void UpdatePosition(Vector2Int newGridPos)
    {
        gridPositioin = newGridPos;
        transform.position = GetUnitWorldPosition();
    }

    private void Start()
    {
        World.instance.MarkWorldPosition(gridPositioin.x, gridPositioin.y, unitType);
    }

    public void DisplayAction(UnitAction action)
    {
        HideActionMarkers();
        Vector2Int clearDirection = GetClearDirection(action.direction, (action.type == UnitAction.ActionType.Move));
        Color color = (action.type == UnitAction.ActionType.Move) ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        //Right
        for (int x = 1; x <= clearDirection.x; x++)
        {
            var actionArea = Instantiate(actionMarkerPrefab, transform.parent);
            actionArea.gameObject.transform.position = GetUnitWorldPosition() + new Vector3(x, 0, 0);
            actionArea.GetComponent<SpriteRenderer>().color = color;
            actionMarkers.Add(actionArea);
        }
        // Left
        for (int x = -1; x >= clearDirection.x; x--)
        {
            var actionArea = Instantiate(actionMarkerPrefab, transform.parent);
            actionArea.gameObject.transform.position = GetUnitWorldPosition() + new Vector3(x, 0, 0);
            actionArea.GetComponent<SpriteRenderer>().color = color;
            actionMarkers.Add(actionArea);
        }
        //Top
        for (int y = 1; y <= clearDirection.y; y++)
        {
            var actionArea = Instantiate(actionMarkerPrefab, transform.parent);
            actionArea.gameObject.transform.position = GetUnitWorldPosition() + new Vector3(0, y, 0);
            actionArea.GetComponent<SpriteRenderer>().color = color;
            actionMarkers.Add(actionArea);
        }
        // Left
        for (int y = -1; y >= clearDirection.y; y--)
        {
            var actionArea = Instantiate(actionMarkerPrefab, transform.parent);
            actionArea.gameObject.transform.position = GetUnitWorldPosition() + new Vector3(0, y, 0);
            actionArea.GetComponent<SpriteRenderer>().color = color;
            actionMarkers.Add(actionArea);
        }
    }

    public void HideActionMarkers()
    {
        foreach (var marker in actionMarkers)
        {
            Destroy(marker);
        }
        actionMarkers.Clear();
    }

    public void DoAction(UnitAction action)
    {
        switch (action.type)
        {
            case (UnitAction.ActionType.Move):
                {
                    StartCoroutine(MoveUnit(this, action.direction));
                    break;
                }
            case (UnitAction.ActionType.MeleeAttack):
                {
                    StartCoroutine(MeleeAttack(this, action.direction, action.damage));
                    break;
                }
            case (UnitAction.ActionType.RangedAttack):
                {
                    RangedAttack(action.direction, action.damage);
                    break;
                }
        }
    }


    public void RangedAttack(Vector2Int direction, int damage)
    {
        Vector2Int attackDirection = GetClearDirection(direction, false); //Stops at target
        Vector2Int targetPoistioin = gridPositioin + attackDirection;

        var projectile = Instantiate(projectilePrefab, transform);
        projectile.Initiaize(gridPositioin, targetPoistioin, attackDirection, damage);
    }

    public IEnumerator MoveUnit(Unit unit, Vector2Int direction)
    {
        Vector3 startinPos = unit.GetUnitWorldPosition();

        // Get new direction based on if obstacles are hit
        Vector2Int checkedDirection = GetClearDirection(direction, true);
        Vector3 endPos = startinPos + new Vector3(checkedDirection.x, checkedDirection.y, 0);

        //Update position in world
        World.instance.MarkWorldPosition(gridPositioin.x, gridPositioin.y, World.WorldObject.Empty);
        unit.gridPositioin += checkedDirection; 
        World.instance.MarkWorldPosition(gridPositioin.x, gridPositioin.y, unitType);

        float elapsedTime = 0;
        float durationInSeconds = MOVE_DURATION;

        while (elapsedTime < durationInSeconds)
        {
            unit.transform.position = Vector2.Lerp(startinPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime/durationInSeconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        unit.transform.position = endPos;
    }

    public IEnumerator MeleeAttack(Unit unit, Vector2Int direction, int damage)
    {
        Vector3 startinPos = unit.GetUnitWorldPosition();

        // Get new direction based on if obstacles are hit
        Vector2Int moveDirection = GetClearDirection(direction, true); //Stops one tile before target
        Vector2Int attackDirection = GetClearDirection(direction, false); //Stops at target
        Vector2Int attackPosition = new Vector2Int(gridPositioin.x + attackDirection.x, gridPositioin.y + attackDirection.y);
        Vector2Int endPosition = new Vector2Int(gridPositioin.x + moveDirection.x, gridPositioin.y + moveDirection.y);

        World.WorldObject hitObject = World.instance.GetObjectAtPosition(attackPosition.x, attackPosition.y);
        bool absorbed = false;
        if (hitObject == World.WorldObject.Empty || hitObject == World.WorldObject.Enemy || hitObject == World.WorldObject.Friend)
        {
            if (World.instance.DamageObjectAtPoint(attackPosition, damage))
            {
                // Move to unit position if unit is killed
                endPosition = attackPosition;
            }
        }
        else if (hitObject == World.WorldObject.Hut && unitType == World.WorldObject.Enemy)
        {
            if (World.instance.AbsorbHutAtPoint(attackPosition, data))
            {
                endPosition = attackPosition;
                World.instance.DamageObjectAtPoint(gridPositioin, 100);
                absorbed = true;
            }
        }

        Debug.Log("Attacking from: " + gridPositioin + ", to: " + endPosition + " With attack direction: " + direction + " and fixed direction: " + attackDirection + ", woth damage: " + damage);

        Vector3 endPos = GetUnitWorldPosition(endPosition);

        //Update position in world
        if (!absorbed)
        {
            World.instance.MarkWorldPosition(gridPositioin.x, gridPositioin.y, World.WorldObject.Empty);
            unit.gridPositioin = endPosition;
            World.instance.MarkWorldPosition(gridPositioin.x, gridPositioin.y, unitType);
        }

        float elapsedTime = 0;
        float durationInSeconds = ATTACK_DURATION;

        while (elapsedTime < durationInSeconds)
        {
            unit.transform.position = Vector2.Lerp(startinPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / durationInSeconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        unit.transform.position = endPos;
    }

    private Vector2Int GetClearDirection(Vector2Int direction, bool excludeHitPos)
    {
        if (direction.x > 0)
        {
            for (int x = 1; x <= direction.x; x++)
            {
                if (World.instance.GetObjectAtPosition(gridPositioin.x + x, gridPositioin.y) != World.WorldObject.Empty)
                {
                    return (excludeHitPos) ? new Vector2Int(x - 1, 0) : new Vector2Int(x, 0);
                }
            }
            return direction;
        }
        if (direction.x < 0)
        {
            for (int x = -1; x >= direction.x; x--)
            {
                if (World.instance.GetObjectAtPosition(gridPositioin.x + x, gridPositioin.y) != World.WorldObject.Empty)
                {
                    return (excludeHitPos) ? new Vector2Int(x + 1, 0) : new Vector2Int(x, 0);
                }
            }
            return direction;
        }
        if (direction.y > 0)
        {
            for (int y = 1; y <= direction.y; y++)
            {
                if (World.instance.GetObjectAtPosition(gridPositioin.x, gridPositioin.y + y) != World.WorldObject.Empty)
                {
                    return (excludeHitPos) ? new Vector2Int(0, y - 1) : new Vector2Int(0, y);
                }
            }
            return direction;
        }
        if (direction.y < 0)
        {
            for (int y = -1; y >= direction.y; y--)
            {
                if (World.instance.GetObjectAtPosition(gridPositioin.x, gridPositioin.y + y) != World.WorldObject.Empty)
                {
                    return (excludeHitPos) ? new Vector2Int(0, y + 1) : new Vector2Int(0, y);
                }
            }
            return direction;
        }
        return direction;
    }


    public Vector3 GetUnitWorldPosition()
    {
        return new Vector3(gridPositioin.x + 0.5f, gridPositioin.y + 0.5f, 0);
    }

    public Vector3 GetUnitWorldPosition(Vector2Int pos)
    {
        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
    }

    // Returns true if dead
    public bool DamageUnit(int damage)
    {
        if (health <= 0) return false;
        this.health -= damage;
        Debug.Log("Unit at: " + gridPositioin + " damaged");
        Debug.Log("UNIT DAMAGED!!!, Health " + health + " Damage: " +damage);
        if (health <= 0)
        {
            health = 0;
            HideActionMarkers();
            Invoke("DestroyUnit", ATTACK_DURATION-0.2f);
            Debug.Log("Unit destroyed");
            return true;
        }
        return false;
    }

    public void DestroyUnit()
    {
        World.instance.CheckVictory();
        Destroy(gameObject);

        var effect = Instantiate(deathEffectPrefab, transform);
        effect.transform.parent = transform.parent;
        Destroy(effect, 6f);
    }

}

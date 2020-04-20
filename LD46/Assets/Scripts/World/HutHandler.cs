using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HutHandler : MonoBehaviour
{
    [SerializeField] private Hut hutPrefab;
    private List<Hut> huts;

    // Start is called before the first frame update
    void Start()
    {
        // Place huts
        huts = new List<Hut>();
        foreach (var point in World.instance.GetAllObjectPoints(World.WorldObject.Hut))
        {
            var h = Instantiate(hutPrefab, transform);
            h.Initialize(point);
            h.transform.position = new Vector3(point.x + 0.5f, point.y + 0.5f, 0);
            huts.Add(h);
        }
    }

    public bool DamageHutAtPoint(Vector2Int point, int damage)
    {
        for (int i = 0; i < huts.Count; i++)
        {
            if (huts[i].gridPosition == point)
            {
                if (huts[i].Damage(damage))
                {
                    huts.Remove(huts[i]);
                    return true;
                }
            }
        }
        return false;
    }

    public bool AbsorbHutAtPoint(Vector2Int point, EnemyData enemy)
    {
        for (int i = 0; i < huts.Count; i++)
        {
            if (huts[i].gridPosition == point)
            {
                if (huts[i].Absorb(enemy)) return true;
                return false;
            }
        }
        return false;
    }

    public void IncrementTurn()
    {
        for (int i = 0; i < huts.Count; i++)
        {
            huts[i].NewTurn();
        }
    }

    public bool AbsorbedHutsExists()
    {
        foreach (var hut in huts)
        {
            if (hut.IsAbsorbed()) return true;
        }
        return false;
    }
}

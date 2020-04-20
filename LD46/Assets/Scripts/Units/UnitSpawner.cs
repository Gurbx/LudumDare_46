using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : MonoBehaviour
{
    private Vector2Int gridPoint;
    private SpriteRenderer renderer;
    List<SpawnerListener> listeners;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        listeners = new List<SpawnerListener>();
    }

    public void SetPosition(Vector2Int gridP)
    {
        this.gridPoint = gridP;
        this.transform.position = new Vector3(gridP.x + 0.5f, gridP.y + 0.5f, 0);
    }

    public void RegisterListener(SpawnerListener lis)
    {
        listeners.Add(lis);
    }

    private void OnMouseEnter()
    {
        renderer.color = new Color(0, 1, 0, 0.75f);
    }

    private void OnMouseExit()
    {
        renderer.color = new Color(0, 1, 0, 0.5f);
    }

    private void OnMouseDown()
    {
        foreach (var l in listeners)
        {
            l.ObjectClicked(gridPoint);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }


}

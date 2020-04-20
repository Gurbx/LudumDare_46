using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHandler : MonoBehaviour, SpawnerListener
{
    [SerializeField] private AudioSource moveSound;
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private UnitSpawner selectionMarker;

    GameHandler gameHandelr;
    private List<Unit> units;


    private List<UnitSpawner> activeSelectionmarkers;

    private void Awake()
    {
        units = new List<Unit>();
    }

    public void HandleUnitPlacments(GameHandler gameHandler)
    {
        this.gameHandelr = gameHandler;
        if (units.Count < GameData.instance.numberOfUnits) PlaceUnit();
    }

    private void PlaceUnit()
    {
        MarkAllEmptyPoints();
    }

    public void MarkAllEmptyPoints()
    {
        HideAllMarkers();
        if (activeSelectionmarkers == null)
        {
            activeSelectionmarkers = new List<UnitSpawner>();
        }

        List<Vector2Int> emptyPonts = World.instance.GetAllObjectPoints(World.WorldObject.Empty);
        foreach (var point in emptyPonts)
        {
            var marker = Instantiate(selectionMarker, transform);
            marker.SetPosition(point);
            marker.RegisterListener(this);
            activeSelectionmarkers.Add(marker);
        }
    }

    void SpawnerListener.ObjectClicked(Vector2Int position)
    {
        var unit = Instantiate(unitPrefab, transform);
        unit.UpdatePosition(position);
        unit.InitializeUnit(GameData.instance.unitHealth, GameData.instance.unitSprite);
        units.Add(unit);
        World.instance.MarkWorldPosition(position.x, position.y, World.WorldObject.Friend);
        HideAllMarkers();

        if (units.Count < GameData.instance.numberOfUnits)
        {
            PlaceUnit();
        }
        else
        {
            gameHandelr.PlacementDone();
        }
    }


    public void HideAllMarkers()
    {
        if (activeSelectionmarkers == null) return;
        foreach (var marker in activeSelectionmarkers)
        {
            Destroy(marker.gameObject);
        }
        activeSelectionmarkers.Clear();
    }


    public void DoActionWithAllUnits(Unit.UnitAction action)
    {
        if (action.type == Unit.UnitAction.ActionType.Move) moveSound.Play();
        foreach (var unit in units)
        {
            unit.DoAction(action);
        }
    }

    public void DisplayActionForAllUnits(Unit.UnitAction action)
    {
        foreach (var unit in units)
        {
            unit.DisplayAction(action);
        }
    }

    public void HideActionMarkersForUnits()
    {
        foreach (var unit in units)
        {
            unit.HideActionMarkers();
        }
    }

    public bool DamageUnitAtPoint(Vector2Int point, int damage)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].gridPositioin == point)
            {
                if (units[i].DamageUnit(damage))
                {
                    units.Remove(units[i]);
                    return true;
                }
            }
        }
        return false;
    }

}

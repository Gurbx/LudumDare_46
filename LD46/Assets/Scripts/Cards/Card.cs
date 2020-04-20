using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Card Data")]
public class Card : ScriptableObject
{
    public string name;
    public string description;
    public Sprite image;
    public int value;
    public Vector2Int direction;
    public int nrTargets; //Only if random

    public Unit.UnitAction unitAction;

    public enum TargetType
    {
        Single,
        All,
        Random
    }

}

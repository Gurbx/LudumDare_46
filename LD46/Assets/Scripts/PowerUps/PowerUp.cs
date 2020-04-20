using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Store Item Data")]
public class PowerUp : ScriptableObject
{
    public string name;
    public Sprite sprite;
    public int extraUnits;
    public int extraHealth;
    public int cost;
}

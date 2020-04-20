using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/World Data")]
public class WorldInput : ScriptableObject
{
    public Vector2Int size;
    public int minHuts, maxHuts;
    public int minEnemies, maxEnemies;
    public List<EnemyTypes> enemies;

    public int rewardUnits;
    public bool offerRandomReward;
    public List<string> tutorialTexts;


    [System.Serializable]
    public struct EnemyTypes
    {
        public int max;
        public EnemyData data;
    }
}

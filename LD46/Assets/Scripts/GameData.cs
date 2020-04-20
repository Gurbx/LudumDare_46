using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [HideInInspector] public int numberOfUnits;
    [HideInInspector] public int unitHealth;
    [HideInInspector] public int attackCellChance;
    [HideInInspector] public int health;
    [HideInInspector] public int levelNumber;
    [HideInInspector] public WorldInput input;
    [HideInInspector] public int money;

    [HideInInspector] public int score;

    [SerializeField] private List<WorldInput> inputs;


    public bool gameIsOver;
    public Sprite unitSprite;

    public static GameData instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        ResetData();
        DontDestroyOnLoad(this);
    }

    public void ResetData()
    {
        numberOfUnits = 1;
        unitHealth = 1;
        attackCellChance = 30;
        health = 3;
        gameIsOver = false;
        levelNumber = 0;
        money = 0;
    }

    public void IncrementLevel()
    {
        money += Random.Range(25, 35);

        if (levelNumber != 0)
        {
            attackCellChance += 10;
            if (attackCellChance >= 75) attackCellChance = 75;
            numberOfUnits += inputs[levelNumber-1].rewardUnits;
        }

        input = inputs[levelNumber];
        levelNumber++;
    }

    public void CalculateScore()
    {
        //TODO?
    }
}

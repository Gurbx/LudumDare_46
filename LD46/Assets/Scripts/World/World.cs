using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Cinemachine;

public class World : MonoBehaviour
{
    [HideInInspector] public WorldInput input;
    [SerializeField] Hut hutPrefab;

    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private List<TileBase> groundTiles;
    [SerializeField] private Tilemap groundMap;

    [SerializeField] public EnemyHandler enemyHandler;
    [SerializeField] UnitHandler unitHandler;
    [SerializeField] HutHandler hutHandler;
    [SerializeField] private HealthUI healthUI;
    [SerializeField] private CinemachineVirtualCamera shakyCam;
    [SerializeField] private Animator fade;

    private static WorldObject[,] worldMap;

    public static World instance;

    public enum WorldObject
    {
        Empty,
        Obstacle,
        Enemy,
        Friend,
        Hut
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        GenerateWorld(GameData.instance.input); //Temp
    }

    private void Start()
    {
        healthUI.SetHP(GameData.instance.health);
    }

    public void GenerateWorld(WorldInput input)
    {
        this.input = input;
        cameraTarget.transform.position = new Vector3(input.size.x/2, input.size.y/2 - input.size.y/9, 0);
        worldMap = new WorldObject[input.size.x, input.size.y];

        // Basic ground
        for (int y = 0; y < input.size.y; y++)
        {
            for (int x = 0; x < input.size.x; x++)
            {
                groundMap.SetTile(new Vector3Int(x, y, 0), groundTiles[Random.Range(0, groundTiles.Count)]);
                worldMap[x, y] = WorldObject.Empty;
            }
        }

        // Create huts
        int hutsToCreate = Random.Range(input.minHuts, input.maxHuts);
        List<Vector2Int> hutPoints = new List<Vector2Int>();
        while (hutPoints.Count < hutsToCreate)
        {
            Vector2Int point = new Vector2Int(Random.Range(0, input.size.x), Random.Range(0, input.size.y));
            if (worldMap[point.x, point.y] == WorldObject.Empty)
            {
                worldMap[point.x, point.y] = WorldObject.Hut;
                hutPoints.Add(point);
            }
        }

        // Create enemy positons
        int enemiesToCreate = Random.Range(input.minEnemies, input.maxEnemies);
        List<Vector2Int> enemySpawnPoints = new List<Vector2Int>();
        while (enemySpawnPoints.Count < enemiesToCreate)
        {
            Vector2Int point = new Vector2Int(Random.Range(0, input.size.x), Random.Range(0, input.size.y));
            if (worldMap[point.x, point.y] == WorldObject.Empty)
            {
                worldMap[point.x, point.y] = WorldObject.Enemy;
                enemySpawnPoints.Add(point);
            }
        }
    }

    public WorldObject GetObjectAtPosition(int x, int y)
    {
        if (x < 0 || x >= worldMap.GetLength(0)
            || y < 0 || y >= worldMap.GetLength(1))
        {
            //Is obstacle if out of bounds
            return WorldObject.Obstacle;
        }
        return worldMap[x, y];
    }

    public void MarkWorldPosition(int x, int y, WorldObject wobj)
    {
        worldMap[x, y] = wobj;
    }

    public Vector2Int GetClosestObjectFromPosition(WorldObject objectType, Vector2Int fromPosition, bool excludeCurretPos)
    {
        float minDistance = float.MaxValue;
        Vector2Int closest = new Vector2Int(-1, -1);

        for (int y = 0; y < worldMap.GetLength(1); y++)
        {
            for (int x = 0; x < worldMap.GetLength(0); x++)
            {
                if (worldMap[x,y] == objectType)
                {
                    Vector2Int coords = new Vector2Int(x, y);
                    if (excludeCurretPos && coords == fromPosition) continue;

                    float distance = Vector2Int.Distance(coords, fromPosition);
                    if (distance < minDistance)
                    {
                        closest = coords;
                    }
                }
            }
        }

        return closest;
    }

    public List<Vector2Int> GetAllObjectPoints(WorldObject objectType)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        for (int y = 0; y < worldMap.GetLength(1); y++)
        {
            for (int x = 0; x < worldMap.GetLength(0); x++)
            {
                if (worldMap[x, y] == objectType) points.Add(new Vector2Int(x, y));
            }
        }
        return points;
    }

    /// Returns true if target is killed
    public bool DamageObjectAtPoint(Vector2Int point, int damage)
    {
        if (enemyHandler.DamageEnemyAtPoint(point, damage) || unitHandler.DamageUnitAtPoint(point, damage) || hutHandler.DamageHutAtPoint(point, damage))
        {
            if (worldMap[point.x, point.y] == WorldObject.Hut) //If hut is destroyed reduce world health
            {
                if (GameData.instance.gameIsOver) return true;
                GameData.instance.health -= 1;
                healthUI.SetHP(GameData.instance.health);
                if (GameData.instance.health <= 0)
                {
                    GameOver();
                }
            }
            if (GetAllObjectPoints(WorldObject.Friend).Count <= 0)
            {
                if (GameData.instance.gameIsOver) return true;
                GameOver();
            }

            worldMap[point.x, point.y] = WorldObject.Empty;
            ScreenShakeHandler.AddScreenShake(5, 5, 0.5f);
            return true;
        }
        return false;
    }

    public void CheckVictory()
    {
        if (GetAllObjectPoints(WorldObject.Friend).Count <= 0)
        {
            if (GameData.instance.gameIsOver) return;
            GameOver();
        }

        if (GetAllObjectPoints(WorldObject.Enemy).Count > 0) return;
        if (hutHandler.AbsorbedHutsExists()) return;

        //Update gamedata with number of units left
        GameData.instance.numberOfUnits = GetAllObjectPoints(WorldObject.Friend).Count;

        fade.SetTrigger("FadeOut");
        Invoke("ToNextLevelScene", 2f);
    }

    public bool AbsorbHutAtPoint(Vector2Int point, EnemyData enemy)
    {
        return hutHandler.AbsorbHutAtPoint(point, enemy);
    }

    private void GameOver()
    {
        GameData.instance.gameIsOver = true;
        shakyCam.Priority = 20;
        Invoke("ToGameOverScene", 3f);
        fade.SetTrigger("FadeOut");
    }

    private void ToGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }

    private void ToNextLevelScene()
    {
        SceneManager.LoadScene("LimboScene", LoadSceneMode.Single);
    }
}

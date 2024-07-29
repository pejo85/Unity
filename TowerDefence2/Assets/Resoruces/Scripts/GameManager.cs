using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject GridManager;
    GridManager_script gridManager_Script;

    [SerializeField] GameObject enemy;
    [SerializeField] GameObject towerCannon;
    [SerializeField] GameObject[] towersFromMarket = new GameObject[1];  
    List<GameObject> towerList;
    GameObject purchasedTower;
    [SerializeField] Button buttonTower;

    public Vector3 mousePosOverTile;


    Enemy_script enemy_script;
    public List<GameObject> enemyList;





    private void Awake()
    {
        gridManager_Script = GridManager.GetComponent<GridManager_script>();
        towerList = new List<GameObject>();
        enemyList = new List<GameObject>();
    }
    


    public void PurchaseCannonTower()
    {
        Vector3 pos = mousePosOverTile;
        pos = new Vector3(pos.x, 0.2f, pos.z);
        purchasedTower = towersFromMarket[0];
        SpawnTower(purchasedTower, pos);
    }

    private void SpawnTower(GameObject Tower, Vector3 pos)
    {
        Tower = Instantiate(towerCannon, pos, Quaternion.identity);
        Tower_script tower_script = Tower.GetComponent<Tower_script>();
        tower_script.dragging = true;
        //tower_script.

        towerList.Add(Tower);
    }

    private void SpawnEnemy()
    {
        GameObject Enemy = Instantiate(enemy, new Vector3(2, 0.2f, 6), Quaternion.identity);
        enemy_script = Enemy.GetComponent<Enemy_script>();
        enemy_script.ObtainPathCells(gridManager_Script.pathCells);
        enemy_script.EnemySpeed = 50;

        enemyList.Add(Enemy);
    }

    public void StartEnemyWave()
    {
        SpawnEnemy();
    }



}

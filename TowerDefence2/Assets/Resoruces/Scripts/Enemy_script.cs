using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy_script : MonoBehaviour
{
    private List<Vector2Int> pathCells;
    public Vector3 startingPos = new Vector3(1f, 0.2f, 6f);
    public Vector3 targetPos;
    [SerializeField] public float EnemySpeed;
    int index = 0;
    [SerializeField] int health = 100;

    private void Start()
    {
        EnemySpeed = 20 * Time.deltaTime * 0.1f;
        transform.position = startingPos;
    }

    public void ObtainPathCells(List<Vector2Int> pathCells)
    {
        this.pathCells = pathCells;
    }

    private void Update()
    {
        Vector3 targetPos = new Vector3 (pathCells[index].x , 0.2f , pathCells[index].y) ;
        MoveEnemy(targetPos);

        if (Vector3.Distance(transform.position , targetPos) < 0.1f)
        {
            if (index == pathCells.Count -1)
            {
                DestroyObject();
            }
            else
            {
                index++;
            }
            
        }
    }

    private void MoveEnemy(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, EnemySpeed);
    }

    public Vector3 GetCurrentSpeed()
    {
        return transform.forward * EnemySpeed;
    }

    private void DestroyObject()
    {
        Destroy(gameObject); 
    }
}

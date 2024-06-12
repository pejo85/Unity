using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_script : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager_script gameManager_script;

    private Vector3 initialMousePos;
    private Vector3 mouseOffset;
    public bool mouseIsClicked;

    public bool isEnemyBoard;
    public bool isPlayerBoard;

    public bool isOcupied;
    public bool isRevealed;
    public bool isUnderAirDefense;
    public bool wasShot;

    [SerializeField] private bool gameStarted;

    //public bool gameOver = false;
    //GameObject[] CubesSateliteNeighboursList = new GameObject[4];


    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManager_script = gameManager.GetComponent<GameManager_script>();
    }

    private void OnMouseOver()
    {
        if (SateliteIsWatching())
        {
            gameManager_script.SateliteHoversOverCube(this.gameObject);
        }
    }

    private void OnMouseExit()
    {
        if ( SateliteIsWatching() )
        {
            gameManager_script.SateliteHoversExitCube(this.gameObject);
        }
    }

    private void OnMouseDown()
    {
        mouseIsClicked = false;

        if (gameStarted)
        {
            initialMousePos = GetMouseWorldPos();
        }
    }

    private void OnMouseUp()
    {
        mouseIsClicked = true;
    
        if (gameStarted)
        {
            if (SateliteIsWatching())
            {
                gameManager_script.SateliteRevealsEnemyTiles(this.gameObject);
                gameManager_script.SateliteStopsWatching();
            }
            else if (gameManager_script.playerMove)
            {
                // When player shoots the enemy
                if (PlayerCanShoot() )
                {
                    gameManager_script.PlayerShootsToEnemy(this.gameObject);
                }
            }
    
        }
    
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void ResetCubeProperties()
    {
        mouseIsClicked = false;
        isOcupied = false;
        isRevealed = false;
        isUnderAirDefense = false;
        wasShot = false;
        gameStarted = false;
        
    }

    public void IsOcupied()
    {
        isOcupied = true;
    }

    public void IsFree()
    {
        isOcupied = false;
    }

    public void GameStarted(bool gameHasStarted)
    {
        gameStarted = gameHasStarted;
    }

    public void RevealTile()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 0;
        isRevealed = true;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool HitTheTarget()
    {
        // Hit the target
        if (isOcupied)
        {
            return true;
        }

        // If missed
        return false;
    }

    public bool SateliteIsWatching()
    {
        return isEnemyBoard && gameManager_script.sateliteIsWatching;
    }

    public bool PlayerCanShoot()
    {
        return mouseIsClicked && isEnemyBoard && !gameManager_script.sateliteIsWatching;
    }

    public void CubeColorChange(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

}

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

    private Vector3 tileDefaultColor;


    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManager_script = gameManager.GetComponent<GameManager_script>();

        tileDefaultColor = gameManager_script.tileDefaultColor;
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
        //GetComponent<SpriteRenderer>().color = Color.white;
        CubeColorChange(tileDefaultColor);
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
        return mouseIsClicked && isEnemyBoard && !gameManager_script.sateliteIsWatching && !gameManager_script.pause;
    }

    public void CubeColorChange(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void CubeColorChange(float r, float g, float b)
    {
        Color color = new Color(r / 255f, g / 255f, b / 255f);
        GetComponent<SpriteRenderer>().color = color;
    }

    public void CubeColorChange(Vector3 RGB)
    {
        Color color = new Color(RGB.x / 255f, RGB.y / 255f, RGB.z / 255f);
        GetComponent<SpriteRenderer>().color = color;
    }

}

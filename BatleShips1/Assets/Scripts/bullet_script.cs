using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_script : MonoBehaviour
{

    private GameManager_script gameManager_script;

    public float rocketNormalSpeed = 7f;
    public float rocketInterseptionlSpeed = 8f;
    private Rigidbody2D rb;
    public bool isPlayerShot;
    public bool interseptingRocket;

    GameObject targetObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager_script = GameObject.Find("GameManager").GetComponent<GameManager_script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interseptingRocket)
        {
            SeekTargetPosition();
        }

        RotateTowardsDirection();
    }

    public void ShootRocket(GameObject victimObject)
    {
        this.targetObject = victimObject;
        Vector2 targetPosition = victimObject.transform.position;

        //this.targetPosition = targetPosition;
        Vector2 startPosition = transform.position;
        Vector2 direction = (targetPosition - startPosition).normalized;

        float distance = Vector2.Distance(startPosition, targetPosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float time = distance / rocketNormalSpeed;
        float vx = direction.x * rocketNormalSpeed;
        float vy = (direction.y * rocketNormalSpeed) + (0.5f * Mathf.Abs(Physics2D.gravity.y) * time);

        rb.velocity = new Vector2(vx, vy);
        rb.rotation = angle;
    }

    void RotateTowardsDirection()
    {
        Vector2 direction = rb.velocity;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    private void SeekTargetPosition()
    {
        rocketNormalSpeed = rocketInterseptionlSpeed;
        Vector3 targetPosition = this.targetObject.transform.position;
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * rocketNormalSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.tag == "Bullet")  // Bullet intersepts another bullet
            {
                collision.GetComponent<Bullet_script>().destroyBullet();
                destroyBullet();
            }
            else
            {
                if (gameManager_script != null)
                {
                    if (gameManager_script.playerMove)
                    {
                        if (collision.GetComponent<Cube_script>().isEnemyBoard && targetObject == collision.gameObject)
                        {
                            gameManager_script.PlayerHitOrMissTarget(collision.gameObject);
                            Destroy(gameObject);
                        }


                    }
                    else if (gameManager_script.enemyMove)
                    {
                        if (collision.GetComponent<Cube_script>().isPlayerBoard && targetObject == collision.gameObject)
                        {
                            gameManager_script.EnemyHitOrMissTarget(collision.gameObject, new Vector2Int((int)collision.transform.position.x, (int)collision.transform.position.y));
                            Destroy(gameObject);
                        }

                    }
                    else
                    {
                        Debug.Log("??????????????? = " + collision.name + " - " + collision.transform.position);
                    }
                }
                else
                {
                    Debug.LogError("GameManager_script reference is null in bullet_script");
                }
            }

            
        }
    }

    public bool BulletHitTarget(Vector3 collisionPos)
    {
        //Debug.Log(Vector2.Distance(transform.position, collisionPos));
        float temp = Mathf.Abs(Vector2.Distance(transform.position, collisionPos));
        return temp < 1f;
    }


    public void destroyBullet()
    {
        gameManager_script.targetAnimObject.SetActive(false);
        gameManager_script.bulletIsInTheAir = false;
        if (interseptingRocket)
        {
            gameManager_script.Explotion(transform.position);
        }
        
        Destroy(gameObject);
    }



}

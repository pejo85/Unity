using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_script : MonoBehaviour
{

    private GameManager_script gameManager_script;

    //public float rocketNormalSpeed = 7f;
    //public float rocketInterseptionlSpeed = 8f;
    public float rocketNormalSpeed;
    public float rocketInterseptionlSpeed;
    private Rigidbody2D rb;
    public bool isPlayerShot;
    public bool interseptingRocket;

    GameObject targetObject;

    public Vector2 targetPosition; // Temp

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager_script = GameObject.Find("GameManager").GetComponent<GameManager_script>();

        rocketNormalSpeed = gameManager_script.rocketNormalSpeed;
        rocketInterseptionlSpeed = gameManager_script.rocketInterseptionlSpeed;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (interseptingRocket)
    //    {
    //        SeekTargetPosition();
    //    }
    //
    //    RotateTowardsDirection();
    //}

    private void FixedUpdate()
    {
        if (interseptingRocket)
        {
            SeekTargetPosition();
            
        }
        RotateTowardsDirection();
        bulletAutoDestroy();
    }

    public void ShootRocket(GameObject victimObject, float rocketSpeed)
    {
        

        this.targetObject = victimObject;
        //Vector2 targetPosition = victimObject.transform.position;
        targetPosition = victimObject.transform.position;


        //this.targetPosition = targetPosition;
        Vector2 startPosition = transform.position;
        Vector2 direction = (targetPosition - startPosition).normalized;

        float distance = Vector2.Distance(startPosition, targetPosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float time = distance / rocketSpeed;
        float vx = direction.x * rocketSpeed;
        float vy = (direction.y * rocketSpeed) + (0.5f * Mathf.Abs(Physics2D.gravity.y) * time);

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
        Vector3 targetPosition;

        if (this.targetObject == null) // Couldnot prevent attac - interception failled
        {
            targetPosition = new Vector3(100,100,100);
        }
        else // standart interception
        {
            targetPosition = this.targetObject.transform.position;
            this.targetPosition = targetPosition; // Temp
            Vector2 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * rocketInterseptionlSpeed;
        }

        
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

    //public bool BulletHitTarget(Vector3 collisionPos)
    //{
    //    float temp = Mathf.Abs(Vector2.Distance(transform.position, collisionPos));
    //    return temp < 1f;
    //}

    public void bulletAutoDestroy()
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, new Vector3(0, 0, 0))) > 50f)
        {
            Destroy(gameObject);
        }
    }

    public void destroyBullet()
    {
        if (!interseptingRocket)
        {
            //gameManager_script.targetAnimObject.SetActive(false);
            gameManager_script.bulletIsInTheAir = false;
        }
        else if (interseptingRocket)
        {
            interseptingRocket = false;

            gameManager_script.targetAnimObject.SetActive(false);  // zemodan ak chamovitane
            gameManager_script.Explotion(transform.position);
    
            gameManager_script.interseptionWasSuccess = true;
            gameManager_script.InterseptionSuccessed();
        }

        //if (interseptingRocket)
        //{
        //    gameManager_script.interseptionWasSuccess = true;
        //    gameManager_script.InterseptionSuccessed();
        //}
        

        Destroy(gameObject);
    }



}

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class bullet_script : MonoBehaviour
{
    //[SerializeField] public GameObject GameManager;

    private GameManager_script gameManager_script;

    public Vector2 targetPosition;
    public float speed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        gameManager_script = GameObject.Find("GameManager").GetComponent<GameManager_script>();
        //gameManager_script = GameManager.GetComponent<GameManager_script>();

        rb = GetComponent<Rigidbody2D>();
        ShootRocket();
    }



    void ShootRocket()
    {
        Vector2 startPosition = transform.position;
        Vector2 direction = (targetPosition - startPosition).normalized;

        float distance = Vector2.Distance(startPosition, targetPosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float time = distance / speed;
        float vx = direction.x * speed;
        float vy = (direction.y * speed) + (0.5f * Mathf.Abs(Physics2D.gravity.y) * time);

        rb.velocity = new Vector2(vx, vy);
        rb.rotation = angle;
    }

    public bool BulletHitTarget(Vector2 collisionPos)
        { 
        return collisionPos == targetPosition;
        }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (BulletHitTarget(collision.gameObject.transform.position))
        {
            gameManager_script.HitOrMissTarget(collision.gameObject);
            Destroy(gameObject);
        }
    }

}

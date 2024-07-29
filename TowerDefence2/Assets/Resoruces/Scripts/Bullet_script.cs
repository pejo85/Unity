using System.Collections;
using System.Collections.Generic;
using TMPro;


//using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bullet_script : MonoBehaviour
{
    [SerializeField] GameObject Tower;
    Tower_script tower_script;
    private Vector3 targetPosition;
    private float speed;

    private void Start()
    {
        tower_script = Tower.GetComponent<Tower_script>();
    }

    public void SetTarget(Vector3 targetPosition, float speed)
    {
        this.targetPosition = targetPosition;
        this.speed = speed;
    }

    private void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Destroy the projectile if it reaches the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                //Destroy(gameObject);
                StartCoroutine(DestroyBullet());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colidet to " + other.name + " - " + other.transform.position);
        tower_script.bulletIsInTheAir = false;

        StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyControl : MonoBehaviour
{
    Vector2 movement;
    public Animator animator;
    float health = 10;

    GameObject player;
    float forceScale = 5;
    [SerializeField]
    float detectionRadius = 3;

    [SerializeField]
    GameObject milkPrefab;
    [SerializeField]
    AudioSource moo;
    [SerializeField]
    AudioSource clink;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    } 

    private void FixedUpdate()
    {
        //feature point: raycast
        RaycastHit2D hit;
        Vector2 targetVec;

        //feature point: layers
        LayerMask layerMask = LayerMask.GetMask("Walls", "Friendly");

        targetVec = (player.transform.position - transform.position).normalized;
        hit = Physics2D.CircleCast(transform.position, .5f,
            targetVec, detectionRadius, layerMask);

        if (hit.collider?.name == "Player")
            GetComponent<Rigidbody2D>().AddForce(targetVec * forceScale);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //feature point: tags
        if (collision.gameObject.tag == "sprout")
        {
            Destroy(collision.gameObject);
            moo.Play();
            health--;

            //feature point: Scoring system
            EnemySpawn.score++;
            //feature point: AddForce
            forceScale *= 1.2f;
            InvokeRepeating("fireAtPlayer", 0.5f, 1f);

            if (health <= 0)
                Destroy(this.gameObject);
            else
                GetComponent<SpriteRenderer>().material.color *= new Color(1, 1, 1, (float)health / 4.0f);
        }
    }

    void fireAtPlayer()
    {
        GameObject milk;
        milk = Instantiate(milkPrefab, transform.position, Quaternion.identity);
        clink.Play();

        Vector2 aim = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        milk.transform.up = aim;
        milk.GetComponent<Rigidbody2D>().velocity = aim * 5;

        Destroy(milk, 1.0f);
    }
}

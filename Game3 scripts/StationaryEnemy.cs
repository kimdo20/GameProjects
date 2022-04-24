using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [SerializeField]
    AudioSource caw;

    int health = 3;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "sprout")
        {
            Destroy(collision.gameObject);
            caw.Play();
            health--;
            EnemySpawn.score++;
            if(health <= 0)
                Destroy(this.gameObject);
        }
    }
}

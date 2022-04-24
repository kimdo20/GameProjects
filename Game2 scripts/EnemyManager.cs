using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject dropPrefab;
    public Vector2 direction;// = new Vector2(1f,0);
    public float speed;// = 1f;
    float edgeBounds = 30f;
    float dropInterval = 1.0f;


    void Start()
    {
        Invoke("ChangeDirection", Random.Range(1f, 3f));
        Invoke("DropItem", dropInterval);
        Invoke("IncSpeed", 2f);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        Vector2 pos = transform.position;

        if (pos.x < -edgeBounds || pos.x > edgeBounds)
        {
            speed *= -1;
            pos.x = Mathf.Clamp(pos.x, -edgeBounds, edgeBounds);
        }

        transform.position = pos;
    }

    void ChangeDirection()
    {
        speed *= -1f;
        GetComponent<SpriteRenderer>().flipX = speed < 0;
        Invoke("ChangeDirection", Random.Range(1f, 3f));
    }

    public void stopDrop()
    {
        CancelInvoke("DropItem");
        CancelInvoke("IncSpeed");
        CancelInvoke("DecSpeed");
    }

    void DropItem()
    {
        Instantiate(dropPrefab, transform.position, Quaternion.identity);

        if (dropInterval > 0.1f)
            dropInterval *= .90f;

        Invoke("DropItem", dropInterval);
    }

    void IncSpeed()
    {
        speed += 1f;
        Invoke("DecSpeed", 2f);
    }

    void DecSpeed()
    {
        speed -= 1f;
        Invoke("IncSpeed", 1f);
    }
}
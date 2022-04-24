using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/* Cat themed:
 * - cat starts off in sitting position, then goes into falling position while dropping
 * - sound effects: meows
 * - bg music (light, fun music)
 * - bg design (cat tree?)
 * - plane = 
 * - basket = blankets?
 */

/* Feature points (mentioned in class)
 * Increase drop rate
 * increase enemy speed
 * game over condition
 * sound / animation(?)
 * variety in dropped items
 * managing baskets
 * lists or arrays?
 * GetInParent()
 */

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    float edgeBounds = 40f;
    public GameObject basketPrefab;

    // Update is called once per frame
    void Update()
    {
        //make diff way for feature point
        Vector3 mousePos = Input.mousePosition;
        Vector3 newPos;

        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //GameObject player = GameObject.Find("Player");

        newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = transform.position.z;
        newPos.y = transform.position.y;
        newPos.x = Mathf.Clamp(newPos.x, -edgeBounds, edgeBounds);

        transform.position = newPos;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // dropped item hits basket (good)
        if (collision.gameObject.tag == "drop")
        {
            Destroy(collision.gameObject);
            GameObject.Find("Boundary").GetComponent<BoundaryManager>().updateScore();
        }
        /*else if (gameObject.tag == "basket")
        {
            Debug.Log("hit a basket?");
        }*/
    }

    void clearObjects()
    {
        foreach (GameObject drop in GameObject.FindGameObjectsWithTag("drop"))
            Destroy(drop);
    }
}

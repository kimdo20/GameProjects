using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* Project summary (from LMS): 
 * For project 3 I created a top-down shooter game in a cozy island setting (itch.io: https://kimd.itch.io/island-survivin).
 * The player starts off with 3 lives, each having 10 health. There are two types of enemies: a stationary bird enemy that damages 
 * the player on contact, and a moving cow enemy that fires milk bottles and chases the player when in range. The bird enemy takes 3 shots to clear,
 * while the cow takes 10 hits and increases the firing amount each time it is hit. Both player bullets (sprout sprites) and enemy bullets (milk bottle sprites) 
 * are unable to pass through trees and other objects placed around the platform, but can fly off of the edges. The player and enemies cannot walk off the edges
 * of the platform, and enemies randomly spawn throughought the middle of it so as not to hit the trees. Enemies also cannot detect the player when they are 
 * standing behind a tree, and stop chasing if they do so. The game progresses through waves and a timer: the first wave starts off with 2 spawned enemies on a 
 * 10 second timer, and once the timer hits zero the next wave starts (each wave doubles the number of enemies, and slightly increases the timer). When spawning, 
 * enemies first have a growing shadow appear on the platform to indicate where they will spawn. After 2 seconds, the actual enemy will appear where the shadow was.
 * A current score and highscore are shown at the top of the screen; after the player runs out of lives, if their current score is higher than the highscore 
 * (which is saved on PlayerPrefs) it will replace it and reflect the changes once they replay the game (prompted by a button on an ending screen). 
 * The game also has 2 types of powerups that randomly spawn on the platform: the berry powerup doubles the player's speed for 2 seconds, while the apple powerup 
 * gives the player an extra life. Each time the player picks up a powerup, a fading in/out notification will appear above their head. There are also multiple 
 * audio sounds implemented throughout the game: the starting, looping background music, the player's squeak sound when they lose health, the bird enemy's chirp 
 * sound when it takes damage, the cow's "moo" sound when it gets hit by a player bullet, the "woosh" sound a player bullet makes each time it is fired, and the 
 * "clink" sound of the milk bottle bullets when they are fired.
 */

public class PlayerControl : MonoBehaviour
{
    // feature point: itch.io upload https://kimd.itch.io/island-survivin
    // feature point: tilemaps
    // feature point: background music
    // feature point: animation state machine
    // feature point: instantiating prefabs

    [SerializeField]
    GameObject sproutPrefab;
    [SerializeField]
    public float forceScale;
    [SerializeField]
    AudioSource squeak;
    [SerializeField]
    AudioSource woosh;

    [SerializeField]
    GameObject applePrefab;
    [SerializeField]
    GameObject berryPrefab;
    TextMeshProUGUI speedDisp;
    TextMeshProUGUI heartDisp;

    int lives = 3;
    float health = 10;
    TextMeshProUGUI livesDisp;
    TextMeshProUGUI healthDisp;

    GameObject display;
    GameObject replayButton;

    Vector2 inputVector, lookVector;

    public float speed = 2f;
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator animator;

    void Start()
    {
        Physics2D.gravity = new Vector2(0, 0);
        livesDisp = GameObject.Find("Lives").GetComponent<TextMeshProUGUI>();
        healthDisp = GameObject.Find("Health").GetComponent<TextMeshProUGUI>();
        speedDisp = GameObject.Find("SpeedBoost").GetComponent<TextMeshProUGUI>();
        heartDisp = GameObject.Find("HeartBoost").GetComponent<TextMeshProUGUI>();
        display = GameObject.Find("Display");
        replayButton = GameObject.Find("Replay");
        display.SetActive(false);
        replayButton.SetActive(false);

        Invoke("SpawnItem", Random.Range(5f, 10f));
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // feature point: animations
        // feature point: blendtrees
        animator.SetFloat("horizontal", movement.x);
        animator.SetFloat("vertical", movement.y);
        animator.SetFloat("speed", movement.sqrMagnitude);

        // feature point: input system
        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 ||
            Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            animator.SetFloat("prevHorz", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("prevVert", Input.GetAxisRaw("Vertical"));
        }

        Vector2 mousePosition;
        mousePosition = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetComponent<PointEffector2D>().enabled = Input.GetButton("Fire1");

        lookVector = (mousePosition - (Vector2) transform.position).normalized;

        if (Input.GetButtonDown("Fire1"))
        {
            //feature point: shooting
            //feature point: mouse aim
            GameObject sprout;
            sprout = Instantiate(sproutPrefab, transform.position, Quaternion.identity);
            sprout.transform.up = lookVector;
            woosh.Play();
            sprout.GetComponent<Rigidbody2D>().velocity = lookVector * 5;
            Destroy(sprout, 1.0f);
        }

        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    //feature point: colliders
    private void OnCollisionEnter2D(Collision2D collision)
    {

        // powerup: gain life
        if (collision.gameObject.tag == "apple")
        {
            Destroy(collision.gameObject);
            StartCoroutine(IntroFade(heartDisp));
            lives++;
            livesDisp.text = "Lives: " + lives;
        }

        // powerup: increase speed
        if(collision.gameObject.tag == "berry")
        {
            Destroy(collision.gameObject);
            StartCoroutine(IntroFade(speedDisp));
            StartCoroutine(speedTimer(5)); //speed is doubled for 5 seconds
        }
        
        // enemy attack
        if(collision.gameObject.tag == "milk")
        {
            Destroy(collision.gameObject);
            damage();
            squeak.Play();
        }

        // stationary enemy
        if(collision.gameObject.tag == "bird")
        {
            damage();
            squeak.Play();
        }
    }

    void damage()
    {
        health -= 1;
        healthDisp.text = "Health: " + health;

        if (health == 0)
        {
            lives--;
            print("current lives: " + lives);
            livesDisp.text = "Lives: " + lives;
            foreach (GameObject milk in GameObject.FindGameObjectsWithTag("milk"))
                Destroy(milk);
            if (lives >= 0)
            {
                health = 10;
                healthDisp.text = "Health: " + health;
            }
            else //lost game
            {
                livesDisp.text = "Lives: 0";
                Destroy(this.gameObject);
                CancelInvoke("SpawnItem");
                EnemySpawn.clear();
                display.SetActive(true);
                replayButton.SetActive(true);
            }
        }
    }

    void SpawnItem()
    {
        int rand = Random.Range(0, 4);
        GameObject powerup;
        switch(rand)
        {
            //speed boost powerup will spawn more
            case 0:
            case 1:
            case 2:
                powerup = Instantiate(berryPrefab, new Vector3(Random.Range(-5.4f, 4.5f), Random.Range(-4.4f, 5.5f), 0), Quaternion.identity);
                StartCoroutine(despawnTimer(powerup, 8));
                break;


            case 3:
                powerup = Instantiate(applePrefab, new Vector3(Random.Range(-5.4f, 4.5f), Random.Range(-4.4f, 5.5f), 0), Quaternion.identity);
                StartCoroutine(despawnTimer(powerup, 8));
                break;
        }
        Invoke("SpawnItem", Random.Range(5f, 10f));
    }

    // feature point: coroutines
    IEnumerator speedTimer(float time)
    {
        speed = 4f;
        yield return new WaitForSeconds(2f);
        speed = 2f;
    }

    IEnumerator despawnTimer(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(item);
    }

    public IEnumerator IntroFade(TextMeshProUGUI text)
    {
        yield return StartCoroutine(FadeTextIn(1f, text));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeTextOut(1f, text));
    }

    IEnumerator FadeTextIn(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            // feature point: time.deltatime
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeTextOut(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    GameObject cowPrefab;
    [SerializeField]
    GameObject birdPrefab;
    [SerializeField]
    GameObject shadowPrefab;
    TextMeshProUGUI waveDisp;
    TextMeshProUGUI timerDisp;

    public static int score;
    int wave = 1;
    int highscore = 0;
    TextMeshProUGUI scoreDisp;
    TextMeshProUGUI highscoreDisp;

    //feature point: lists
    List<GameObject> enemies = new List<GameObject>();

    [SerializeField]
    private float timer;
    int numEnemies;
    

    public static bool endGame = false;

    void Start()
    {
        score = 0;
        highscore = PlayerPrefs.GetInt("HighScore", 0);
        scoreDisp = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        highscoreDisp = GameObject.Find("Highscore").GetComponent<TextMeshProUGUI>();
        highscoreDisp.text = "Highscore: " + highscore;

        waveDisp = GameObject.Find("Waves").GetComponent<TextMeshProUGUI>();
        timerDisp = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        //10 seconds starting time & 2 enemies
        timer = 10f;
        numEnemies = 2;
        StartCoroutine(SpawnEnemies());
    }

    public void Update()
    {
        scoreDisp.text = "Score: " + score;
    }

    public void FixedUpdate()
    {
        if (!endGame)
        {
            timerDisp.text = "Timer: " + string.Format("{0:00.00}", timer);
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
                timer = Mathf.Clamp(timer, 0f, Mathf.Infinity);
                if (timer == 0)
                {
                    wave++;
                    waveDisp.text = "Wave: " + wave;
                    timer = 10f + wave * 2; //additional time is added for each wave
                    numEnemies += 2; //number of enemies increases by 2 each wave
                    StartCoroutine(SpawnEnemies());
                }
            }
        }
        else
            clearAll();
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numEnemies; i++)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-5.4f, 4.5f), Random.Range(-4.4f, 5.5f), 0);
            GameObject marker, enemy;

            int rand = Random.Range(0, 3);
            switch (rand)
            {
                // stationary bird enemy more likely to spawn (so as not to overwhelm the player)
                case 0:
                case 1:
                    marker = Instantiate(shadowPrefab, spawnPoint, Quaternion.identity);
                    yield return StartCoroutine(FadeIn(1f, marker));
                    yield return new WaitForSeconds(1);
                    Destroy(marker);
                    enemy = Instantiate(birdPrefab, spawnPoint, Quaternion.identity);
                    enemies.Add(enemy);
                    break;
                case 2:
                    marker = Instantiate(shadowPrefab, spawnPoint, Quaternion.identity);
                    yield return StartCoroutine(FadeIn(1f, marker));
                    yield return new WaitForSeconds(2);
                    Destroy(marker);
                    enemy = Instantiate(cowPrefab, spawnPoint, Quaternion.identity);
                    enemies.Add(enemy);
                    break;
            }
        }
    }

    IEnumerator FadeIn(float t, GameObject enemy)
    {
        SpriteRenderer enemyColor = enemy.GetComponent<SpriteRenderer>();
        enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, 0);
        while (enemyColor.color.a < .5f)
        {
            enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, enemyColor.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public static void clear()
    {
        endGame = true;
    }

    void clearAll()
    {
        CancelInvoke("SpawnEnemies");
        CancelInvoke("FadeIn");
        foreach (GameObject milk in GameObject.FindGameObjectsWithTag("milk"))
            Destroy(milk);
        foreach (GameObject enemy in enemies)
            Destroy(enemy);
    }

    private void OnDisable()
    {
        if (score > highscore)
        {
            //feature point: playerprefs
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    public void playGame()
    {
        // feature point: gizmos / debug lines
        Debug.Log("replay/play button pressed");
        endGame = false;

        //feature point: loadscene
        SceneManager.LoadScene("ShooterScene");
    }
}

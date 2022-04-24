using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//feature point: itch.io upload https://kimd.itch.io/rain-dropper

public class BoundaryManager : MonoBehaviour
{
    private List<GameObject> baskets; //feature point: arrays
    GameObject display;
    GameObject replayButton;
    int myScore; //feature point: scoring system
    int lives = 3;
    [SerializeField]
    AudioSource tumble;
    [SerializeField]
    AudioSource bgm;


    void Start()
    {
        bgm.Play(); //feature point: background music
        display = GameObject.Find("Display");
        replayButton = GameObject.Find("Replay");
        display.SetActive(false);
        replayButton.SetActive(false);

        baskets = new List<GameObject>(3);
        myScore = 0;

        foreach (GameObject _basket in GameObject.FindGameObjectsWithTag("basket"))
            baskets.Add(_basket);
    }

    void ConnectBasketToPlayer(GameObject x, GameObject a)
    {
        baskets.Add(x);
    }

    public void updateScore()
    {
        myScore += 1;
        GameObject.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: " + myScore;
    }

    //feature point: colliders
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "drop") //feature point: tags
        {
            lives--;
            Destroy(baskets[lives]);
            clearObjects();
            tumble.Play(); //feature point: triggered sounds -- audio played on collision

            if (lives == 0) //end of game
            {
                bgm.Pause();
                GameObject.Find("Enemy").GetComponent<EnemyManager>().stopDrop();
                GameObject.Find("Enemy").SetActive(false);
                display.SetActive(true);
                replayButton.SetActive(true);
            }
        }
    }

    public void playGame()
    {
        SceneManager.LoadScene("DropperScene"); //feature point: loadscene
    }

    void clearObjects()
    {
        foreach (GameObject drop in GameObject.FindGameObjectsWithTag("drop"))
            Destroy(drop);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// feature point : itch.io upload: https://kimd.itch.io/cat-in-a-room

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Vector2 _bounds;
    Vector2 _pos;
    Vector2 _tempPos; 
    Vector2 _bedx;
    Vector2 _bedy;
    Vector2 _tablex;
    Vector2 _tabley;
    public Vector2 _catPos;
    float lastDist = 100f;
    int lightUsage = 6;

    // feature point : scoring system : keep track of how many moves were made and display it
    int score = 0;

    bool won = false;
    bool validMove = true;
    [SerializeField]
    AudioSource meow;
    [SerializeField]
    AudioSource purr;
    [SerializeField]
    AudioSource wall;

    GameObject replayButton, upButton, downButton, leftButton, rightButton, flashlightButton;

    public enum Click { idle, left, right, up, down };

    void Start()
    {
        // feature point: UI buttons : all movement buttons + flashlight and replay buttons - called by onClick

        replayButton = GameObject.Find("Replay");
        upButton = GameObject.Find("Up");
        downButton = GameObject.Find("Down");
        leftButton = GameObject.Find("Left");
        rightButton = GameObject.Find("Right");
        flashlightButton = GameObject.Find("Flashlight");
        replayButton = GameObject.Find("Replay");
        replayButton.SetActive(false);

        // 8x4 room (starting from 0,0)
        _bounds = new Vector2(7, 3);
        // bed is 3x2 in the upper left corner
        _bedx = new Vector2(0, 2);
        _bedy = new Vector2(2, 3);
        // table is 1x2 at (5,0)(5,1)
        _tablex = new Vector2(5, 5);
        _tabley = new Vector2(0, 1);
        _pos.x = Mathf.Clamp(0f, 0f, _bounds.x);
        _pos.y = Mathf.Clamp(0f, 0f, _bounds.y);
        // cat is at (5,2)
        _catPos = new Vector2(5, 2);
    }

    public void playGame()
    {
        // feature point: loadscene : loads into game scene from the start screen

        SceneManager.LoadScene("AdventureScene");
    }

    void randomCat()
    {
        //re-position the cat in a spot within the upper right side of the room
        _catPos = new Vector2(Random.Range(3,8), Random.Range(2,3));
    }

    public string move(int val)
    {
        string msg = "No change";
        if (_tempPos == _catPos)
        {
            score++;
            meow.Stop();
            msg = "You found your cat!";
            won = true;
            return msg;
        }

        // feature point: enforcing boundaries(?) : does not allow player to move if there is a wall/bed/table

        msg = checkCollide();
        if(validMove)
        {
            // feature point: Dynamic volume

            meow.volume = 1.0f - Mathf.Clamp(Vector2.Distance(_pos, _catPos) / 7f, 0f, .99f);
            meow.Play();
            score++;

            float dist = Vector2.Distance(_tempPos, _catPos);
            msg = "You move " + (Click)val + ". ";
            if (dist < lastDist)
                msg = "You get closer to the cat";
            else if (dist > lastDist)
                msg = "You get further away from the cat";
            lastDist = dist;
        }
        return msg;
    }

    public void updateScore()
    {
        // feature point: UI Text : displays the score / number of moves made by the player

        TextMeshProUGUI scoreDisp = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        scoreDisp.text = "Moves: " + score;
    }

    string checkCollide()
    {
        //checks collision with wall
        if ((_tempPos.x > _bounds.x || _tempPos.x < 0) ||
            (_tempPos.y > _bounds.y || _tempPos.y < 0))
        {
            meow.Stop();
            validMove = false;

            // feature point: Triggered sounds : plays bump sound when hitting a wall
            wall.Play();

            return "You hit a Wall! You did not move.";
        }
        //bump against bed
        if (_tempPos.x >= _bedx.x && _tempPos.x <= _bedx.y &&
            _tempPos.y >= _bedy.x && _tempPos.y <= _bedy.y)
        {
            meow.Stop();
            validMove = false;
            wall.Play();
            return "You hit the bed! You did not move.";
        }
        //bump against table
        if (_tempPos.x == _tablex.x && _tempPos.y >= _tabley.x && _tempPos.y <= _tabley.y)
        {
            meow.Stop();
            validMove = false;
            wall.Play();
            return "You hit the table! You did not move.";
        }
        //is a valid move if passes here
        validMove = true;
        return "";
    }

    public void onClick(int val)
    {
        TextMeshProUGUI msg = GameObject.Find("Display").GetComponent<TextMeshProUGUI>();

        if ((Click)val == Click.left)
        {
            _tempPos = _pos;
            _tempPos += Vector2.left;
        }
        else if ((Click)val == Click.right)
        {
            _tempPos = _pos;
            _tempPos += Vector2.right;
        }
        else if ((Click)val == Click.up)
        {
            _tempPos = _pos;
            _tempPos += Vector2.up;
        }
        else if ((Click)val == Click.down)
        {
            _tempPos = _pos;
            _tempPos += Vector2.down;
        }

        msg.text = move(val);
        if (validMove)
        {
            _pos = _tempPos;
            updateScore();
        }
        print("\nCurr position: " + _pos + "\nCat position: " + _catPos);

        hasWon();
    }

    public void flashlight()
    {
        TextMeshProUGUI msg = GameObject.Find("Display").GetComponent<TextMeshProUGUI>();
        if (lightUsage > 0)
        {
            lightUsage--;
            msg.text = "Your phone shines light! You are " + (int)Vector2.Distance(_pos, _catPos) + " meters away from the cat. Your phone appears to be on " + lightUsage + "% remaining battery.";
        }
        else
        {
            msg.text = "Oops your phone ran out of power! The light doesn't turn on..";
            GameObject.Find("Flashlight").GetComponent<Button>().interactable = false;
        }
    }

    void hasWon()
    {
        if(won)
        {
            purr.Play();
            upButton.SetActive(false);
            downButton.SetActive(false);
            leftButton.SetActive(false);
            rightButton.SetActive(false);
            flashlightButton.SetActive(false);
            replayButton.SetActive(true);
        }
    }

    public void replay()
    {
        purr.Stop();
        randomCat();
        GameObject.Find("Display").GetComponent<TextMeshProUGUI>().text = "";
        GameObject.Find("Score").GetComponent<TextMeshProUGUI>().text = "Moves: 0";
        upButton.SetActive(true);
        downButton.SetActive(true);
        leftButton.SetActive(true);
        rightButton.SetActive(true);
        flashlightButton.SetActive(true);
        replayButton.SetActive(false);
        score = 0;
        lightUsage = 6;
        won = false;
        lastDist = 100f;
        _pos.x = Mathf.Clamp(0f, 0f, _bounds.x);
        _pos.y = Mathf.Clamp(0f, 0f, _bounds.y);
    }
}

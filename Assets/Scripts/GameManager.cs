using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;    
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance=null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        //NextLevel();
    }

    private void NewGame()
    {
        lives = 3;
        coins = 0;

        LoadLevel(0,1);
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        /*if (world == 1 && stage == 10)
        {
            LoadLevel(world + 1, 1); 
        }*/

        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);  
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0)
        {
            LoadLevel(world,stage);
        }else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        // Invoke(nameof(NewGame), 3f);
        NewGame();
    }

    public void AddCoin()
    {
        coins++;

        if(coins == 100)
        {
            AddLife();
            coins = 0;
        }
    }

    public void AddLife()
    {
        lives++;
    }
}

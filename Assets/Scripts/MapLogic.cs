using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.TextCore.Text;

public class MapLogic : MonoBehaviour
{
    [SerializeField] GameObject mapRound1;
    [SerializeField] GameObject mapRound2;
    [SerializeField] GameObject mapRound3;
    [SerializeField] GameObject mapWinner;
    [SerializeField] GameObject mapGameOver;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] int maxHealth = 100;
    private bool playerOneWin1, playerOneWin2, playerTwoWin1, playerTwoWin2, pause, gameEnded = false;
    private HealthBar playerOneHealth;
    private HealthBar playerTwoHealth;

    void Start()
    {
        spawnPlayers();
        mapRound1.SetActive(true);
        Destroy(mapRound1, 2.7f);
        cleanVariables();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause)
            {
                resume();
            } else
            {
                mapPause();
            }
        }
    }

    public void mapPause()
    {
        Time.timeScale = 0;
        pause = true;
        playerOneHealth.gameObject.SetActive(false);
        playerTwoHealth.gameObject.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void resume()
    {
        pauseMenu.SetActive(false);
        playerOneHealth.gameObject.SetActive(true);
        playerTwoHealth.gameObject.SetActive(true);
        pause = false;
        Time.timeScale = 1;

    }

    public void mapRound2Start()
    {
        if (mapRound2 != null)
        {
            mapRound2.SetActive(true);
            Destroy(mapRound2, 2.7f);
        }
    }

    public void mapRound3Start()
    {
        if (mapRound3 != null)
        {
            mapRound3.SetActive(true);
            Destroy(mapRound3, 2.7f);
        }
    }

    public void mapEnd(string winner)
    {
        gameEnded = true;
        mapWinner.GetComponent<TextMeshProUGUI>().SetText(winner);
        mapGameOver.SetActive(true);
    }

    public void restartHealth()
    {
        playerOneHealth.setHealth(maxHealth);
        playerTwoHealth.setHealth(maxHealth);
    }

    public void exitMap()
    {
        resumeTimeScale();
        SceneManager.LoadScene("MainScene");
    }

    public void restartGame()
    {
        resumeTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void spawnPlayers()
    {
        spawnPlayer1();
        spawnPlayer2();
    }

    public void spawnPlayer1()
    {
        string p1Character = LogicScript.characterSelect[0].Remove(LogicScript.characterSelect[0].Length - 1, 1);
        Vector3 position = new Vector3(0,0,0);
        GameObject p1Obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Characters/" + p1Character + ".prefab", typeof(GameObject));
        GameObject playerInstance = p1Obj;
        p1Obj = null;

        switch (p1Character)
        {
            case "HeavyBandit":
                playerInstance.GetComponent<Bandit>().isPlayerOne = true;
                position = new Vector3(-1.825f, -0.938f, 0f);
                break;
            case "LightBandit":
                playerInstance.GetComponent<Bandit>().isPlayerOne = true;
                position = new Vector3(-1.825f, -0.938f, 0f);
                break;
            case "Knight":
                playerInstance.GetComponent<Knight>().isPlayerOne = true;
                position = new Vector3(-1.825f, -0.938f, 0f);
                break;
            case "MedievalKing":
                playerInstance.GetComponent<MedievalKing>().isPlayerOne = true;
                position = new Vector3(-1.825f, -0.53f, 0f);
                break;
            case "MartialHero":
                playerInstance.GetComponent<MartialHero>().isPlayerOne = true;
                position = new Vector3(-1.825f, -0.769417882f, 0f);
                break;
        }
        playerInstance.transform.localScale = new Vector2(-Mathf.Abs(playerInstance.transform.localScale.x), playerInstance.transform.localScale.y);
        Instantiate(playerInstance, position, Quaternion.identity);
    }

    public void spawnPlayer2()
    {
        string p2Character = LogicScript.characterSelect[1].Remove(LogicScript.characterSelect[1].Length - 1, 1);
        GameObject p2Obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Characters/" + LogicScript.characterSelect[1].Remove(LogicScript.characterSelect[1].Length - 1, 1) + ".prefab", typeof(GameObject));
        Vector3 position = new Vector3(0, 0, 0);
        GameObject playerInstance = p2Obj;
        p2Obj = null;

        switch (p2Character)
        {
            case "HeavyBandit":
                position = new Vector3(2.050f, -0.938f, 0f);
                break;
            case "LightBandit":
                position = new Vector3(2.050f, -0.938f, 0f);
                break;
            case "Knight":
                position = new Vector3(2.050f, -0.938f, 0f);
                break;
            case "MedievalKing":
                position = new Vector3(2.050f, -0.53f, 0f);
                break;
            case "MartialHero":
                position = new Vector3(2.04999995f, -0.769417882f, 0f);
                break;
        }
        Instantiate(playerInstance, position, Quaternion.identity);
    }

    void cleanVariables()
    {
        playerOneWin1 = false;
        playerOneWin2 = false;
        playerTwoWin1 = false;
        playerTwoWin2 = false;
        gameEnded = false;
        playerOneHealth = GameObject.Find("HealthBarP1").GetComponent<HealthBar>();
        playerTwoHealth = GameObject.Find("HealthBarP2").GetComponent<HealthBar>();
        playerOneHealth.setMaxHealth(maxHealth);
        playerTwoHealth.setMaxHealth(maxHealth);
    }

    void resumeTimeScale()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    // Getters / Setters
    public HealthBar PlayerOneHealth
    {
        get { return playerOneHealth; }
        set { playerOneHealth = value; }
    }

    public HealthBar PlayerTwoHealth
    {
        get { return playerTwoHealth; }
        set { playerTwoHealth = value; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public bool PlayerOneWin1
    {
        get { return playerOneWin1; }
        set { playerOneWin1 = value; }
    }

    public bool PlayerOneWin2
    {
        get { return playerOneWin2; }
        set { playerOneWin2 = value; }
    }

    public bool PlayerTwoWin1
    {
        get { return playerTwoWin1; }
        set { playerTwoWin1 = value; }
    }

    public bool PlayerTwoWin2
    {
        get { return playerTwoWin2; }
        set { playerTwoWin2 = value; }
    }

    public bool Pause
    {
        get { return pause; }
        set { pause = value; }
    }

    public bool GameEnded
    {
        get { return gameEnded; }
        set { gameEnded = value; }
    }

}

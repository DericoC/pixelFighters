using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Map1Logic : MonoBehaviour
{
    public GameObject mapRound1;
    public GameObject mapRound2;
    public GameObject mapRound3;
    public GameObject mapWinner;
    public GameObject mapGameOver;
    public int maxHealth = 100;
    private bool p1Win1, p1Win2, p2Win1, p2Win2, gameEnded = false;
    private HealthBar playerOneHealth;
    private HealthBar playerTwoHealth;

    void Start()
    {
        spawnPlayers();
        mapRound1.SetActive(true);
        Destroy(mapRound1, 2.7f);
        cleanVariables();
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
        SceneManager.LoadScene("MainScene");
    }

    public void restartGame()
    {
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
        GameObject p1Obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Characters/" + p1Character + ".prefab", typeof(GameObject));
        if (p1Character == "HeavyBandit" || p1Character == "LightBandit")
        {
            p1Obj.GetComponent<Bandit>().isPlayerOne = true;
        }
        else if (p1Character == "Knight")
        {
            p1Obj.GetComponent<Knight>().isPlayerOne = true;
        }
        p1Obj.transform.localScale = new Vector2(-Mathf.Abs(p1Obj.transform.localScale.x), p1Obj.transform.localScale.y);
        Instantiate(p1Obj, new Vector3(-1.825f, -0.938f, 0f), Quaternion.identity);
    }

    public void spawnPlayer2()
    {
        Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Characters/" + LogicScript.characterSelect[1].Remove(LogicScript.characterSelect[1].Length - 1, 1) + ".prefab", typeof(GameObject)), new Vector3(2.050f, -0.938f, 0f), Quaternion.identity);
    }

    void cleanVariables()
    {
        p1Win1 = false;
        p1Win2 = false;
        p2Win1 = false;
        p2Win2 = false;
        gameEnded = false;
        playerOneHealth = GameObject.Find("HealthBarP1").GetComponent<HealthBar>();
        playerTwoHealth = GameObject.Find("HealthBarP2").GetComponent<HealthBar>();
        playerOneHealth.setMaxHealth(maxHealth);
        playerTwoHealth.setMaxHealth(maxHealth);
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

    public bool P1Win1
    {
        get { return p1Win1; }
        set { p1Win1 = value; }
    }

    public bool P1Win2
    {
        get { return p1Win2; }
        set { p1Win2 = value; }
    }

    public bool P2Win1
    {
        get { return p2Win1; }
        set { p2Win1 = value; }
    }

    public bool P2Win2
    {
        get { return p2Win2; }
        set { p2Win2 = value; }
    }

    public bool GameEnded
    {
        get { return gameEnded; }
        set { gameEnded = value; }
    }

}

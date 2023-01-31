using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Map1Logic : MonoBehaviour
{
    public GameObject map1Round1;
    public GameObject map1Round2;
    public GameObject map1Round3;
    public GameObject map1Winner;
    public GameObject map1GameOver;
    public int maxHealth = 100;
    public static bool p1Win1, p1Win2, p2Win1, p2Win2 = false;
    private HealthBar playerOneHealth;
    private HealthBar playerTwoHealth;

    void Start()
    {
        spawnPlayers();
        map1Round1.SetActive(true);
        Destroy(map1Round1, 2.7f);
        playerOneHealth = GameObject.Find("HealthBarP1").GetComponent<HealthBar>();
        playerTwoHealth = GameObject.Find("HealthBarP2").GetComponent<HealthBar>();
        playerOneHealth.setMaxHealth(maxHealth);
        playerTwoHealth.setMaxHealth(maxHealth);
    }

    public void map1Round2Start()
    {
        map1Round2.SetActive(true);
        Destroy(map1Round2, 2.7f);
    }

    public void map1Round3Start()
    {
        map1Round3.SetActive(true);
        Destroy(map1Round3, 2.7f);
    }

    public void map1End(string winner)
    {
        map1Winner.GetComponent<TextMeshProUGUI>().SetText(winner);
        map1GameOver.SetActive(true);
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
        //Player 1
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
        p1Obj.GetComponent<SpriteRenderer>().flipX = true;
        Instantiate(p1Obj, new Vector3(-1.825f, -0.938f, 0f), Quaternion.identity);

        //Player 2
        Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Characters/" + LogicScript.characterSelect[1].Remove(LogicScript.characterSelect[1].Length - 1, 1) + ".prefab", typeof(GameObject)), new Vector3(2.050f, -0.938f, 0f), Quaternion.identity);
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
    
}

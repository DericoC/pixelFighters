using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;

public class LogicScript : MonoBehaviour
{
    [SerializeField] GameObject welcome;
    [SerializeField] GameObject chooseCharacter;
    [SerializeField] GameObject characters;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject controlsToggle;
    [SerializeField] GameObject chooseArena;
    public static List<string> characterSelect;

    void Start()
    {
        welcome.SetActive(true);
        chooseCharacter.SetActive(false);
        chooseArena.SetActive(false);
        characterSelect = new List<string>();
    }

    void Update()
    {
        if (characterSelect.Count == 2)
        {
            showArena();
        }
    }

    public void characterSelected(int child)
    {
        Toggle toggle = characters.transform.GetChild(child).gameObject.GetComponent<Toggle>();
        if (toggle.isOn)
        {
            characterSelect.Add(toggle.name);
        } else
        {
            if (characterSelect.Contains(toggle.name))
            {
                characterSelect.Remove(toggle.name);
            }
        }
    }

    public void showCharacter()
    {
        welcome.SetActive(false);
        chooseCharacter.SetActive(true);
    }

    public void showControlsMenu()
    {
        if (controlsToggle.GetComponent<Toggle>().isOn)
        {
            characters.SetActive(false);
            controlsMenu.SetActive(true);
        } else
        {
            controlsMenu.SetActive(false);
            characters.SetActive(true);
        }
    }

    public void showArena()
    {
        chooseCharacter.SetActive(false);
        chooseArena.SetActive(true);
    }

    public void backToCharacters()
    {
        chooseArena.SetActive(false);
        chooseCharacter.SetActive(true);
        chooseCharacter.transform.GetChild(0).GetChild(int.Parse(characterSelect[1][characterSelect[1].Length - 1].ToString())).gameObject.GetComponent<Toggle>().isOn = false;
        chooseCharacter.transform.GetChild(0).GetChild(int.Parse(characterSelect[0][characterSelect[0].Length - 1].ToString())).gameObject.GetComponent<Toggle>().isOn = false;
        characterSelect.Clear();
    }

    public void goToMap1()
    {
        SceneManager.LoadScene("Map1");
        
    }

    public void goToMap2()
    {
        SceneManager.LoadScene("Map2");
    }

    public void goToMap3()
    {
        SceneManager.LoadScene("Map3");
    }
}

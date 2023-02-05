using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensorBandit : MonoBehaviour
{
    [SerializeField] private bool isPlatform = false;
    private int colCount = 0;
    private float disableTimer;

    private void OnEnable()
    {
        colCount = 0;
    }

    public bool State()
    {
        if (disableTimer > 0)
            return false;
        return colCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            isPlatform = other.CompareTag("Platform");
        }
        colCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        colCount--;
    }

    void Update()
    {
        disableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        disableTimer = duration;
    }

    //Getters / Setters
    public bool IsPlatform
    {
        get { return isPlatform; }
        set { isPlatform = value; }
    }
}
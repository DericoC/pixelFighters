using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void setMaxHealth(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void setHealth(int value)
    {
        slider.value = value;
    }

    public int getHealth()
    {
        return (int) slider.value;
    }
}

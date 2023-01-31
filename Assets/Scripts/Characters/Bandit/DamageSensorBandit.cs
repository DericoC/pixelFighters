using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSensorBandit : MonoBehaviour
{
    private Bandit bandit;

    private void Start()
    {
        bandit = GetComponentInParent<Bandit>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "GroundSensor")
        {
            if (collision.CompareTag("Player") && bandit.isPlayerOne)
            {
                damage(collision, true);
            }
            else if (collision.CompareTag("Player") && !bandit.isPlayerOne)
            {
                damage(collision, false);
            }
        }
    }

    void damage(Collider2D collision, bool isPlayerOne)
    {
        if (collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            bandit.damageTaken(10, isPlayerOne);
        }
        else if (collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            bandit.damageTaken(2, isPlayerOne);
        }
        else if (collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Combat Idle"))
        {
            bandit.damageTaken(4, isPlayerOne);
        }
    }
}


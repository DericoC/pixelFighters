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

    void damage(Collider2D other, bool isPlayerOne)
    {
        if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 4.71)
        {
            bandit.damageTaken(10, isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && (Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 2.50 && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 4.70))
        {
            bandit.damageTaken(5, isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 2.49)
        {
            bandit.damageTaken(3, isPlayerOne);
        }
    }
}


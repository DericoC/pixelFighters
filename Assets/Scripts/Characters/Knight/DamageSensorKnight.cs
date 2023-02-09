using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DamageSensorKnight : MonoBehaviour
{
    private Knight knight;

    private void Start()
    {
        knight = GetComponentInParent<Knight>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "GroundSensor")
        {
            if (collision.CompareTag("Player") && knight.isPlayerOne)
            {
                damage(collision, true);
            }
            else if (collision.CompareTag("Player") && !knight.isPlayerOne)
            {
                damage(collision, false);
            }
        }
    }

    void damage(Collider2D other, bool isPlayerOne)
    {
        if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 4.71)
        {
            knight.damageTaken(10, isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && (Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 2.50 && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 4.70))
        {
            knight.damageTaken(5, isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 2.49)
        {
            knight.damageTaken(3, isPlayerOne);
        }
    }
}

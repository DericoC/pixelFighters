using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSensorMedievalKing : MonoBehaviour
{
    private MedievalKing MedievalKing;

    private void Start()
    {
        MedievalKing = GetComponentInParent<MedievalKing>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "GroundSensor")
        {
            if (collision.CompareTag("Player") && MedievalKing.isPlayerOne)
            {
                damage(collision, true);
            }
            else if (collision.CompareTag("Player") && !MedievalKing.isPlayerOne)
            {
                damage(collision, false);
            }
        }
    }

    void damage(Collider2D other, bool isPlayerOne)
    {
        if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 4.71)
        {
            MedievalKing.damageTaken(10, isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && (Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 2.50 && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 4.70))
        {
            MedievalKing.damageTaken(5, isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 2.49)
        {
            MedievalKing.damageTaken(3, isPlayerOne);
        }
    }
}


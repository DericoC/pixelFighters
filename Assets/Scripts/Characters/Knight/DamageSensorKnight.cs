using System.Collections;
using System.Collections.Generic;
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
        if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            knight.damageTaken(10, isPlayerOne);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSensorScript : MonoBehaviour
{
    private dynamic character;

    private void Start()
    {
        getCharacterInParent(transform.parent.name.Replace("(Clone)", ""));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "GroundSensor")
        {
            if (collision.CompareTag("Player") && character.isPlayerOne)
            {
                damage(collision, true);
            }
            else if (collision.CompareTag("Player") && !character.isPlayerOne)
            {
                damage(collision, false);
            }
        }
    }

    void damage(Collider2D other, bool isPlayerOne)
    {
        float multiplier = 1;
        if (other.CompareTag("Player"))
        {
            multiplier = getCharacterDamageMultiplier(other, other.name.Replace("(Clone)", ""));
        }
        
        if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 4.71)
        {
            character.damageTaken(Mathf.RoundToInt(10 * multiplier), isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && (Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= 2.50 && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 4.70))
        {
            character.damageTaken(Mathf.RoundToInt(5 * multiplier), isPlayerOne);
        }
        else if (other.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) <= 2.49)
        {
            character.damageTaken(Mathf.RoundToInt(3 * multiplier), isPlayerOne);
        }
    }

    float getCharacterDamageMultiplier(Collider2D col, string c)
    {
        switch (c)
        {
            case "HeavyBandit":
                return col.gameObject.GetComponent<Bandit>().damageMultiplier;
            case "LightBandit":
                return col.gameObject.GetComponent<Bandit>().damageMultiplier;
            case "Knight":
                return col.gameObject.GetComponent<Knight>().damageMultiplier;
            case "MedievalKing":
                return col.gameObject.GetComponent<MedievalKing>().damageMultiplier;
            case "MartialHero":
                return col.gameObject.GetComponent<MartialHero>().damageMultiplier;
            default:
                return 1;
        }
    }

    public void getCharacterInParent(string c)
    {
        switch (c)
        {
            case "HeavyBandit":
                character = GetComponentInParent<Bandit>();
                break;
            case "LightBandit":
                character = GetComponentInParent<Bandit>();
                break;
            case "Knight":
                character = GetComponentInParent<Knight>();
                break;
            case "MedievalKing":
                character = GetComponentInParent<MedievalKing>();
                break;
            case "MartialHero":
                character = GetComponentInParent<MartialHero>();
                break;
        }
    }
}

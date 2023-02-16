using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartialHero : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 1.5f;
    [SerializeField] float jumpForce = 3.5f;

    [Header("Audio")]
    [SerializeField] AudioClip swordAttack;
    [SerializeField] AudioClip swordAttackEnemy;
    [SerializeField] AudioClip swordAttackTwo;
    [SerializeField] AudioClip swordAttackEnemyTwo;
    private AudioSource audioSource;
    [SerializeField] public bool isPlayerOne = false;
    private SpriteRenderer sprite;
    private Animator animator;
    private Rigidbody2D body2d;
    private GroundSensorMartialHero groundSensor;
    private BoxCollider2D boxCollider;
    private MapLogic logicScript;
    private PlatformLogic platformLogic;
    private bool grounded, combatIdle, isDead, damageAni, doubleJump = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<MapLogic>();
        platformLogic = GameObject.Find("PlatformCollider").GetComponent<PlatformLogic>();
        groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensorMartialHero>();
        logicScript.PlayerOneHealth.setHealth(logicScript.MaxHealth);
        logicScript.PlayerTwoHealth.setHealth(logicScript.MaxHealth);
    }
    void Update()
    {
        if (!logicScript.GameEnded)
        {
            //Check if character just landed on the ground
            if (!grounded && groundSensor.State())
            {
                grounded = true;
                doubleJump = false;
                animator.SetBool("Grounded", grounded);
            }
            //Check if character just started falling
            if (grounded && !groundSensor.State())
            {
                grounded = false;
                animator.SetBool("Grounded", grounded);
            }
            if (Input.GetKey("d") && isPlayerOne && !isDead)
            {
                moveRight();
            }
            else if (Input.GetKey("a") && isPlayerOne && !isDead)
            {
                moveLeft();
            }
            else if (Input.GetKey(KeyCode.RightArrow) && !isPlayerOne && !isDead)
            {
                moveRight();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !isPlayerOne && !isDead)
            {
                moveLeft();
            }
            if ((Input.GetKey("s") && isPlayerOne && !isDead && groundSensor.IsPlatform) || (Input.GetKey(KeyCode.DownArrow) && !isPlayerOne && !isDead && groundSensor.IsPlatform))
            {
                StartCoroutine(platformLogic.disableCollision(boxCollider));
            }
            //Set AirSpeed in animator
            animator.SetFloat("AirSpeed", body2d.velocity.y);
            // -- Handle Animations --
            //Death
            if (isDead && Input.GetKeyDown("r") && isPlayerOne)
            {
                recover();
            }
            else if (isDead && Input.GetKeyDown(KeyCode.Return) && !isPlayerOne)
            {
                recover();
            }
            else if (logicScript.PlayerOneHealth.getHealth() <= 0 && isPlayerOne && !isDead)
            {
                death();
            }
            else if (logicScript.PlayerTwoHealth.getHealth() <= 0 && !isPlayerOne && !isDead)
            {
                death();
            }
            //Hurt
            else if (damageAni)
            {
                animator.SetTrigger("Hurt");
                damageAni = false;
            }
            //Attack
            else if (Input.GetKeyDown(KeyCode.Space) && isPlayerOne)
            {
                animator.SetTrigger("Attack");
                Invoke("attack", 0.19f);
            }
            else if (Input.GetKeyDown(KeyCode.RightShift) && !isPlayerOne)
            {
                animator.SetTrigger("Attack");
                Invoke("attack", 0.19f);
            }
            //Change between idle and combat idle
            else if (Input.GetKeyDown("f") && isPlayerOne)
            {
                combatIdle = !combatIdle;
            }
            else if (Input.GetKeyDown(KeyCode.RightControl) && !isPlayerOne)
            {
                combatIdle = !combatIdle;
            }
            //Jump
            else if (Input.GetKeyDown("w") && isPlayerOne && !isDead)
            {
                if (grounded || doubleJump)
                {
                    jump();
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && !isPlayerOne && !isDead)
            {
                if (grounded || doubleJump)
                {
                    jump();
                }
            }
            //Run
            else if (Mathf.Abs(body2d.velocity.x) > 0.6)
            {
                animator.SetInteger("AnimState", 2);
            }
            //Combat Idle
            else if (combatIdle)
            {
                animator.SetInteger("AnimState", 1);
            }
            //Idle
            else
            {
                animator.SetInteger("AnimState", 0);
            }
            //Check if player fell of the map
            fellOfMap();
        }
    }
    void jump()
    {
        animator.SetTrigger("Jump");
        grounded = false;
        animator.SetBool("Grounded", grounded);
        body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
        groundSensor.Disable(0.2f);
        doubleJump = !doubleJump;
    }
    void moveRight()
    {
        transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        body2d.velocity = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? new Vector2(speed / 4.5f, body2d.velocity.y) : new Vector2(speed, body2d.velocity.y);
    }
    void moveLeft()
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        body2d.velocity = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? new Vector2(-(speed / 4.5f), body2d.velocity.y) : new Vector2(-speed, body2d.velocity.y);
    }
    void recover()
    {
        isDead = false;
        animator.SetTrigger("Recover");
        logicScript.PlayerOneHealth.setHealth(logicScript.MaxHealth);
        logicScript.PlayerTwoHealth.setHealth(logicScript.MaxHealth);
        logicScript.restartHealth();
        damageAni = false;
        if (isPlayerOne)
        {
            logicScript.PlayerTwoWin1 = true;
        }
        else if (!isPlayerOne)
        {
            logicScript.PlayerOneWin1 = true;
        }
        if (logicScript.PlayerOneWin1 && logicScript.PlayerTwoWin1)
        {
            logicScript.mapRound3Start();
        }
        else
        {
            logicScript.mapRound2Start();
        }
    }
    public void damageTaken(int amount, bool isPlayerOne)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(isPlayerOne ? swordAttackEnemy : swordAttackEnemyTwo, 1.0f);

        if (isPlayerOne)
        {
            int damageTaken = logicScript.PlayerOneHealth.getHealth() - amount;
            logicScript.PlayerOneHealth.setHealth(damageTaken);
        }
        else
        {
            int damageTaken = logicScript.PlayerTwoHealth.getHealth() - amount;
            logicScript.PlayerTwoHealth.setHealth(damageTaken);
        }
        damageAni = true;
    }
    void death()
    {
        animator.SetTrigger("Death");
        isDead = true;
        if (isPlayerOne) //Player2 Wins
        {
            if (logicScript.PlayerTwoWin1)
            {
                logicScript.PlayerTwoWin2 = true;
            }
        }
        else if (!isPlayerOne) //Player1 Wins
        {
            if (logicScript.PlayerOneWin1)
            {
                logicScript.PlayerOneWin2 = true;
            }
        }
        if (logicScript.PlayerOneWin1 && logicScript.PlayerOneWin2)
        {
            logicScript.mapEnd("PLAYER 1 WINS");
        }
        else if (logicScript.PlayerTwoWin1 && logicScript.PlayerTwoWin2)
        {
            logicScript.mapEnd("PLAYER 2 WINS");
        }
    }
    void fellOfMap()
    {
        if (!((logicScript.PlayerOneWin1 && logicScript.PlayerOneWin2) || (logicScript.PlayerTwoWin1 && logicScript.PlayerTwoWin2)))
        {
            if (body2d.position.y < -4.5f)
            {
                death();
                recover();
                Destroy(this);
                if (isPlayerOne)
                    logicScript.spawnPlayer1();
                else
                    logicScript.spawnPlayer2();
            }
        }
    }

    void attack()
    {
        audioSource.PlayOneShot(isPlayerOne ? swordAttack : swordAttackTwo, 1.0f);
    }
}
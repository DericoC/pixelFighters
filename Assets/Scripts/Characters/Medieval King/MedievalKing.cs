using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class MedievalKing : MonoBehaviour
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
    private GroundSensorMedievalKing groundSensor;
    private BoxCollider2D boxCollider;
    private MapLogic logicScript;
    private PlatformLogic platformLogic;
    private bool grounded, combatIdle, isDead, damageAni, doubleJump = false;

    #region Movement
    void jump()
    {
        if (grounded || doubleJump)
        {
            animator.SetTrigger("Jump");
            grounded = false;
            animator.SetBool("Grounded", grounded);
            body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
            groundSensor.Disable(0.2f);
            doubleJump = !doubleJump;
        }
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

    #endregion Movement

    #region Mechanics
    //Handles the Recover Mechanic
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
    //Handles Receiving player damage
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
    //Handles Death Related Events
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
    //Checks if the player fell out of the bounds of the map
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
    //Plays Attack Sound
    void attack()
    {
        audioSource.PlayOneShot(isPlayerOne ? swordAttack : swordAttackTwo, 1.0f);
    }
    //Executes the Attack Sequence
    public async void AttackWithDelay(float delaySeconds)
    {
        animator.SetTrigger("Attack");
        // Delay execution of the attack method by the specified amount of time
        await Task.Delay((int)(delaySeconds * 1000));
        // Call the original attack method
        attack();
    }
    //Changes Player Animation State
    public void PlayerAnimation(int State)
    {
        animator.SetInteger("AnimState", State);
    }
    //Checks the current ground state
    public void TerrainCheck()
    {
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
    }

    #endregion Mechanics

    public void PlayerLogic()
    {
        if (!isDead)
        {
            //Checks the current player grounded status
            TerrainCheck();
            //Move Right Player One
            if (Input.GetKey("d") && isPlayerOne)
            {
                moveRight();
            }
            //Move Left Player One
            else if (Input.GetKey("a") && isPlayerOne)
            {
                moveLeft();
            }
            //Move Right Player Two
            else if (Input.GetKey(KeyCode.RightArrow) && !isPlayerOne)
            {
                moveRight();
            }
            //Move Left Player Two
            else if (Input.GetKey(KeyCode.LeftArrow) && !isPlayerOne)
            {
                moveLeft();
            }
            //Drop from Platform Player 1 & 2
            if ((Input.GetKey("s") && isPlayerOne && groundSensor.IsPlatform) || (Input.GetKey(KeyCode.DownArrow) && !isPlayerOne && groundSensor.IsPlatform))
            {
                StartCoroutine(platformLogic.disableCollision(boxCollider));
            }
            //Attack Player 1
            if (Input.GetKeyDown(KeyCode.Space) && isPlayerOne)
            {
                AttackWithDelay(0.19f);
            }
            //Attack Player 2
            else if (Input.GetKeyDown(KeyCode.RightShift) && !isPlayerOne)
            {
                AttackWithDelay(0.19f);
            }
            //Change between idle and combat idle Player One
            else if (Input.GetKeyDown("f") && isPlayerOne)
            {
                combatIdle = !combatIdle;
            }
            //Change between idle and combat idle Player Two
            else if (Input.GetKeyDown(KeyCode.RightControl) && !isPlayerOne)
            {
                combatIdle = !combatIdle;
            }
            //Jump Player One
            else if (Input.GetKeyDown("w") && isPlayerOne)
            {
                jump();
            }
            //Jump Player Two
            else if (Input.GetKeyDown(KeyCode.UpArrow) && !isPlayerOne)
            {
                jump();
            }
            //Set AirSpeed in animator
            animator.SetFloat("AirSpeed", body2d.velocity.y);
            // -- Handle Animations --
            //Hurt Animation
            if (damageAni)
            {
                animator.SetTrigger("Hurt");
                damageAni = false;
            }
            //Run Animation
            else if (Mathf.Abs(body2d.velocity.x) > 0.6)
            {
                PlayerAnimation(2);
            }
            //Combat Idle Animation
            else if (combatIdle)
            {
                PlayerAnimation(1);
            }
            //Idle Animation
            else
            {
                PlayerAnimation(0);
            }
            //Player One Died
            if (logicScript.PlayerOneHealth.getHealth() <= 0 && isPlayerOne)
            {
                death();
            }
            //Player Two Died
            else if (logicScript.PlayerTwoHealth.getHealth() <= 0 && !isPlayerOne)
            {
                death();
            }
        }
        //Recover Player One
        if (isDead && Input.GetKeyDown("r") && isPlayerOne)
        {
            recover();
        }
        //Recover Player Two
        else if (isDead && Input.GetKeyDown(KeyCode.Return) && !isPlayerOne)
        {
            recover();
        }
    }
    
    void Start()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<MapLogic>();
        platformLogic = GameObject.Find("PlatformCollider").GetComponent<PlatformLogic>();
        groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensorMedievalKing>();
        logicScript.PlayerOneHealth.setHealth(logicScript.MaxHealth);
        logicScript.PlayerTwoHealth.setHealth(logicScript.MaxHealth);
    }

    void Update()
    {
        if (!logicScript.GameEnded)
        {
            PlayerLogic();
            fellOfMap();
        }
    }
}
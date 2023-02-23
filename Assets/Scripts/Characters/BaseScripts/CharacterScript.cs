using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class CharacterScript : MonoBehaviour
{
    private float _speed = 1.5f;
    private float _jumpForce = 3.5f;
    private AudioClip _swordAttack;
    private AudioClip _swordAttackEnemy;
    private AudioClip _swordAttackTwo;
    private AudioClip _swordAttackEnemyTwo;
    private bool _isPlayerOne = false;
    private AudioSource audioSource;
    private string _character;
    private Animator animator;
    private Rigidbody2D body2d;
    private GroundSensorScript groundSensor;
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
            body2d.velocity = new Vector2(body2d.velocity.x, _jumpForce);
            groundSensor.Disable(0.2f);
            doubleJump = !doubleJump;
        }
    }

    void moveRight()
    {
        transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        body2d.velocity = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? new Vector2(_speed / 4.5f, body2d.velocity.y) : new Vector2(_speed, body2d.velocity.y);
    }

    void moveLeft()
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        body2d.velocity = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? new Vector2(-(_speed / 4.5f), body2d.velocity.y) : new Vector2(-_speed, body2d.velocity.y);
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
        if (_isPlayerOne)
        {
            logicScript.PlayerTwoWin1 = true;
        }
        else if (!_isPlayerOne)
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
        audioSource.PlayOneShot(isPlayerOne ? _swordAttackEnemy : _swordAttackEnemyTwo, 1.0f);

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
        if (_isPlayerOne) //Player2 Wins
        {
            if (logicScript.PlayerTwoWin1)
            {
                logicScript.PlayerTwoWin2 = true;
            }
        }
        else if (!_isPlayerOne) //Player1 Wins
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
                if (_isPlayerOne)
                    logicScript.spawnPlayer1();
                else
                    logicScript.spawnPlayer2();
            }
        }
    }

    //Executes the Attack Sequence
    public async void AttackWithDelay(float delaySeconds)
    {
        animator.SetTrigger("Attack");
        await Task.Delay((int)(delaySeconds * 1000));
        //attack();
        audioSource.PlayOneShot(_isPlayerOne ? _swordAttack : _swordAttackTwo, 1.0f);
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
            TerrainCheck();
            //Move
            if ((Input.GetKey("d") && _isPlayerOne) || (Input.GetKey(KeyCode.RightArrow) && !_isPlayerOne))
            {
                moveRight();
            }
            else if ((Input.GetKey("a") && _isPlayerOne) || (Input.GetKey(KeyCode.LeftArrow) && !_isPlayerOne))
            {
                moveLeft();
            }

            //Drop from platform
            if ((Input.GetKey("s") && _isPlayerOne && groundSensor.IsPlatform) || (Input.GetKey(KeyCode.DownArrow) && !_isPlayerOne && groundSensor.IsPlatform))
            {
                StartCoroutine(platformLogic.disableCollision(boxCollider));
            }

            //Attack Player
            if ((Input.GetKeyDown(KeyCode.Space) && _isPlayerOne) || (Input.GetKeyDown(KeyCode.RightShift) && !_isPlayerOne))
            {
                AttackWithDelay(0.19f);
            }
            //Change between idle and combat idle
            else if ((Input.GetKeyDown("f") && _isPlayerOne && ("HeavyBandit".Equals(_character) || "LightBandit".Equals(_character))) || (Input.GetKeyDown(KeyCode.RightAlt) && !_isPlayerOne && ("HeavyBandit".Equals(_character) || "LightBandit".Equals(_character))))
            {
                combatIdle = !combatIdle;
            }
            //Jump Player One
            else if ((Input.GetKeyDown("w") && _isPlayerOne) || (Input.GetKeyDown(KeyCode.UpArrow) && !_isPlayerOne))
            {
                jump();
            }
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

            //Player Died
            if ((logicScript.PlayerOneHealth.getHealth() <= 0 && _isPlayerOne) || (logicScript.PlayerTwoHealth.getHealth() <= 0 && !_isPlayerOne))
            {
                death();
            }
        }

        //Recover Player One
        if ((isDead && Input.GetKeyDown("r") && _isPlayerOne) || (isDead && Input.GetKeyDown(KeyCode.Return) && !_isPlayerOne))
        {
            recover();
        }
    }

    public void Starter(float speed, float jumpForce, AudioClip swordAttack, AudioClip swordAtackEnemy, AudioClip swordAttackTwo, AudioClip swordAttackEnemyTwo, bool isPlayerOne, string characterName)
    {
        _speed = speed;
        _jumpForce = jumpForce;
        _swordAttack = swordAttack;
        _swordAttackEnemy = swordAtackEnemy;
        _swordAttackTwo = swordAttackTwo;
        _swordAttackEnemyTwo = swordAttackEnemyTwo;
        _isPlayerOne = isPlayerOne;
        _character = characterName;
        groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensorScript>();
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<MapLogic>();
        platformLogic = GameObject.Find("PlatformCollider").GetComponent<PlatformLogic>();
        logicScript.PlayerOneHealth.setHealth(logicScript.MaxHealth);
        logicScript.PlayerTwoHealth.setHealth(logicScript.MaxHealth);
    }

    public void Updater()
    {
        if (!logicScript.GameEnded)
        {
            if (!logicScript.Pause)
            {
                PlayerLogic();
                fellOfMap();
            }
        }
    }
}

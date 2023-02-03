using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Bandit : MonoBehaviour
{
    [SerializeField] float m_speed = 1.5f;
    [SerializeField] float m_jumpForce = 5.0f;
    [SerializeField] public bool isPlayerOne = false;
    private static int p1Health, p2Health;
    private SpriteRenderer m_sprite;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private GroundSensorBandit m_groundSensor;
    private BoxCollider2D m_boxCollider;
    private Map1Logic logicScript;
    private PlatformLogic platformLogic;
    private bool m_grounded, m_combatIdle, m_isDead, m_damageAni, doubleJump = false;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_sprite = GetComponent<SpriteRenderer>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<Map1Logic>();
        platformLogic = GameObject.Find("PlatformCollider").GetComponent<PlatformLogic>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensorBandit>();
        p1Health = logicScript.MaxHealth;
        p2Health = logicScript.MaxHealth;
    }

    void Update()
    {
        if (!logicScript.GameEnded)
        {
            //Check if character just landed on the ground
            if (!m_grounded && m_groundSensor.State())
            {
                m_grounded = true;
                doubleJump = false;
                m_animator.SetBool("Grounded", m_grounded);
            }

            //Check if character just started falling
            if (m_grounded && !m_groundSensor.State())
            {
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
            }

            if (Input.GetKey("d") && isPlayerOne && !m_isDead)
            {
                moveRight();
            }
            else if (Input.GetKey("a") && isPlayerOne && !m_isDead)
            {
                moveLeft();
            }
            else if (Input.GetKey(KeyCode.RightArrow) && !isPlayerOne && !m_isDead)
            {
                moveRight();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !isPlayerOne && !m_isDead)
            {
                moveLeft();
            }

            if ((Input.GetKey("s") && isPlayerOne && !m_isDead && m_groundSensor.IsPlatform) || (Input.GetKey(KeyCode.DownArrow) && !isPlayerOne && !m_isDead && m_groundSensor.IsPlatform))
            {
                StartCoroutine(platformLogic.disableCollision(m_boxCollider));
            }

            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

            // -- Handle Animations --
            //Death
            if (m_isDead && Input.GetKeyDown("r") && isPlayerOne)
            {
                recover();
            }
            else if (m_isDead && Input.GetKeyDown(KeyCode.Return) && !isPlayerOne)
            {
                recover();
            }
            else if (p1Health <= 0 && isPlayerOne)
            {
                death();
            }
            else if (p2Health <= 0 && !isPlayerOne)
            {
                death();
            }
            //Hurt
            else if (m_damageAni)
            {
                m_animator.SetTrigger("Hurt");
                m_damageAni = false;
            }

            //Attack
            else if (Input.GetKeyDown(KeyCode.Space) && isPlayerOne)
            {
                m_animator.SetTrigger("Attack");
            }

            else if (Input.GetKeyDown(KeyCode.RightShift) && !isPlayerOne)
            {
                m_animator.SetTrigger("Attack");
            }

            //Change between idle and combat idle
            else if (Input.GetKeyDown("f") && isPlayerOne)
            {
                m_combatIdle = !m_combatIdle;
            }

            else if (Input.GetKeyDown(KeyCode.RightControl) && !isPlayerOne)
            {
                m_combatIdle = !m_combatIdle;
            }

            //Jump
            else if (Input.GetKeyDown("w") && isPlayerOne && !m_isDead)
            {
                if (m_grounded || doubleJump)
                {
                    jump();
                }

            }

            else if (Input.GetKeyDown(KeyCode.UpArrow) && !isPlayerOne && !m_isDead)
            {
                if (m_grounded || doubleJump)
                {
                    jump();
                }
            }

            //Run
            else if (Mathf.Abs(m_body2d.velocity.x) > 0.6)
            {
                m_animator.SetInteger("AnimState", 2);
            }

            //Combat Idle
            else if (m_combatIdle)
            {
                m_animator.SetInteger("AnimState", 1);
            }

            //Idle
            else
            {
                m_animator.SetInteger("AnimState", 0);
            }

            //Check if player fell of the map
            fellOfMap();
        }
    }

    void jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
        doubleJump = !doubleJump;
    }

    void moveRight()
    {
        transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        m_body2d.velocity = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? new Vector2(m_speed / 4.5f, m_body2d.velocity.y) : new Vector2(m_speed, m_body2d.velocity.y);
    }

    void moveLeft()
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        m_body2d.velocity = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ? new Vector2(-(m_speed / 4.5f), m_body2d.velocity.y) : new Vector2(-m_speed, m_body2d.velocity.y);
    }

    void recover()
    {
        m_animator.SetTrigger("Recover");
        m_isDead = false;
        p1Health = logicScript.MaxHealth;
        p2Health = logicScript.MaxHealth;
        logicScript.restartHealth();
        m_damageAni = false;

        if (isPlayerOne)
        {
            logicScript.P2Win1 = true;
        }
        else if (!isPlayerOne)
        {
            logicScript.P1Win1 = true;
        }

        if (logicScript.P1Win1 && logicScript.P2Win1)
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
        if (isPlayerOne)
        {
            p1Health -= amount;
            logicScript.PlayerOneHealth.setHealth(p1Health);
        }
        else
        {
            p2Health -= amount;
            logicScript.PlayerTwoHealth.setHealth(p2Health);
        }
        m_damageAni = true;
    }

    void death()
    {
        m_animator.SetTrigger("Death");
        m_isDead = true;

        if (isPlayerOne) //Player2 Wins
        {
            if (logicScript.P2Win1)
            {
                logicScript.P2Win2 = true;
            }
        }
        else if (!isPlayerOne) //Player1 Wins
        {
            if (logicScript.P1Win1)
            {
                logicScript.P1Win2 = true;
            }
        }
        if (logicScript.P1Win1 && logicScript.P1Win2)
        {
            logicScript.mapEnd("PLAYER 1 WINS");
        }
        else if (logicScript.P2Win1 && logicScript.P2Win2)
        {
            logicScript.mapEnd("PLAYER 2 WINS");
        }
    }

    void fellOfMap()
    {
        if (!((logicScript.P1Win1 && logicScript.P1Win2) || (logicScript.P2Win1 && logicScript.P2Win2)))
        {
            if (m_body2d.position.y < -4.5f)
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
}
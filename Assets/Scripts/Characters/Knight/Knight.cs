using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] float m_speed = 1.5f;
    [SerializeField] float m_jumpForce = 3.5f;
    [SerializeField] public bool isPlayerOne = false;
    private static int p1Health, p2Health;
    private SpriteRenderer m_sprite;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private GroundSensorKnight m_groundSensor;
    private Map1Logic logicScript;
    private bool m_grounded, m_combatIdle, m_isDead, m_damageAni = false;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_sprite = GetComponent<SpriteRenderer>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<Map1Logic>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensorKnight>();
        p1Health = logicScript.MaxHealth;
        p2Health = logicScript.MaxHealth;
    }

    void Update()
    {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
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

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

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
        else if (p1Health <= 0 && isPlayerOne && !m_isDead)
        {
            death();
        }
        else if (p2Health <= 0 && !isPlayerOne && !m_isDead)
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
            m_animator.SetTrigger("Attack1");
        }

        else if (Input.GetKeyDown(KeyCode.RightShift) && !isPlayerOne)
        {
            m_animator.SetTrigger("Attack1");
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
        else if (Input.GetKeyDown("w") && m_grounded && isPlayerOne && !m_isDead)
        {
            jump();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && m_grounded && !isPlayerOne && !m_isDead)
        {
            jump();
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
    }

    void jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }

    void moveRight()
    {
        m_sprite.flipX = true;
        m_body2d.velocity = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ? new Vector2(m_speed / 4.5f, m_body2d.velocity.y) : new Vector2(m_speed, m_body2d.velocity.y);
    }

    void moveLeft()
    {
        m_sprite.flipX = false;
        m_body2d.velocity = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ? new Vector2(-(m_speed / 4.5f), m_body2d.velocity.y) : new Vector2(-m_speed, m_body2d.velocity.y);
    }

    void recover()
    {
        m_animator.SetTrigger("Roll");
        m_isDead = false;
        p1Health = logicScript.MaxHealth;
        p2Health = logicScript.MaxHealth;
        logicScript.restartHealth();
        m_damageAni = false;

        if (isPlayerOne)
        {
            Map1Logic.p2Win1 = true;
        }
        else if (!isPlayerOne)
        {
            Map1Logic.p1Win1 = true;
        }

        if (Map1Logic.p1Win1 && Map1Logic.p2Win1)
        {
            logicScript.map1Round3Start();
        }
        else
        {
            logicScript.map1Round2Start();
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
            if (Map1Logic.p2Win1)
            {
                Map1Logic.p2Win2 = true;
            }
        }
        else if (!isPlayerOne) //Player1 Wins
        {
            if (Map1Logic.p1Win1)
            {
                Map1Logic.p1Win2 = true;
            }
        }
        if (Map1Logic.p1Win1 && Map1Logic.p1Win2)
        {
            logicScript.map1End("PLAYER 1 WINS");
        }
        else if (Map1Logic.p2Win1 && Map1Logic.p2Win2)
        {
            logicScript.map1End("PLAYER 2 WINS");
        }
    }
}

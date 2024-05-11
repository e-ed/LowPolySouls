using Cinemachine;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : Actor
{
    public float speed = 300f;
    public float rollSpeed = 10f;
    private Rigidbody rb;
    public Transform cam;
    CinemachineFreeLook vcam;
    //private bool isGrounded = true;
    public Animator animator;
    private bool hasTarget = false;
    GameObject enemyTarget;
    AudioSource audioSource;
    public List<GameObject> inventory = new List<GameObject>();
    private float maxStamina = 100f;
    private float stamina = 100f;
    public float rollStaminaCost = 20f;
    public float attackStaminaCost = 20f;
    public float staminaGain;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //weaponSocket = GameObject.FindGameObjectsWithTag("WeaponSocket")[0];
        MaxHP = 100;
        CurrentHP = 100;
        Level = 1;
        Strength = 10;
        Dexterity = 10;
        Intelligence = 10;
        vcam = FindObjectOfType<CinemachineFreeLook>();
        cam = GameObject.Find("Camera").transform;


    }

    private void Start()
    {
        //inventory.Add((GameObject) Resources.Load("Low-Poly Weapons/Prefabs/Sword"));
        //inventory.Add((GameObject)Resources.Load("Low-Poly Weapons/Prefabs/Dagger"));
        //GameObject.Find("Player").GetComponent<PlayerScript>().inventory.Add((GameObject)Resources.Load("PurePoly/Free_Swords/Prefabs/PP_Sword_0222"));
        //GameObject.Find("Player").GetComponent<PlayerScript>().inventory.Add((GameObject)Resources.Load("Club"));
    }



    void Update()
    {
        handleRollInput();
        handleTargetInput();
        handleCombatInput();
        handleStaminaGain();
    }

    private void FixedUpdate()
    {
        handleMovementInput();
    }

    void handleCombatInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && (stamina > attackStaminaCost))
        {
            Attack();
            stamina -= attackStaminaCost;

        }

        if (Input.GetMouseButton(1) && !isAttacking)
        {
            Block();
        }

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Block", false);
        }

    }

    public void Block()
    {
        animator.SetBool("Block", true);
    }

    public override void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    private void resetTarget()
    {
        hasTarget = false;
        vcam.LookAt = gameObject.transform.GetChild(0);
        vcam.m_Orbits[0].m_Radius = 9;
        vcam.m_Orbits[1].m_Radius = 8;
        vcam.m_Orbits[2].m_Radius = 7.5f;
    }

    void handleTargetInput()
    {
        if (hasTarget == true && enemyTarget == null)
        {
            resetTarget();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (hasTarget)
            {
                resetTarget();
                return;
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Debug.Log(enemy.name);
            }

            // targetting the first enemy found
            if (enemies.Length != 0)
            {
                Debug.Log(enemies[0]);
                vcam.LookAt = enemies[0].transform;
                vcam.m_Orbits[0].m_Radius = 12;
                vcam.m_Orbits[1].m_Radius = 11;
                vcam.m_Orbits[2].m_Radius = 10.5f;
                enemyTarget = enemies[0];
                hasTarget = true;
            }
            else
            {
                hasTarget = false;
                vcam.LookAt = gameObject.transform.GetChild(0);
            }
            Debug.Log(enemyTarget);

        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            string randomEnemy = Random.Range(1, 9).ToString();
            Instantiate(Resources.Load<Actor>("Prefabs/Enemy" + randomEnemy), (gameObject.transform.position + new Vector3(1, 2, 0)), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            rb.AddForce(new Vector3(0, 25, 0), ForceMode.Impulse);
        }

    }

    void handleMovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = verticalInput * camForward;
        Vector3 rightRelative = horizontalInput * camRight;
        Vector3 movementDirection = forwardRelative + rightRelative;

        Vector3 inputDir = new Vector3(movementDirection.x, 0f, movementDirection.z).normalized;
        Vector3 movement = inputDir * speed * Time.deltaTime;

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        animator.SetFloat("Horizontal", Mathf.Abs(movement.x));
        animator.SetFloat("Vertical", Mathf.Abs(movement.z));



        if (inputDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(inputDir);
            //Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        }

    }

    void handleRollInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isRolling || stamina < rollStaminaCost) return;

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            camForward.y = 0;
            camRight.y = 0;

            Vector3 forwardRelative = verticalInput * camForward;
            Vector3 rightRelative = horizontalInput * camRight;
            Vector3 movementDirection = forwardRelative + rightRelative;

            Vector3 inputDir = new Vector3(movementDirection.x, 0f, movementDirection.z).normalized;


            // Add a boost in the direction of the input while rolling
            Vector3 boostForce = inputDir * rollSpeed;
            rb.AddForce(boostForce, ForceMode.Impulse);

            animator.SetTrigger("isRolling");
            isRolling = true;
            //isGrounded = false;

            stamina -= rollStaminaCost;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //isGrounded = true;
        animator.SetFloat("JumpOrFall", 0);

    }

    private void Step()
    {
        audioSource.Play();
    }

    void handleStaminaGain()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaGain * Time.deltaTime;
            staminaGain *= 1.01f;
        } else
        {
            staminaGain = 0.5f;
        }
    }

    public float Stamina
    {
        get { return stamina; }
    }
}


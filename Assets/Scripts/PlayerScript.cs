using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    private int souls;
    private TextMeshPro soulsText;
    private DataHandler dataHandler;
    public int jumpForce;
    public delegate void DataLoadedHandler();
    public static event DataLoadedHandler OnDataLoaded;
    private bool hasDied = false;
    private float rollCooldown = 0.6f; // Half a second cooldown
    private float lastRollTime;

    public int Souls
    {
        get { return souls; }
        set { souls = value; }
    }

    public bool HasDied
    {
        get { return hasDied; }
        set { hasDied = value; }
    }

    void Awake()
    {
        
    }



    private void LoadPlayerData()
    {
        if (DataHandler.Instance != null)
        {
            PlayerData data = DataHandler.Instance.LoadData();
            if (data != null)
            {
                // Apply loaded data
                Level = data.level;
                Souls = data.souls;
                Strength = data.strength;
                Dexterity = data.dexterity;
                Intelligence = data.intelligence;
                Vector3 newPosition = new Vector3(data.positionX, data.positionY, data.positionZ);
                rb.MovePosition(newPosition);

                EventManager.TriggerEvent("UpdateSoulsPanel", souls);
            }
            else
            {
                Debug.LogError("Loaded data is null");
            }
        }
        else
        {
            Debug.LogError("DataHandler instance is null");
        }
    }



    private void updateSoulsPanel()
    {
        if (soulsText != null)
        {
            // Update the text to reflect the current soul count
            soulsText.text = souls.ToString();
        }
    }

    void GainSouls(int soulAmount)
    {
        // Add the souls to the player's total souls
        souls += soulAmount;
        EventManager.TriggerEvent("UpdateSoulsPanel", souls);
    }

    void OnEnemyDied(object soulsGained)
    {
        // Give souls to the player when an enemy dies
        GainSouls((int)soulsGained); // Adjust soul value based on game logic
    }

    void OnEnable()
    {
        EventManager.StartListening("EnemyDied", OnEnemyDied);
        EventManager.StartListening("PlayerDied", OnPlayerDied);
    }

    void OnPlayerDied(object obj)
    {
        if (!hasDied)
        {
            hasDied = true;
            animator.SetTrigger("Dying");
            Invoke("Respawn", 5f);  // Call Respawn after 5 seconds
        }
    }

    private void Respawn()
    {
        Vector3 respawnPosition = new Vector3(2, 2, 107); // Define a method or variable to get respawn location
        RespawnPlayer(respawnPosition);
    }

    private void RespawnPlayer(Vector3 respawnPosition)
    {
        rb.MovePosition(respawnPosition);
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        CurrentHP = MaxHP;
        stamina = maxStamina;
        hasDied = false;  // Player is no longer dead
        //animator.SetBool("Block", false);  // Reset animation states if necessary
        animator.SetTrigger("Respawn");  // Optionally play a respawn animation
        rb.velocity = Vector3.zero;  // Stop any momentum the player had
    }



    private void LoadDungeonScene()
    {
        SceneManager.LoadScene("Dungeon");
    }

    void OnDisable()
    {
        EventManager.StopListening("EnemyDied", OnEnemyDied);
    }

    private void Start()
    {

        MaxHP = 100;
        CurrentHP = 100;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        dataHandler = GetComponent<DataHandler>();
        LoadPlayerData();
        //weaponSocket = GameObject.FindGameObjectsWithTag("WeaponSocket")[0];
        vcam = FindObjectOfType<CinemachineFreeLook>();
        cam = GameObject.Find("Camera").transform;

        //inventory.Add((GameObject) Resources.Load("Low-Poly Weapons/Prefabs/Sword"));
        //inventory.Add((GameObject)Resources.Load("Low-Poly Weapons/Prefabs/Dagger"));
        //GameObject.Find("Player").GetComponent<PlayerScript>().inventory.Add((GameObject)Resources.Load("PurePoly/Free_Swords/Prefabs/PP_Sword_0222"));
        //GameObject.Find("Player").GetComponent<PlayerScript>().inventory.Add((GameObject)Resources.Load("Club"));
    }



    void Update()
    {
        handleUnstuckMe();

        if (hasDied) return;

        handleRollInput();
        handleTargetInput();
        handleCombatInput();
        handleStaminaGain();
    }

    void handleUnstuckMe()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            rb.MovePosition(new Vector3(2, 2, 107));
        }
    }

    private void FixedUpdate()
    {
        if (hasDied) return;
        handleMovementInput();
    }

    void handleCombatInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (stamina > attackStaminaCost) { 
                Attack();
            }
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            CurrentHP += 30;
            animator.SetTrigger("Heal");
        }
    }

    public void Block()
    {
        animator.SetBool("Block", true);
    }

    public override void Attack()
    {
        // doing it inside the animator instead
        // isAttacking = true;
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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Force);
        }

    }

    void handleMovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (cam == null) return;
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
        }
    }

    void handleRollInput()
    {
        if (CanRoll() && Input.GetKeyDown(KeyCode.Space))
        {
            if (stamina < rollStaminaCost) return;

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
            //isRolling = true;
            //isGrounded = false;

            stamina -= rollStaminaCost;
            lastRollTime = Time.time;
        }
    }

    bool CanRoll()
    {
        return Time.time >= lastRollTime + rollCooldown;
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
        }
    }

    public float Stamina
    {
        get { return stamina; }
    }

    private void SetIsRollingBoolean()
    {
        isRolling = true;
    }

    private void SetIsRollingBooleanToFalse()
    {
        isRolling = false;
    }
}


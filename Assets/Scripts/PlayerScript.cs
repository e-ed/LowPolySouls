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
    GameObject targetCircle;
    GameObject currentTarget;
    public float rayDistance;
    float enemyHeight;
    Transform playerNeck;
    private CenterCamera followRecenter;
    public int baseXP = 10;
    public float exponent = 1.7f;
    PlayerScript player;
    public GameObject levelUpPanel;


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

    public int GetXPRequiredForLevel()
    {
        Debug.Log("Level: " + Level);
        return Mathf.RoundToInt(baseXP * Mathf.Pow(player.Level, exponent));
    }

    public bool canPlayerLevelUp()
    {
        return souls > GetXPRequiredForLevel();
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
                Strength = data.strength;
                setMaxHp();
                CurrentHP = MaxHP;
                Dexterity = data.dexterity;
                Intelligence = data.intelligence;
                Souls = data.souls;
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
        resetTarget();
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
        player = gameObject.GetComponent<PlayerScript>();
        rb = GetComponent<Rigidbody>();
        LoadPlayerData();
        animator = GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        dataHandler = GetComponent<DataHandler>();
        //weaponSocket = GameObject.FindGameObjectsWithTag("WeaponSocket")[0];
        vcam = FindObjectOfType<CinemachineFreeLook>();
        cam = GameObject.Find("Camera").transform;
        targetCircle = (GameObject)Resources.Load("Target Circle");
        playerNeck = GameObject.FindWithTag("Neck").transform;
        followRecenter = vcam.GetComponent<CenterCamera>();
        audioSource = GetComponent<AudioSource>();


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
        if (levelUpPanel.activeSelf)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (stamina > attackStaminaCost)
            {
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
            if (CurrentHP > MaxHP)
            {
                CurrentHP = MaxHP;
            }
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
        vcam.m_YAxis.m_MaxSpeed = 0.05f;
        vcam.m_XAxis.m_MaxSpeed = 3;

        hasTarget = false;
        //vcam.GetComponent<CinemachineTargetGroup>().RemoveMember(enemyTarget.transform);
        vcam.LookAt = playerNeck;
        //vcam.Follow = gameObject.transform;
        vcam.m_Orbits[0].m_Radius = 6;
        vcam.m_Orbits[1].m_Radius = 5;
        vcam.m_Orbits[2].m_Radius = 4.5f;
        Destroy(currentTarget);
    }

    void RecenterCamera()
    {
        if (followRecenter != null)
        {
            followRecenter.Recenter = true; // This will start the recentering process
        }
    }

    void handleTargetInput()
    {
        if (hasTarget && enemyTarget == null)
        {
            resetTarget();
        }

        if (hasTarget)
        {
            //Ray ray = new Ray(vcam.transform.position+new Vector3(0, 0, -1f), vcam.transform.forward);
            //LayerMask layerMask = LayerMask.GetMask("Player", "Enemy");
            //RaycastHit[] hits = Physics.RaycastAll(ray, 50f, layerMask);

            //// Iterate over each hit object
            //foreach (RaycastHit hit in hits)
            //{
            //    Debug.Log("Hit: " + hit.collider.name);
            //    // Do something with each hit, like applying damage or effects
            //}

            //Debug.DrawRay(vcam.transform.position, vcam.transform.forward, Color.red, 2.0f); // '2.0f' sets how long the ray is visible

            // RecenterCamera();
            // Update position based on the enemy's height
            //RectTransform rt = currentTarget.GetComponent<RectTransform>();
            //float offsetY = enemyHeight - 0.25f; // Adjust as needed
            //rt.anchoredPosition = new Vector2(0, offsetY); // Adjust Y position to align with enemy height
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (hasTarget)
            {
                resetTarget();
                return;
            }

            LayerMask mask = LayerMask.GetMask("Enemy");

            // Set up the direction and distance of the raycast
            Vector3 rayOrigin = transform.position + new Vector3(0, 1, 0);
            Vector3 rayDirection = transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, mask))
            {
                hasTarget = true;
                enemyTarget = hit.collider.gameObject;

                //vcam.GetComponent<CinemachineTargetGroup>().AddMember(enemyTarget.transform, 2, 0);
                //vcam.Follow = vcam.GetComponent<CinemachineTargetGroup>().m_Target;
                //vcam.m_XAxis.m_MaxSpeed = 0;
                vcam.m_YAxis.m_MaxSpeed = 0;

                //vcam.Follow = enemyTarget.transform;
                vcam.LookAt = enemyTarget.transform;

                Collider enemyCollider = enemyTarget.GetComponent<Collider>();


                if (enemyCollider != null)
                {
                    Canvas enemyCanvas = enemyTarget.GetComponentInChildren<Canvas>();
                    Transform targetSpawnPoint = enemyCanvas.transform.Find("TargetCircleSpawnPoint");
                    if (enemyCanvas != null)
                    {
                        if (currentTarget != null)
                        {
                            Destroy(currentTarget);
                        }
                        currentTarget = Instantiate(targetCircle, targetSpawnPoint.transform, false); // Instantiate as a child of the canvas
                        currentTarget.transform.localPosition += new Vector3(0, 0, -0.5f);
                        //RectTransform rt = currentTarget.GetComponent<RectTransform>();
                        //rt.anchorMin = new Vector2(0.5f, 0.5f); // Centered relative to the Canvas
                        //rt.anchorMax = new Vector2(0.5f, 0.5f); // Centered relative to the Canvas
                        //rt.pivot = new Vector2(0.5f, 0.5f);   // Centered pivot
                        //rt.anchoredPosition = new Vector2(0, enemyHeight / 2); // Adjust as needed
                    }
                }
            }

            // Draw the ray in the scene view for debugging
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red, 2.0f); // '2.0f' sets how long the ray is visible
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            string randomEnemy = Random.Range(1, 9).ToString();
            Instantiate(Resources.Load<Actor>("Prefabs/Enemy" + randomEnemy), (transform.position + new Vector3(1, 2, 0)), Quaternion.identity);
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

        //vcam.m_XAxis.Value -= (horizontalInput*5);
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

    public void IncreaseStrength()
    {
        player.Strength++;
        setMaxHp();
    }

    public void IncreaseDexterity()
    {
        player.Dexterity++;
    }

    public void IncreaseIntelligence()
    {
        player.Intelligence++;
    }

    private void setMaxHp()
    {
        MaxHP = 80 + (Strength * 2);
        EventManager.TriggerEvent("SetMaxHp", null);
    }

    public void levelUpAndDecreaseSouls()
    {
        Souls -= GetXPRequiredForLevel();
        player.Level++;
    }
}


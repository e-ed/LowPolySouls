using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float lastRollTime;
    GameObject targetCircle;
    GameObject currentTarget;
    public float rayDistance;
    float enemyHeight;
    Transform playerNeck;
    public int baseXP = 10;
    public float exponent = 1.7f;
    PlayerScript player;
    public GameObject levelUpPanel;
    public int flaskCharges = 10;
    public TextMeshProUGUI charges;
    public bool isHealing;
    public GameObject bossHpBar;
    public Dictionary<string, TransformData> enemies = new Dictionary<string, TransformData>();


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

    public float Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }

    public int GetXPRequiredForLevel()
    {
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

    void resetBossesAggrodState()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Boss");

        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<EnemyNavScript>().hasAggrodOnce = false;
        }
    }


    void OnPlayerDied(object obj)
    {
        if (!hasDied)
        {
            hasDied = true;
            animator.SetTrigger("Dying");
            bossHpBar.SetActive(false);
            Invoke("Respawn", 5f);  // Call Respawn after 5 seconds
            Invoke("removeAllEnemies", 4.8f);
            //Invoke("resetBossesAggrodState", 5f);
            Invoke("spawnEnemies", 5f);
        }
    }

    void removeAllEnemies()
    {
        GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        // Combine the two arrays
        GameObject[] allEnemies = enemiesList.Concat(bosses).ToArray();

        foreach (GameObject gameObj in allEnemies)
        {
            Destroy(gameObj);
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
        flaskCharges = 10;
        EventManager.TriggerEvent("flaskChargesChanged", player.flaskCharges);
        resetTarget();
    }

    void saveEnemyData()
    {
        GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        // Combine the two arrays
        GameObject[] allEnemies = enemiesList.Concat(bosses).ToArray();


        foreach (GameObject go in enemiesList)
        {
            TransformData enemyTransform = new TransformData(go.transform);
            enemies.Add(new string(go.name), enemyTransform);
            Debug.Log("Adding " + go.name + " with position " + enemyTransform.position);
        }

        foreach (GameObject go in bosses)
        {
            TransformData enemyTransform = new TransformData(go.transform);
            enemies.Add(new string(go.name.Split(" ")[0]), enemyTransform);
            Debug.Log("Adding " + go.name.Split(" ")[0] + " with position " + go.transform);

        }
    }

    void spawnEnemies()
    {
        foreach (KeyValuePair<string, TransformData> enemy in enemies)
        {
            Debug.Log(enemy.Key.Split(" ")[0] + " - " + enemy.Value);
            Actor spawnedEnemy = Instantiate(Resources.Load<Actor>("Prefabs/" + enemy.Key.Split(" ")[0]), enemy.Value.position, enemy.Value.rotation);
            if (spawnedEnemy.GetComponent<EnemyScript>().isBossType)
            {
                spawnedEnemy.GetComponent<BossHealthBar>().bossHealthBar = bossHpBar;
            }
        }
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
        audioSource = GetComponent<AudioSource>();

        saveEnemyData();

        EventManager.TriggerEvent("flaskChargesChanged", player.flaskCharges);

        //inventory.Add((GameObject) Resources.Load("Low-Poly Weapons/Prefabs/Sword"));
        //inventory.Add((GameObject)Resources.Load("Low-Poly Weapons/Prefabs/Dagger"));
        //GameObject.Find("Player").GetComponent<PlayerScript>().inventory.Add((GameObject)Resources.Load("PurePoly/Free_Swords/Prefabs/PP_Sword_0222"));
        //GameObject.Find("Player").GetComponent<PlayerScript>().inventory.Add((GameObject)Resources.Load("Club"));
    }

    void Update()
    {
        handleDevCommands();

        if (hasDied) return;

        handleRollInput();
        handleTargetInput();
        handleCombatInput();
        handleStaminaGain();
        handleTargetCameraChange();
    }

    void handleTargetCameraChange()
    {
        if (hasTarget)
        {
            vcam.m_RecenterToTargetHeading.m_enabled = true;
            vcam.m_RecenterToTargetHeading.m_WaitTime = 0;
            vcam.m_RecenterToTargetHeading.m_RecenteringTime = 0;
            gameObject.transform.LookAt(enemyTarget.transform);
        }
    }

    void handleDevCommands()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            rb.MovePosition(new Vector3(2, 2, 107));
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            rb.MovePosition(new Vector3(16, 0, 161));
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            GainSouls(1000);
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking || animator.GetBool("Block") || isHealing || hasDied) return;
        handleMovementInput();
    }

    void handleCombatInput()
    {
        if (levelUpPanel != null && levelUpPanel.activeSelf)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (stamina > attackStaminaCost)
            {
                Attack();
                // doing it inside animator instead
                // to avoid "false positives" with attack
                //stamina -= attackStaminaCost;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

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
            if (flaskCharges < 1)
            {
                flaskCharges = 0;
                return;
            }
            animator.SetTrigger("Heal");
        }
    }

    public void Block()
    {
        rb.velocity = Vector3.zero;
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
        vcam.m_RecenterToTargetHeading.m_enabled = false;

        hasTarget = false;
        //vcam.GetComponent<CinemachineTargetGroup>().RemoveMember(enemyTarget.transform);
        vcam.LookAt = playerNeck;
        //vcam.Follow = gameObject.transform;
        vcam.m_Orbits[0].m_Radius = 9;
        vcam.m_Orbits[1].m_Radius = 6;
        vcam.m_Orbits[2].m_Radius = 2.5f;
        Destroy(currentTarget);
    }

    void handleTargetInput()
    {
        if (hasTarget && enemyTarget == null)
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isRolling)
        {
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Force);
        }
    }


    void handleMovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (cam == null)
        {
            cam = GameObject.Find("Camera").transform;
        }

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

        float horizontal = hasTarget ? horizontalInput : Mathf.Abs(movement.x);
        float vertical = hasTarget ? verticalInput : Mathf.Abs(movement.z);

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);


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



            animator.SetTrigger("isRolling");


        }
    }

    bool CanRoll()
    {
        return ((Time.time >= lastRollTime + rollCooldown));
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
        player.CurrentHP = MaxHP;
        EventManager.TriggerEvent("SetMaxHp", null);
    }

    public void levelUpAndDecreaseSouls()
    {
        Souls -= GetXPRequiredForLevel();
        player.Level++;
    }
}

public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Transform transform)
    {
        // Copy the position, rotation, and scale from the Transform
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
    }
}

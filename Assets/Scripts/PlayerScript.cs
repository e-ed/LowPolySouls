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
    private float rollCooldown = 0.6f;
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
    public Dictionary<string, EnemiesData> enemies = new Dictionary<string, EnemiesData>();


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
            soulsText.text = souls.ToString();
        }
    }

    void GainSouls(int soulAmount)
    {
        souls += soulAmount;
        EventManager.TriggerEvent("UpdateSoulsPanel", souls);
    }

    void OnEnemyDied(object soulsGained)
    {
        GainSouls((int)soulsGained);
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
            Invoke("Respawn", 5f); 
            Invoke("removeAllEnemies", 4.8f);
            //Invoke("resetBossesAggrodState", 5f);
            Invoke("spawnEnemies", 5f);
        }
    }

    void removeAllEnemies()
    {
        GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        GameObject[] allEnemies = enemiesList.Concat(bosses).ToArray();

        foreach (GameObject gameObj in allEnemies)
        {
            Destroy(gameObj);
        }
    }

    private void Respawn()
    {
        Vector3 respawnPosition = new Vector3(2, 2, 107); 
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
        hasDied = false;  
        //animator.SetBool("Block", false); 
        animator.SetTrigger("Respawn"); 
        rb.velocity = Vector3.zero;
        flaskCharges = 10;
        EventManager.TriggerEvent("flaskChargesChanged", player.flaskCharges);
        resetTarget();
    }

    void saveEnemyData()
    {
        GameObject[] enemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        GameObject[] allEnemies = enemiesList.Concat(bosses).ToArray();


        foreach (GameObject go in enemiesList)
        {
            EnemiesData enemyTransform = new EnemiesData(go.transform);
            enemies.Add(new string(go.name), enemyTransform);
            Debug.Log("Adding " + go.name + " with position " + enemyTransform.position);
        }

        foreach (GameObject go in bosses)
        {
            EnemiesData enemyTransform = new EnemiesData(go.transform);
            enemies.Add(new string(go.name.Split(" ")[0]), enemyTransform);
            Debug.Log("Adding " + go.name.Split(" ")[0] + " with position " + go.transform);

        }
    }

    void spawnEnemies()
    {
        foreach (KeyValuePair<string, EnemiesData> enemy in enemies)
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
            vcam.m_RecenterToTargetHeading.m_RecenteringTime = 0.1f;
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
        vcam.LookAt = playerNeck;
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

            Vector3 rayOrigin = transform.position + new Vector3(0, 1, 0);
            Vector3 rayDirection = transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, mask))
            {
                hasTarget = true;

                enemyTarget = hit.collider.gameObject;

                vcam.m_YAxis.m_MaxSpeed = 0;
                vcam.m_XAxis.m_MaxSpeed = 0;

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
                        currentTarget = Instantiate(targetCircle, targetSpawnPoint.transform, false);
                        currentTarget.transform.localPosition += new Vector3(0, 0, -0.5f);
                    }
                }
            }

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


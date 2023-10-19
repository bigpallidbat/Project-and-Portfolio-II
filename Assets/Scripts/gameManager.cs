using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("----- player stuff ------")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject[] SpawnPoints;
    public GameObject playerSpawnPoint;
    public GameObject[] Doors;
    public DoorController _DC;

    [Header("---------- UI ----------")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuInv;
    public Image playerHpBar;
    public Image playerStamBar;
    [SerializeField] TMP_Text enemiesRemainingText;
    [SerializeField] TMP_Text AmmoCurrent;
    [SerializeField] TMP_Text AmmoMax;
    [SerializeField] TMP_Text GrenadeCount;
    [SerializeField] TMP_Text MedkitCount;
    [SerializeField] GameObject playerDamageFlash;

    [Header("----- GameMode/Level -----")]
    [SerializeField] static int gameModeChosen;

   // enum GameMode { SpecialEnemy = 1, EnemyCount, SpawnerDestroy, ItemRecovery };
    
    public bool isPaused;
    float timeScaleOrig;
    static int enemiesRemaining;
    static int currentLevel;
    GameObject Door;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        timeScaleOrig = Time.timeScale;
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel > 1)
        {
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();
            if (!sceneManager.scenechange)
            {
                playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
                playerScript.setStats();
            }
            else if (sceneManager.scenechange)
            {
                sendDoor(DoorController.doorNumber);

            }
        }
        if (isPaused)
        {
            stateUnpause();
        }



        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
        if(Input.GetButtonDown("Inventory") && menuActive == null)
        {
            statePause();
            menuActive = menuInv;
            menuActive.SetActive(isPaused);
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemainingText.text = enemiesRemaining.ToString("0");

        if (enemiesRemaining <= 0)
        {
            //end game
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
    public void updateGameGoal()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public IEnumerator youWin()
    {
        yield return new WaitForSeconds(3);
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void sendDoor(int doornum)
    {
        if (findDoor(doornum))
        {
            sceneManager.scenechange = false;
            playerScript.spawnPlayer(Quaternion.Euler(0, 0, 0));
        }

    }

    bool findDoor(int doornum)
    {
        for (int i = 0; i < Doors.Length; ++i)
        {
            // Door = Doors[i];
            _DC = Doors[i].GetComponent<DoorController>();
            if (_DC.DN == doornum)
            {
                playerSpawnPoint = SpawnPoints[i];
                return true;
            }

        }
        return false;

    }

    public IEnumerator playerFlash()
    {
        playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageFlash.SetActive(false);
    }

    public void updateAmmo(int cur, int max)
    {
        AmmoCurrent.text = cur.ToString("F0");
        AmmoMax.text = max.ToString("F0");
    }

    public void updateAmmo(int cur)
    {
        AmmoCurrent.text = cur.ToString("F0");
    }
    
    public void updateMedkit(int cur)
    {
        MedkitCount.text = cur.ToString("F0");
    }

    public void updateGrenade(int cur)
    {
        GrenadeCount.text = cur.ToString("F0");
    }

    public void setCurrLevel(int level)
    {
        currentLevel = level;
    }
}
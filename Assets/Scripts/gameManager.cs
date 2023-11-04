using System.Collections;
using System.Collections.Generic;
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
    public Image BossHPBar;
    public Image BossHpFill;
    [SerializeField] TMP_Text enemiesRemainingText;
    [SerializeField] TMP_Text GoalText;
    [SerializeField] GameObject WaveParent;
    [SerializeField] TMP_Text WaveMax;
    [SerializeField] TMP_Text WaveCur;
    [SerializeField] TMP_Text AmmoCurrent;
    [SerializeField] TMP_Text AmmoMax;
    [SerializeField] TMP_Text GrenadeCount;
    [SerializeField] TMP_Text MedkitCount;
    [SerializeField] GameObject playerDamageFlash;

    [Header("----- GameMode/Level -----")]
    [SerializeField] static int gameModeChosen;
    [SerializeField] List<GameObject> spawnerList;
    [SerializeField] GameObject EndDoor;

   public enum Levels { MainMenu ,SpecialEnemy , SpawnerDestroy =3, Boss, horror ,Wave , Devwork = 10 };
    
    public bool isPaused;
    float timeScaleOrig;
    static int enemiesRemaining;
    static int currentLevel;
    GameObject Door;
    int goalAmount;
    int waveMax;
    public int waveCur;
    public static bool miniGoalAcquired;
    public static Levels currentlevel;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        timeScaleOrig = Time.timeScale;
        emergencyCheck();
        //Debug.Log(SceneManager.GetActiveScene().buildIndex);
        if (currentlevel != Levels.MainMenu)
        {
            
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();
            EndDoor = GameObject.FindWithTag("EndDoor");
            EndDoor.SetActive(false);
            if (!sceneManager.scenechange || DoorController.doorNumber == -1)
            {
                miniGoalAcquired = false; 
                playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
                if(currentlevel == Levels.SpecialEnemy)
                {
                    playerScript.setStats();
                }
            }
            else if (sceneManager.scenechange)
            {
                sendDoor(DoorController.doorNumber);

            }
            catchGoal();
        }
        
        if (isPaused)
        {
            stateUnpause();
        }

        
    }

    void emergencyCheck()
    {
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings)
        {
            currentlevel = Levels.Devwork;
        }
        else if((SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2 ))
        {
            currentlevel = Levels.SpecialEnemy;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 3 && currentlevel == Levels.MainMenu)
        {
            currentlevel = Levels.SpawnerDestroy;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            currentlevel= Levels.Boss;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 5)
        {
            currentlevel = Levels.Wave;
        }
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
 
    }

   
    public void updateSpawners()
    {
        goalAmount--;
        if( goalAmount <= 0)
        {
            miniGoalAcquired = true;
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpauseWithCursor()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void updateGameGoal()
    {
        //statePause();
        //menuActive = menuWin;
        //menuActive.SetActive(true);

        if(currentlevel == Levels.SpecialEnemy)
        {
            EndDoor.SetActive(true);
        }
        else if(currentlevel == Levels.SpawnerDestroy)
        {
            //replace updateSpawners with this.
        }
        else if(currentlevel == Levels.Boss)
        {
            EndDoor.SetActive(true);
        }
        else if(currentlevel == Levels.horror)
        {
            EndDoor.SetActive(true);
        }
        else if(currentlevel == Levels.Wave)
        {
            EndDoor.SetActive(true);
        }

        
    }

    private void checkGoal()
    {
        if (enemiesRemaining <= 0)
        {
            updateGameGoal();
        }
    }

    public void minorUpdateGoal(int amount)
    {
        if (currentlevel == Levels.SpecialEnemy)
        {
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();

            checkGoal();
        }
        else if (currentlevel == Levels.SpawnerDestroy)
        {
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();

            checkGoal();
        }
        else if (currentlevel == Levels.Boss)
        {
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();

            checkGoal();

        }
        else if(currentlevel == Levels.horror)
        {

        }
        else if(currentlevel == Levels.Wave)
        {
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();
        }
    }

    private void setGameGoal(int amount)
    {
        if (currentlevel == Levels.SpecialEnemy)
        {
            GoalText.text = "Ascend the Tower and defeat the Irregularity!";
            enemiesRemainingText.text = amount.ToString();
        }
        else if (currentlevel == Levels.SpawnerDestroy)
        {
            GoalText.text = "Traverse the Farmland and destroy the enemy hives";
            enemiesRemainingText.text = amount.ToString();
        }
        else if (currentlevel == Levels.Boss)
        {
            GoalText.text = "Defeat Tomas the Destroyer";
            enemiesRemainingText.text = amount.ToString();
        }
        else if(currentlevel == Levels.horror)
        {

        }
        else if(currentlevel == Levels.Wave)
        {

        }
    }

    void catchGoal()
    {
        List<GameObject> objs = new List<GameObject>();
        GameObject[] js = GameObject.FindGameObjectsWithTag("Goal Point");

        for(int i = 0; i < js.Length; i++)
        {
            objs.Add(js[i]);
        }
        enemiesRemaining = objs.Count;
        setGameGoal(objs.Count);
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
            sceneManager.scenechange = false;
        if (findDoor(doornum))
        {

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
        currentlevel = (Levels)level;
    }

    public int getEnemiesRemaining()
    {
        return enemiesRemaining;
    }
    public void setEnemiesRemaining(int amount)
    {
        enemiesRemaining = amount;
        enemiesRemainingText.text = enemiesRemaining.ToString();
    }

    public void setWaveCur(int num)
    {
        WaveCur.text = num.ToString();
    }
    public void setWaveMax(int num)
    {
        WaveMax.text = num.ToString();
    }

}
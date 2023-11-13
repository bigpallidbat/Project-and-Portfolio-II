using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
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
    [SerializeField] GameObject menuHint;
    public Image playerHpBar;
    public Image playerStamBar;
    public Image BossHPBar;
    public Image BossHpFill;
    [SerializeField] TMP_Text enemiesRemainingText;
    [SerializeField] TMP_Text GoalText;
    [SerializeField] TMP_Text WaveMax;
    [SerializeField] TMP_Text WaveCur;
    [SerializeField] TMP_Text AmmoCurrent;
    [SerializeField] TMP_Text AmmoMax;
    [SerializeField] TMP_Text GrenadeCount;
    [SerializeField] TMP_Text MedkitCount;
    [SerializeField] GameObject playerDamageFlash;
    [SerializeField] GameObject CyanKey;
    [SerializeField] GameObject MagentaKey;
    [SerializeField] GameObject GreenKey;
    [SerializeField] TMP_Text KeyCount;

    [Header("----- GameMode/Level -----")]
    [SerializeField] static int gameModeChosen;
    [SerializeField] List<GameObject> spawnerList;
    [SerializeField] GameObject EndDoor;
    [SerializeField] GameObject boss;
    public GameObject keyMaster;

    [Header("-----  Music/sounds  -----")]
    [SerializeField] AudioSource themes;
    [SerializeField] AudioClip pauseMenu;
    [SerializeField] AudioClip defeat;
    [SerializeField] AudioClip YouWin;
    [SerializeField] string[] mixerList;
    [SerializeField] AudioMixer mixer;

    public enum Levels { MainMenu ,SpecialEnemy , SpawnerDestroy = 3, Boss, horror , Wave, Voxel, Credits , Devwork = 10 };

    int keyCount;
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
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (currentlevel != Levels.MainMenu)
        {


            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();
            EndDoor = GameObject.FindWithTag("EndDoor");
            if(EndDoor != null) EndDoor.SetActive(false);
            if (!sceneManager.scenechange || DoorController.doorNumber == -1)
            {
                miniGoalAcquired = false; 
                playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
            }
            else if (sceneManager.scenechange)
            {
                sendDoor(DoorController.doorNumber);

            }
             catchGoal();
        }
        
        
        
    }

    private void Start()
    {
        if (currentlevel == Levels.MainMenu)
        {
            setVolumes();
        }
        else if (checkLevel())
        {

            menuActive = menuHint;
            menuActive.SetActive(true);
            statePause();
        }
    }
    
    bool checkLevel()
    {
        if(currentlevel == Levels.MainMenu || currentlevel == Levels.Credits)
        {
            return false;
        }
        else if(currentlevel == Levels.Boss || currentlevel == Levels.Wave)
        {
            return false;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }

    void setVolumes()
    {
        if (mixerList.Length > 0)
        {
            for (int i = 0; i < mixerList.Length; i++)
            {
                if(mixer.SetFloat(mixerList[i], PlayerPrefs.GetFloat(mixerList[i])))
                {
                    float value;
                    mixer.GetFloat(mixerList[i], out value);
                    Debug.Log(value);
                }
            }
        }
    }

        void emergencyCheck()
    {
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings -1)
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
            currentlevel = Levels.horror;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 6)
        {
            currentlevel = Levels.Wave;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 7)
        {
            currentlevel = Levels.Voxel;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 8)
        {
            currentlevel = Levels.Credits;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuActive == null && currentlevel != Levels.MainMenu)
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

    public void horrorEnd()
    {
        EndDoor.SetActive(true);
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
            miniGoalAcquired = true;
            EndDoor.SetActive(true);
        }
        else if(currentlevel == Levels.Boss)
        {
            EndDoor.SetActive(true);
            //StartCoroutine(youWin());
        }
        else if(currentlevel == Levels.horror)
        {
            HorrorBoss bossScript = boss.GetComponent<HorrorBoss>();
            bossScript.Hunt();
            
        }
        else if(currentlevel == Levels.Wave)
        {
           
        }
        else if (currentlevel == Levels.Voxel)
        {
            StartCoroutine(youWin());
        }
    }

    private void checkGoal()
    {
        if (enemiesRemaining <= 0)
        {
            updateGameGoal();
        }
    }

    public void setDoorActive()
    {
        EndDoor.SetActive(true);
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
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();

            checkGoal();
        }
        else if(currentlevel == Levels.Wave)
        {
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();

            
        }
        else if (currentlevel == Levels.Voxel)
        {
            enemiesRemaining += amount;
            enemiesRemainingText.text = enemiesRemaining.ToString();

            checkGoal();
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
            boss = GameObject.FindWithTag("Boss");

            GoalText.text = "Collect all cursed dolls to kill the stalker";
            enemiesRemainingText.text = amount.ToString();
        }
        else if(currentlevel == Levels.Voxel)
        {
            GoalText.text = "Find The hidden enemy and destroy it.";
            enemiesRemainingText.text = amount.ToString();
        }
    }

    void catchGoal()
    {
        enemiesRemaining = 0;
        List<GameObject> objs = new List<GameObject>();
        GameObject[] js = GameObject.FindGameObjectsWithTag("Goal Point");

        for(int i = 0; i < js.Length; i++)
        {
            objs.Add(js[i]);
            enemiesRemaining++;
        }
        
        setGameGoal(objs.Count);
    }

    public IEnumerator youWin()
    {
        yield return new WaitForSeconds(1);
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
           // sceneManager.scenechange = false;
        if (findDoor(doornum))
        {

            //playerScript.spawnPlayer(Quaternion.Euler(0, 0, 0));
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
        waveCur = num;
        WaveCur.text = num.ToString();
    }
    public void setWaveMax(int num)
    {
        waveMax = num;
        WaveMax.text = num.ToString();
    }

    public void SetGKeyActive()
    {
        GreenKey.SetActive(true);
    }

    public void SetCKeyActive()
    {
        CyanKey.SetActive(true);
    }

    public void SetMKeyActive()
    {
        MagentaKey.SetActive(true);
    }

    public void updateKeyCount(int amount)
    {
        keyCount += amount;
        KeyCount.text = keyCount.ToString();
    }
}
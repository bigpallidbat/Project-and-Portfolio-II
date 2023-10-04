using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("----- player stuff ------")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPoint;
    public GameObject[] Doors;
    public DoorController _DC;

    [Header("---------- UI ----------")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    public Image playerHpBar;
    public Image playerStamBar;
    [SerializeField] TMP_Text enemiesRemainingText;

    public bool isPaused;
    float timeScaleOrig;
    int enemiesRemaining;
    GameObject Door;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPoint = GameObject.FindWithTag("Player Spawn Point");
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
    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void sendDoor(int doornum, GameObject obj)
    {
        /*
        Doors = GameObject.FindGameObjectsWithTag("Door");
        for(int i = 0; i < Doors.Length;++i)
        {
            Door = Doors[i];
            _DC = Door.GetComponent<DoorController>();
            if(_DC.doorNumber == doornum)
            {
               // playerSpawnPoint.transform.position = _DC.doorSpawn.position;
                break;
            }
        }
        */
        Door = obj;
        _DC = Door.GetComponent<DoorController>();
        playerSpawnPoint.transform.position = _DC.doorSpawn.position;
        playerScript.spawnPlayer();
        Debug.Log(playerSpawnPoint.transform.position);
    }
}

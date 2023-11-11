using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DungeonDoor : MonoBehaviour, IInteract
{
    [SerializeField] bool isMimic;
    [SerializeField] bool isLocked;
    [SerializeField] bool isMonsterDoor;
    [SerializeField] bool greenLock;
    [SerializeField] bool cyanlock;
    [SerializeField] bool magentalock;
    [SerializeField] GameObject keyMaster;
    [SerializeField] GameObject mimic;
    [SerializeField] Collider settrap;
    bool wasActivated;
    bool opening;
    float startingPos;
    Vector3 curPos;
    float endingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position.y;
        curPos = transform.position;
        endingPos = transform.position.y + 3.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (opening && curPos.y <= endingPos)
        {
            curPos.y += 2 * Time.deltaTime;
            if (curPos.y >= endingPos)
            {
                curPos.y = endingPos;
                wasActivated = true;
            }
            transform.position = curPos;
        }
        else if (!opening && curPos.y >= startingPos)
        {
            curPos.y -= 20 * Time.deltaTime;
            if (curPos.y <= startingPos)
            {
                curPos.y = startingPos;
                if (isMimic) mimic.GetComponent<DungenDoorWay>().wakeUp();
            }
            transform.position = curPos;
            
        }
        if (wasActivated && !isMimic) Destroy(gameObject);
        else settrap.enabled = true;
        //curPos = Mathf.Lerp(transform.position.y, endingPos, Time.deltaTime * 4);
        //transform.position = curPos;
    }

    public void Activate()
    {
        if (isLocked) if (keyMaster.GetComponent<KeyMaster>().useSmallKey()) opening = true;
            else if (greenLock) if (keyMaster.GetComponent<KeyMaster>().useGreenGeeKey()) opening = true;
                else if (cyanlock) if (keyMaster.GetComponent<KeyMaster>().useCyanGeeKey()) opening = true;
                    else if (magentalock) if (keyMaster.GetComponent<KeyMaster>().useMagentaGeeKey()) opening = true;
                        else opening = true;
        wasActivated = true;
    }

    public void closeMimic()
    {
        opening = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetActionable(this);
        }
    }
}

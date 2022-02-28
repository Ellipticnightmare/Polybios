using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopterController : MonoBehaviour
{
    bool isController;
    int tMod;
    public static int soldierCount;
    public heliState HeliState = heliState.free;
    public heliHealthState HeliHealthState = heliHealthState.intact;
    public float interactCountdown;
    public AudioClip pickup, injured, dropoff;
    new private RectTransform transform;
    public RectTransform playArea;
    private Rect canvasRect;
    GameObject solider;

    [Range(1, 5)]
    public int speed;
    private void Start()
    {
        transform = GetComponent<RectTransform>();
        canvasRect = playArea.rect;
    }

    // Update is called once per frame
    void Update()
    {
        isController = GameManager.isController;
        tMod = GameManager.tMod;
        float hInput = 0;
        float vInput = 0;
        if (!isController)
        {
            KeyCode heliRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliRight"));
            KeyCode heliLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliLeft"));
            KeyCode heliUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliUp"));
            KeyCode heliDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliDown"));

            float lNorm = (Input.GetKey(heliLeft)) ? (transform.anchoredPosition.x > -350) ? 1 : 0 : 0;
            float rNorm = (Input.GetKey(heliRight)) ? (transform.anchoredPosition.x < 350) ? 1 : 0 : 0;
            float uNorm = (Input.GetKey(heliUp)) ? (transform.anchoredPosition.y < 145) ? 1 : 0 : 0;
            float dNorm = (Input.GetKey(heliDown)) ? (transform.anchoredPosition.y > -145) ? 1 : 0 : 0;

            hInput = rNorm - lNorm;
            vInput = uNorm - dNorm;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.FireEndCabinet();
            }

            if (hInput != 0)
                transform.localScale = new Vector3(hInput, 1, 1);

            Vector2 move = new Vector2(hInput, vInput);
            transform.anchoredPosition += move;
        }
        switch (HeliState) //Run the interaction Countdowns
        {
            case heliState.free:
                interactCountdown = 1;
                break;
            case heliState.tree:
                if (interactCountdown <= 0)
                {
                    GameManager.FireAudioBit(injured);
                    interactCountdown = 6;
                    switch (HeliHealthState)
                    {
                        case heliHealthState.intact:
                            HeliHealthState = heliHealthState.dented;
                            break;
                        case heliHealthState.dented:
                            HeliHealthState = heliHealthState.damaged;
                            break;
                        case heliHealthState.damaged:
                            HeliHealthState = heliHealthState.critical;
                            break;
                        case heliHealthState.critical:
                            FireDeath();
                            break;
                    }
                }
                else
                    interactCountdown -= Time.deltaTime;
                break;
            case heliState.soldier:
                if (interactCountdown <= 0 && soldierCount < 3)
                {
                    interactCountdown = 1;
                    soldierCount++;
                    Destroy(solider);
                    HeliState = heliState.free;
                    GameManager.FireAudioBit(pickup);
                }
                else
                    interactCountdown -= Time.deltaTime;
                break;
            case heliState.hospital:
                if (interactCountdown <= 0 && soldierCount > 0)
                {
                    GameManager.SavedSoldier();
                    GameManager.FireAudioBit(dropoff);
                    interactCountdown = 2;
                    soldierCount--;
                }
                else
                    interactCountdown -= Time.deltaTime;
                break;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag.ToString())
        {
            case "tree":
                callBackLogger.throwError("collided");
                HeliState = heliState.tree;
                break;
            case "soldier":
                callBackLogger.throwError("collided");
                HeliState = heliState.soldier;
                solider = collision.gameObject;
                break;
            case "hospital":
                callBackLogger.throwError("collided");
                HeliState = heliState.hospital;
                break;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        HeliState = heliState.free;
    }
    public void FireDeath()
    {
        callBackLogger.sendMessage("Helicopter died");
    }

    public enum heliState
    {
        free,
        tree,
        soldier,
        hospital
    };
    public enum heliHealthState
    {
        intact,
        dented,
        damaged,
        critical
    };
}
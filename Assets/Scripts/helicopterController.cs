using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopterController : MonoBehaviour
{
    bool isController;
    int tMod, soldierCount;
    public heliState HeliState = heliState.free;
    public heliHealthState HeliHealthState = heliHealthState.intact;
    public float interactCountdown;
    public AudioClip pickup, injured, dropoff;

    // Update is called once per frame
    void Update()
    {
        isController = GameManager.isController;
        tMod = GameManager.tMod;
        float hInput = 0;
        float vInput = 0;
        float movSpeed = 0;
        if (!isController)
        {
            KeyCode heliRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliRight"));
            KeyCode heliLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliLeft"));
            KeyCode heliUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliUp"));
            KeyCode heliDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("heliDown"));

            float lNorm = (Input.GetKey(heliLeft)) ? 1 : 0;
            float rNorm = (Input.GetKey(heliRight)) ? 1 : 0;
            float uNorm = (Input.GetKey(heliUp)) ? 1 : 0;
            float dNorm = (Input.GetKey(heliDown)) ? 1 : 0;

            hInput = lNorm - rNorm;
            vInput = uNorm - dNorm;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.FireEndCabinet();
            }

            Vector3 moveDirection = new Vector3(hInput, 0, vInput);
            this.gameObject.transform.Translate(moveDirection * movSpeed * Time.deltaTime * tMod);
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
                    GameManager.FireAudioBit(pickup);
                    interactCountdown = 3;
                    soldierCount++;
                }
                else
                    interactCountdown -= Time.deltaTime;
                break;
            case heliState.hospital:
                if (interactCountdown <= 0 && soldierCount > 0)
                {
                    GameManager.FireAudioBit(dropoff);
                    interactCountdown = 2;
                    soldierCount--;
                }
                else
                    interactCountdown -= Time.deltaTime;
                break;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag.ToString())
        {
            case "tree":
                HeliState = heliState.tree;
                break;
            case "soldier":
                HeliState = heliState.soldier;
                break;
            case "hospital":
                HeliState = heliState.hospital;
                break;
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
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
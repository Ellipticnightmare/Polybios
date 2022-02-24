using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    #region Variables
    #region Accessible
    [Range(1, 5)]
    public int hSpeed, vSpeed, baseSpeed;
    public Image cScreen;
    public Sprite monsterCursor, interactCursor, pickupCursor;
    public AudioClip needQuarter, gainQuarter;
    #endregion
    #region Protected
    CharacterController control;
    static float sanity;
    bool isController;
    int tMod;
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        #region Basic
        control = this.GetComponent<CharacterController>();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Basic
        isController = GameManager.isController;
        tMod = GameManager.tMod;
        float hInput = 0;
        float vInput = 0;
        float movSpeed = 0;
        if (!isController) //Keyboard specific logic
        {
            KeyCode right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right"));
            KeyCode left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left"));
            KeyCode up = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up"));
            KeyCode down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down"));
            KeyCode interact = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Interact"));
            KeyCode crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Crouch"));
            KeyCode flashlight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight"));

            float lNorm = (Input.GetKey(left)) ? 1 : 0;
            float rNorm = (Input.GetKey(right)) ? 1 : 0;
            float uNorm = (Input.GetKey(up)) ? 1 : 0;
            float dNorm = (Input.GetKey(down)) ? 1 : 0;

            hInput = lNorm - rNorm;
            vInput = uNorm - dNorm;

            movSpeed = Input.GetKey(crouch) ? baseSpeed * .45f : baseSpeed;

            this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * tMod, 0));
            Camera.main.transform.Rotate(new Vector3(-(Input.GetAxisRaw("Mouse Y") * tMod), 0, 0));

            if (PlayerPrefs.GetString("Interact").Length > 1)
            {
                if (Input.GetKeyDown(interact))
                    RunInteract();
            }
            else
            {
                if (Input.GetMouseButtonDown((int)interact))
                    RunInteract();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.FirePause();
            }
        }
        else //Controller specific logic
        {

        }
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 15))
        {
            switch (hit.collider.gameObject.layer)
            {
                case 8: //Monster
                    cScreen.sprite = monsterCursor;
                    break;
                case 9: //Interact
                    cScreen.sprite = interactCursor;
                    break;
                case 10: //Item
                    cScreen.sprite = pickupCursor;
                    break;
                default:
                    cScreen.sprite = null;
                    break;
            }
        }
        else
            cScreen.sprite = null;

        Vector3 moveDirection = this.transform.TransformDirection(hInput, 0, vInput);
        control.Move(moveDirection * movSpeed * Time.deltaTime * tMod);
        #endregion
    }
    public void RunInteract()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 15))
        {
            switch (hit.collider.gameObject.tag.ToString())
            {
                case "cabinet":
                    if (GameManager.HasQuarter())
                        GameManager.FireStartCabinet();
                    else
                        GameManager.FireAudioLine(needQuarter);
                    break;
                case "quarter":
                    if (!GameManager.HasQuarter())
                    {
                        GameManager.FireAudioBit(gainQuarter);
                        GameManager.quarterCount++;
                    }
                    break;
            }
        }
    }
}
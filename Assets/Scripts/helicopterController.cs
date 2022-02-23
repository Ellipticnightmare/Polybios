using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopterController : MonoBehaviour
{
    bool isController;
    int tMod;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag.ToString())
        {
            case "tree":
                break;
            case "soldier":
                break;
            case "hospital":
                break;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutToMainMenu : MonoBehaviour
{
    public void Exit()
    {
        File.Delete(Application.persistentDataPath + "/saves/" + PlayerPrefs.GetString("curUser") + ".save");
        SceneManager.LoadScene("MainMenu");
    }
}
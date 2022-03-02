using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public List<keyBindingData> defaultKeyBindings = new List<keyBindingData>();
    List<string> saveNames = new List<string>();
    public Dropdown saveSelect;
    public InputField newName;
    public GameObject mainPanel, newPlayerPanel, saveSelectPanel, confirmQuitPanel;
    public Text displaySelectedName;
    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
    }
    public void hitPlay()
    {
        saveSelect.ClearOptions();
        saveNames.Clear();
        var fileInfo = Directory.GetFiles(Application.persistentDataPath + "/saves", "*.save");
        if (fileInfo.Length > 0)
        {
            foreach (var item in fileInfo)
            {
                string saveData = readMetadata(item);
                saveNames.Add(saveData);
            }
            saveSelect.AddOptions(saveNames);
            UpdateFileSelection(0);
            mainPanel.SetActive(false);
            saveSelectPanel.SetActive(true);
        }
        else
            PromptNewCharacter();
    }
    public void hitQuit()
    {
        ToggleQuitPanel();
    }
    public void ToggleQuitPanel() => confirmQuitPanel.SetActive(!confirmQuitPanel);
    void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    void QuitGame() => Application.Quit();
    public void UpdateFileSelection(int value)
    {
        string newUserName = saveSelect.options[value].text;
        newUserName = newUserName.Substring(0, newUserName.IndexOf(","));
        PlayerPrefs.SetString("curUser", newUserName);
        displaySelectedName.text = newUserName;
    }
    public void ConfirmSelectedSave() => PlayGame();

    public void PromptNewCharacter()
    {
        mainPanel.SetActive(false);
        saveSelectPanel.SetActive(false);
        newPlayerPanel.SetActive(true);
    }
    public void SaveNewCharacter() => CheckNewNameInput();
    void CheckNewNameInput()
    {
        string newNameCheck = newName.text;
        if(File.Exists(Application.persistentDataPath + "/saves/" + newNameCheck + ".save"))
        {
            newName.text = "";
            newName.transform.Find("Placeholder").GetComponent<Text>().text = "Username already taken";
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/saves/" + newNameCheck + ".save");
            playerData newSave = new playerData();
            newSave.playerPostion = new SVector3(3.57f, 1.04f, 2.34f);
            newSave.userName = newName.text;
            newSave.playTime = 0f;
            newSave.keyBindings = defaultKeyBindings.ToArray();
            newSave.level = 0;
            newSave.polybiosState = "asleep";
            newSave.quarterCount = 0;
            newSave.score = 0;
            bf.Serialize(file, newSave);

            PlayerPrefs.SetString("curUser", newName.text);
            Debug.Log(PlayerPrefs.GetString("curUser"));
            PlayGame();
        }
    }
    public void DeletePlayer()
    {
        File.Delete(Application.persistentDataPath + "/saves/" + PlayerPrefs.GetString("curUser") + ".save");
        hitPlay();
    }

    string readMetadata(string item)
    {
        string outString = null;
        string playerName = null;
        double playTime = 0;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(item, FileMode.Open);
        playerData check = (playerData)bf.Deserialize(file);
        file.Close();
        playTime = check.playTime;
        playerName = check.userName;
        outString = playerName + ", " + System.TimeSpan.FromSeconds(playTime).ToString(@"m\:ss");
        return outString;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Pixelation.Scripts;

public class GameManager : MonoBehaviour
{
    #region Variables
    #region Accessible
    public static bool isController = false;
    public static int tMod = 1;
    public static int quarterCount = 0;
    public Camera badEndCam, mainCam, endWatchCam;
    public AudioSource SFX, music, vLine;
    public MonoBehaviour fpsControl, heliControl;
    public AudioClip neutralMusic, curseMusic, endMusic;
    #region UI
    public Text quarterDisplay, heliTimer, heliScore;
    public GameObject heliObj, fpsObj, levelHolder, keyBindUI, pauseMenu, endActor, mainCanvas, endCanvas;
    public GameObject[] soldierCountDisplay;
    public heliLevel[] heliLevels;
    public int curHeliLevel = 0;
    public Button[] keyBindButtons;
    #endregion
    #endregion
    #region Protected
    static bool isPause, inHeli, hasStartedHeliBefore;
    static AudioSource _SFX, _music, _vl;
    static MonoBehaviour _fps, _heli;
    gameState GameState = gameState.asleep;
    static float heligameTimer;
    static int heliscore, savedSoldierCount;
    string directory, curKeyToBind;
    double storedPlayTime;
    bool isReadingForKey = false;
    List<keyBindingData> keybindings = new List<keyBindingData>();
    Button curKeyBind;
    #region UI
    static Text _qDisplay, heliDisplay, heliscoreDisplay;
    static GameObject _hObj, _fObj;
    GameObject levelDisplay;
    #endregion
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        directory = Application.persistentDataPath + "/saves/" + PlayerPrefs.GetString("curUser") + ".save";
        Debug.Log(directory);
        StartCoroutine(ReadData());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        mainCam.enabled = false;
    }
    void Setup()
    {
        _SFX = SFX;
        _music = music;
        _vl = vLine;
        _fps = fpsControl;
        _heli = heliControl;
        _qDisplay = quarterDisplay;
        _hObj = heliObj;
        _fObj = fpsObj;
        heliDisplay = heliTimer;
        heliscoreDisplay = heliScore;
        CheckGameState();
        FindObjectOfType<FirstPersonController>().Setup();
    }
    public void SaveGame(bool returnToMenu) => StartCoroutine(saveGame(returnToMenu));
    IEnumerator ReadData()
    {
        if (File.Exists(directory))
        {
            FirstPersonController player = FindObjectOfType<FirstPersonController>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(directory, FileMode.Open);
            playerData check = (playerData)bf.Deserialize(file);
            file.Close();
            keybindings.AddRange(check.keyBindings);
            foreach (var item in keyBindButtons)
            {
                foreach (var item2 in keybindings)
                {
                    if (item2.keyBindName == item.name)
                    {
                        item.GetComponentInChildren<Text>().text = item.name + ": " + keyName(item2.keybindData);
                        PlayerPrefs.SetString(item.name, item2.keybindData);
                    }
                }
            }
            yield return new WaitForEndOfFrame();
            if(check.userName == PlayerPrefs.GetString("curUser"))
            {
                player.transform.position = check.playerPostion;
                player.transform.rotation = check.playerRotation;
                quarterCount = check.quarterCount;
                storedPlayTime = check.playTime;
            }
            yield return new WaitForEndOfFrame();
            curHeliLevel = check.level;
            Debug.LogError(check.level.ToString());
            heliscore = check.score;
            GameState = (gameState)System.Enum.Parse(typeof(gameState), check.polybiosState);
            yield return new WaitForEndOfFrame();
            Setup();
        }
        else
        {
            callBackLogger.throwError("Couldn't find player");
            File.Delete(Application.persistentDataPath + "/saves/" + PlayerPrefs.GetString("curUser") + ".save");
            SceneManager.LoadScene("MainMenu");
        }
    }
    IEnumerator saveGame(bool returnToMenu)
    {
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(directory);
        playerData saveData = new playerData();
        yield return new WaitForEndOfFrame();
        saveData.playerPostion = player.transform.position;
        saveData.playerRotation = player.transform.rotation;
        saveData.userName = PlayerPrefs.GetString("curUSer");
        yield return new WaitForEndOfFrame();
        saveData.quarterCount = quarterCount;
        saveData.score = heliscore;
        saveData.polybiosState = GameState.ToString();
        saveData.level = curHeliLevel;
        yield return new WaitForEndOfFrame();
        saveData.keyBindings = keybindings.ToArray();
        yield return new WaitForEndOfFrame();
        saveData.playTime = storedPlayTime + Time.timeSinceLevelLoad;
        yield return new WaitForEndOfFrame();
        bf.Serialize(file, saveData);
        file.Close();
        yield return new WaitForEndOfFrame();
        if (returnToMenu)
            SceneManager.LoadScene("MainMenu");
        else
            TogglePause();
    }
    string keyName(string inButton)
    {
        string output = null;
        if (int.TryParse(inButton, out int number))
        {
            switch (inButton)
            {
                case "0":
                    output = "Left Mouse";
                    break;
                case "1":
                    output = "Right Mouse";
                    break;
            }
        }
        else
            output = (inButton == "LeftShift") ? "Left Shift" : (inButton == "RightShift") ? "Right Shift" : inButton;
        return output;
    }
    void UpdateKeyBindUI()
    {
        foreach(var item in keyBindButtons)
        {
            foreach(var item2 in keybindings)
            {
                if(item2.keyBindName == item.name)
                {
                    item.GetComponentInChildren<Text>().text = item.name + ": " + keyName(item2.keybindData);
                    PlayerPrefs.SetString(item.name, item2.keybindData);
                }
            }
        }
    }
    public void StartReadKey(Button curButton)
    {
        isReadingForKey = true;
        curKeyBind = curButton;
        curKeyToBind = curButton.name.ToString();
    }

    public void toggleKeyBinds() => keyBindUI.SetActive(!keyBindUI.activeInHierarchy);
    public void TogglePause()
    {
        isPause = !isPause;
        tMod = (isPause) ? 0 : 1;
        pauseMenu.SetActive(isPause);
        Cursor.visible = isPause;
        Cursor.lockState = (isPause) ? CursorLockMode.None : CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (heliDisplay != null)
        {
            _qDisplay.text = quarterCount.ToString();
            if (heliObj.activeInHierarchy)
            {
                foreach (var item in soldierCountDisplay)
                {
                    item.SetActive(false);
                }
                for (int i = 1; i <= helicopterController.soldierCount; i++)
                {
                    if (helicopterController.soldierCount > 0)
                        soldierCountDisplay[i - 1].SetActive(true);
                }
                int displayTimer = (int)heligameTimer;
                heliDisplay.text = displayTimer.ToString();
                heliscoreDisplay.text = heliscore.ToString();
                if (inHeli)
                {
                    if (heligameTimer > 0)
                        heligameTimer -= Time.deltaTime;
                    else
                        FailedHeliLevel();

                    if (savedSoldierCount == heliLevels[curHeliLevel].soldierCount)
                        FinishedHeliLevel();
                }
            }
        }
    }
    public void CheckGameState()
    {
        mainCam.GetComponent<Pixelation>().BlockCount = 240;
        _music.clip = curseMusic;
        switch (GameState)
        {
            case gameState.asleep:
                _music.clip = neutralMusic;
                break;
            case gameState.active:
                _music.clip = curseMusic;
                break;
            case gameState.cursing:
                this.GetComponent<Pixelation>().BlockCount = 128;
                break;
        }
        _music.Play();
    }
    public void UpdateGameState()
    {
        switch (GameState)
        {
            case gameState.asleep:
                GameState = gameState.active;
                music.clip = curseMusic;
                break;
            case gameState.active:
                mainCam.GetComponent<Pixelation>().BlockCount = 128;
                GameState = gameState.cursing;
                break;
            case gameState.cursing:
                BadEnding();
                _fps.enabled = false;
                mainCam.gameObject.SetActive(false);
                break;
        }
    }
    public static void FirePause()
    {
        callBackLogger.sendMessage("Pressed Pause Key");
        FindObjectOfType<GameManager>().TogglePause();
    }
    public void BadEnding()
    {
        callBackLogger.sendMessage("BadEnding");
        _fps.enabled = true;
        _heli.enabled = false;
        inHeli = false;
        _fObj.SetActive(false);
        _hObj.SetActive(false);
        Destroy(FindObjectOfType<FirstPersonController>().cScreen.gameObject);
        badEndCam.gameObject.SetActive(true);
        endWatchCam.gameObject.SetActive(true);
        endActor.SetActive(true);
        mainCanvas.SetActive(false);
        endCanvas.SetActive(true);
        _music.clip = endMusic;
        _music.Play();
    }
    public static void FireStartCabinet()
    {
        if (!hasStartedHeliBefore)
        {
            hasStartedHeliBefore = true;
            FindObjectOfType<GameManager>().UpdateGameState();
            FindObjectOfType<GameManager>().LaunchInitialLevel();
        }
        callBackLogger.sendMessage("StartedCabinet");
        _heli.enabled = true;
        _fps.enabled = false;
        _hObj.SetActive(true);
        _fObj.SetActive(false);
        inHeli = true;
        _music.clip = FindObjectOfType<GameManager>().curseMusic;
        _music.Play();
    }
    public static void FireEndCabinet()
    {
        callBackLogger.sendMessage("EndedCabinet");
        _fps.enabled = true;
        _heli.enabled = false;
        inHeli = false;
        _fObj.SetActive(true);
        _hObj.SetActive(false);
        FindObjectOfType<GameManager>().CheckGameState();
    }
    public static void SavedSoldier()
    {
        heliscore += (int)heligameTimer;
        savedSoldierCount++;
        Debug.LogError(savedSoldierCount + " soldiers saved");
    }
    public void LaunchInitialLevel()
    {
        savedSoldierCount = 0;
        Destroy(levelDisplay);
        levelDisplay = Instantiate(heliLevels[curHeliLevel].levelShow, levelHolder.transform);
        heligameTimer = 120;
    }
    public void FinishedHeliLevel()
    {
        savedSoldierCount = 0;
        Destroy(levelDisplay);
        curHeliLevel++;
        callBackLogger.sendMessage(curHeliLevel.ToString());
        if (curHeliLevel < 10)
        {
            if (curHeliLevel == 4)
                UpdateGameState();
            levelDisplay = Instantiate(heliLevels[curHeliLevel].levelShow, levelHolder.transform);
            heligameTimer = 120;
        }
        else
        {
            UpdateGameState();
        }
    }
    public void FailedHeliLevel()
    {
        savedSoldierCount = 0;
        Destroy(levelDisplay);
        levelDisplay = Instantiate(heliLevels[curHeliLevel].levelShow, levelHolder.transform.position, levelHolder.transform.rotation);
        heligameTimer = 120;
        heliscore = 0;
        FireEndCabinet();
    }
    private void OnGUI()
    {
        Event e = Event.current;
        if (isReadingForKey)
        {
            if(e.isKey)
            {
                isReadingForKey = false;
                string outputCheck = e.keyCode.ToString();
                foreach(var item in keybindings)
                {
                    if (item.keyBindName == curKeyToBind)
                        item.keybindData = outputCheck;
                }
                UpdateKeyBindUI();
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                isReadingForKey = false;
                string outputCheck = Input.GetKey(KeyCode.LeftShift) ? "LeftShift" : "RightShift";
                foreach(var item in keybindings)
                {
                    if (item.keyBindName == curKeyToBind)
                        item.keybindData = outputCheck;
                }
                UpdateKeyBindUI();
            }
        }
    }
    #region variable data/code
    public static bool HasQuarter()
    {
        bool output = false;
        if (quarterCount > 0)
            output = true;
        return output;
    }
    #endregion
    #region FX
    #region VFX
    public static void FireGlitchEffect()
    {
        callBackLogger.sendMessage("Firing Glitch VFX");
    }
    #endregion
    #region SFX
    public static void FireAudioLine(AudioClip inClip)
    {
        callBackLogger.sendMessage("Firing Audio Line");
        _vl.clip = inClip;
        _vl.Play();
    }
    public static void FireAudioBit(AudioClip inClip)
    {
        callBackLogger.sendMessage("Firing Audio Effect");
        _SFX.clip = inClip;
        _SFX.Play();
    }
    #endregion
    #endregion
    #region enums
    public enum gameState
    {
        asleep,
        active,
        cursing
    };
    #endregion
}
#region Externals
[System.Serializable]
public class keyBindingData
{
    public string keyBindName, keybindData;
}
[System.Serializable]
public struct SVector3
{
    public float x, y, z;
    public SVector3(float X, float Y, float Z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public override string ToString()
    {
        return System.String.Format("[{0},{1},{2}]", x, y, z);
    }
    public static implicit operator Vector3(SVector3 iValue)
    {
        return new Vector3(iValue.x, iValue.y, iValue.z);
    }
    public static implicit operator SVector3(Vector3 iValue)
    {
        return new SVector3(iValue.x, iValue.y, iValue.z);
    }
}
[System.Serializable]
public struct SQuat
{
    public float x, y, z, w;
    public SQuat(float X, float Y, float Z, float W)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
    }
    public override string ToString()
    {
        return System.String.Format("[{0},{1},{2},{3}]", x, y, z, w);
    }
    public static implicit operator Quaternion(SQuat iValue)
    {
        return new Quaternion(iValue.x, iValue.y, iValue.z, iValue.w);
    }
    public static implicit operator SQuat(Quaternion iVale)
    {
        return new SQuat(iVale.x, iVale.y, iVale.z, iVale.w);
    }
}
[System.Serializable]
public class playerData
{
    public string userName, polybiosState;
    public double playTime;
    public int quarterCount, score, level;
    public SVector3 playerPostion;
    public SQuat playerRotation;
    public keyBindingData[] keyBindings;
}
[System.Serializable]
public struct heliLevel
{
    public int soldierCount;
    public GameObject levelShow;
}
#endregion
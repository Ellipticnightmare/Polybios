using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Pixelation.Scripts;

public class GameManager : MonoBehaviour
{
    #region Variables
    #region Accessible
    public static bool isController = false;
    public static int tMod = 1;
    public static int quarterCount = 0;
    public Camera badEndCam;
    public AudioSource SFX, music, vLine;
    public MonoBehaviour fpsControl, heliControl;
    public AudioClip neutralMusic, curseMusic;
    #region UI
    public Text quarterDisplay;
    public GameObject heliObj, fpsObj;
    #endregion
    #endregion
    #region Protected
    static bool isPause;
    static AudioSource _SFX, _music, _vl;
    static MonoBehaviour _fps, _heli;
    gameState GameState = gameState.asleep;
    #region UI
    static Text _qDisplay;
    static GameObject _hObj, _fObj;
    #endregion
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _SFX = SFX;
        _music = music;
        _vl = vLine;
        _fps = fpsControl;
        _heli = heliControl;
        _qDisplay = quarterDisplay;
        _hObj = heliObj;
        _fObj = fpsObj;
        CheckGameState();
    }

    // Update is called once per frame
    void Update()
    {
        quarterDisplay.text = quarterCount.ToString();
    }
    public void CheckGameState()
    {
        this.GetComponent<Pixelation>().BlockCount = 240;
        music.clip = curseMusic;
        switch (GameState)
        {
            case gameState.asleep:
                music.clip = neutralMusic;
                break;
            case gameState.active:
                break;
            case gameState.influencing:
                this.GetComponent<Pixelation>().BlockCount = 128;
                break;
            case gameState.cursing:
                this.GetComponent<Pixelation>().BlockCount = 128;
                break;
        }
        music.Play();
    }
    public void UpdateGameState()
    {
        switch (GameState)
        {
            case gameState.asleep:
                GameState = gameState.active;
                break;
            case gameState.active:
                GameState = gameState.influencing;
                music.clip = curseMusic;
                break;
            case gameState.influencing:
                this.GetComponent<Pixelation>().BlockCount = 128;
                break;
            case gameState.cursing:
                BadEnding();
                _fps.enabled = false;
                Camera.main.gameObject.SetActive(false);
                break;
        }
    }
    public static void FirePause()
    {
        callBackLogger.sendMessage("Pressed Pause Key");
        isPause = !isPause;
        tMod = (isPause) ? 0 : 1;
    }
    public void BadEnding()
    {
        callBackLogger.sendMessage("BadEnding");
        tMod = 0;
        badEndCam.gameObject.SetActive(true);
    }
    public static void FireStartCabinet()
    {
        callBackLogger.sendMessage("StartedCabinet");
        _heli.enabled = true;
        _fps.enabled = false;
        _hObj.SetActive(true);
        _fObj.SetActive(false);
    }
    public static void FireEndCabinet()
    {
        callBackLogger.sendMessage("EndedCabinet");
        quarterCount--;
        _fps.enabled = true;
        _heli.enabled = false;
        _fObj.SetActive(true);
        _hObj.SetActive(false);
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
        influencing,
        cursing
    };
    #endregion
}
#region Externals
[System.Serializable]
public struct keyBindingData
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
#endregion
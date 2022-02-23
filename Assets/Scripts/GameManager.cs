using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    #region Accessible
    public static bool isController = false;
    public static int tMod = 1;
    public Camera badEndCam;
    #endregion
    #region Protected
    bool isPause;
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FirePause();
        }
    }
    public void FirePause()
    {
        callBackLogger.sendMessage("Pressed Pause Key");
        isPause = !isPause;
        tMod = (isPause) ? 0 : 1;
    }
    public void BadEnding()
    {
        callBackLogger.sendMessage("BadEnding");
        tMod = 0;
        Camera.main.gameObject.SetActive(false);
        FindObjectOfType<FirstPersonController>().enabled = false;
        badEndCam.gameObject.SetActive(true);
    }

    #region VFX
    public void FireGlitchEffect()
    {
        callBackLogger.sendMessage("Firing Glitch VFX");
    }
    #endregion
}
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
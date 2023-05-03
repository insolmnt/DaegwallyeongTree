using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public UniversalMediaPlayer Player;
    public SettingRawImage CamRawImage;

    [Header("Data")]
    public CamData Data;

    private void Start()
    {
        Load();
        CamRawImage.Load();

        Player.Path = string.Format("rtsp://{1}:{2}@{0}:554/cam/realmonitor?channel=1&subtype=0", Data.Ip, Data.Id, Data.Pass);
        Player.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CamRawImage.ShowKeystoneSetting(!CamRawImage.IsShowKeystoneSetting);
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        DataManager.SetDataSecurity("Cam", Data);
    }
    public void Load()
    {
        Data = DataManager.GetDataSecurity<CamData>("Cam");
        if(Data == null)
        {
            Data = new CamData();
        }
    }
}

[System.Serializable]
public class CamData
{
    public string Ip = "192.168.0.49";
    public string Id = "admin";
    public string Pass = "admin";
}
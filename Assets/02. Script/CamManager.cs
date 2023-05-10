using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    static public CamManager Instance;

    public UniversalMediaPlayer Player;
    public SettingRawImage CamRawImage;

    [Header("Data")]
    public CamData Data;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Load();
        Save();

        CamRawImage.Load();

        string path = string.Format("rtsp://{1}:{2}@{0}:554/cam/realmonitor?channel=1&subtype=0", Data.Ip, Data.Id, Data.Pass); ;
        Player.Path = path;
        Player.Play();

        Debug.Log("PATH : " + path);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CamRawImage.ShowKeystoneSetting(!CamRawImage.IsShowKeystoneSetting);
        }

        if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.ScrollLock) || Input.GetKeyDown(KeyCode.SysReq))
        {
            StartCoroutine(Capture());
        }
    }

    public IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);
        screenTex.ReadPixels(area, 0, 0);
        //screenTex.LoadImage(screenTex.EncodeToPNG());

        var bytes = screenTex.EncodeToPNG();
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\캡처_" + System.DateTime.Now.ToString("yyMMdd_HHmmss") + "_전체.png";
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("Saved to " + path);


        var image = CamRawImage.Raw.texture;
        RenderTexture current = RenderTexture.active;
        RenderTexture tex = new RenderTexture(image.width, image.height, 0);

        Graphics.Blit(image, tex);
        RenderTexture.active = tex;

        var texture2D = new Texture2D(image.width, image.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, image.width, image.height), 0, 0);
        texture2D.Apply();
        bytes = texture2D.EncodeToPNG();
        path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\캡처_" + System.DateTime.Now.ToString("yyMMdd_HHmmss") + "_카메라.png";
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("Saved to " + path);


        UiManager.Instance.ShowMessage("스크린샷 저장 완료 (바탕화면)");
    }

    [ContextMenu("Save")]
    public void Save()
    {
        DataManager.SetData("Cam", Data);
    }
    [ContextMenu("Load")]
    public void Load()
    {
        Data = DataManager.GetData<CamData>("Cam");
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
    public string Pass = "123456789z";
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    static public UiManager Instance;


    public SettingSun SettingSun;
    public SoundManager SettingSound;
    public SettingRawImage SettingImage;
    public ShutdownManager SettingShutdown;
    public SeasonTree SettingTree;

    public Text MsgText;
    public Text DebugText;

    public PlayableDirector MainTimeline;
    public SeasonTree Manager;

    public GameObject[] ObjectList;


    [Header("Data")]
    public int ShowFps = 0;
    public int Fps = 0;
    public float FpsTime = 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;

        Application.wantsToQuit += ApplicationQuit;
    }
    private bool ApplicationQuit()
    {
        Quit();
        return false;
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }


    public void ShowMessage(string str)
    {
        CancelInvoke("HideMessage");
        MsgText.transform.parent.gameObject.SetActive(true);
        MsgText.text = str;

        Invoke("HideMessage", 3f);
    }

    private void HideMessage()
    {
        MsgText.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        FpsTime += Time.deltaTime;
        Fps++;
        if(FpsTime >= 1)
        {
            FpsTime -= 1;
            ShowFps = Fps;
            Fps = 0;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            for (int i = 0; i < ObjectList.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    ObjectList[i].gameObject.SetActive(!ObjectList[i].gameObject.activeSelf);
                    ShowMessage(ObjectList[i].name + " : " + (ObjectList[i].gameObject.activeSelf ? "ON" : "OFF"));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MsgBox.IsShow)
            {
                MsgBox.Close();
            }
            else if (SettingShutdown.IsSettingView)
            {
                SettingShutdown.ShowSettingPanel(false);
            }
            else if (SettingSun.IsShowSetting)
            {
                SettingSun.ShowSetting(false);
            }
            else if (SettingSound.IsShowSetting)
            {
                SettingSound.ShowSetting(false);
            }
            else if (SettingImage.IsShowKeystoneSetting)
            {
                SettingImage.ShowKeystoneSetting(false);
            }
            else if(SettingTree.IsShowSetting)
            {
                SettingTree.ShowSetting(false);
            }
            else
            {
                MsgBox.Show("프로그램을 종료하시겠습니까?").SetButtonType(MsgBoxButtons.OK_CANCEL).OnResult((result) =>
                {
                    switch (result)
                    {
                        case DialogResult.YES_OK:
                            Quit();
                            break;
                    }
                });
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (MsgBox.IsShow)
            {
                MsgBox._lastMsgBox.ButtonClickEvent((int)DialogResult.YES_OK);
            }
        }


        if (Input.GetKeyDown(KeyCode.F12))
        {
            DebugText.gameObject.SetActive(!DebugText.gameObject.activeSelf);
        }

        if (DebugText.gameObject.activeSelf && Manager.CurrentSeason >= 0)
        {
            DebugText.text = "FPS : " + ShowFps + "\n" + MainTimeline.time.ToString("F1") + " / " + MainTimeline.duration.ToString("F1") + "\n"
                + ((SeasonType)(Manager.CurrentSeason)).ToString() + " - " + Manager.CurrentSeasonTime.ToString("F1");
        }


        if (CamManager.Instance.CamRawImage.IsShowKeystoneSetting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(MainTimeline.state == PlayState.Paused)
            {
                MainTimeline.Play();
            }
            else
            {
                MainTimeline.Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MainTimeline.time -= 5f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MainTimeline.time += 5f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Time.timeScale = 20f;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            Time.timeScale = 0.5f;
        } 
        else if(Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
    }

    public void SetTime(float time)
    {
        MainTimeline.time = time;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource BgmSource;
    public AudioSource EnvSource;
    public AudioSource RainSource;
    public AudioSource TigerSource;
    public AudioSource TreeSource;


    public GameObject SettingPanel;

    public SliderCtr BgmVolumeSlider;
    public SliderCtr EnvVolumeSlider;
    public SliderCtr RainVolumeSlider;
    public SliderCtr TigerVolumeSlider;
    public SliderCtr TreeVolumeSlider;
    [Header("Data")]
    public SoundData Data = new SoundData();
    public bool IsShowSetting = false;

    private void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKeyDown(KeyCode.F3))
        {
            ShowSetting(!IsShowSetting);
        }
        if (IsShowSetting == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowSetting(false);
        }
    }
    public void Save()
    {
        DataManager.SetData("Sound", Data);
    }
    private bool mIsLoad = false;
    public void Load()
    {
        Data = DataManager.GetData<SoundData>("Sound");
        if (Data == null)
        {
            Data = new SoundData();
        }

        SetData();
    }

    public void ShowSetting(bool isShow)
    {
        SettingPanel.gameObject.SetActive(isShow);
        IsShowSetting = isShow;
        if (isShow)
        {
            mIsLoad = false;
            BgmVolumeSlider.Val = Data.BgmVolume;
            EnvVolumeSlider.Val = Data.EnvVolume;
            RainVolumeSlider.Val = Data.RainVolume;
            TigerVolumeSlider.Val = Data.TigerVolume;
            TreeVolumeSlider.Val = Data.TreeVolume;

            mIsLoad = true;
        }
        else
        {
            Save();
        }
    }

    public void OnSettingSliderChange()
    {
        if (mIsLoad == false)
        {
            return;
        }

        Data.BgmVolume = BgmVolumeSlider.Val;
        Data.EnvVolume = EnvVolumeSlider.Val;
        Data.RainVolume = RainVolumeSlider.Val; 
        Data.TigerVolume = TigerVolumeSlider.Val;
        Data.TreeVolume = TreeVolumeSlider.Val;
        SetData();

    }
    public void SetData()
    {
        BgmSource.volume = Data.BgmVolume;
        EnvSource.volume = Data.EnvVolume;
        RainSource.volume = Data.RainVolume;
        TigerSource.volume = Data.TigerVolume;
        TreeSource.volume = Data.TreeVolume;
    }
}


[System.Serializable]
public class SoundData
{
    public float BgmVolume = 0.5f;
    public float EnvVolume = 0.3f;
    public float RainVolume = 0.2f;
    public float TigerVolume = 1f;
    public float TreeVolume = 0.5f;
}
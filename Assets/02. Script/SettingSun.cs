using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSun : MonoBehaviour
{
    public SunManager Manager;

    public GameObject SettingPanel;
    public Text DateTimeText;

    public SliderCtr RotationSlider;
    public SliderCtr TimeOffsetSlider;

    public Image ShadowColorImage;


    [Header("Data")]
    public bool IsShowSetting = false;

    private bool mIsLoad = false;
    public void ShowSetting(bool isShow)
    {
        IsShowSetting = isShow;

        SettingPanel.gameObject.SetActive(isShow);

        if (isShow)
        {
            mIsLoad = false;

            RotationSlider.Val = Manager.Data.Rotation;
            TimeOffsetSlider.Val = Manager.Data.OffsetMinute;
            ShadowColorImage.color = Manager.Data.ShadowColor;

            mIsLoad = true;
        }
        else
        {
            Manager.Save();
        }
    }

    public void OnDataChange()
    {
        if(mIsLoad == false)
        {
            return;
        }

        Manager.Data.Rotation = RotationSlider.Val;
        Manager.Data.OffsetMinute = (int)TimeOffsetSlider.Val;

        Manager.SetRotation();
        Manager.Ca();
    }

    public void OnShadowColorButtonClick()
    {
        ColorSetting.Instance.ShowSetting("그림자 색상 설정", Manager.Data.ShadowColor, true, (color) =>
        {
            Manager.Data.ShadowColor = color;
            Manager.SetSahdowColor();
            ShadowColorImage.color = Manager.Data.ShadowColor;
        });
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShowSetting(!IsShowSetting);
        }
        if(IsShowSetting == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowSetting(false);
        }

        DateTimeText.text = DateTime.Now.AddMinutes(Manager.Data.OffsetMinute).ToString("[yyyy-MM-dd] HH:mm:ss");
    }
}

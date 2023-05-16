using Fenderrio.ImageWarp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SettingRawImage : MonoBehaviour
{
    public string Section;
    public RawImageWarp Raw;
    public List<RawImageWarp> SubRaw;


    public CanvasGroup KeystoneSettingPanel;

    [Header("우상, 우하, 좌하, 좌상")]
    public RectTransform[] KeystoneArrowList;
    public RectTransform[] BezierHandleList;
    public UILineRenderer[] LineList;

    public Toggle OnlyStraightToggle;
    public Slider ScaleSlider;

    public Toggle FlipXToggle;
    public Toggle FlipYToggle;

    public Slider ViewRectXSlider;
    public Slider ViewRectWidthSlider;
    public Slider ViewRectYSlider;
    public Slider ViewRectHeightSlider;


    public Texture2D LineTexture;

    [Header("Data")]
    public RawImageData Data;
    private Texture CurrentTexture;
    private bool IsShowLine = false;
    public bool IsShowKeystoneSetting = false;



    [ContextMenu("Save")]
    public void Save()
    {
        DataManager.SetData(Section, Data);
    }
    [ContextMenu("Load")]
    public void Load()
    {
        Data = DataManager.GetData<RawImageData>(Section);
        if(Data == null)
        {
            Data = new RawImageData();
        }
        SetKeystone();
        SetViewRect();
    }

    public void OnFlipXToggleChange()
    {
        Data.FlipX = FlipXToggle.isOn;
        Raw.flipX = Data.FlipX;
    }
    public void OnFlipYToggleChange()
    {
        Data.FlipY = FlipYToggle.isOn;
        Raw.flipY = Data.FlipY;
    }

    public void OnViewRectSliderChange()
    {
        if(mIsLoad == false)
        {
            return;
        }


        Data.ViewRect.x = ViewRectXSlider.value;
        Data.ViewRect.y = ViewRectYSlider.value;
        Data.ViewRect.width = ViewRectWidthSlider.value;
        Data.ViewRect.height = ViewRectHeightSlider.value;
        SetViewRect();
    }

    public void ShowLine(bool isShow)
    {
        IsShowLine = isShow;
        if (isShow)
        {
            CurrentTexture = Raw.texture;
            Raw.texture = LineTexture;
        }
        else if(CurrentTexture != null)
        {
            Raw.texture = CurrentTexture;
        }
    }

    public void SetViewRect()
    {
        Raw.uvRect = Data.ViewRect;
        foreach(var sub in SubRaw)
        {
            sub.uvRect = Data.ViewRect;
        }
    }
    public void SetKeystone()
    {
        Raw.flipX = Data.FlipX;
        Raw.flipY = Data.FlipY;

        Raw.cornerOffsetTR = Data.PointList[0];
        Raw.cornerOffsetBR = Data.PointList[1];
        Raw.cornerOffsetBL = Data.PointList[2];
        Raw.cornerOffsetTL = Data.PointList[3];

        float canvasWidth = Raw.rectTransform.rect.width;
        //float canvasWidth = 800f / MonitorCanvas.re.y * MonitorCanvas.renderingDisplaySize.x;
        float canvasHeight = Raw.rectTransform.rect.height;
        //Debug.Log("캔버스 : " + canvasWidth + ", " + canvasHeight);

        //Top A왼 B오
        //Right A위 B아래
        //Bottom A오 B왼
        //Left A아래  B위

        var top = new Vector3(
            (canvasWidth + Raw.cornerOffsetTR.x - Raw.cornerOffsetTL.x) / 3f,
            (Raw.cornerOffsetTR.y - Raw.cornerOffsetTL.y) / 3f,
            0);
        var left = new Vector3(
            (Raw.cornerOffsetTL.x - Raw.cornerOffsetBL.x) / 3f,
            (canvasHeight + Raw.cornerOffsetTL.y - Raw.cornerOffsetBL.y) / 3f,
            0);
        var right = new Vector3(
            (Raw.cornerOffsetBR.x - Raw.cornerOffsetTR.x) / 3f,
            -(canvasHeight + Raw.cornerOffsetTR.y - Raw.cornerOffsetBR.y) / 3f,
            0);
        var bottom = new Vector3(
            -(canvasWidth + Raw.cornerOffsetBR.x - Raw.cornerOffsetBL.x) / 3f,
            -(Raw.cornerOffsetBR.y - Raw.cornerOffsetBL.y) / 3f,
            0);


        if (Data.isOnltyStraight)
        {
            Raw.topBezierHandleA = (Vector2)top * Data.StraightDataList[0];
            Raw.topBezierHandleB = -(Vector2)top * Data.StraightDataList[1];

            Raw.leftBezierHandleA = (Vector2)left * Data.StraightDataList[6];
            Raw.leftBezierHandleB = -(Vector2)left * Data.StraightDataList[7];

            Raw.rightBezierHandleA = (Vector2)right * Data.StraightDataList[2];
            Raw.rightBezierHandleB = -(Vector2)right * Data.StraightDataList[3];

            Raw.bottomBezierHandleA = (Vector2)bottom * Data.StraightDataList[4];
            Raw.bottomBezierHandleB = -(Vector2)bottom * Data.StraightDataList[5];
        }
        else
        {
            Raw.topBezierHandleA = (Vector2)top + Data.BezierList[0];
            Raw.topBezierHandleB = -(Vector2)top + Data.BezierList[1];

            Raw.leftBezierHandleA = (Vector2)left + Data.BezierList[6];
            Raw.leftBezierHandleB = -(Vector2)left + Data.BezierList[7];

            Raw.rightBezierHandleA = (Vector2)right + Data.BezierList[2];
            Raw.rightBezierHandleB = -(Vector2)right + Data.BezierList[3];

            Raw.bottomBezierHandleA = (Vector2)bottom + Data.BezierList[4];
            Raw.bottomBezierHandleB = -(Vector2)bottom + Data.BezierList[5];
        }


        foreach(var sub in SubRaw)
        {
            sub.cornerOffsetTR = Raw.cornerOffsetTR;
            sub.cornerOffsetBR = Raw.cornerOffsetBR;
            sub.cornerOffsetBL = Raw.cornerOffsetBL;
            sub.cornerOffsetTL = Raw.cornerOffsetTL;

            sub.topBezierHandleA = Raw.topBezierHandleA;
            sub.topBezierHandleB = Raw.topBezierHandleB;

            sub.leftBezierHandleA = Raw.leftBezierHandleA;
            sub.leftBezierHandleB = Raw.leftBezierHandleB;

            sub.rightBezierHandleA = Raw.rightBezierHandleA;
            sub.rightBezierHandleB = Raw.rightBezierHandleB;

            sub.bottomBezierHandleA = Raw.bottomBezierHandleA;
            sub.bottomBezierHandleB = Raw.bottomBezierHandleB;
        }
    }



    private bool mIsLoad = false;
    public void ShowKeystoneSetting(bool isShow)
    {
        keyCheck = false;
        keyDownTime = 0;

        KeystonePointDown = -1;
        KeystoneBezierPointDown = -1;
        KeystoneBezierPointDownX = -1;
        KeystoneBezierPointDownY = -1;


        //MultiDisplayManagerUI.Instance.CheckKeystoneAlpha();
        if (isShow)
        {
            mIsLoad = false;
            gameObject.SetActive(true); //Monitor.IsUse
            KeystoneSettingPanel.alpha = 1;
            //OutputRawImagePrefab.gameObject.SetActive(); //Monitor.IsUse
            OnlyStraightToggle.isOn = Data.isOnltyStraight;
            FlipXToggle.isOn = Data.FlipX;
            FlipYToggle.isOn = Data.FlipY;

            ViewRectXSlider.value = Data.ViewRect.x;
            ViewRectYSlider.value = Data.ViewRect.y;
            ViewRectWidthSlider.value = Data.ViewRect.width;
            ViewRectHeightSlider.value = Data.ViewRect.height;

            mIsLoad = true;
            SetButtonText();
            SetData();
            SetViewRect();
        }
        else
        {
            ShowLine(false);
            gameObject.SetActive(false);
            Save();
        }

        IsShowKeystoneSetting = isShow;
    }



    public void SetData()
    {
        for (int i = 0; i < KeystoneArrowList.Length; i++)
        {
            KeystoneArrowList[i].anchoredPosition = Data.PointList[i];
        }

        BezierHandleList[0].anchoredPosition = Raw.topBezierLocalPositionHandleA;
        BezierHandleList[1].anchoredPosition = Raw.topBezierLocalPositionHandleB;
        BezierHandleList[2].anchoredPosition = Raw.rightBezierLocalPositionHandleA;
        BezierHandleList[3].anchoredPosition = Raw.rightBezierLocalPositionHandleB;
        BezierHandleList[4].anchoredPosition = Raw.bottomBezierLocalPositionHandleA;
        BezierHandleList[5].anchoredPosition = Raw.bottomBezierLocalPositionHandleB;
        BezierHandleList[6].anchoredPosition = Raw.leftBezierLocalPositionHandleA;
        BezierHandleList[7].anchoredPosition = Raw.leftBezierLocalPositionHandleB;


        if (LineList != null && LineList.Length > 0)
        {
            LineList[0].Points = new Vector2[] { BezierHandleList[0].anchoredPosition, Raw.cornerLocalPositionTL };
            LineList[1].Points = new Vector2[] { BezierHandleList[1].anchoredPosition, Raw.cornerLocalPositionTR };
            LineList[2].Points = new Vector2[] { BezierHandleList[2].anchoredPosition, Raw.cornerLocalPositionTR };
            LineList[3].Points = new Vector2[] { BezierHandleList[3].anchoredPosition, Raw.cornerLocalPositionBR };
            LineList[4].Points = new Vector2[] { BezierHandleList[4].anchoredPosition, Raw.cornerLocalPositionBR };
            LineList[5].Points = new Vector2[] { BezierHandleList[5].anchoredPosition, Raw.cornerLocalPositionBL };
            LineList[6].Points = new Vector2[] { BezierHandleList[6].anchoredPosition, Raw.cornerLocalPositionBL };
            LineList[7].Points = new Vector2[] { BezierHandleList[7].anchoredPosition, Raw.cornerLocalPositionTL };
        }
    }

    public void OnStraightToggleChange()
    {
        if (IsShowKeystoneSetting == false)
        {
            return;
        }
        Data.isOnltyStraight = OnlyStraightToggle.isOn;
        SetButtonText();


        SetKeystone();
        Invoke("SetData", 0.1f);
    }

    private void SetButtonText()
    {
        if (Data.isOnltyStraight) //IJKL
        {
            BezierHandleList[0].GetComponentInChildren<Text>().text = "Q+JL";
            BezierHandleList[1].GetComponentInChildren<Text>().text = "W+JL";
            BezierHandleList[2].GetComponentInChildren<Text>().text = "W+IK";
            BezierHandleList[3].GetComponentInChildren<Text>().text = "S+IK";
            BezierHandleList[4].GetComponentInChildren<Text>().text = "S+JL";
            BezierHandleList[5].GetComponentInChildren<Text>().text = "A+JL";
            BezierHandleList[6].GetComponentInChildren<Text>().text = "A+IK";
            BezierHandleList[7].GetComponentInChildren<Text>().text = "Q+IK";
        }
        else
        {
            BezierHandleList[0].GetComponentInChildren<Text>().text = "4";
            BezierHandleList[1].GetComponentInChildren<Text>().text = "5";
            BezierHandleList[2].GetComponentInChildren<Text>().text = "T";
            BezierHandleList[3].GetComponentInChildren<Text>().text = "G";
            BezierHandleList[4].GetComponentInChildren<Text>().text = "V";
            BezierHandleList[5].GetComponentInChildren<Text>().text = "C";
            BezierHandleList[6].GetComponentInChildren<Text>().text = "D";
            BezierHandleList[7].GetComponentInChildren<Text>().text = "E";
        }
    }

    public void OnScaleSliderChange()
    {
        KeystoneSettingPanel.transform.localScale = Vector3.one * ScaleSlider.value;
    }


    private void Clear()
    {
        Data.PointList = new Vector2[]
        {
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero
        };

        Data.BezierList = new Vector2[]
        {
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero
        };
        Data.StraightDataList = new float[]
        {
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1
        };

        SetKeystone();
        Invoke("SetData", 0.1f);
    }


    private int KeystoneBezierPointDown = -1;
    private Vector2 BezierPointDownMousePosition;
    private Vector2 BezierPointDownData;
    //private Vector2 CurrentMousePoint;

    public void OnBezierKeystoneArrowPointDown(int index)
    {
        keyCheck = false;
        keyDownTime = 0;
        if (Input.GetMouseButton(0))
        {
            KeystoneBezierPointDown = index;
            BezierPointDownMousePosition = Input.mousePosition;

            if (Data.isOnltyStraight)
            {
                BezierPointDownData = new Vector2(Data.StraightDataList[index], Data.StraightDataList[index]);
            }
            else
            {
                BezierPointDownData = Data.BezierList[index];
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (Data.isOnltyStraight)
            {
                Data.StraightDataList[index] = 1;
            }
            else
            {
                Data.BezierList[index] = Vector2.zero;
            }
            SetKeystone();
            Invoke("SetData", 0.1f);
        }
    }


    private int KeystoneBezierPointDownX = -1;
    public void OnBezierKeystoneArrowPointDownX(int index)
    {
        keyCheck = false;
        keyDownTime = 0;
        if (Input.GetMouseButton(0))
        {
            KeystoneBezierPointDownX = index;
            BezierPointDownMousePosition = Input.mousePosition;
            BezierPointDownData = Data.BezierList[index];
        }

        if (Input.GetMouseButton(1))
        {
            Data.BezierList[index] = new Vector2(0, Data.BezierList[index].y);
            SetKeystone();
            Invoke("SetData", 0.1f);
        }
    }

    private int KeystoneBezierPointDownY = -1;
    public void OnBezierKeystoneArrowPointDownY(int index)
    {
        if (Input.GetMouseButton(0))
        {
            KeystoneBezierPointDownY = index;
            BezierPointDownMousePosition = Input.mousePosition;
            BezierPointDownData = Data.BezierList[index];
        }

        if (Input.GetMouseButton(1))
        {
            Data.BezierList[index] = new Vector2(Data.BezierList[index].x, 0);
            SetKeystone();
            Invoke("SetData", 0.1f);
        }
    }

    private int KeystonePointDown = -1;
    private Vector2 PointDownMousePosition;
    private Vector2 PointDownArrowPosition;

    public void OnKeystoneArrowPointDown(int index)
    {
        keyCheck = false;
        keyDownTime = 0;
        if (Input.GetMouseButton(0))
        {
            KeystonePointDown = index;
            PointDownMousePosition = Input.mousePosition;
            PointDownArrowPosition = KeystoneArrowList[index].anchoredPosition;


            if (Data.isOnltyStraight)
            {
                switch (index)
                {
                    case 0:
                        BezierPointDownData = new Vector2(Data.StraightDataList[1], Data.StraightDataList[2]);
                        break;

                    case 1:
                        BezierPointDownData = new Vector2(Data.StraightDataList[4], Data.StraightDataList[3]);
                        break;

                    case 2:
                        BezierPointDownData = new Vector2(Data.StraightDataList[5], Data.StraightDataList[6]);
                        break;

                    case 3:
                        BezierPointDownData = new Vector2(Data.StraightDataList[0], Data.StraightDataList[7]);
                        break;
                }
            }


        }

        if (Input.GetMouseButton(1))
        {
            Data.PointList[index] = Vector2.zero;
            SetKeystone();
            Invoke("SetData", 0.1f);
        }
    }



    bool keyCheck = false;
    private float keyDownTime = 0;
    private void Update()
    {
        if (IsShowKeystoneSetting == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            ShowLine(!IsShowLine);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    KeystoneSettingPanel.alpha = i / 10f;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Home))
        {
            MsgBox.Show("초기화 하시겠습니까?")
                .SetButtonType(MsgBoxButtons.OK_CANCEL)
                .OnResult((result) =>
                {
                    MsgBox.Close();
                    switch (result)
                    {
                        case DialogResult.YES_OK:
                            Clear();
                            break;
                    }
                });
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowKeystoneSetting(false);
        }

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            OnlyStraightToggle.isOn = !OnlyStraightToggle.isOn;
            if (OnlyStraightToggle.isOn)
            {
                //UIManager.Instance.ShowGameMessage("직선 보정만 사용");
            }
            else
            {
                //UIManager.Instance.ShowGameMessage("곡선 보정 사용");
            }
        }
        //키스톤
        if (Input.GetKeyDown(KeyCode.W))
        {
            KeystonePointDown = 0;
            PointDownMousePosition = Input.mousePosition;
            PointDownArrowPosition = KeystoneArrowList[0].anchoredPosition;
            keyCheck = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            KeystonePointDown = -1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            KeystonePointDown = 1;
            PointDownMousePosition = Input.mousePosition;
            PointDownArrowPosition = KeystoneArrowList[1].anchoredPosition;
            keyCheck = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            KeystonePointDown = -1;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            KeystonePointDown = 2;
            PointDownMousePosition = Input.mousePosition;
            PointDownArrowPosition = KeystoneArrowList[2].anchoredPosition;
            keyCheck = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            KeystonePointDown = -1;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            KeystonePointDown = 3;
            PointDownMousePosition = Input.mousePosition;
            PointDownArrowPosition = KeystoneArrowList[3].anchoredPosition;
            keyCheck = true;
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            KeystonePointDown = -1;
        }


        //if (Input.GetKeyDown(KeyCode.PageDown))
        //{
        //    Monitor.Keystone.OnlyStraightToggle.isOn = !Monitor.Keystone.OnlyStraightToggle.isOn;
        //}


        if (KeystonePointDown >= 0)
        {
            if (keyDownTime > 0)
            {
                keyDownTime -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PointDownMousePosition -= Vector2.left;
                keyDownTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                PointDownMousePosition -= Vector2.right;
                keyDownTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PointDownMousePosition -= Vector2.up;
                keyDownTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PointDownMousePosition -= Vector2.down;
                keyDownTime = 1f;
            }

            if (keyDownTime < 0)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    PointDownMousePosition -= Vector2.left;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    PointDownMousePosition -= Vector2.right;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    PointDownMousePosition -= Vector2.up;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    PointDownMousePosition -= Vector2.down;
                }
            }




            if (Data.isOnltyStraight)
            {
                switch (KeystonePointDown)
                {
                    case 0:
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            Data.StraightDataList[1] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.J))
                        {
                            Data.StraightDataList[1] += 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            Data.StraightDataList[2] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            Data.StraightDataList[2] += 0.005f;
                            keyDownTime = 1f;
                        }
                        //if (Input.GetKeyDown(KeyCode.Keypad5))
                        //{
                        //    Data.StraightDataList[1] = 1f;
                        //    Data.StraightDataList[2] = 1f;
                        //}

                        if (keyDownTime < 0)
                        {
                            if (Input.GetKey(KeyCode.L))
                            {
                                Data.StraightDataList[1] -= 0.005f;
                            }
                            if (Input.GetKey(KeyCode.J))
                            {
                                Data.StraightDataList[1] += 0.005f;
                            }
                            if (Input.GetKey(KeyCode.I))
                            {
                                Data.StraightDataList[2] -= 0.005f;
                            }
                            if (Input.GetKey(KeyCode.K))
                            {
                                Data.StraightDataList[2] += 0.005f;
                            }
                        }
                        break;

                    case 1:
                        //BezierPointDownData = new Vector2(Data.StraightDataList[4], Data.StraightDataList[3]);
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            Data.StraightDataList[4] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.J))
                        {
                            Data.StraightDataList[4] += 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            Data.StraightDataList[3] += 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            Data.StraightDataList[3] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        //if (Input.GetKeyDown(KeyCode.Keypad5))
                        //{
                        //    Data.StraightDataList[3] = 1f;
                        //    Data.StraightDataList[4] = 1f;
                        //}

                        if (keyDownTime < 0)
                        {
                            if (Input.GetKey(KeyCode.L))
                            {
                                Data.StraightDataList[4] -= 0.005f;
                            }
                            if (Input.GetKey(KeyCode.J))
                            {
                                Data.StraightDataList[4] += 0.005f;
                            }
                            if (Input.GetKey(KeyCode.I))
                            {
                                Data.StraightDataList[3] += 0.005f;
                            }
                            if (Input.GetKey(KeyCode.K))
                            {
                                Data.StraightDataList[3] -= 0.005f;
                            }
                        }
                        break;

                    case 2:
                        //BezierPointDownData = new Vector2(Data.StraightDataList[5], Data.StraightDataList[6]);
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            Data.StraightDataList[5] += 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.J))
                        {
                            Data.StraightDataList[5] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            Data.StraightDataList[6] += 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            Data.StraightDataList[6] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        //if (Input.GetKeyDown(KeyCode.Keypad5))
                        //{
                        //    Data.StraightDataList[5] = 1f;
                        //    Data.StraightDataList[6] = 1f;
                        //}

                        if (keyDownTime < 0)
                        {
                            if (Input.GetKey(KeyCode.L))
                            {
                                Data.StraightDataList[5] += 0.005f;
                            }
                            if (Input.GetKey(KeyCode.J))
                            {
                                Data.StraightDataList[5] -= 0.005f;
                            }
                            if (Input.GetKey(KeyCode.I))
                            {
                                Data.StraightDataList[6] += 0.005f;
                            }
                            if (Input.GetKey(KeyCode.K))
                            {
                                Data.StraightDataList[6] -= 0.005f;
                            }
                        }
                        break;

                    case 3:
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            Data.StraightDataList[0] += 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.J))
                        {
                            Data.StraightDataList[0] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            Data.StraightDataList[7] -= 0.005f;
                            keyDownTime = 1f;
                        }
                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            Data.StraightDataList[7] += 0.005f;
                            keyDownTime = 1f;
                        }
                        //if (Input.GetKeyDown(KeyCode.Keypad5))
                        //{
                        //    Data.StraightDataList[7] = 1f;
                        //    Data.StraightDataList[0] = 1f;
                        //}

                        if (keyDownTime < 0)
                        {
                            if (Input.GetKey(KeyCode.L))
                            {
                                Data.StraightDataList[0] += 0.005f;
                            }
                            if (Input.GetKey(KeyCode.J))
                            {
                                Data.StraightDataList[0] -= 0.005f;
                            }
                            if (Input.GetKey(KeyCode.I))
                            {
                                Data.StraightDataList[7] -= 0.005f;
                            }
                            if (Input.GetKey(KeyCode.K))
                            {
                                Data.StraightDataList[7] += 0.005f;
                            }
                        }
                        //BezierPointDownData = new Vector2(Data.StraightDataList[0], Data.StraightDataList[7]);
                        break;
                }
            }


            KeystoneArrowList[KeystonePointDown].anchoredPosition =
                PointDownArrowPosition +
                (-PointDownMousePosition + new Vector2(Input.mousePosition.x, Input.mousePosition.y)) * (Input.GetKey(KeyCode.LeftShift) ? 0.3f : 1); ;

            KeystoneArrowList[KeystonePointDown].anchoredPosition = KeystoneArrowList[KeystonePointDown].anchoredPosition;

            Data.PointList[KeystonePointDown] = KeystoneArrowList[KeystonePointDown].anchoredPosition;

            SetKeystone();
            SetData();

            if (keyCheck == false && Input.GetMouseButton(0) == false)
            {
                KeystonePointDown = -1;
            }
        }

        if (Data.isOnltyStraight == false)
        {
            //곡선
            //"4";
            //"5";
            //"T";
            //"G";
            //"V";
            //"C";
            //"D";
            //"E";
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                KeystoneBezierPointDown = 0;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[0];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                KeystoneBezierPointDown = 1;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[1];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                KeystoneBezierPointDown = 2;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[2];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.T))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                KeystoneBezierPointDown = 3;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[3];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                KeystoneBezierPointDown = 4;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[4];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                KeystoneBezierPointDown = 5;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[5];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                KeystoneBezierPointDown = 6;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[6];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                KeystoneBezierPointDown = -1;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                KeystoneBezierPointDown = 7;
                BezierPointDownMousePosition = Input.mousePosition;
                BezierPointDownData = Data.BezierList[7];
                keyCheck = true;
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                KeystoneBezierPointDown = -1;
            }
        }





        if (KeystoneBezierPointDown >= 0)
        {
            if (keyDownTime > 0)
            {
                keyDownTime -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                BezierPointDownMousePosition -= Vector2.left;
                keyDownTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                BezierPointDownMousePosition -= Vector2.right;
                keyDownTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                BezierPointDownMousePosition -= Vector2.up;
                keyDownTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                BezierPointDownMousePosition -= Vector2.down;
                keyDownTime = 1f;
            }

            if (keyDownTime < 0)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    BezierPointDownMousePosition -= Vector2.left * 0.5f;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    BezierPointDownMousePosition -= Vector2.right * 0.5f;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    BezierPointDownMousePosition -= Vector2.up * 0.5f;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    BezierPointDownMousePosition -= Vector2.down * 0.5f;
                }
            }

            if (Data.isOnltyStraight)
            {
                switch (KeystoneBezierPointDown)
                {
                    case 0: //+x
                    case 5://+x
                        Data.StraightDataList[KeystoneBezierPointDown] =
                            BezierPointDownData.x +
                            (-BezierPointDownMousePosition.x + Input.mousePosition.x) * 0.005f;
                        break;

                    case 1: //-x
                    case 4://-x
                        Data.StraightDataList[KeystoneBezierPointDown] =
                            BezierPointDownData.x -
                            (-BezierPointDownMousePosition.x + Input.mousePosition.x) * 0.005f;
                        break;

                    case 2://+y
                    case 7://+y
                        Data.StraightDataList[KeystoneBezierPointDown] =
                            BezierPointDownData.y -
                            (-BezierPointDownMousePosition.y + Input.mousePosition.y) * 0.005f;
                        break;

                    case 3://-y
                    case 6://-y
                        Data.StraightDataList[KeystoneBezierPointDown] =
                            BezierPointDownData.y +
                            (-BezierPointDownMousePosition.y + Input.mousePosition.y) * 0.005f;
                        break;
                }
            }
            else
            {
                Data.BezierList[KeystoneBezierPointDown] =
                    BezierPointDownData +
                    (-BezierPointDownMousePosition + new Vector2(Input.mousePosition.x, Input.mousePosition.y)) / 1f * (Input.GetKey(KeyCode.LeftShift) ? 0.3f : 1);
            }


            SetKeystone();
            SetData();

            if (keyCheck == false && Input.GetMouseButton(0) == false)
            {
                KeystoneBezierPointDown = -1;
            }
        }

        if (KeystoneBezierPointDownX >= 0)
        {
            Data.BezierList[KeystoneBezierPointDownX] =
                BezierPointDownData + //0.7f
                (-BezierPointDownMousePosition + new Vector2(Input.mousePosition.x, BezierPointDownMousePosition.y)) / 1f * (Input.GetKey(KeyCode.LeftShift) ? 0.3f : 1);

            SetKeystone();
            SetData();

            if (Input.GetMouseButton(0) == false)
            {
                KeystoneBezierPointDownX = -1;
            }
        }

        if (KeystoneBezierPointDownY >= 0)
        {
            Data.BezierList[KeystoneBezierPointDownY] =
                BezierPointDownData +
                (-BezierPointDownMousePosition + new Vector2(BezierPointDownMousePosition.x, Input.mousePosition.y)) / 1f * (Input.GetKey(KeyCode.LeftShift) ? 0.3f : 1);

            SetKeystone();
            SetData();

            if (Input.GetMouseButton(0) == false)
            {
                KeystoneBezierPointDownY = -1;
            }
        }
    }
}



[System.Serializable]
public class RawImageData
{
    /// <summary>
    /// 키스톤 사용을 위한 좌표
    /// </summary>
    public Vector2[] PointList = new Vector2[]
    {
        Vector2.zero,
        Vector2.zero,
        Vector2.zero,
        Vector2.zero
    };

    /// <summary>
    /// 키스톤 곡선 사용을 위한 좌표
    /// </summary>
    public Vector2[] BezierList = new Vector2[]
    {
        Vector2.zero,
        Vector2.zero,
        Vector2.zero,
        Vector2.zero,
        Vector2.zero,
        Vector2.zero,
        Vector2.zero,
        Vector2.zero
    };


    public bool isOnltyStraight = false;


    public float[] StraightDataList = new float[]
    {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1
    };

    public bool FlipX = false;
    public bool FlipY = false;
    public Rect ViewRect = new Rect(0, 0, 1, 1);
}
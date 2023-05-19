using StartPage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;
using UnityEngine.Playables;
using static UnityEngine.ParticleSystem;

[ExecuteInEditMode]
public class SeasonTree : MonoBehaviour
{
    public Animator TreeAnimator;

    public VisualEffect[] VfxList;

    public SeasonData[] SeasonData;

    public Material TreeSnowMaterial;


    [Header("동물 타임라인")]
    public PlayableDirector TigerTimeline;
    public PlayableDirector RabbitTimeline;
    public GameObject[] RabbitList;

    public Animator SnowFloor;


    [Header("설정")]
    public GameObject SettingPanel;
    private bool mIsLoad = false;
    public bool IsShowSetting = false;
    public SliderCtr RotationSlider;
    public SliderCtr ScaleSlider;
    public SliderCtr[] CountSlider;

    [Header("Data")]
    public TreeData Data;

    public int CurrentSeason = -1;
    public float CurrentSeasonTime = 0;
    public bool IsCahnge = false;
    public float CurrentTime = 0;
    public float TargetTime = 5;
    [Range(0, 1)]
    public float TVal = 0;
    private float BeforeTVal = 0;
    public SeasonData TargetData;

    [GradientUsage(true)]
    public Gradient StartLeavesColors;
    [GradientUsage(true)]
    public Gradient StartPositionGradient;


    private void Start()
    {
        Load();
        Save();

        foreach(var data in SeasonData)
        {
            foreach(var pa in data.Particle)
            {
                pa.gameObject.SetActive(false);
            }
        }
        foreach(var vfx in VfxList)
        {
            vfx.gameObject.SetActive(false);
        }

        

        CurrentSeason = -1;
        SeasonChange(0);


    }
    public void Save()
    {
        DataManager.SetData("Tree", Data);
    }
    public void Load()
    {
        Data = DataManager.GetData<TreeData>("Tree");
        if(Data == null)
        {
            Data = new TreeData();
        }

        SetData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ShowSetting(!IsShowSetting);
        }
        CurrentSeasonTime += Time.deltaTime;
        if (RabbitList != null)
        {
            foreach (var vfx in VfxList)
            {
                for (int i = 0; i < RabbitList.Length; i++)
                {
                    vfx.SetVector3("Cube Position " + i, RabbitList[i].transform.position);
                }
            }
        }
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    SeasonChange(0);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    SeasonChange(1);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    SeasonChange(2);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    SeasonChange(3);
            //}
            if (IsCahnge)
        {
            CurrentTime += Time.deltaTime;
            if(CurrentTime >= TargetTime)
            {
                CurrentTime = TargetTime;
                IsCahnge = false;
            }

            TVal = CurrentTime / TargetTime;
        }
        CheckTval();
    }

    private void OnValidate()
    {
        CheckTval();
    }

    private void CheckTval()
    {
        if (BeforeTVal != TVal)
        {
            BeforeTVal = TVal;
            var t = TargetData.Curve.Evaluate(TVal);
            Gradient color1 = CustomUtils.GradientLerp(StartLeavesColors, TargetData.LeavesColors, t);
            Gradient color2 = CustomUtils.GradientLerp(StartPositionGradient, TargetData.PositionGradient, t);
            foreach (var vfx in VfxList)
            {
                vfx.SetGradient("Leaves Colors", color1);
                vfx.SetGradient("Position Gradient", color2);
            }
        }
    }

    public void SeasonChange(int season)
    {
        SeasonDataChange(season);

        CurrentSeasonTime = 0;
        BeforeTVal = -1;
        IsCahnge = true;
        CurrentTime = 0;
        CheckTval();
        if (SeasonChangeEvent != null)
        {
            StopCoroutine(SeasonChangeEvent);
        }
        SeasonChangeEvent = StartCoroutine(Season(season));
    }

    private Coroutine SeasonChangeEvent = null;
    private Coroutine ChangeFallingLeavesAmoutEvent = null;
    public void SeasonDataChange(int season)
    {
        if(CurrentSeason != season)
        {
            Debug.Log("계절 체인지! " + CurrentSeason + " -> " + season);
            var before = SeasonData[(season - 1 + SeasonData.Length) % SeasonData.Length];
            StartLeavesColors = before.LeavesColors;
            StartPositionGradient = before.PositionGradient;

            foreach (var pa in before.Particle)
            {
                var main = pa.main;
                main.loop = false;
            }


            TargetData = SeasonData[season];

            if (SeasonChangeEvent != null)
            {
                StopCoroutine(SeasonChangeEvent);
            }
            SeasonChangeEvent = StartCoroutine(Season(season));


            if (ChangeFallingLeavesAmoutEvent != null)
            {
                StopCoroutine(ChangeFallingLeavesAmoutEvent);
            }
            ChangeFallingLeavesAmoutEvent = StartCoroutine(ChangeFallingLeavesAmout(TargetData.FallingLeavesAmout));


            if(season != 1)
            {
                foreach (var pa in TargetData.Particle)
                {
                    var main = pa.main;
                        main.loop = true;
                    pa.gameObject.SetActive(false);
                    pa.gameObject.SetActive(true);
                    pa.Play();
                }
            }
        }
        CurrentSeason = season;

    }

    IEnumerator ChangeFallingLeavesAmout(int val)
    {
        float time = 0;

        float startVal = VfxList[0].GetFloat("Falling leaves Amount");
        float targetTime = 5f;
        while (time < targetTime)
        {
            time += Time.deltaTime;
            yield return null;

            foreach (var vfx in VfxList)
            {
                vfx.SetFloat("Falling leaves Amount", Mathf.Lerp(startVal, val, time / targetTime));
            }
        }
    }
    IEnumerator ShowVfx(VisualEffect vfx)
    {
        vfx.SetFloat("Leaves Size", 0);
        vfx.gameObject.SetActive(true);

        yield return StartCoroutine(ChangeVfxVal(vfx, "Leaves Size", 1f));
    }
    IEnumerator ChangeVfxVal(VisualEffect vfx, string id, float target, float targetTime = 5f)
    {
        float time = 0;

        float start = vfx.GetFloat(id);
        while (time < targetTime)
        {
            time += Time.deltaTime;
            yield return null;
            vfx.SetFloat(id, Mathf.Lerp(start, target, time / targetTime));
        }
        vfx.SetFloat(id, target);
    }
    //IEnumerator ChangeLeavesSize(VisualEffect vfx, float target)
    //{
    //    float time = 0;

    //    float start = vfx.GetFloat("Leaves Size");
    //    float targetTime = 5f;
    //    while (time < targetTime)
    //    {
    //        time += Time.deltaTime;
    //        yield return null;
    //        vfx.SetFloat("Leaves Size", Mathf.Lerp(start, target, time / targetTime));
    //    }
    //}

    IEnumerator DonwVfx(VisualEffect vfx)
    {
        float time = 0;
        float fadeTime = 5;
        vfx.SetBool("Drop all leaves", true);

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            yield return null;
            vfx.SetFloat("Leaves motion", Mathf.Lerp(1, 0f, time / fadeTime));
        }
    }

    IEnumerator ChangeParticleRate(ParticleSystem particle, float target, float fadeTime)
    {
        float time = 0;
        var emission = particle.emission;
        float start = emission.rateOverTime.constant;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            yield return null;

            emission.rateOverTime = new MinMaxCurve(Mathf.Lerp(start, target, time / fadeTime));
        }
    }

    IEnumerator Season(int season)
    {
        CurrentSeasonTime = 0;
        switch (season)
        {
            case 0:
                SnowFloor.gameObject.SetActive(false);
                TreeSnowMaterial.color = Color.clear;
                foreach (var vfx in VfxList)
                {
                    vfx.SetBool("Drop all leaves", false);
                    vfx.SetFloat("Leaves motion", 1);
                    vfx.SetFloat("Tunbulence Intensity", 0.3f);
                    //vfx.SetFloat("Alpha", 1f);
                    vfx.gameObject.SetActive(false);
                }

                TreeAnimator.gameObject.SetActive(false);
                TreeAnimator.gameObject.SetActive(true);


                yield return new WaitForSeconds(5f); //나무 등장 애니메이션


                foreach (var vfx in VfxList)
                {
                    StartCoroutine(ShowVfx(vfx));
                    yield return new WaitForSeconds(2f);
                }
                break;
            case 1:
                foreach(var vfx in VfxList)
                {
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(ChangeVfxVal(vfx, "Leaves Size", 1.5f));
                }

                foreach (var pa in SeasonData[0].Particle)
                {
                    pa.gameObject.SetActive(false);
                }

                yield return new WaitForSeconds(10f);
                foreach (var pa in TargetData.Particle)
                {
                    var main = pa.main;
                    main.loop = true;
                    var emission = pa.emission;
                    emission.rateOverTime = 10;
                    pa.gameObject.SetActive(false);
                    pa.gameObject.SetActive(true);
                    pa.Play();
                }
                StartCoroutine(ChangeParticleRate(TargetData.Particle[0], 100, 5));
                yield return new WaitForSeconds(15f);
                StartCoroutine(ChangeParticleRate(TargetData.Particle[0], 10, 5));
                yield return new WaitForSeconds(4f);
                foreach (var pa in TargetData.Particle)
                {
                    var main = pa.main;
                    main.loop = false;
                }
                break;


            case 2:

                foreach (var vfx in VfxList)
                {
                    StartCoroutine(ChangeVfxVal(vfx, "Collide Loss", 0f, 1f));
                }

                yield return new WaitForSeconds(7f);
                StartCoroutine(DonwVfx(VfxList[0]));
                yield return new WaitForSeconds(5f);
                StartCoroutine(DonwVfx(VfxList[1]));
                yield return new WaitForSeconds(15f);

                StartCoroutine(ChangeVfxVal(VfxList[0], "Tunbulence Intensity", 0f, 3f));
                StartCoroutine(ChangeVfxVal(VfxList[1], "Tunbulence Intensity", 0f, 3f));

                //StartCoroutine(ChangeVfxVal(VfxList[0], "Alpha", 0.5f, 3f));
                //StartCoroutine(ChangeVfxVal(VfxList[1], "Alpha", 0.5f, 3f));
                break;


            case 3:
                yield return new WaitForSeconds(10f);

                SnowFloor.gameObject.SetActive(true);

                StartCoroutine(ChangeVfxVal(VfxList[0], "Leaves Size", 0f));
                StartCoroutine(ChangeVfxVal(VfxList[1], "Leaves Size", 0f));

                StartCoroutine(ChangeVfxVal(VfxList[2], "Leaves Size", 0.4f));
                StartCoroutine(ChangeVfxVal(VfxList[3], "Leaves Size", 0.4f));
                StartCoroutine(ChangeVfxVal(VfxList[4], "Leaves Size", 0.4f));
                yield return new WaitForSeconds(3f);

                StartCoroutine(DonwVfx(VfxList[2]));
                yield return new WaitForSeconds(1f);
                StartCoroutine(DonwVfx(VfxList[3]));
                yield return new WaitForSeconds(1f);
                StartCoroutine(DonwVfx(VfxList[4]));
                yield return new WaitForSeconds(1f);

                TreeSnowMaterial.DOFade(0.7f, 10f);


                foreach (var vfx in VfxList)
                {
                    StartCoroutine(ChangeVfxVal(vfx, "Collide Loss", 0.1f, 1f));
                    StartCoroutine(ChangeVfxVal(vfx, "Tunbulence Intensity", 0.3f, 1f));
                }
                yield return new WaitForSeconds(20f);

                foreach (var pa in SeasonData[3].Particle)
                {
                    var main = pa.main;
                    main.loop = false;
                }
                yield return new WaitForSeconds(20f);
                SnowFloor.SetTrigger("Hide");

                break;

            case 4://겨울 이후
                TreeSnowMaterial.DOFade(0f, 1f);
                yield return new WaitForSeconds(1f);

                TreeAnimator.SetTrigger("Hide");
                yield return new WaitForSeconds(1.5f);//종료 애니메이션
                TreeAnimator.gameObject.SetActive(false);
                break;
        }
    }

    [ContextMenu("봄")]
    private void TestSeason0()
    {
        SeasonChange(0);
    }
    [ContextMenu("여름")]
    private void TestSeason1()
    {
        SeasonChange(1);
    }
    [ContextMenu("가을")]
    private void TestSeason2()
    {
        SeasonChange(2);
    }
    [ContextMenu("겨울")]
    private void TestSeason3()
    {
        SeasonChange(3);
    }

    private void SetSeason(int index)
    {
        SeasonData[index].LeavesColors = VfxList[0].GetGradient("Leaves Colors");
        SeasonData[index].PositionGradient = VfxList[0].GetGradient("Position Gradient");
        var particle = SeasonData[index].Particle;
        foreach(var pa in particle)
        {
            var main = pa.main;
            main.loop = true;
            pa.gameObject.SetActive(false);
            pa.gameObject.SetActive(true);
        }
    }
    [ContextMenu("현재값 봄으로")]
    private void SetSeason0()
    {
        SetSeason(0);
    }
    [ContextMenu("현재값 여름으로")]
    private void SetSeason1()
    {
        SetSeason(1);
    }
    [ContextMenu("현재값 가을로")]
    private void SetSeason2()
    {
        SetSeason(2);
    }
    [ContextMenu("현재값 겨울로")]
    private void SetSeason3()
    {
        SetSeason(3);
    }

    public void SetData()
    {
        for (int i = 0; i < 5; i++)
        {
            VfxList[i].SetFloat("Leaves Amount", Data.VfxAmount[i]);
        }

        TreeAnimator.transform.parent.localScale = Vector3.one * Data.Scale;
        TreeAnimator.transform.parent.localEulerAngles = new Vector3(0, Data.Rotation, 0);

    }
    public void ShowSetting(bool isShow)
    {
        Cursor.visible = isShow;
        IsShowSetting = isShow;
        SettingPanel.gameObject.SetActive(IsShowSetting);
        if (isShow)
        {
            mIsLoad = false;

            RotationSlider.Val = Data.Rotation;
            ScaleSlider.Val = Data.Scale;
            for (int i = 0; i < Data.VfxAmount.Length; i++)
            {
                CountSlider[i].Val = Data.VfxAmount[i];
            }

            mIsLoad = true;
        }
        else
        {
            Save();
        }
    }
    public void OnSettingChange()
    {
        if(mIsLoad == false)
        {
            return;
        }

        Data.Rotation = RotationSlider.Val;
        Data.Scale = ScaleSlider.Val;
        for(int i=0; i < Data.VfxAmount.Length; i++)
        {
            Data.VfxAmount[i] = (int)CountSlider[i].Val;
        }

        SetData();
    }

}

[System.Serializable]
public class TreeData
{
    public int[] VfxAmount = new int[] { 200, 500, 1000, 2000, 2000 };
    public float Rotation = 45;
    public float Scale = 1;
}
public enum SeasonType
{
    봄, 여름, 가을, 겨울, 겨울끝, 봄이전
}


//[System.Serializable]
//public class SeasonTreeData
//{
//    public string Name;
//    [GradientUsage(true)]
//    public Gradient LeavesColors;
//    [GradientUsage(true)]
//    public Gradient PositionGradient;

//    public AnimationCurve Curve;

//    public int FallingLeavesAmout = 10;


//    public ParticleSystem[] Particle;
//}

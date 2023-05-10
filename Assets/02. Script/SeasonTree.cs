using StartPage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;
using UnityEngine.Playables;

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

    [Header("Data")]
    public TreeData Data;

    public int CurrentSeason = -1;
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

        for(int i=0; i<5; i++)
        {
            VfxList[i].SetFloat("Leaves Amount", Data.VfxAmount[i]);
        }
    }

    private void Update()
    {
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


            foreach (var pa in TargetData.Particle)
            {
                var main = pa.main;
                main.loop = true;
                pa.gameObject.SetActive(false);
                pa.gameObject.SetActive(true);
                pa.Play();
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
        float time = 0;

        vfx.SetFloat("Leaves Size", 0);
        vfx.gameObject.SetActive(true);

        float targetTime = 5f;
        while(time < targetTime)
        {
            time += Time.deltaTime;
            yield return null;
            vfx.SetFloat("Leaves Size", Mathf.Lerp(0, 1.5f, time / targetTime));
        }
    }

    IEnumerator Season(int season)
    {
        switch (season)
        {
            case 0:
                TreeSnowMaterial.color = Color.clear;
                foreach (var vfx in VfxList)
                {
                    vfx.SetBool("Drop all leaves", false);
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

                yield return new WaitForSeconds(5f);
                foreach (var pa in SeasonData[0].Particle)
                {
                    pa.gameObject.SetActive(false);
                }
                break;


            case 2:
                yield return new WaitForSeconds(7f);
                VfxList[0].SetBool("Drop all leaves", true);
                yield return new WaitForSeconds(5f);
                VfxList[1].SetBool("Drop all leaves", true);
                break;


            case 3:
                yield return new WaitForSeconds(3f);
                TreeSnowMaterial.DOFade(0.7f, 3f);

                float time = 0;
                float targetTime = 5f;
                while (time < targetTime)
                {
                    foreach (var vfx in VfxList)
                    {
                        time += Time.deltaTime;
                        yield return null;
                        vfx.SetFloat("Leaves Size", Mathf.Lerp(1.5f, 0.3f, time / targetTime));
                    }
                }
                yield return new WaitForSeconds(3f);

                VfxList[2].SetBool("Drop all leaves", true);
                yield return new WaitForSeconds(1f);
                VfxList[3].SetBool("Drop all leaves", true);
                yield return new WaitForSeconds(1f);
                VfxList[4].SetBool("Drop all leaves", true);
                yield return new WaitForSeconds(1f);


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

}

[System.Serializable]
public class TreeData
{
    public int[] VfxAmount = new int[] { 200, 500, 1000, 2000, 2000 };
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

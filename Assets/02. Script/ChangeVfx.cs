using StartPage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChangeVfx : MonoBehaviour
{
    public VisualEffect Vfx;

    public Gradient ColorGradient1;
    public Gradient ColorGradient2;

    public float CurrentTime = 0;
    public float ShowTime = 10;

    private void Update()
    {
        CurrentTime += Time.deltaTime;
        if(CurrentTime < ShowTime)
        {
            Vfx.SetGradient("Position Gradient", CustomUtils.GradientLerp(ColorGradient1, ColorGradient2, CurrentTime / ShowTime));
            //Leaves Colors
        }
    }

}

[System.Serializable]
public class ChangeVfxData
{
    public string Name;
    public Gradient ColorGradient;
}

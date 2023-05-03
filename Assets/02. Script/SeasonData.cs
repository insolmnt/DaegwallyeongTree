using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonData : MonoBehaviour
{
    public string Name;
    [GradientUsage(true)]
    public Gradient LeavesColors;
    [GradientUsage(true)]
    public Gradient PositionGradient;

    public AnimationCurve Curve;

    public int FallingLeavesAmout = 10;


    public ParticleSystem[] Particle;
}

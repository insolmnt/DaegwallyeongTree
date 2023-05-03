using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[TrackColor(0f, 0.5f, 1f)]
[TrackClipType(typeof(LightControlClip))]
[TrackBindingType(typeof(SeasonTree))]
public class LightControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<LightControlMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        SeasonTree trackBinding = director.GetGenericBinding(this) as SeasonTree;
       if (trackBinding == null)
           return;
       driver.AddFromName<SeasonTree>(trackBinding.gameObject, "CurrentSeason");
#endif
        base.GatherProperties(director, driver);
    }
}

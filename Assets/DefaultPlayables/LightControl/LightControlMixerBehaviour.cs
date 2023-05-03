using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LightControlMixerBehaviour : PlayableBehaviour
{
    int Season;

    SeasonTree m_TrackBinding;
    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as SeasonTree;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened)
        {
            Season = m_TrackBinding.CurrentSeason;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount ();

        Color blendedColor = Color.clear;

        for (int i = inputCount - 1 ; i >= 0 ; i--)
        {
            float inputWeight = playable.GetInputWeight(i);

            if(inputWeight > 0)
            {
                ScriptPlayable<LightControlBehaviour> inputPlayable = (ScriptPlayable<LightControlBehaviour>)playable.GetInput(i);
                LightControlBehaviour input = inputPlayable.GetBehaviour();

                m_TrackBinding.SeasonDataChange(input.Season);
                m_TrackBinding.TVal = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                break;
            }
        }
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        m_FirstFrameHappened = false;

        if(m_TrackBinding == null)
            return;

        m_TrackBinding.CurrentSeason = Season;
    }
}

﻿/*! \cond PRIVATE */
#if VIDEO_ENABLED
using UnityEngine.Video;

namespace DarkTonic.MasterAudio
{
    public class VideoPlayerTracker
    {
        private VideoPlayerTracker() { }

        public VideoPlayerTracker(VideoPlayer player, SoundGroupVariation variation)
        {
            Player = player;
            Variation = variation;
        }

        public VideoPlayer Player { get; }
        public bool IsPlaying { get; private set; }
        public SoundGroupVariation Variation { get; }

        public void StartedPlaying()
        {
            IsPlaying = true;
        }

        public void StoppedPlayings()
        {
            IsPlaying = false;
        }
    }
}
#endif
/*! \endcond */
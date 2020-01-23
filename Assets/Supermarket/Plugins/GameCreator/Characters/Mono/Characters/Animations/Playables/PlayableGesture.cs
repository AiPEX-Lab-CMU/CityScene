namespace GameCreator.Characters
{
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class PlayableGesture : PlayableBase
    {
        public AnimationClip animationClip { get; private set; }

        private double endFreezeTime = -100.0;

        // INITIALIZERS: --------------------------------------------------------------------------

        private static AnimationClip GESTURE_DEBUG;

        private PlayableGesture(AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
            : base(avatarMask, fadeIn, fadeOut, speed, 1f)
        {
            if (GESTURE_DEBUG != null) GESTURE_DEBUG = animationClip;
            this.animationClip = animationClip;
        }

        public static PlayableGesture Create<TInput0, TOutput>(
            AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            PlayableGesture gesture = new PlayableGesture(
                animationClip, avatarMask,
                fadeIn, fadeOut, speed
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);

            input1.SetTime(0f);
            input1.SetSpeed(speed);
            input1.SetDuration(animationClip.length);

            gesture.Setup(ref graph, ref input0, ref input1, ref output);
            return gesture;
        }

        public static PlayableGesture CreateAfter(
            AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, PlayableBase previous)
        {
            PlayableGesture gesture = new PlayableGesture(
                animationClip, avatarMask,
                fadeIn, fadeOut, speed
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);

            input1.SetTime(0f);
            input1.SetSpeed(speed);
            input1.SetDuration(animationClip.length);

            gesture.Setup(ref graph, previous, ref input1);
            return gesture;
        }

        // UPDATE: --------------------------------------------------------------------------------

        public override bool Update()
        {
            if (this.endFreezeTime > 0f && Time.time > this.endFreezeTime)
            {
                this.Stop(0f);
                return true;
            }

            if (this.Input1.IsDone())
            {
                this.Stop(0f);
                return true;
            }

            float time = (float)this.Input1.GetTime();
            if (time + this.fadeOut >= this.Input1.GetDuration())
            {
                float t = ((float)this.Input1.GetDuration() - time) / this.fadeOut;

                t = Mathf.Clamp01(t);
                this.UpdateMixerWeights(t);
            }
            else if (time <= this.fadeIn)
            {
                float t = time / this.fadeIn;

                t = Mathf.Clamp01(t);
                this.UpdateMixerWeights(t);
            }
            else
            {
                this.UpdateMixerWeights(1f);
            }

            return false;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void StretchDuration(float extraTime)
        {
            if (this.Input1.GetDuration() - this.Input1.GetTime() < extraTime)
            {
                this.Input1.SetSpeed(0f);
                this.endFreezeTime = Time.time + extraTime;
            }
        }
    }
}

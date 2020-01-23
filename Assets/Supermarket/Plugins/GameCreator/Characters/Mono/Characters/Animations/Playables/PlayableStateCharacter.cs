namespace GameCreator.Characters
{
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class PlayableStateCharacter : PlayableStateRTC
    {
        private readonly CharacterAnimation character;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        private PlayableStateCharacter(CharacterState stateAsset, AvatarMask avatarMask,
            CharacterAnimation character, int layer,
            float time, float speed, float weight)
            : base(avatarMask, layer, time, speed, weight)
        {
            this.character = character;
            this.CharacterState = stateAsset;
        }

        // STATIC CONSTRUCTORS: -------------------------------------------------------------------

        public static PlayableStateCharacter Create<TInput0, TOutput>(
            CharacterState stateAsset, AvatarMask avatarMask,
            CharacterAnimation character, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            PlayableStateCharacter state = new PlayableStateCharacter(
                stateAsset, avatarMask,
                character, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                stateAsset.GetRuntimeAnimatorController()
            );

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, ref input0, ref input1, ref output);
            return state;
        }

        public static PlayableStateCharacter CreateAfter(
            CharacterState stateAsset, AvatarMask avatarMask,
            CharacterAnimation character, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, PlayableBase previous)
        {
            PlayableStateCharacter state = new PlayableStateCharacter(
                stateAsset, avatarMask,
                character, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                stateAsset.GetRuntimeAnimatorController()
            );

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, previous, ref input1);
            return state;
        }

        public static PlayableStateCharacter CreateBefore(
            CharacterState stateAsset, AvatarMask avatarMask,
            CharacterAnimation character, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, PlayableBase next)
        {
            PlayableStateCharacter state = new PlayableStateCharacter(
                stateAsset, avatarMask,
                character, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                stateAsset.GetRuntimeAnimatorController()
            );

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, ref input1, next);
            return state;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnExitState()
        {
            this.CharacterState.ExitState(this.character);
        }
    }
}

namespace GameCreator.Characters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class CharacterAnimatorRotation
    {
        private const float ROTATION_SMOOTH = 0.25f;

        [Serializable]
        private class AnimFloat
        {
            public float target { get; set; }
            public float value { get; private set; }
            private float speed;

            public AnimFloat(float value)
            {
                this.target = value;
                this.value = value;
                this.speed = 0f;
            }

            public float Update()
            {
                this.value = Mathf.SmoothDampAngle(
                    this.value,
                    this.target,
                    ref this.speed,
                    ROTATION_SMOOTH
                );

                return this.value;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private readonly AnimFloat x = new AnimFloat(0f);
        private readonly AnimFloat y = new AnimFloat(0f);
        private readonly AnimFloat z = new AnimFloat(0f);

        // UPDATER: -------------------------------------------------------------------------------

        public Quaternion Update()
        {
            return Quaternion.Euler(
                this.x.Update(),
                this.y.Update(),
                this.z.Update()
            );
        }

        // PUBLIC GETTERS: ------------------------------------------------------------------------

        public Quaternion GetCurrentRotation()
        {
            return Quaternion.Euler(this.x.value, this.y.value, this.z.value);
        }

        public Quaternion GetTargetRotation()
        {
            return Quaternion.Euler(this.x.target, this.y.target, this.z.target);
        }

        // PUBLIC SETTERS: ------------------------------------------------------------------------

        public void SetPitch(float value)
        {
            this.x.target = value;
        }

        public void SetYaw(float value)
        {
            this.y.target = value;
        }

        public void SetRoll(float value)
        {
            this.z.target = value;
        }

        public void SetQuaternion(Quaternion rotation)
        {
            this.SetPitch(rotation.eulerAngles.x);
            this.SetYaw(rotation.eulerAngles.y);
            this.SetRoll(rotation.eulerAngles.z);
        }
    }
}
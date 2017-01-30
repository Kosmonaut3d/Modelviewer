using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MGSkinnedAnimationAux
{
    /// <summary>
    /// An animation clip is a set of keyframes with associated bones.
    /// </summary>
    public class AnimationClip
    {
        #region Keyframe and Bone nested class

        /// <summary>
        /// An Keyframe is a rotation and translation for a moment in time.
        /// It would be easy to extend this to include scaling as well.
        /// </summary>
        public class Keyframe
        {
            public double Time;             // The keyframe time
            public Quaternion Rotation;     // The rotation for the bone
            public Vector3 Translation;     // The translation for the bone

            public Matrix Transform
            {
                get
                {
                    return Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation);
                }
                set
                {
                    Matrix transform = value;
                    transform.Right = Vector3.Normalize(transform.Right);
                    transform.Up = Vector3.Normalize(transform.Up);
                    transform.Backward = Vector3.Normalize(transform.Backward);
                    Rotation = Quaternion.CreateFromRotationMatrix(transform);
                    Translation = transform.Translation;
                }
            }
        }

        /// <summary>
        /// Keyframes are grouped per bone for an animation clip
        /// </summary>
        public class Bone
        {
            /// <summary>
            /// Each bone has a name so we can associate it with a runtime model
            /// </summary>
            private string name = "";

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            private List<Keyframe> keyframes = new List<Keyframe>();

            /// <summary>
            /// The bone name for these keyframes
            /// </summary>
            public string Name { get { return name; } set { name = value; } }

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            public List<Keyframe> Keyframes { get { return keyframes; } }
        }

        #endregion

        /// <summary>
        /// The bones for this animation
        /// </summary>
        private List<Bone> bones = new List<Bone>();

        /// <summary>
        /// Name of the animation clip
        /// </summary>
        public string Name;

        /// <summary>
        /// Duration of the animation clip
        /// </summary>
        public double Duration;

        /// <summary>
        /// The bones for this animation clip with their keyframes
        /// </summary>
        public List<Bone> Bones { get { return bones; } }
    }
}

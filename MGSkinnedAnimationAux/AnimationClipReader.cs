using Microsoft.Xna.Framework.Content;

namespace MGSkinnedAnimationAux
{
    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
        {
            AnimationClip clip = new AnimationClip();
            clip.Name = input.ReadString();
            clip.Duration = input.ReadDouble();

            int boneCnt = input.ReadInt32();
            for (int i = 0; i < boneCnt; i++)
            {
                AnimationClip.Bone bone = new AnimationClip.Bone();
                clip.Bones.Add(bone);

                bone.Name = input.ReadString();

                int cnt = input.ReadInt32();

                for (int j = 0; j < cnt; j++)
                {
                    AnimationClip.Keyframe keyframe = new AnimationClip.Keyframe();
                    keyframe.Time = input.ReadDouble();
                    keyframe.Rotation = input.ReadQuaternion();
                    keyframe.Translation = input.ReadVector3();

                    bone.Keyframes.Add(keyframe);

                }

            }

            return clip;
        }
    }
}

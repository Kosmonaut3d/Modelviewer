using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace MGSkinnedAnimationAux
{
    public class ModelExtraReader : ContentTypeReader<ModelExtra>
    {
        protected override ModelExtra Read(ContentReader input, ModelExtra existingInstance)
        {
            ModelExtra extra = new ModelExtra();
            extra.Skeleton = input.ReadObject<List<int>>();
            extra.Clips = input.ReadObject<List<AnimationClip>>();

            return extra;
        }
    }
}

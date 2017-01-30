using MGSkinnedAnimationAux;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MGSkinnedAnimationPipeline
{
    [ContentTypeWriter]
    public class ModelExtraWriter : ContentTypeWriter<ModelExtra>
    {
        protected override void Write(ContentWriter output, ModelExtra extra)
        {
            output.WriteObject(extra.Skeleton);
            output.WriteObject(extra.Clips);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelExtraReader).AssemblyQualifiedName;
        }
    }
}

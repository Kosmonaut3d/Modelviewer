using System.Collections.Generic;
using MGSkinnedAnimationAux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ModelViewer.Renderer.ShaderModules.Helper
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel
    {
        #region Fields

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        private Model model = null;

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        private ModelExtra modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();


        private Matrix[] skeleton;

        private Matrix[] boneTransforms;

        /// <summary>
        /// The model asset name
        /// </summary>
        private string assetName = "";

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        private AnimationPlayer player = null;


        private bool hasSkinnedVertexType = false;
        private bool hasNormals = false;
        private bool hasTexCoords = false;

        #endregion

        #region Properties

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public List<Bone> Bones { get { return bones; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return modelExtra.Clips; } }

        public bool HasAnimation()
        {
            return modelExtra != null;
        }

        #endregion

        #region Construction and Loading

        /// <summary>
        /// Constructor. Creates the model from an XNA model
        /// </summary>
        /// <param name="assetName">The name of the asset for this model</param>
        public AnimatedModel(string assetName)
        {
            this.assetName = assetName;

        }

        /// <summary>
        /// Load the model asset from content
        /// </summary>
        /// <param name="content"></param>
        public bool LoadContent(ContentManager content)
        {
            bool success = false;
            this.model = content.Load<Model>(assetName);
            modelExtra = model.Tag as ModelExtra;

            //System.Diagnostics.Debug.Assert(modelExtra != null);
            if (modelExtra != null)
            {
                ObtainBones();

                boneTransforms = new Matrix[bones.Count];

                skeleton = new Matrix[modelExtra.Skeleton.Count];
                success = true;
            }
            
            VertexElement[] test = model.Meshes[0].MeshParts[0].VertexBuffer.VertexDeclaration.GetVertexElements();

            for (int index = 0; index < test.Length; index++)
            {
                var t = test[index];
                if (t.VertexElementUsage == VertexElementUsage.BlendWeight)
                {
                    hasSkinnedVertexType = true;
                }
                else if (t.VertexElementUsage == VertexElementUsage.Normal)
                {
                    hasNormals = true;
                }
                else if (t.VertexElementUsage == VertexElementUsage.TextureCoordinate)
                {
                    hasTexCoords = true;
                }
            }

            return success;
        }


        #endregion

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in model.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach(Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion

        #region Animation Management

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip, bool looping = true, int keyframestart = 0, int keyframeend = 0, int fps = 24)
        {
            // Create a clip player and assign it to this model
            player = new AnimationPlayer(clip, this, looping, keyframestart, keyframeend, fps);
            return player;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Update animation for the model.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (player != null)
            {
                player.Update(gameTime);
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draw the model
        /// </summary>
        /// <param name="graphics">The graphics device to draw on</param>
        /// <param name="camera">A camera to determine the view</param>
        /// <param name="world">A world matrix to place the model</param>
        public void Draw(Matrix world, Matrix view, Matrix viewProjection, Vector3 cameraPosition, AnimatedModelShader _skinnedShader, AnimatedModelShader.EffectPasses pass, bool computeTransform)
        {
            if (model == null)
                return;

            //
            // Compute all of the bone absolute transforms
            //
            if (modelExtra != null && hasSkinnedVertexType && hasNormals && hasTexCoords)
            {
                if (computeTransform)
                {

                    for (int i = 0; i < bones.Count; i++)
                    {
                        Bone bone = bones[i];
                        bone.ComputeAbsoluteTransform();

                        boneTransforms[i] = bone.AbsoluteTransform;
                    }

                    for (int s = 0; s < modelExtra.Skeleton.Count; s++)
                    {
                        Bone bone = bones[modelExtra.Skeleton[s]];
                        skeleton[s] = bone.SkinTransform*bone.AbsoluteTransform;
                    }
                }

                if (bones.Count > 1)
                {
                    switch (pass)
                    {
                        case AnimatedModelShader.EffectPasses.Unskinned:
                            pass = AnimatedModelShader.EffectPasses.Skinned;
                            break;
                        case AnimatedModelShader.EffectPasses.UnskinnedNormalMapped:
                            pass = AnimatedModelShader.EffectPasses.SkinnedNormalMapped;
                            break;
                        case AnimatedModelShader.EffectPasses.UnskinnedDepth:
                            pass = AnimatedModelShader.EffectPasses.SkinnedDepth;
                            break;
                    }
                }
                _skinnedShader.DrawMesh(model, world, view, viewProjection, cameraPosition, pass, skeleton);
            }
            else
            {

                if (!hasNormals) pass = AnimatedModelShader.EffectPasses.NoNormalUnskinned;

                if (!hasTexCoords)
                    pass = AnimatedModelShader.EffectPasses.NoNormalNoTexUnskinned;

                _skinnedShader.DrawMesh(model, world, view, viewProjection, cameraPosition, pass, null);
            }

            //// Draw the model.
            //foreach (ModelMesh modelMesh in model.Meshes)
            //{
            //    foreach (Effect effect in modelMesh.Effects)
            //    {
            //        if (effect is BasicEffect)
            //        {
            //            BasicEffect beffect = effect as BasicEffect;
            //            beffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
            //            beffect.View = camera.View;
            //            beffect.Projection = camera.Projection;
            //            beffect.EnableDefaultLighting();
            //            beffect.PreferPerPixelLighting = true;
            //        }

            //        if (effect is SkinnedEffect)
            //        {
            //            SkinnedEffect seffect = effect as SkinnedEffect;
            //            seffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
            //            seffect.View = camera.View;
            //            seffect.Projection = camera.Projection;
            //            seffect.EnableDefaultLighting();
            //            seffect.PreferPerPixelLighting = true;
            //            seffect.SetBoneTransforms(skeleton);
            //        }
            //    }

            //    modelMesh.Draw();
            //}
        }


        #endregion

    }
}

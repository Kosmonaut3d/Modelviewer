using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ModelViewer.Renderer.ShaderModules
{
    public class SkyboxRenderModule
    {
        private Effect _shaderEffect;
        private GraphicsDevice _graphicsDevice;

        private VertexBuffer _sphereVertexBuffer;
        private IndexBuffer _sphereIndexBuffer;
        private int _spherePrimitiveCount; 

        private EffectParameter _worldViewProjParameter;
        private EffectParameter _worldParameter;
        private EffectParameter _skyboxTexture;

        private EffectPass _drawSkyboxPass;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Needs to be called to load all the shader fx files
        /// </summary>
        /// <param name="content"></param>
        /// <param name="shaderPath">The path in our content to the shader file, for example "Shaders/Skybox"</param>
        /// <param name="sphereLoadingPath">should be of size 1</param>
        public void Load(ContentManager content, string shaderPath, string sphereLoadingPath)
        {
            _shaderEffect = content.Load<Effect>(shaderPath);

            _worldViewProjParameter = _shaderEffect.Parameters["WorldViewProj"];
            _worldParameter = _shaderEffect.Parameters["World"];
            _skyboxTexture = _shaderEffect.Parameters["SkyboxTexture"];

            _drawSkyboxPass = _shaderEffect.Techniques["DrawSkybox"].Passes[0];
            
            ModelMeshPart sphere = content.Load<Model>(sphereLoadingPath).Meshes[0].MeshParts[0];
            _sphereIndexBuffer = sphere.IndexBuffer;
            _sphereVertexBuffer = sphere.VertexBuffer;
            _spherePrimitiveCount = sphere.PrimitiveCount;
        }

        public void SetSkybox(TextureCube cubemap)
        {
            _skyboxTexture.SetValue(cubemap);
        }
        

        public void Draw(Matrix viewProjection, Vector3 origin, float size)
        {
            _graphicsDevice.SetVertexBuffer(_sphereVertexBuffer);
            _graphicsDevice.Indices = (_sphereIndexBuffer);
            int primitiveCount = _spherePrimitiveCount;

            Matrix world = Matrix.CreateScale(size) * Matrix.CreateTranslation(origin);
            
            _worldViewProjParameter.SetValue(world * viewProjection);
            _worldParameter.SetValue(world);

            _drawSkyboxPass.Apply();

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
        }
    }
        
    
}

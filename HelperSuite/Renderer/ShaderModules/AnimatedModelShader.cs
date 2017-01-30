using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
Copyright 2017 by kosmonautgames

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace HelperSuite.Renderer.ShaderModules
{
    /// <summary>
    /// A shader to draw a uniform color across a mesh
    /// </summary>
    public class AnimatedModelShader
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Variables

        private GraphicsDevice _graphicsDevice;

        private Effect _shaderEffect;

        private EffectParameter _worldViewProjParameter;
        private EffectParameter _worldITParameter;
        private EffectParameter _worldParameter;
        private EffectParameter _bonesParameter;
        private EffectParameter _cameraPositionParameter;
        private EffectParameter _normalMapParameter;
        private EffectParameter _albedoMapParameter;
        private EffectParameter _albedoColorParameter;
        private EffectParameter _environmentMapParameter;
        private EffectParameter _fresnelMapParameter;

        private EffectPass _unskinnedNormalMappedPass;
        private EffectPass _unskinnedPass;
        private EffectPass _skinnedPass;
        private EffectPass _skinnedNormalMappedPass;

        private Texture2D _normalMap;
        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                if (_normalMap != value)
                {
                    _normalMap = value;
                    _normalMapParameter.SetValue(_normalMap);
                }
            }
        }

        private Color _albedoColor;
        public Color AlbedoColor
        {
            get
            {
                return _albedoColor;
            }

            set
            {
                if (_albedoColor != value)
                {
                    _albedoColor = value;
                    _albedoColorParameter.SetValue(_albedoColor.ToVector4());
                }
            }
        }

        private Texture2D _albedoMap;

        public Texture2D AlbedoMap
        {
            get { return _albedoMap; }
            set
            {
                if (_albedoMap != value)
                {
                    _albedoMap = value;
                    _albedoMapParameter.SetValue(_albedoMap);
                }
            }
        }

        private Texture _fresnelMap;

        public Texture FresnelMap
        {
            get { return _fresnelMap; }
            set
            {
                if (_fresnelMap != value)
                {
                    _fresnelMap = value;
                    _fresnelMapParameter.SetValue(_fresnelMap);
                }
            }
        }

        private TextureCube _environmentMap;
        public TextureCube EnvironmentMap
        {
            get { return _environmentMap; }
            set
            {
                if (_environmentMap != value)
                {
                    _environmentMap = value;
                    _environmentMapParameter.SetValue(_environmentMap);
                }
            }
        }


        public enum EffectPasses
        {
            Unskinned, UnskinnedNormalMapped, Skinned, SkinnedNormalMapped
        };


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions

        /// <summary>
        /// Needs to be called to load all the shader fx files
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        public void Load(ContentManager content, string shaderPath)
        {
            _shaderEffect = content.Load<Effect>(shaderPath);

            _worldViewProjParameter = _shaderEffect.Parameters["ViewProj"];
            _worldITParameter = _shaderEffect.Parameters["WorldIT"];
            _worldParameter = _shaderEffect.Parameters["World"];
            _bonesParameter = _shaderEffect.Parameters["Bones"];
            _cameraPositionParameter = _shaderEffect.Parameters["CameraPosition"];

            _normalMapParameter = _shaderEffect.Parameters["NormalMap"];
            _albedoColorParameter = _shaderEffect.Parameters["AlbedoColor"];
            _albedoMapParameter = _shaderEffect.Parameters["AlbedoMap"];
            _environmentMapParameter = _shaderEffect.Parameters["EnvironmentMap"];
            _fresnelMapParameter = _shaderEffect.Parameters["FresnelMap"];

            _unskinnedPass = _shaderEffect.Techniques["Unskinned"].Passes[0];
            _unskinnedNormalMappedPass = _shaderEffect.Techniques["UnskinnedNormalMapped"].Passes[0];

            _skinnedPass = _shaderEffect.Techniques["Skinned"].Passes[0];
            _skinnedNormalMappedPass = _shaderEffect.Techniques["SkinnedNormalMapped"].Passes[0];
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Base draw
        /// </summary>
        /// <param name="model"></param>
        /// <param name="world"></param>
        /// <param name="viewProjection"></param>
        /// <param name="cameraPosition"></param>
        /// <param name="effectPass"></param>
        /// <param name="bones"></param>
        public void DrawMesh(Model model, Matrix world, Matrix viewProjection, Vector3 cameraPosition, EffectPasses effectPass, Matrix[] bones = null)
        {
            _worldViewProjParameter.SetValue(viewProjection);
            _worldITParameter.SetValue(world/*Matrix.Transpose(Matrix.Invert(world))*/);
            _worldParameter.SetValue(world);
            if(bones!=null)
             _bonesParameter.SetValue(bones);
            _cameraPositionParameter.SetValue(cameraPosition);

            for (int index = 0; index < model.Meshes.Count; index++)
            {
                var modelMesh = model.Meshes[index];

                for (int i = 0; i < modelMesh.MeshParts.Count; i++)
                {
                    var modelMeshPart = modelMesh.MeshParts[i];

                    DrawMeshPart(modelMeshPart, effectPass);
                }
            }
        }

        /// <summary>
        /// Draw Mesh with the effect applied
        /// </summary>
        /// <param name="modelMeshPart"></param>
        /// <param name="worldViewProjection"></param>
        /// <param name="effectPass"></param>
        private void DrawMeshPart(ModelMeshPart modelMeshPart, EffectPasses effectPass)
        {
            _graphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer);
            _graphicsDevice.Indices = (modelMeshPart.IndexBuffer);
            int primitiveCount = modelMeshPart.PrimitiveCount;
            int vertexOffset = modelMeshPart.VertexOffset;
            int startIndex = modelMeshPart.StartIndex;
            
            switch (effectPass)
            {
                case EffectPasses.Unskinned:
                    _unskinnedPass.Apply();
                    break;
                case EffectPasses.UnskinnedNormalMapped:
                    _unskinnedNormalMappedPass.Apply();
                    break;
                case EffectPasses.Skinned:
                    _skinnedPass.Apply();
                    break;
                case EffectPasses.SkinnedNormalMapped:
                    _skinnedNormalMappedPass.Apply();
                    break;
            }

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex, primitiveCount);
        }
        

    }
}

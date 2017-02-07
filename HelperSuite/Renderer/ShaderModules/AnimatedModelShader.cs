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

namespace ModelViewer.Renderer.ShaderModules
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
        private EffectParameter _viewParameter;
        private EffectParameter _bonesParameter;
        private EffectParameter _cameraPositionParameter;
        private EffectParameter _normalMapParameter;
        private EffectParameter _useNormalMapParameter;
        private EffectParameter _albedoMapParameter;
        private EffectParameter _albedoColorParameter;
        private EffectParameter _useAlbedoMapParameter;
        private EffectParameter _environmentMapParameter;
        private EffectParameter _fresnelMapParameter;
        private EffectParameter _roughnessMapParameter;
        private EffectParameter _useRoughnessMapParameter;
        private EffectParameter _metallicMapParameter;
        private EffectParameter _useMetallicMapParameter;
        private EffectParameter _aoMapParameter;
        private EffectParameter _useAoMapParameter;
        private EffectParameter _heightMapParameter;
        public EffectParameter UsePomParameter;
        private EffectParameter _useBumpmapParameter;
        private EffectParameter _pomScaleParameter;

        public EffectParameter DepthMapParameter;

        private EffectParameter _useLinearParameter;

        private EffectParameter _roughnessParameter;
        private EffectParameter _metallicParameter;

        private EffectPass _noNormalUnskinnedPass;
        private EffectPass _noNormalNoTexUnskinnedPass;
        private EffectPass _unskinnedNormalMappedPass;
        private EffectPass _unskinnedPass;
        private EffectPass _skinnedPass;
        private EffectPass _skinnedNormalMappedPass;
        private EffectPass _unskinnedDepthPass;
        private EffectPass _skinnedDepthPass;

        private bool _useLinear = true;
        public bool UseLinear
        {
            get { return _useLinear; }
            set {
                if (_useLinear != value)
                {
                    _useLinear = value;
                    _useLinearParameter.SetValue(_useLinear);
                } }
        }

        private Texture2D _aoMap;
        public Texture2D AoMap
        {
            get { return _aoMap; }
            set
            {
                if (_aoMap != value)
                {
                    _aoMap = value;
                    _aoMapParameter.SetValue(_aoMap);
                }
            }
        }

        private bool _useAo;
        public bool UseAo
        {
            get { return _useAo; }
            set
            {
                _useAo = value; 
                _useAoMapParameter.SetValue(_useAo);
            }
        }
        
        private Texture2D _normalMap;
        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                if (_normalMap != value)
                {
                    _normalMap = value;
                    _useNormalMapParameter.SetValue(_normalMap!=null);
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
                    _useAlbedoMapParameter.SetValue(_albedoMap != null);
                    _albedoMapParameter.SetValue(_albedoMap);
                }
            }
        }

        private Texture2D _heightMap;

        public Texture2D HeightMap
        {
            get { return _heightMap; }
            set
            {
                if (_heightMap != value)
                {
                    _heightMap = value;
                    _useBumpmapParameter.SetValue(_heightMap!=null);
                    _heightMapParameter.SetValue(_heightMap);
                }
            }
        }

        private float _pomScale;
        public float PomScale
        {
            get
            {
                return _pomScale;
            }

            set
            {
                if (Math.Abs(_pomScale - value) > 0.00001f)
                {
                    _pomScale = value;
                    _pomScaleParameter.SetValue(_pomScale);
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

        public float Roughness
        {
            get { return _roughness; }
            set {
                if (Math.Abs(_roughness - value) > 0.0001f)
                {
                    _roughness = value;
                    _roughnessParameter.SetValue(_roughness);
                } }
        }

        private float _roughness;

        private Texture2D _roughnessMap;

        public Texture2D RoughnessMap
        {
            get { return _roughnessMap; }
            set
            {
                if (_roughnessMap != value)
                {
                    _roughnessMap = value;
                    _useRoughnessMapParameter.SetValue(_roughnessMap != null);
                    _roughnessMapParameter.SetValue(_roughnessMap);
                }
            }
        }

        public float Metallic
        {
            get { return _metallic; }
            set
            {
                if (Math.Abs(_metallic - value) > 0.0001f)
                {
                    _metallic = value;
                    _metallicParameter.SetValue(_metallic);
                }
            }
        }

        private float _metallic;

        private Texture2D _metallicMap;

        public Texture2D MetallicMap
        {
            get { return _metallicMap; }
            set
            {
                if (_metallicMap != value)
                {
                    _metallicMap = value;
                    _useMetallicMapParameter.SetValue(_metallicMap != null);
                    _metallicMapParameter.SetValue(_metallicMap);
                }
            }
        }


        public enum EffectPasses
        {
            Unskinned, UnskinnedNormalMapped, Skinned, SkinnedNormalMapped,
            SkinnedDepth,
            UnskinnedDepth,
            NoNormalUnskinned,
            NoNormalNoTexUnskinned
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
            _viewParameter = _shaderEffect.Parameters["View"];
            _bonesParameter = _shaderEffect.Parameters["Bones"];
            _cameraPositionParameter = _shaderEffect.Parameters["CameraPosition"];

            DepthMapParameter = _shaderEffect.Parameters["DepthMap"];

            _normalMapParameter = _shaderEffect.Parameters["NormalMap"];
            _useNormalMapParameter = _shaderEffect.Parameters["UseNormalMap"];
            _albedoColorParameter = _shaderEffect.Parameters["AlbedoColor"];
            _albedoMapParameter = _shaderEffect.Parameters["AlbedoMap"];
            _useAlbedoMapParameter = _shaderEffect.Parameters["UseAlbedoMap"];

            _aoMapParameter = _shaderEffect.Parameters["AoMap"];
            _useAoMapParameter = _shaderEffect.Parameters["UseAo"];

            _environmentMapParameter = _shaderEffect.Parameters["EnvironmentMap"];
            _fresnelMapParameter = _shaderEffect.Parameters["FresnelMap"];
            
            _roughnessParameter = _shaderEffect.Parameters["Roughness"];
            _roughnessMapParameter = _shaderEffect.Parameters["RoughnessMap"];
            _useRoughnessMapParameter = _shaderEffect.Parameters["UseRoughnessMap"];
            _metallicParameter = _shaderEffect.Parameters["Metallic"];
            _metallicMapParameter = _shaderEffect.Parameters["MetallicMap"];
            _useMetallicMapParameter = _shaderEffect.Parameters["UseMetallicMap"];

            _useLinearParameter = _shaderEffect.Parameters["UseLinear"];

            _heightMapParameter = _shaderEffect.Parameters["HeightMap"];
            UsePomParameter = _shaderEffect.Parameters["UsePOM"];
            _useBumpmapParameter = _shaderEffect.Parameters["UseBumpmap"];
            _pomScaleParameter = _shaderEffect.Parameters["POMScale"];

            _noNormalUnskinnedPass = _shaderEffect.Techniques["NoNormal_Unskinned"].Passes[0];
            _noNormalNoTexUnskinnedPass = _shaderEffect.Techniques["NoNormalNoTex_Unskinned"].Passes[0];
            _unskinnedPass = _shaderEffect.Techniques["Unskinned"].Passes[0];
            _unskinnedNormalMappedPass = _shaderEffect.Techniques["UnskinnedNormalMapped"].Passes[0];

            _skinnedPass = _shaderEffect.Techniques["Skinned"].Passes[0];
            _skinnedNormalMappedPass = _shaderEffect.Techniques["SkinnedNormalMapped"].Passes[0];
            
            _unskinnedDepthPass = _shaderEffect.Techniques["UnskinnedDepth"].Passes[0];
            _skinnedDepthPass = _shaderEffect.Techniques["SkinnedDepth"].Passes[0];
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
        public void DrawMesh(Model model, Matrix world, Matrix view, Matrix viewProjection, Vector3 cameraPosition, EffectPasses effectPass, Matrix[] bones = null)
        {
            _worldViewProjParameter.SetValue(viewProjection);
            _worldITParameter.SetValue(world/*Matrix.Transpose(Matrix.Invert(world))*/);
            _worldParameter.SetValue(world);
            _viewParameter.SetValue(view);
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
                case EffectPasses.NoNormalNoTexUnskinned:
                    _noNormalNoTexUnskinnedPass.Apply();
                    break;
                case EffectPasses.NoNormalUnskinned:
                    _noNormalUnskinnedPass.Apply();
                    break;
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
                case EffectPasses.SkinnedDepth:
                    _skinnedDepthPass.Apply();
                    break;
                case EffectPasses.UnskinnedDepth:
                    _unskinnedDepthPass.Apply();
                    break;
            }

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex, primitiveCount);
        }
        

    }
}

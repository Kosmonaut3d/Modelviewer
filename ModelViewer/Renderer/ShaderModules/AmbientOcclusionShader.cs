using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModelViewer.Renderer.ShaderModules.Helper;

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
    public class AmbientOcclusionShader
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Variables

        private GraphicsDevice _graphicsDevice;
        private QuadRenderer _quadRenderer;

        private Effect _shaderEffect;

        private EffectParameter _resolutionParameter;
        private EffectParameter _inverseResolutionParameter;
        private EffectParameter _frustumCornersParameter;
        private EffectParameter _samplesParameter;
        private EffectParameter _sampleRadiusParameter;
        private EffectParameter _sampleStrengthParameter;

        private EffectParameter _depthMapParameter;
        private EffectParameter _targetMapParameter;

        private EffectPass _ssaoPass;
        private EffectPass _blurVerticalPass;
        private EffectPass _blurHorizontalPass;

        private Vector3[] _frustumCorners;

        public Vector3[] FrustumCorners
        {
            get { return _frustumCorners; }
            set
            {
                _frustumCorners = value; 
                _frustumCornersParameter.SetValue(_frustumCorners);
            }
        }

        private Texture2D _depthMap;
        public Texture2D DepthMap
        {
            get { return _depthMap; }
            set
            {
                if (_depthMap != value)
                {
                    _depthMap = value;
                    _depthMapParameter.SetValue(_depthMap);
                }
            }
        }

        private Texture2D _targetMap;
        public Texture2D TargetMap
        {
            get { return _targetMap; }
            set
            {
                if (_targetMap != value)
                {
                    _targetMap = value;
                    _targetMapParameter.SetValue(_targetMap);
                }
            }
        }

        private Vector2 _resolution;

        public Vector2 Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = value; 
                _resolutionParameter.SetValue(_resolution);
                _inverseResolutionParameter.SetValue(Vector2.One/_resolution);
            }
        }

        private int _samples;

        public int Samples
        {
            get { return _samples; }
            set
            {
                _samples = value;
                _samplesParameter.SetValue(_samples);
            }
        }

        private float _sampleRadii;

        public float SampleRadii
        {
            get { return _sampleRadii; }
            set
            {
                _sampleRadii = value;
                _sampleRadiusParameter.SetValue(_sampleRadii);
            }
        }

        private float _sampleStrength;

        public float SampleStrength
        {
            get { return _sampleStrength; }
            set
            {
                _sampleStrength = value;
                _sampleStrengthParameter.SetValue(_sampleStrength);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions

        /// <summary>
        /// Needs to be called to load all the shader fx files
        /// </summary>
        /// <param name="content"></param>
        /// <param name="shaderPath"></param>
        public void Load(ContentManager content, string shaderPath)
        {
            _shaderEffect = content.Load<Effect>(shaderPath);

            _resolutionParameter = _shaderEffect.Parameters["Resolution"];
            _inverseResolutionParameter = _shaderEffect.Parameters["InverseResolution"];
            _frustumCornersParameter = _shaderEffect.Parameters["FrustumCorners"];
            _samplesParameter = _shaderEffect.Parameters["Samples"];
            _sampleRadiusParameter = _shaderEffect.Parameters["SampleRadius"];
            _sampleStrengthParameter = _shaderEffect.Parameters["Strength"];

            _depthMapParameter = _shaderEffect.Parameters["DepthMap"];
            _targetMapParameter = _shaderEffect.Parameters["TargetMap"];
            
            _ssaoPass = _shaderEffect.Techniques["SSAO"].Passes[0];
            _blurVerticalPass = _shaderEffect.Techniques["BilateralVertical"].Passes[0];
            _blurHorizontalPass = _shaderEffect.Techniques["BilateralHorizontal"].Passes[0];
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _quadRenderer = new QuadRenderer();
        }
        
        public void DrawAmbientOcclusion()
        {
            _ssaoPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);
        }
        
        internal void BlurAmbientOcclusion(RenderTarget2D ao, RenderTarget2D blur0, RenderTarget2D blur1)
        {
            TargetMap = ao;

            _graphicsDevice.SetRenderTarget(blur0);
            _blurVerticalPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);

            _graphicsDevice.SetRenderTarget(blur1);
            TargetMap = blur0;
            _blurHorizontalPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);

        }
    }
}

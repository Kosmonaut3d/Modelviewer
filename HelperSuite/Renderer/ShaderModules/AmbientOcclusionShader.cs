using System;
using DeferredEngine.Renderer.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModelViewer.HelperSuite.GUIRenderer.Helper;

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

        private EffectParameter _depthMapParameter;
        
        private EffectPass _ssao;

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

            _resolutionParameter = _shaderEffect.Parameters["Resolution"];
            _inverseResolutionParameter = _shaderEffect.Parameters["InverseResolution"];
            _frustumCornersParameter = _shaderEffect.Parameters["FrustumCorners"];
            _samplesParameter = _shaderEffect.Parameters["Samples"];
            _sampleRadiusParameter = _shaderEffect.Parameters["SampleRadius"];

            _depthMapParameter = _shaderEffect.Parameters["DepthMap"];
            
            _ssao = _shaderEffect.Techniques["SSAO"].Passes[0];
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _quadRenderer = new QuadRenderer();
        }
        
        public void Draw(RenderTarget2D output)
        {
            //_graphicsDevice.SetRenderTarget(output);

            _ssao.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);
           
        }
    }
}

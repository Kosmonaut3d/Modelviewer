using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperSuite.HelperSuite.ContentLoader;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Logic;
using HelperSuite.Renderer.ShaderModules;
using HelperSuite.Renderer.ShaderModules.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.Renderer
{
    public class Renderer
    {
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private Matrix _view;
        private Matrix _projection;
        private Matrix _viewProjection;
        private bool _viewProjectionHasChanged;

        private ThreadSafeContentManager _contentManager;

        private Texture2D rollTexture2D;
        private TextureCube skyboxCube;
        private Texture fresnelMap;

        private Model model;
        private Matrix YupOrientation;

        //Modules
        private SkyboxRenderModule _skyboxRenderModule;
        private AnimatedModelShader _animatedModelShader;

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphics = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            
            _skyboxRenderModule.Initialize(_graphics);
            _animatedModelShader.Initialize(_graphics);

            YupOrientation = Matrix.CreateRotationX((float) (Math.PI/2));
        }

        public void Load(ContentManager contentManager)
        {
            _contentManager = new ThreadSafeContentManager(contentManager.ServiceProvider) { RootDirectory = "Content" };

            rollTexture2D = _contentManager.Load<Texture2D>("Graphical User Interface/ring");
            skyboxCube = _contentManager.Load<TextureCube>("ShaderModules/Skybox/skyboxCubemap");
            fresnelMap = _contentManager.Load<Texture>("ShaderModules/AnimatedModelShader/fresnel2");

            model = _contentManager.Load<Model>("ShaderModules/Skybox/isosphere"/*"ShaderModules/AnimatedModelShader/cube"*/);

            _skyboxRenderModule = new SkyboxRenderModule();
            _skyboxRenderModule.Load(_contentManager, "ShaderModules/Skybox/skybox", "ShaderModules/Skybox/isosphere");
            _skyboxRenderModule.SetSkybox(skyboxCube);

            _animatedModelShader = new AnimatedModelShader();
            _animatedModelShader.Load(_contentManager, "ShaderModules/AnimatedModelShader/AnimatedModelShader");
            _animatedModelShader.EnvironmentMap = skyboxCube;
            _animatedModelShader.FresnelMap = fresnelMap;
        }

        public void Draw(Camera camera, MainLogic mainLogic, Vector3 modelPosition, GameTime gameTime)
        {
            UpdateViewProjection(camera);

            AnimatedModelShader.EffectPasses pass = AnimatedModelShader.EffectPasses.Unskinned;

            _graphics.BlendState = BlendState.Opaque;
            _graphics.DepthStencilState = DepthStencilState.Default;

            object loadedModel = mainLogic.modelLoader.LoadedObject;
            AnimatedModel usedModel = loadedModel != null ? (AnimatedModel)loadedModel : null;

            {
                object loadedMaterial = mainLogic.albedoLoader.LoadedObject;
                Texture2D loadedAlbedo = loadedMaterial != null ? (Texture2D) loadedMaterial : null;
                _animatedModelShader.AlbedoMap = loadedAlbedo;
            }

            {
                object loadedMaterial = mainLogic.normalLoader.LoadedObject;
                Texture2D loadedNormal = loadedMaterial != null ? (Texture2D)loadedMaterial : null;
                _animatedModelShader.NormalMap = loadedNormal;

                if(loadedNormal!=null)
                pass = AnimatedModelShader.EffectPasses.UnskinnedNormalMapped;
            }
            {
                object loadedMaterial = mainLogic.roughnessLoader.LoadedObject;
                Texture2D loadedAlbedo = loadedMaterial != null ? (Texture2D)loadedMaterial : null;
                _animatedModelShader.RoughnessMap = loadedAlbedo;
            }
            {
                object loadedMaterial = mainLogic.metallicLoader.LoadedObject;
                Texture2D loadedAlbedo = loadedMaterial != null ? (Texture2D)loadedMaterial : null;
                _animatedModelShader.MetallicMap = loadedAlbedo;
            }


            float scale = (float) Math.Pow(10, GameSettings.m_size)/10;
            Matrix size = Matrix.CreateScale(scale);

            Matrix world = (GameSettings.m_orientationy ? YupOrientation : Matrix.Identity) * Matrix.CreateTranslation(/*-usedModel.Meshes[0].BoundingSphere.Center*/ - modelPosition/ scale) * size ;

            _animatedModelShader.AlbedoColor = GameSettings.bgColor;
            _animatedModelShader.Roughness = GameSettings.m_roughness;
            _animatedModelShader.Metallic = GameSettings.m_metallic;
            _animatedModelShader.UseLinear = GameSettings.r_UseLinear;

            if (loadedModel != null)
            {
                //_animatedModelShader.DrawMesh((Model) loadedModel, world, _viewProjection, camera.Position, pass);
                usedModel.Draw(world, _viewProjection, camera.Position, _animatedModelShader, pass);

                if(usedModel.Clips.Count>0 && GameSettings.m_updateAnimation)
                   usedModel.Update(gameTime);

                if (GameSettings.m_startClip)
                {
                    usedModel.PlayClip(usedModel.Clips[0], true);
                    GameSettings.m_startClip = false;
                }

            }
            else
            {
                _animatedModelShader.DrawMesh(model, world, _viewProjection, camera.Position, pass);
            }


            _graphics.RasterizerState = RasterizerState.CullClockwise;
            _skyboxRenderModule.Draw(Matrix.CreateTranslation(camera.Position) *  _viewProjection, Vector3.Zero, 300);

            DrawInteractivityAnimation(gameTime);
        }

        private void DrawInteractivityAnimation(GameTime gameTime)
        {
            _spriteBatch.Begin();
            
            _spriteBatch.Draw(rollTexture2D, new Rectangle(10, GameSettings.g_ScreenHeight - 80, 20, 20), null, Color.White, (float)-gameTime.TotalGameTime.TotalSeconds * 3, new Vector2(rollTexture2D.Width / 2, rollTexture2D.Height / 2), SpriteEffects.None, 0);

            _spriteBatch.End();

        }

        private void UpdateViewProjection(Camera camera)
        {
            _viewProjectionHasChanged = camera.HasChanged;

            //If the camera didn't do anything we don't need to update this stuff
            if (_viewProjectionHasChanged)
            {
                //We have processed the change, now setup for next frame as false
                camera.HasChanged = false;
                camera.HasMoved = false;

                //View matrix
                _view = Matrix.CreateLookAt(camera.Position, camera.Lookat, camera.Up);

                _projection = Matrix.CreatePerspectiveFieldOfView(camera.FieldOfView,
                    GameSettings.g_ScreenWidth / (float)GameSettings.g_ScreenHeight, 1, GameSettings.g_FarPlane);
                //_projection = Matrix.CreateOrthographic(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight, -100, 100);

                _viewProjection = _view * _projection;
            }

        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModelViewer.HelperSuite.GUIRenderer;
using ModelViewer.Logic;

namespace ModelViewer.Static
{
    /// <summary>
    /// Manages our different screens and passes information accordingly
    /// </summary>
    public class ScreenManager
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  VARIABLES
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private DebugScreen _debug;
        private GuiLogicSample _guiLogicSample;
        private MainLogic _mainLogic;
        private GUIRenderer _guiRenderer;
        private Renderer.Renderer _renderer;

        private bool _wasInactive = true;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  FUNCTIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _debug.Initialize(graphicsDevice);
            _mainLogic.Initialize(graphicsDevice);
            _guiRenderer.Initialize(graphicsDevice);
            _guiLogicSample.Initialize(_mainLogic);
            _renderer.Initialize(graphicsDevice);
        }

        //Update per frame
        public void Update(GameTime gameTime, bool isActive)
        {
            if (!isActive)
            {
                _wasInactive = true;
                return;
            }

            //if (_wasInactive)
            //{
            //    Input.mouseLastState = Mouse.GetState();
            //    _wasInactive = false;
            //}


            Input.Update(gameTime);
            
            _guiLogicSample.Update(gameTime);

            _mainLogic.Update(gameTime);

            _debug.Update(gameTime);
        }

        //Load content
        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _debug = new DebugScreen();
            _debug.LoadContent(content);

            _mainLogic = new MainLogic();
            _mainLogic.Load(content);

            _guiRenderer = new GUIRenderer();
            _guiRenderer.Load(content);
            
            _guiLogicSample = new GuiLogicSample();
            _guiLogicSample.Load(content);
            
            _renderer = new Renderer.Renderer();
            _renderer.Load(content);
        }

        public void Unload(ContentManager content)
        {
            content.Dispose();
            _mainLogic.Unload();
        }
        
        public void Draw(GameTime gameTime)
        {
            _renderer.Draw(_mainLogic.GetCamera(), _mainLogic, _mainLogic.modelPosition, gameTime);

            _mainLogic.Draw(gameTime);

            //Our renderer gives us information on what id is currently hovered over so we can update / manipulate objects in the logic functions
            _debug.Draw(gameTime);

            _guiRenderer.Draw(_guiLogicSample.getCanvas());
        }

        public void UpdateResolution()
        {
            _guiLogicSample.UpdateResolution();
            _renderer.UpdateRenderTargets();
        }
    }
}

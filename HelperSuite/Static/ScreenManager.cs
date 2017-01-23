using HelperSuite.HelperSuite.GUIRenderer;
using HelperSuite.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OceanRender.Main
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
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  FUNCTIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _debug.Initialize(graphicsDevice);
            _mainLogic.Initialize(graphicsDevice);
            _guiRenderer.Initialize(graphicsDevice);
            _guiLogicSample.Initialize(_mainLogic);
        }

        //Update per frame
        public void Update(GameTime gameTime, bool isActive)
        {
            if (!isActive) return;

            Input.Update(gameTime);

            _guiLogicSample.Update(gameTime);

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
            
        }

        public void Unload(ContentManager content)
        {
            //content.Dispose();
            _mainLogic.Unload();
        }
        
        public void Draw(GameTime gameTime)
        {
            

            //Our renderer gives us information on what id is currently hovered over so we can update / manipulate objects in the logic functions
            _debug.Draw(gameTime);

            _mainLogic.Draw();

            _guiRenderer.Draw(_guiLogicSample.getCanvas());

        }

        public void UpdateResolution()
        {
            _guiLogicSample.UpdateResolution();
        }
    }
}

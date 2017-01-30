using System;
using HelperSuite.HelperSuite.GUI;
using HelperSuite.HelperSuite.GUIHelper;
using HelperSuite.HelperSuite.GUIRenderer;
using HelperSuite.HelperSuite.GUIRenderer.Helper;
using HelperSuite.HelperSuite.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.Logic
{
    public class GuiLogicSample
    {
        private GUICanvas screenCanvas;

        private GUIList baseList;
        private GUITextBlock _sizeBlock;

        private GUIStyle defaultStyle;

        private GUIContentLoader _guiContentLoader;

        public void Initialize(MainLogic mainLogic)
        {

            defaultStyle = new GUIStyle
            {
                BlockColor = Color.Gray,
                TextColor = Color.White,
                TextFont = GUIRenderer.MonospaceFont
            };

            screenCanvas = new GUICanvas(Vector2.Zero, new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), 0, GUIStyle.GUIAlignment.None);

            GuiListToggle toggleList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(toggleList);

            toggleList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "test textblock", GUIRenderer.MonospaceFont, Color.Gray, Color.White));

            baseList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.None, screenCanvas.Dimensions);
            toggleList.AddElement(baseList);

            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "debug info", GUIRenderer.MonospaceFont, Color.Gray, Color.White )
            {
                //    _lightEnableToggle.ToggleObject = editorObject;
                //_lightEnableToggle.Toggle = (editorObject as PointLightSource).IsEnabled;
                //_lightEnableToggle.ToggleProperty = typeof(PointLightSource).GetProperty("IsEnabled");
                //ToggleObject = (GameSettings as GameSettings),
                ToggleField = typeof(GameSettings).GetField("ui_debug"),
                Toggle = (bool)typeof(GameSettings).GetField("ui_debug").GetValue(null)
            });
            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "this is a generic testblock and it tests wrap ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));

            baseList.AddElement(_sizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "Model Size: "+GameSettings.m_size, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 3, Color.DimGray, Color.Black )
            {
                SliderField = typeof(GameSettings).GetField("m_size"),
                SliderValue = (float)typeof(GameSettings).GetField("m_size").GetValue(null)
            });

            //baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200,35), "Button", GUIRenderer.MonospaceFont, Color.Gray, Color.White )
            //    {
            //        ButtonObject = mainLogic,
            //        ButtonMethod = typeof( MainLogic ).GetMethod("TestFunction")
            //    }
            //);

            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "Model: Y up", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                //    _lightEnableToggle.ToggleObject = editorObject;
                //_lightEnableToggle.Toggle = (editorObject as PointLightSource).IsEnabled;
                //_lightEnableToggle.ToggleProperty = typeof(PointLightSource).GetProperty("IsEnabled");
                //ToggleObject = (GameSettings as GameSettings),
                ToggleField = typeof(GameSettings).GetField("m_orientationy"),
                Toggle = (bool)typeof(GameSettings).GetField("m_orientationy").GetValue(null)
            });
            GuiTextBlockLoadDialog modelLoader;
            baseList.AddElement(modelLoader = new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Model:", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = _guiContentLoader,
                ButtonMethod = _guiContentLoader.GetType().GetMethod("LoadContentFile").MakeGenericMethod(typeof(Model)),
            }
            );
            baseList.AddElement(new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "LoadFile:", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = _guiContentLoader,
                ButtonMethod = _guiContentLoader.GetType().GetMethod("LoadContentFile").MakeGenericMethod(typeof(Texture2D))
            }
            );

            mainLogic.modelLoader = modelLoader;

            baseList.AddElement(new GUIColorPicker(new Vector2(0, 0), new Vector2(200,200), Color.Gray, GUIRenderer.MonospaceFont)
            {
                ToggleField = typeof(GameSettings).GetField("bgColor"),
                ToggleObject = (Color)typeof(GameSettings).GetField("bgColor").GetValue(null)
            });

        }

        public void Load(ContentManager content)
        {
            _guiContentLoader = new GUIContentLoader();
            _guiContentLoader.Load(content);
        }

        public void UpdateResolution()
        {
            screenCanvas.Dimensions = new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight);
            screenCanvas.ParentResized(screenCanvas.Dimensions);
        }

        public void Update(GameTime gameTime)
        {
            GameStats.UIWasClicked = false;
            if (GameSettings.ui_DrawUI)
            {
                _sizeBlock.Text.Clear();
                _sizeBlock.Text.Append("Model Size: ");
                _sizeBlock.Text.Concat((float) Math.Pow(10, GameSettings.m_size), 2);

                screenCanvas.Update(gameTime, Input.GetMousePosition().ToVector2(), Vector2.Zero);


            }
        }

        public GUICanvas getCanvas()
        {
            return screenCanvas;
        }

    }
}

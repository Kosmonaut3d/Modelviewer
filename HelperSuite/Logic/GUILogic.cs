using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperSuite.HelperSuite.GUI;
using HelperSuite.HelperSuite.GUIRenderer;
using HelperSuite.HelperSuite.Static;
using Microsoft.Xna.Framework;

namespace HelperSuite.Logic
{
    public class GuiLogicSample
    {
        private GUICanvas screenCanvas;

        private GUIList baseList;

        private GUIStyle defaultStyle;

        public GuiLogicSample()
        {

            defaultStyle = new GUIStyle()
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
                Toggle = (bool)typeof(GameSettings).GetField("ui_debug").GetValue(null),
            });
            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(100, 35), "toggle2", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(100, 35), "this is a generic testblock and it tests wrap ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "this is a generic testblock and it tests wrap ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.DimGray, Color.Red )
            {
                SliderField = typeof(GameSettings).GetField("testvaluer"),
                SliderValue = (float)typeof(GameSettings).GetField("testvaluer").GetValue(null),
            });
            baseList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.DimGray, Color.Green)
            {
                SliderField = typeof(GameSettings).GetField("testvalueg"),
                SliderValue = (float)typeof(GameSettings).GetField("testvalueg").GetValue(null),
            });
            baseList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.DimGray, Color.Blue)
            {
                SliderField = typeof(GameSettings).GetField("testvalueb"),
                SliderValue = (float)typeof(GameSettings).GetField("testvalueb").GetValue(null),
            });
        }

        public void UpdateResolution()
        {
            screenCanvas.Dimensions = new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight);
            screenCanvas.ParentResized(screenCanvas.Dimensions);
        }

        public void Update(GameTime gameTime)
        {
            if(GameSettings.ui_DrawUI)
            screenCanvas.Update(gameTime, Input.GetMousePosition().ToVector2(), Vector2.Zero);
        }

        public GUICanvas getCanvas()
        {
            return screenCanvas;
        }

    }
}

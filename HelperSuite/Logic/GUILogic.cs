using System;
using HelperSuite.HelperSuite.GUI;
using HelperSuite.HelperSuite.GUIHelper;
using HelperSuite.HelperSuite.GUIRenderer;
using HelperSuite.HelperSuite.GUIRenderer.Helper;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Renderer.ShaderModules.Helper;
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
        private GUITextBlock _roughnessBlock;
        private GUITextBlock _metallicBlock;

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

            GUIList animationList = new GUIList(Vector2.Zero, new Vector2(200,70), 0, GUIStyle.GUIAlignment.BottomLeft, screenCanvas.Dimensions);
            animationList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "play animation", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = this.GetType().GetMethod("PlayAnimation"),
            });
            animationList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "Update Animation", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ToggleField = typeof(GameSettings).GetField("m_updateAnimation"),
                Toggle = (bool)typeof(GameSettings).GetField("m_updateAnimation").GetValue(null)
            });

            screenCanvas.AddElement(animationList);

            baseList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);
            
            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "debug info", GUIRenderer.MonospaceFont, Color.Gray, Color.White )
            {
                ToggleField = typeof(GameSettings).GetField("ui_debug"),
                Toggle = (bool)typeof(GameSettings).GetField("ui_debug").GetValue(null)
            });
            //baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "this is a generic testblock and it tests wrap ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 3, Color.Gray, Color.Black )
            {
                SliderField = typeof(GameSettings).GetField("m_size"),
                SliderValue = (float)typeof(GameSettings).GetField("m_size").GetValue(null)
            });
            baseList.AddElement(_sizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Size: " + GameSettings.m_size, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            
            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "Orientation: Y", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ToggleField = typeof(GameSettings).GetField("m_orientationy"),
                Toggle = (bool)typeof(GameSettings).GetField("m_orientationy").GetValue(null)
            });
            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "Linear Lighting", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ToggleField = typeof(GameSettings).GetField("r_UseLinear"),
                Toggle = (bool)typeof(GameSettings).GetField("r_UseLinear").GetValue(null)
            });
            GuiTextBlockLoadDialog modelLoader;
            baseList.AddElement(modelLoader = new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Model:", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = _guiContentLoader,
                ButtonMethod = _guiContentLoader.GetType().GetMethod("LoadContentFile").MakeGenericMethod(typeof(AnimatedModel)),
            }
            );
            mainLogic.modelLoader = modelLoader;

            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Textures", GUIRenderer.MonospaceFont, Color.DimGray, Color.White, GUIStyle.TextAlignment.Center));
            GuiListToggle textureList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.None, screenCanvas.Dimensions);

            {
                GuiTextBlockLoadDialog albedoLoader;
                textureList.AddElement(
                    albedoLoader =
                        new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Albedo:",
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                        {
                            ButtonObject = _guiContentLoader,
                            ButtonMethod =
                                _guiContentLoader.GetType()
                                    .GetMethod("LoadContentFile")
                                    .MakeGenericMethod(typeof(Texture2D))
                        }
                );
                mainLogic.albedoLoader = albedoLoader;

                GuiTextBlockLoadDialog normalLoader;
                textureList.AddElement(
                    normalLoader =
                        new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Normal:",
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                        {
                            ButtonObject = _guiContentLoader,
                            ButtonMethod =
                                _guiContentLoader.GetType()
                                    .GetMethod("LoadContentFile")
                                    .MakeGenericMethod(typeof(Texture2D))
                        }
                );
                mainLogic.normalLoader = normalLoader;

                GuiTextBlockLoadDialog roughnessLoader;
                textureList.AddElement(
                    roughnessLoader =
                        new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Roughness:",
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                        {
                            ButtonObject = _guiContentLoader,
                            ButtonMethod =
                                _guiContentLoader.GetType()
                                    .GetMethod("LoadContentFile")
                                    .MakeGenericMethod(typeof(Texture2D))
                        }
                );
                mainLogic.roughnessLoader = roughnessLoader;

                GuiTextBlockLoadDialog metallicLoader;
                textureList.AddElement(
                    metallicLoader =
                        new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Metallic:",
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                        {
                            ButtonObject = _guiContentLoader,
                            ButtonMethod =
                                _guiContentLoader.GetType()
                                    .GetMethod("LoadContentFile")
                                    .MakeGenericMethod(typeof(Texture2D))
                        }
                );
                mainLogic.metallicLoader = metallicLoader;
            }
            baseList.AddElement(textureList);
            textureList.IsToggled = false;

            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Default Values", GUIRenderer.MonospaceFont, Color.DimGray, Color.White, GUIStyle.TextAlignment.Center));
            GuiListToggle colorList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.None, screenCanvas.Dimensions);
            {
                colorList.AddElement(new GUIColorPicker(new Vector2(0, 0), new Vector2(200, 200), Color.Gray,
                    GUIRenderer.MonospaceFont)
                {
                    ToggleField = typeof(GameSettings).GetField("bgColor"),
                    ToggleObject = (Color) typeof(GameSettings).GetField("bgColor").GetValue(null)
                });

                colorList.AddElement(
                    _roughnessBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "Roughness: " + GameSettings.m_roughness,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                colorList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("m_roughness"),
                    SliderValue = (float) typeof(GameSettings).GetField("m_roughness").GetValue(null)
                });

                colorList.AddElement(
                    _metallicBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 35), "Metallic: " + GameSettings.m_metallic,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                colorList.AddElement(new GuiSlider(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("m_metallic"),
                    SliderValue = (float) typeof(GameSettings).GetField("m_metallic").GetValue(null)
                });
            }
            baseList.AddElement(colorList);
            colorList.IsToggled = false;
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

        public void PlayAnimation()
        {
            GameSettings.m_startClip = true;
        }

        public void Update(GameTime gameTime)
        {
            GameStats.UIWasClicked = false;
            if (GameSettings.ui_DrawUI)
            {
                _sizeBlock.Text.Clear();
                _sizeBlock.Text.Append("Model Size: ");
                _sizeBlock.Text.Concat((float) Math.Pow(10, GameSettings.m_size), 2);

                _roughnessBlock.Text.Clear();
                _roughnessBlock.Text.Append("Roughness: ");
                _roughnessBlock.Text.Concat(GameSettings.m_roughness, 2);

                _metallicBlock.Text.Clear();
                _metallicBlock.Text.Append("Metallic: ");
                _metallicBlock.Text.Concat(GameSettings.m_metallic, 2);

                screenCanvas.Update(gameTime, Input.GetMousePosition().ToVector2(), Vector2.Zero);


            }
        }

        public GUICanvas getCanvas()
        {
            return screenCanvas;
        }

    }
}

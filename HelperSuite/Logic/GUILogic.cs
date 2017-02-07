using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModelViewer.HelperSuite.GUI;
using ModelViewer.HelperSuite.GUIHelper;
using ModelViewer.HelperSuite.GUIRenderer;
using ModelViewer.HelperSuite.GUIRenderer.Helper;
using ModelViewer.HelperSuite.Static;
using ModelViewer.Renderer.ShaderModules.Helper;

namespace ModelViewer.Logic
{
    public class GuiLogicSample
    {
        private GUICanvas screenCanvas;

        private GUIList baseList;
        private GUITextBlock _sizeBlock;
        private GUITextBlock _roughnessBlock;
        private GUITextBlock _metallicBlock;
        private GUITextBlock _pomBlock;
        private GUITextBlock _pomQualityBlock;

        private GUITextBlock _aoRadiiBlock;
        private GUITextBlock _aoSamplesBlock;
        private GUITextBlock _aoStrengthBlock;

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

            baseList = new GuiListToggleScroll(new Vector2(-20,0), new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);
            
            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "debug info", GUIRenderer.MonospaceFont, Color.Gray, Color.White )
            {
                ToggleField = typeof(GameSettings).GetField("ui_debug"),
                Toggle = (bool)typeof(GameSettings).GetField("ui_debug").GetValue(null)
            });

            baseList.AddElement(_sizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Size: " + GameSettings.m_size, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), -2, 4, Color.Gray, Color.Black )
            {
                SliderField = typeof(GameSettings).GetField("m_size"),
                SliderValue = (float)typeof(GameSettings).GetField("m_size").GetValue(null)
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 25), "Center Model", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = mainLogic,
                ButtonMethod = mainLogic.GetType().GetMethod("CenterModel")
            });
            
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

                GuiTextBlockLoadDialog bumpLoader;
                textureList.AddElement(
                    bumpLoader =
                        new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "Heightmap:",
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                        {
                            ButtonObject = _guiContentLoader,
                            ButtonMethod =
                                _guiContentLoader.GetType()
                                    .GetMethod("LoadContentFile")
                                    .MakeGenericMethod(typeof(Texture2D))
                        }
                );
                mainLogic.bumpLoader = bumpLoader;
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
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Roughness: " + GameSettings.m_roughness,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                colorList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), 0, 1, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("m_roughness"),
                    SliderValue = (float) typeof(GameSettings).GetField("m_roughness").GetValue(null)
                });

                colorList.AddElement(
                    _metallicBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Metallic: " + GameSettings.m_metallic,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                colorList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), 0, 1, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("m_metallic"),
                    SliderValue = (float) typeof(GameSettings).GetField("m_metallic").GetValue(null)
                });
            }
            colorList.IsToggled = false;
            baseList.AddElement(colorList);

            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Bump Mapping", GUIRenderer.MonospaceFont, Color.DimGray, Color.White, GUIStyle.TextAlignment.Center));
            GuiListToggle parallaxList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.None, screenCanvas.Dimensions);
            {
                parallaxList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 40), "Parallax OcclusionMapping", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                {
                    ToggleField = typeof(GameSettings).GetField("r_UsePOM"),
                    Toggle = (bool)typeof(GameSettings).GetField("r_UsePOM").GetValue(null)
                });

                parallaxList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "POM Cutoff", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                {
                    ToggleField = typeof(GameSettings).GetField("r_POMCutoff"),
                    Toggle = (bool)typeof(GameSettings).GetField("r_POMCutoff").GetValue(null)
                });

                parallaxList.AddElement(
                    _pomBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Bump Scale: " + GameSettings.pomScale,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                parallaxList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), -0.5f, 0.5f, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("pomScale"),
                    SliderValue = (float)typeof(GameSettings).GetField("pomScale").GetValue(null)
                });

                parallaxList.AddElement(
                    _pomQualityBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "POM Quality: " + GameSettings.r_POMQuality,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                parallaxList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), 0.5f, 3, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("r_POMQuality"),
                    SliderValue = (float)typeof(GameSettings).GetField("r_POMQuality").GetValue(null)
                });
            }
            parallaxList.IsToggled = false;
            baseList.AddElement(parallaxList);

            //AO

            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Ambient Occlusion", GUIRenderer.MonospaceFont, Color.DimGray, Color.White, GUIStyle.TextAlignment.Center));
            GuiListToggle aoList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.None, screenCanvas.Dimensions);
            {
                aoList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "Enable AO", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                {
                    ToggleField = typeof(GameSettings).GetField("ao_Enable"),
                    Toggle = (bool)typeof(GameSettings).GetField("ao_Enable").GetValue(null)
                });

                aoList.AddElement(
                    _aoRadiiBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "AO Radius: " + GameSettings.ao_Radii,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));
                aoList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), 0, 8, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("ao_Radii"),
                    SliderValue = (float)typeof(GameSettings).GetField("ao_Radii").GetValue(null)
                });

                aoList.AddElement(
                    _aoSamplesBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "AO Samples ppx: " + GameSettings.ao_Samples,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));

                aoList.AddElement(new GuiSliderInt(Vector2.Zero, new Vector2(200, 25), 0, 64, 2, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("ao_Samples"),
                    SliderValue = (int)typeof(GameSettings).GetField("ao_Samples").GetValue(null)
                });

                aoList.AddElement(
                    _aoStrengthBlock =
                        new GUITextBlock(Vector2.Zero, new Vector2(200,25), "AO Strength: " + GameSettings.ao_Strength,
                            GUIRenderer.MonospaceFont, Color.Gray, Color.White));

                aoList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 25), 0, 4, Color.Gray, Color.Black)
                {
                    SliderField = typeof(GameSettings).GetField("ao_Strength"),
                    SliderValue = (float)typeof(GameSettings).GetField("ao_Strength").GetValue(null)
                });

                aoList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "Blur AO", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                {
                    ToggleField = typeof(GameSettings).GetField("ao_UseBlur"),
                    Toggle = (bool)typeof(GameSettings).GetField("ao_UseBlur").GetValue(null)
                });

                aoList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "Half resolution", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
                {
                    ToggleField = typeof(GameSettings).GetField("ao_HalfRes"),
                    Toggle = (bool)typeof(GameSettings).GetField("ao_HalfRes").GetValue(null)
                });

            }
            aoList.IsToggled = false;

            baseList.AddElement(aoList);

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
            GameStats.UIWasUsed = false;
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

                _pomBlock.Text.Clear();
                _pomBlock.Text.Append("Height Scale: ");
                _pomBlock.Text.Concat(GameSettings.pomScale,2);

                _pomQualityBlock.Text.Clear();
                _pomQualityBlock.Text.Append("POM Quality: ");
                _pomQualityBlock.Text.Concat(GameSettings.r_POMQuality, 2);

                _aoRadiiBlock.Text.Clear();
                _aoRadiiBlock.Text.Append("AO Radius: ");
                _aoRadiiBlock.Text.Concat(GameSettings.ao_Radii, 3);

                _aoSamplesBlock.Text.Clear();
                _aoSamplesBlock.Text.Append("AO Samples ppx: ");
                _aoSamplesBlock.Text.Concat(GameSettings.ao_Samples);

                _aoStrengthBlock.Text.Clear();
                _aoStrengthBlock.Text.Append("AO Strength: ");
                _aoStrengthBlock.Text.Concat(GameSettings.ao_Strength, 2);

                

                screenCanvas.Update(gameTime, Input.GetMousePosition().ToVector2(), Vector2.Zero);
            }

            //Safety
            if (!Input.IsLMBPressed() && GameStats.UIElementEngaged)
                GameStats.UIElementEngaged = false;
        }

        public GUICanvas getCanvas()
        {
            return screenCanvas;
        }

    }
}

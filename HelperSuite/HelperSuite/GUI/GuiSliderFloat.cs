using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModelViewer.HelperSuite.Static;
using ModelViewer.Logic;

namespace ModelViewer.HelperSuite.GUI
{
    class GuiSliderFloat : GUIBlock
    {
        protected bool IsEngaged = false;

        protected const float SliderIndicatorSize = 15;
        protected const float SliderIndicatorBorder = 10;
        protected const float SliderBaseHeight = 5;

        private Vector2 SliderPosition;

        protected float _sliderPercent;

        private float _sliderValue;
        public float SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                _sliderPercent = (_sliderValue - MinValue)/(MaxValue - MinValue);
            }
        }

        public float MaxValue = 1; 
        public float MinValue;

        protected Color _sliderColor;

        public PropertyInfo SliderProperty;
        public FieldInfo SliderField;
        public Object SliderObject;

        public GuiSliderFloat(Vector2 position, Vector2 dimensions, float min, float max, Color color, Color sliderColor, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = new Vector2()) : base(position, dimensions, color, layer, alignment, ParentDimensions)
        {
            _sliderColor = sliderColor;
            MinValue = min;
            MaxValue = max;
            _sliderValue = min;
        }

        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (GameStats.UIElementEngaged && !IsEngaged) return;

            //Break Engagement
            if (IsEngaged && !Input.IsLMBPressed())
            {
                GameStats.UIElementEngaged = false;
                IsEngaged = false;
            }

            if (!Input.IsLMBPressed()) return;

            Vector2 bound1 = Position + parentPosition /*+ SliderIndicatorBorder*Vector2.UnitX*/;
            Vector2 bound2 = bound1 + Dimensions/* - 2*SliderIndicatorBorder * Vector2.UnitX*/;

            if (mousePosition.X >= bound1.X && mousePosition.Y >= bound1.Y && mousePosition.X < bound2.X &&
                mousePosition.Y < bound2.Y + 1)
            {
                GameStats.UIElementEngaged = true;
                IsEngaged = true;
            }

            if (IsEngaged)
            {
                GameStats.UIWasUsed = true;

                float lowerx = bound1.X + SliderIndicatorBorder;
                float upperx = bound2.X - SliderIndicatorBorder;

                _sliderPercent = MathHelper.Clamp((mousePosition.X - lowerx)/(upperx - lowerx), 0, 1);

                _sliderValue = _sliderPercent*(MaxValue - MinValue) + MinValue;

                if (SliderObject != null)
                {
                    if (SliderField != null)
                        SliderField.SetValue(SliderObject, SliderValue, BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(SliderObject, SliderValue);
                }
                else
                {
                    if (SliderField != null)
                        SliderField.SetValue(null, SliderValue, BindingFlags.Static | BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(null, SliderValue);
                }
            }
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            guiRenderer.DrawQuad(parentPosition + Position, Dimensions, Color);
            
            Vector2 slideDimensions = new Vector2(Dimensions.X - SliderIndicatorBorder*2, SliderBaseHeight);
            guiRenderer.DrawQuad(parentPosition + Position + new Vector2(SliderIndicatorBorder, 
                Dimensions.Y* 0.5f - SliderBaseHeight * 0.5f), slideDimensions, Color.DarkGray);

            //slideDimensions = new Vector2(slideDimensions.X + SliderIndicatorSize* 0.5f, slideDimensions.Y);
            guiRenderer.DrawQuad(parentPosition + Position + new Vector2(SliderIndicatorBorder - SliderIndicatorSize* 0.5f,
                 Dimensions.Y * 0.5f - SliderIndicatorSize * 0.5f) + _sliderPercent*slideDimensions * Vector2.UnitX, new Vector2(SliderIndicatorSize, SliderIndicatorSize), _sliderColor);
        }
    }

    class GuiSliderInt : GuiSliderFloat
    {
        public int MaxValueInt = 1;
        public int MinValueInt = 0;
        public int StepSize = 1;

        public int _sliderValue;
        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                _sliderPercent = (float)(_sliderValue - MinValue) / (MaxValue - MinValue);
            }
        }

        public GuiSliderInt(Vector2 position, Vector2 dimensions, int min, int max, int stepSize, Color color, Color sliderColor, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = new Vector2()) : base(position, dimensions, min, max, color, sliderColor, layer, alignment, ParentDimensions)
        {
            MaxValueInt = max;
            MinValueInt = min;
            StepSize = stepSize;
        }


        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (GameStats.UIElementEngaged && !IsEngaged) return;

            //Break Engagement
            if (IsEngaged && !Input.IsLMBPressed())
            {
                GameStats.UIElementEngaged = false;
                IsEngaged = false;
            }

            if (!Input.IsLMBPressed()) return;

            Vector2 bound1 = Position + parentPosition /*+ SliderIndicatorBorder*Vector2.UnitX*/;
            Vector2 bound2 = bound1 + Dimensions/* - 2*SliderIndicatorBorder * Vector2.UnitX*/;

            if (mousePosition.X >= bound1.X && mousePosition.Y >= bound1.Y && mousePosition.X < bound2.X &&
                mousePosition.Y < bound2.Y + 1)
            {
                GameStats.UIElementEngaged = true;
                IsEngaged = true;
            }

            if (IsEngaged)
            {
                GameStats.UIWasUsed = true;

                float lowerx = bound1.X + SliderIndicatorBorder;
                float upperx = bound2.X - SliderIndicatorBorder;

                _sliderPercent = MathHelper.Clamp((mousePosition.X - lowerx) / (upperx - lowerx), 0, 1);

                _sliderValue = (int) (_sliderPercent * (float)(MaxValue - MinValue) + MinValue) / StepSize * StepSize;

                _sliderPercent = (float)_sliderValue/( MaxValueInt - MinValueInt);

                if (SliderObject != null)
                {
                    if (SliderField != null) SliderField.SetValue(SliderObject, SliderValue, BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(SliderObject, SliderValue);
                }
                else
                {
                    if (SliderField != null) SliderField.SetValue(null, SliderValue, BindingFlags.Static | BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(null, SliderValue);
                }
            }
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            guiRenderer.DrawQuad(parentPosition + Position, Dimensions, Color);

            Vector2 slideDimensions = new Vector2(Dimensions.X - SliderIndicatorBorder * 2, SliderBaseHeight);
            guiRenderer.DrawQuad(parentPosition + Position + new Vector2(SliderIndicatorBorder,
                Dimensions.Y * 0.5f - SliderBaseHeight * 0.5f), slideDimensions, Color.DarkGray);

            //slideDimensions = new Vector2(slideDimensions.X + SliderIndicatorSize* 0.5f, slideDimensions.Y);
            guiRenderer.DrawQuad(parentPosition + Position + new Vector2(SliderIndicatorBorder - SliderIndicatorSize * 0.5f,
                 Dimensions.Y * 0.5f - SliderIndicatorSize * 0.5f) + _sliderPercent * slideDimensions * Vector2.UnitX, new Vector2(SliderIndicatorSize, SliderIndicatorSize), _sliderColor);
        }

    }
}
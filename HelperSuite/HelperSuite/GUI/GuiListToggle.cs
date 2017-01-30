using HelperSuite.HelperSuite.Static;
using HelperSuite.Logic;
using Microsoft.Xna.Framework;

namespace HelperSuite.HelperSuite.GUI
{
    class GuiListToggle : GUIList
    {
        private static float ToggleButtonHeight = 14;
        private static float ArrowButtonHeight = 8;

        private static readonly Color HoverColor = Color.Tomato;
        
        private Vector2 _toggleDimensions;

        private bool _isHovered;

        public bool IsToggled = true;

        public GuiListToggle(Vector2 position, Vector2 defaultDimensions, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = new Vector2()) : base(position, defaultDimensions, layer, alignment, ParentDimensions)
        {
            _toggleDimensions = new Vector2(defaultDimensions.X, ToggleButtonHeight);
        }

        public override void AddElement(GUIElement element)
        {
            // I think it is acceptable to make a for loop everytime an element is added
            float height = ToggleButtonHeight;

            for (int i = 0; i < _children.Count; i++)
            {
                height += _children[i].Dimensions.Y;
            }
            //element.Position = new Vector2(0, height);

            DefaultDimensions = new Vector2(DefaultDimensions.X, height + element.Dimensions.Y);
            
            //In Order
            _children.Add(element);
        }

        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (!IsEnabled) return;

            _isHovered = false;

            Vector2 bound1 = Position + parentPosition;
            Vector2 bound2 = bound1 + _toggleDimensions;

            if (mousePosition.X >= bound1.X && mousePosition.Y >= bound1.Y && mousePosition.X < bound2.X &&
                mousePosition.Y < bound2.Y)
            {
                _isHovered = true;
                if(Input.WasLMBClicked())
                {
                    GameStats.UIWasClicked = true;

                    IsToggled = !IsToggled;
                }
            }

            if (IsToggled)
            {
                float height = ToggleButtonHeight;
                for (int index = 0; index < _children.Count; index++)
                {
                    GUIElement child = _children[index];
                    child.Update(gameTime, mousePosition, parentPosition + Position + height*Vector2.UnitY);

                    height += _children[index].Dimensions.Y;
                }
            }
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            if (!IsEnabled) return;
            //Draw toggle element
            guiRenderer.DrawQuad(parentPosition + Position, _toggleDimensions, _isHovered ? HoverColor : Color.DimGray);

            //arrow
            if (IsToggled)
            {
                guiRenderer.DrawQuad(
                    parentPosition + Position + _toggleDimensions*0.5f - ArrowButtonHeight*Vector2.One*0.5f,
                    new Vector2(ArrowButtonHeight, ArrowButtonHeight*0.25f), Color.White);
                guiRenderer.DrawQuad(
                    parentPosition + Position + _toggleDimensions*0.5f - ArrowButtonHeight*Vector2.UnitX*0.25f,
                    new Vector2(ArrowButtonHeight, ArrowButtonHeight*0.5f)*0.5f, Color.White);
            }
            else
            {
                guiRenderer.DrawQuad(
                    parentPosition + Position + _toggleDimensions * 0.5f - ArrowButtonHeight * Vector2.UnitX * 0.5f,
                    new Vector2(ArrowButtonHeight, ArrowButtonHeight * 0.25f), Color.White);
                guiRenderer.DrawQuad(
                    parentPosition + Position + _toggleDimensions * 0.5f - ArrowButtonHeight * new Vector2(0.25f, 0.5f),
                    new Vector2(ArrowButtonHeight, ArrowButtonHeight * 0.5f) * 0.5f, Color.White);
            }

            if (IsToggled)
            {
                float height = ToggleButtonHeight;

                for (int index = 0; index < _children.Count; index++)
                {
                    GUIElement child = _children[index];
                    child.Draw(guiRenderer, parentPosition + Position + height * Vector2.UnitY, mousePosition);

                    height += _children[index].Dimensions.Y;
                }
            }
        }
    }
}
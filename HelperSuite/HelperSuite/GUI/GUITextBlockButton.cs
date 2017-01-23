using System;
using System.Reflection;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.HelperSuite.GUI
{
    public class GUITextBlockButton : GUITextBlock
    {
        public bool Toggle;
        
        private static readonly float ButtonBorder = 2;

        private static readonly Color HoverColor = Color.Tomato;
        
        private bool _isHovered = false;
        
        public MethodInfo ButtonMethod;
        public Object ButtonObject;

        public GUITextBlockButton(Vector2 position, Vector2 dimensions, String text, SpriteFont font, Color blockColor, Color textColor, GUIStyle.TextAlignment textAlignment = GUIStyle.TextAlignment.Center, Vector2 textBorder = default(Vector2), int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 parentDimensions = default(Vector2)) : base(position, dimensions, text, font, blockColor, textColor, textAlignment, textBorder, layer)
        {

        }
        

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition)
        {
            guiRenderer.DrawQuad(parentPosition + Position, Dimensions, Color.DimGray);
            guiRenderer.DrawQuad(parentPosition + Position + Vector2.One * ButtonBorder, Dimensions - 2*Vector2.One*ButtonBorder, _isHovered ? HoverColor : Color);
            guiRenderer.DrawText(parentPosition + Position + _fontPosition, Text, TextFont, TextColor);
        }

        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            _isHovered = false;

            Vector2 bound1 = Position + parentPosition;
            Vector2 bound2 = bound1 + Dimensions;

            if (mousePosition.X >= bound1.X && mousePosition.Y >= bound1.Y && mousePosition.X < bound2.X &&
                mousePosition.Y < bound2.Y)
            {
                _isHovered = true;

                if (!Input.WasLMBClicked()) return;

                GameStats.UIWasClicked = true;

                if (ButtonObject != null)
                {
                    if (ButtonMethod != null) ButtonMethod.Invoke(ButtonObject, null);
                }
            }
        }

    }
    
}
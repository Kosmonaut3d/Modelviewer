using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HelperSuite.HelperSuite.GUIHelper;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.HelperSuite.GUI
{
    public class GuiTextBlockLoadDialog : GUITextBlock
    {
        public bool Toggle;
        
        private static readonly float ButtonBorder = 2;

        private static readonly Color HoverColor = Color.LightGray;

        private Vector2 _declarationTextDimensions;
        
        private bool _isHovered = false;

        private short _isLoaded = 0; //0 -> 1 -> 2

        //Load
        private Task _loadTaskReference;
        private Object _loadedObject;
        private int _loadedObjectPointer = -1;
        private StringBuilder _loadedObjectName = new StringBuilder(100);
        
        public MethodInfo ButtonMethod;
        public GUIContentLoader ButtonObject;

        public GuiTextBlockLoadDialog(Vector2 position, Vector2 dimensions, String text, SpriteFont font, Color blockColor, Color textColor, GUIStyle.TextAlignment textAlignment = GUIStyle.TextAlignment.Center, Vector2 textBorder = default(Vector2), int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 parentDimensions = default(Vector2)) : base(position, dimensions, text, font, blockColor, textColor, textAlignment, textBorder, layer)
        {
            _loadedObjectName.Append("...");
        }

        protected override void ComputeFontPosition()
        {
            if (_text == null) return;
            _declarationTextDimensions = TextFont.MeasureString(_text);

            //Let's check wrap!

            //FontWrap(ref textDimension, Dimensions);
            
            _fontPosition = Dimensions * 0.5f * Vector2.UnitY + _textBorder * Vector2.UnitX - _declarationTextDimensions * 0.5f * Vector2.UnitY;
        }

        protected void ComputeObjectNameLength()
        {
            if (_loadedObjectName.Length > 0)
            {
                //Max length
                Vector2 textDimensions = TextFont.MeasureString(_loadedObjectName);

                float characterLength = textDimensions.X/_loadedObjectName.Length;

                Vector2 buttonLeft = (_declarationTextDimensions + _fontPosition * 1.5f) * Vector2.UnitX;
                Vector2 spaceAvailable = Dimensions - 2*Vector2.One*ButtonBorder - buttonLeft -
                                         (2 + _textBorder.X)*Vector2.UnitX;

                int characters = (int) (spaceAvailable.X/characterLength);

                _loadedObjectName.Length = characters < _loadedObjectName.Length ? characters : _loadedObjectName.Length;
            }
        }


        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            Vector2 buttonLeft = (_declarationTextDimensions + _fontPosition * 1.2f)*Vector2.UnitX;
            guiRenderer.DrawQuad(parentPosition + Position, Dimensions, Color);
            guiRenderer.DrawQuad(parentPosition + Position + buttonLeft + Vector2.One * ButtonBorder, Dimensions - 2*Vector2.One*ButtonBorder - buttonLeft - (2+_textBorder.X)*Vector2.UnitX, _isHovered ? HoverColor : Color.DimGray);

            Vector2 indicatorButton = parentPosition + new Vector2(Dimensions.X - (2 + _textBorder.X), Dimensions.Y/2 - 4);

            guiRenderer.DrawQuad(indicatorButton, Vector2.One * 8, _isLoaded < 1 ? Color.Red : (_isLoaded < 2 ? Color.Yellow : Color.LimeGreen ));

            guiRenderer.DrawText(parentPosition + Position + _fontPosition, Text, TextFont, TextColor);

            //Description
            guiRenderer.DrawText(parentPosition + Position + buttonLeft + new Vector2(4, _fontPosition.Y), _loadedObjectName, TextFont, TextColor);

            //Show texture if _isHovered

            if (_isHovered && _isLoaded == 2)
            {
                //compute position

                Vector2 position = mousePosition;

                float overborder = position.X + 200 - GameSettings.g_ScreenWidth;

                if (overborder > 0)
                    position.X -= overborder;

                guiRenderer.DrawImage(position, new Vector2(200,200), (Texture2D) ButtonObject.ContentArray[_loadedObjectPointer], Color.White, true);
            }

        }

        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (_loadTaskReference != null)
            {
                _isLoaded = (short) (_loadTaskReference.IsCompleted ? 2 : 1);
            }
            else
            {
                _isLoaded = 0;
            }

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
                    string s = null;
                    object[] args = new object[] {_loadTaskReference, _loadedObjectPointer, s};
                    if (ButtonMethod != null) ButtonMethod.Invoke(ButtonObject, args);
                    
                    _loadTaskReference = (Task) args[0];
                    _loadedObjectPointer = (int) args[1];
                    _loadedObjectName.Clear();
                    _loadedObjectName.Append((string)args[2]);

                    ComputeObjectNameLength();
                }
            }
        }

    }
    
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModelViewer.HelperSuite.GUI
{
    public class GUIStyle
    {
        public Color BlockColor;
        public Color TextColor;
        public SpriteFont TextFont;

        public enum GUIAlignment
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public enum TextAlignment
        {
            Left, Center, Right
        }
    }
}

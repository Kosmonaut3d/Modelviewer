using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.HelperSuite.GUI
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
        };

        public enum TextAlignment
        {
            Left, Center, Right
        };


        public GUIStyle()
        {
            
        }
    }
}

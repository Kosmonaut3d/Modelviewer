using System.Text;
using HelperSuite.HelperSuite.GUI;
using HelperSuite.HelperSuite.GUIRenderer.Helper;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.HelperSuite.GUIRenderer
{
    public class GUIRenderer
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        public Vector2 Resolution;
        
        private Texture2D _plainWhite;
        public static SpriteFont MonospaceFont;

        private int _foregroundIndex;
        private ForegroundImage[] foregroundImages = new ForegroundImage[10];
        public struct ForegroundImage
        {
            public Texture2D tex;
            public Vector2 pos;
            public Vector2 dim;
            public Color color;
        };
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _foregroundIndex = 0;
            
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            Resolution = new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight);

            _plainWhite = new Texture2D(graphicsDevice, 1,1);
            _plainWhite.SetData(new Color[] { Color.White });
        }

        public void Load(ContentManager content)
        {
            MonospaceFont = content.Load<SpriteFont>("Fonts/monospace");
        }
        
        public void Draw(GUICanvas canvas)
        {
            if (!GameSettings.ui_DrawUI) return;

            _foregroundIndex = 0;
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            _spriteBatch.Begin();
            canvas.Draw(this, Vector2.Zero, Input.GetMousePosition().ToVector2());

            //Now draw foregroundImages
            for (int index = 0; index <= _foregroundIndex-1; index++)
            {
                ForegroundImage image = foregroundImages[index];
                DrawImage(image.pos, image.dim, image.tex, image.color, false);
            }
            _spriteBatch.End();
        }

        public void DrawQuad(Vector2 pos, Vector2 dim, Color color)
        {
            _spriteBatch.Draw(_plainWhite, RectangleFromVectors(pos, dim), color);
        }

        public void DrawImage(Vector2 pos, Vector2 dim, Texture2D tex, Color color, bool drawLater)
        {
            if (!drawLater)
            {
                _spriteBatch.Draw(tex, RectangleFromVectors(pos, dim), color);
            }
            else
            {
                foregroundImages[_foregroundIndex].color = color;
                foregroundImages[_foregroundIndex].dim = dim;
                foregroundImages[_foregroundIndex].pos = pos;
                foregroundImages[_foregroundIndex].tex = tex;

                _foregroundIndex++;
            }
        }
    

        private Rectangle RectangleFromVectors(Vector2 pos, Vector2 dim)
        {
            return new Rectangle((int) pos.X, (int) pos.Y, (int) dim.X, (int) dim.Y);
        }

        public void DrawText(Vector2 position, StringBuilder text, SpriteFont textFont, Color textColor)
        {
            _spriteBatch.DrawString(textFont, text, new Vector2((int)position.X, (int)position.Y), textColor);
        }

        public void CalculateCoordinates(float x, float y, float w, float h, Vector2 resolution, out Vector2 v1, out Vector2 v2)
        {
            v1 = new Vector2(x, y) / resolution;
            v2 = new Vector2(x + w, y + h) / resolution;

            //Transform into VPS
            v1 = v1 * 2 - Vector2.One;
            v1.Y = -v1.Y;

            v2 = v2 * 2 - Vector2.One;
            v2.Y = -v2.Y;
        }

    }
}

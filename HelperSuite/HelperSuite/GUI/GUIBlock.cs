using Microsoft.Xna.Framework;

namespace HelperSuite.HelperSuite.GUI
{
    /// <summary>
    /// Just a colored block
    /// </summary>
    public class GUIBlock : GUIElement
    {
        public Color Color;

        public GUIBlock(Vector2 position, Vector2 dimensions, Color color, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = default(Vector2))
        {
            Position = position;
            Dimensions = dimensions;
            Color = color;
            Layer = layer;
            Alignment = alignment;
            if (Alignment != GUIStyle.GUIAlignment.None)
            {
                ParentResized(ParentDimensions);
            }
            
        }

        private GUIBlock()
        {
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            guiRenderer.DrawQuad(parentPosition+Position, Dimensions, Color);
        }

        public override void ParentResized(Vector2 dimensions)
        {
            Position = GUICanvas.UpdateAlignment(Alignment, dimensions, Dimensions, Position);
        }

        public override int Layer { get; set; }
        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            //;
        }
        

        public override GUIStyle.GUIAlignment Alignment { get; set; }
    }
}
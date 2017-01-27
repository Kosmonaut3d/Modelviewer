using Microsoft.Xna.Framework;

namespace HelperSuite.HelperSuite.GUI
{
    class GUIColorPicker : GUIBlock
    {
        public GUIColorPicker(Vector2 position, Vector2 dimensions, Color color, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = new Vector2()) : base(position, dimensions, color, layer, alignment, ParentDimensions)
        {
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            //guiRenderer.DrawQuad(parentPosition + Position, Dimensions, Color);
            guiRenderer.DrawColorQuad(parentPosition + Position, Dimensions, Color.White);
            guiRenderer.DrawQuad(parentPosition + Position, Dimensions * new Vector2(0.5f, 1), Color.White);
            guiRenderer.DrawColorQuad2(parentPosition + Position, Dimensions * new Vector2(0.5f, 1), Color.Red);
        }
    }
}
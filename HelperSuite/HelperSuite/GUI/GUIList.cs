using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ModelViewer.HelperSuite.GUI
{
    public class GUIList : GUIElement
    {
        public bool IsEnabled = true;
        public Vector2 DefaultDimensions;

        protected List<GUIElement> _children = new List<GUIElement>();

        /// <summary>
        /// A list has a unified width/height of the elements. Each element is rendered below the other one
        /// </summary>
        /// <param name="position"></param>
        /// <param name="defaultDimensions"></param>
        /// <param name="layer"></param>
        /// <param name="alignment"></param>
        /// <param name="ParentDimensions"></param>
        public GUIList(Vector2 position, Vector2 defaultDimensions, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = default(Vector2))
        {
            DefaultDimensions = defaultDimensions;
            Alignment = alignment;
            Position = position;
            OffsetPosition = position;
            Layer = layer;
            if (Alignment != GUIStyle.GUIAlignment.None)
            {
                ParentResized(ParentDimensions);
            }
        }

        //Draw the GUI, cycle through the children
        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            if (!IsEnabled) return;

            float height = 0;

            for (int index = 0; index < _children.Count; index++)
            {
                GUIElement child = _children[index];
                child.Draw(guiRenderer, parentPosition + Position + height * Vector2.UnitY, mousePosition);

                height += _children[index].Dimensions.Y;
            }
        }
        
        //Adjust things when resized
        public override void ParentResized(Vector2 parentDimensions)
        {
            //for (int index = 0; index < _children.Count; index++)
            //{
            //    GUIElement child = _children[index];
            //    child.ParentResized(ElementDimensions);
            //}

            Position = GUICanvas.UpdateAlignment(Alignment, parentDimensions, DefaultDimensions, Position, OffsetPosition);
        }
        

        public virtual void AddElement(GUIElement element)
        {
            //element.Position = new Vector2(0, _children.Count*DefaultDimensions.Y);
            //element.Dimensions = DefaultDimensions;

           
            // I think it is acceptable to make a for loop everytime an element is added
            float height = 0;
            for (int i = 0; i < _children.Count; i++)
            {
                height += _children[i].Dimensions.Y;
            }
            //element.Position = new Vector2(0,height);

            DefaultDimensions = new Vector2(DefaultDimensions.X, height+element.Dimensions.Y);
            
            //In Order
            _children.Add(element);
        }

        public override int Layer { get; set; }

        /// <summary>
        /// Update our logic
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="mousePosition"></param>
        /// <param name="parentPosition"></param>
        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (!IsEnabled) return;

            float height = 0;
            for (int index = 0; index < _children.Count; index++)
            {
                GUIElement child = _children[index];
                child.Update(gameTime, mousePosition, parentPosition + Position + height * Vector2.UnitY);

                height += _children[index].Dimensions.Y;
            }
        }

        public override GUIStyle.GUIAlignment Alignment { get; set; }
    }
}
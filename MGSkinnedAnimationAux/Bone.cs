using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MGSkinnedAnimationAux
{
    /// <summary>
    /// Bones in this model are represented by this class, which
    /// allows a bone to have more detail associatd with it.
    /// 
    /// This class allows you to manipulate the local coordinate system
    /// for objects by changing the scaling, translation, and rotation.
    /// These are indepenent of the bind transformation originally supplied
    /// for the model. So, the actual transformation for a bone is
    /// the product of the:
    /// 
    /// Scaling
    /// Bind scaling (scaling removed from the bind transform)
    /// Rotation
    /// Translation
    /// Bind Transformation
    /// Parent Absolute Transformation
    /// 
    /// </summary>
	public class Bone
    {
        #region Fields

        /// <summary>
        /// Any parent for this bone
        /// </summary>
        private Bone parent = null;

        /// <summary>
        /// The children of this bone
        /// </summary>
        private List<Bone> children = new List<Bone>();

        /// <summary>
        /// The bind transform is the transform for this bone
        /// as loaded from the original model. It's the base pose.
        /// I do remove any scaling, though.
        /// </summary>
        private Matrix bindTransform = Matrix.Identity;

        /// <summary>
        /// The bind scaling component extracted from the bind transform
        /// </summary>
        private Vector3 bindScale = Vector3.One;

        /// <summary>
        /// Any translation applied to the bone
        /// </summary>
        private Vector3 translation = Vector3.Zero;

        /// <summary>
        /// Any rotation applied to the bone
        /// </summary>
        private Quaternion rotation = Quaternion.Identity;

        /// <summary>
        /// Any scaling applied to the bone
        /// </summary>
        private Vector3 scale = Vector3.One;

        #endregion 

        #region Properties

        /// <summary>
        /// The bone name
        /// </summary>
		public string Name = "";

        /// <summary>
        /// The bone bind transform
        /// </summary>
        public Matrix BindTransform { get {return bindTransform;} }

        /// <summary>
        /// Inverse of absolute bind transform for skinnning
        /// </summary>
        public Matrix SkinTransform { get; set; }

        /// <summary>
        /// Bone rotation
        /// </summary>
		public Quaternion Rotation {get {return rotation;} set {rotation = value;}}

        /// <summary>
        /// Any translations
        /// </summary>
		public Vector3 Translation {get {return translation;} set {translation = value;}}

        /// <summary>
        /// Any scaling
        /// </summary>
        public Vector3 Scale { get { return scale; } set { scale = value; } }

        /// <summary>
        /// The parent bone or null for the root bone
        /// </summary>
        public Bone Parent { get { return parent; } }

        /// <summary>
        /// The children of this bone
        /// </summary>
        public List<Bone> Children { get { return children; } }

        /// <summary>
        /// The bone absolute transform
        /// </summary>
        public Matrix AbsoluteTransform = Matrix.Identity;

        #endregion

        #region Operations

        /// <summary>
        /// Constructor for a bone object
        /// </summary>
        /// <param name="name">The name of the bone</param>
        /// <param name="bindTransform">The initial bind transform for the bone</param>
        /// <param name="parent">A parent for this bone</param>
        public Bone(string name, Matrix bindTransform, Bone parent)
        {
            this.Name = name;
            this.parent = parent;
            if (parent != null)
                parent.children.Add(this);

            // I am not supporting scaling in animation in this
            // example, so I extract the bind scaling from the 
            // bind transform and save it. 

            this.bindScale = new Vector3(bindTransform.Right.Length(),
                bindTransform.Up.Length(), bindTransform.Backward.Length());

            bindTransform.Right = bindTransform.Right / bindScale.X;
            bindTransform.Up = bindTransform.Up / bindScale.Y;
            bindTransform.Backward = bindTransform.Backward / bindScale.Y;
            this.bindTransform = bindTransform;

            // Set the skinning bind transform
            // That is the inverse of the absolute transform in the bind pose

            ComputeAbsoluteTransform();
            SkinTransform = Matrix.Invert(AbsoluteTransform);
        }

        /// <summary>
        /// Compute the absolute transformation for this bone.
        /// </summary>
        public void ComputeAbsoluteTransform()
        {
            Matrix transform = Matrix.CreateScale(Scale * bindScale) *
                Matrix.CreateFromQuaternion(Rotation) *
                Matrix.CreateTranslation(Translation) *
                BindTransform;

            if (Parent != null)
            {
                // This bone has a parent bone
                AbsoluteTransform = transform * Parent.AbsoluteTransform;
            }
            else
            {   // The root bone
                AbsoluteTransform = transform;
            }
        }

        /// <summary>
        /// This sets the rotation and translation such that the
        /// rotation times the translation times the bind after set
        /// equals this matrix. This is used to set animation values.
        /// </summary>
        /// <param name="m">A matrix include translation and rotation</param>
        public void SetCompleteTransform(Matrix m)
        {
            Matrix setTo = m * Matrix.Invert(BindTransform);

            Translation = setTo.Translation;
            Rotation = Quaternion.CreateFromRotationMatrix(setTo);
        }

        #endregion

    }
}

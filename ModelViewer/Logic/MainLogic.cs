using System;
using System.IO;
using System.Windows.Forms;
using HelperSuite.ContentLoader;
using HelperSuite.GUIHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModelViewer.Static;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using GuiTextBlockLoadDialog = ModelViewer.HelperSuiteExtension.GUI.GuiTextBlockLoadDialog;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ModelViewer.Logic
{
    public class MainLogic
    {
        private ContentManager _contentManager;
        
        public Texture2D loadedTexture;
        public GuiTextBlockLoadDialog modelLoader;
        public GuiTextBlockLoadDialog albedoLoader;
        public GuiTextBlockLoadDialog normalLoader;
        public GuiTextBlockLoadDialog roughnessLoader;
        public GuiTextBlockLoadDialog metallicLoader;
        public GuiTextBlockLoadDialog bumpLoader;
        private Texture2D rollTexture2D;
        private SpriteBatch _spriteBatch;
        private Camera _camera;
        public Vector3 modelPosition;
        public Matrix modelRotation = Matrix.Identity;
        private double defaultphi = 0;


        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);

            _camera = new Camera(new Vector3(15,-15,12), new Vector3(0,0,0));
        }

        public void Load(ContentManager contentManager)
        {
            _contentManager = new ThreadSafeContentManager(contentManager.ServiceProvider) {RootDirectory = "Content"};

            rollTexture2D = _contentManager.Load<Texture2D>("Graphical User Interface/ring");

        }

        public void CenterModel()
        {
            modelPosition = Vector3.Zero;
            modelRotation = Matrix.Identity;

            defaultphi = 0;
        }

        private void MouseEvents(Camera camera, ref Vector3 position)
        {
            if (!DebugScreen.ConsoleOpen && Input.WasKeyPressed(Keys.Space))
            {
                CenterModel();
            }
            if (Input.mouseState.ScrollWheelValue != Input.mouseLastState.ScrollWheelValue)
            {
                camera.DistanceFromCenter += ((float)(Input.mouseState.ScrollWheelValue - Input.mouseLastState.ScrollWheelValue)/100);
                camera.DistanceFromCenter = camera.DistanceFromCenter.Clamp(0.01f, 1000);

                Vector3 length = (camera.Position - camera.Lookat);
                length.Normalize();
                //camera.Forward = new Vector3(x, y, z);
                camera.Position = camera.Lookat + length*camera.DistanceFromCenter;
            }
            if (Input.mouseState.RightButton == ButtonState.Pressed)
            {
                if (!GameSettings.RotateOrbit)
                {
                    float mouseAmount = 0.01f;

                    Vector3 direction = camera.Forward;
                    direction.Normalize();

                    Vector3 normal = Vector3.Cross(direction, camera.Up);

                    float y = Input.mouseState.Y - Input.mouseLastState.Y;
                    float x = Input.mouseState.X - Input.mouseLastState.X;

                    y *= GameSettings.g_ScreenHeight/800.0f;
                    x *= GameSettings.g_ScreenWidth/1280.0f;

                    camera.Forward += x*mouseAmount*normal;

                    camera.Forward -= y*mouseAmount*camera.Up;
                    camera.Forward.Normalize();
                }
                else
                {
                    var tan = Math.Atan2(-camera.Forward.X, -camera.Forward.Y);

                    //lookat

                    //tan += (float)(_mouseState.X - _lastMouseState.X) / 60;

                    //Convert into spherical coordinates.
                    var x = camera.Forward.X;
                    var y = camera.Forward.Y;
                    var z = camera.Position.Z;

                    var r = camera.DistanceFromCenter; //Math.Sqrt(x*x + y*y + z*z);
                    var tau = Math.Acos(z/r);
                    var phi = Math.Atan2(y, x);

                    phi += (Input.mouseState.X - Input.mouseLastState.X)/60.0f;
                    if(!Input.IsKeyDown(Keys.LeftControl))
                    tau = (tau + (Input.mouseState.Y - Input.mouseLastState.Y)/100.0f).Clamp(0.01f, Math.PI - 0.1f);

                    //Convert back
                    x = (float) (r*Math.Sin(tau)*Math.Cos(phi));
                    y = (float) (r*Math.Sin(tau)*Math.Sin(phi));
                    z = (float) (r*Math.Cos(tau));

                    //camera.Forward = new Vector3(x, y, 0);
                    camera.Position = camera.Lookat + new Vector3(-x, -y, z);
                    camera.Forward = (camera.Lookat - camera.Position);

                    if (!Input.IsKeyDown(Keys.LeftAlt))
                    {
                        r = modelPosition.Length(); //Math.Sqrt(x*x + y*y + z*z);
                        tau = Math.Acos(modelPosition.Z / r);
                        phi = Math.Atan2(modelPosition.Y, modelPosition.X);

                        phi += (Input.mouseState.X - Input.mouseLastState.X) / 60.0f;
                        tau = (tau + (Input.mouseState.Y - Input.mouseLastState.Y) / 100.0f).Clamp(0.01f, Math.PI - 0.1f);

                        //Convert back
                        x = (float)(r * Math.Sin(tau) * Math.Cos(phi));
                        y = (float)(r * Math.Sin(tau) * Math.Sin(phi));
                        z = modelPosition.Z;// (float)(r * Math.Cos(tau));
                        modelPosition = new Vector3(x,y,z);
                    }

                    if (Input.IsKeyDown(Keys.LeftShift))
                    {
                        defaultphi += (Input.mouseState.X - Input.mouseLastState.X) / 60.0f;
                        modelRotation = Matrix.CreateRotationZ((float) (defaultphi));
                    }
                }
            }
            if (Input.mouseState.LeftButton == ButtonState.Pressed)
            {
                float x = (Input.mouseState.X - Input.mouseLastState.X) / 1000.0f * camera.DistanceFromCenter;
                float y = -(Input.mouseState.Y - Input.mouseLastState.Y) / 1000.0f * camera.DistanceFromCenter;
                
                Vector3 normal = Vector3.Cross(camera.Up, camera.Forward);
                Vector3 realup = Vector3.Cross(normal, camera.Forward);
                
                normal.Normalize();
                realup.Normalize();

                position += normal * x + realup * y;
                //camera.Position += normal * x + realup * y;

            }

        }
        
        public void Update(GameTime gameTime)
        {
            if(!GUIControl.UIWasUsed)
            MouseEvents(_camera, ref modelPosition);      
        }

        public void Draw(GameTime gameTime)
        {
            
        }

        public void Unload()
        {
            string MGCBpathDirectory = Application.StartupPath + "/Content/Runtime";
            DirectoryInfo di = new DirectoryInfo(MGCBpathDirectory);

            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            _contentManager.Dispose();
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}

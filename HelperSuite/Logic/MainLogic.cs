using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelperSuite.HelperSuite.ContentLoader;
using HelperSuite.HelperSuite.GUI;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace HelperSuite.Logic
{
    public class MainLogic
    {
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        private Task _task;
        
        public Texture2D loadedTexture;
        public GuiTextBlockLoadDialog modelLoader;
        private Texture2D rollTexture2D;
        private SpriteBatch _spriteBatch;
        private Camera _camera;


        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            _camera = new Camera(new Vector3(5,5,1), new Vector3(0,0,0));
        }

        public void Load(ContentManager contentManager)
        {
            _contentManager = new ThreadSafeContentManager(contentManager.ServiceProvider) {RootDirectory = "Content"};

            rollTexture2D = _contentManager.Load<Texture2D>("Graphical User Interface/ring");

        }


        private static void MouseEvents(Camera camera)
        {

            if (Input.mouseState.ScrollWheelValue != Input.mouseLastState.ScrollWheelValue)
            {
                camera.DistanceFromCenter += ((float)(Input.mouseState.ScrollWheelValue - Input.mouseLastState.ScrollWheelValue)/100);
                camera.DistanceFromCenter = camera.DistanceFromCenter.Clamp(0.01f, 1000);

                Vector3 length = (camera.Position - camera.Lookat);
                length.Normalize();
                //camera.Forward = new Vector3(x, y, z);
                camera.Position = camera.Lookat + length*camera.DistanceFromCenter;
            }
            if (Input.mouseState.LeftButton == ButtonState.Pressed)
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
                    tau = (tau + (Input.mouseState.Y - Input.mouseLastState.Y)/100.0f).Clamp(0.01f, Math.PI - 0.1f);

                    //Convert back
                    x = (float) (r*Math.Sin(tau)*Math.Cos(phi));
                    y = (float) (r*Math.Sin(tau)*Math.Sin(phi));
                    z = (float) (r*Math.Cos(tau));

                    //camera.Forward = new Vector3(x, y, 0);
                    camera.Position = camera.Lookat + new Vector3(-x, -y, z);
                    camera.Forward = (camera.Lookat - camera.Position);
                    

                }
            }

        }

        public void TestFunction()
        {

            string completeFilePath = null;
            string copiedFilePath = null;

            string fileName = null;
            string shortFileName = null;
            string fileEnding = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Application.StartupPath,
                Filter =
                    "image files (*.png, .jpg, .jpeg, .bmp, .gif)|*.png;*.jpg;*.bmp;*.jpeg;*.gif|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = false
            };

            //"c:\\";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                completeFilePath = openFileDialog1.FileName;
                if (openFileDialog1.SafeFileName != null)
                    fileName = openFileDialog1.SafeFileName;

                //Make it test instead of test.jpg;
                string[] split = fileName.Split('.');

                shortFileName = split[0];
                fileEnding = split[1];
                
                if (shortFileName != null)
                    copiedFilePath = Application.StartupPath + "/" + fileName;
            }

            _task = Task.Factory.StartNew(() => {
                try
            {
                if (copiedFilePath != null)
                    File.Copy(completeFilePath, copiedFilePath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            //todo write exceptions here
            if (copiedFilePath == null) return;
            if (!File.Exists(copiedFilePath)) return;
                
            string MGCBpathDirectory = Application.StartupPath + "/Content/MGCB/";
            string MGCBpathExe = Application.StartupPath + "/Content/MGCB/mgcb.exe";

            //Create pProcess
                Process pProcess = new Process {StartInfo = {FileName = MGCBpathExe}};

                //strCommand is path and file name of command to run

                completeFilePath = completeFilePath.Replace("\\", "/");

            Debug.Assert(fileName != null, "fileName != null");
            pProcess.StartInfo.Arguments = "/@:Content/mgcb/runtimepipeline.txt /build:"+ fileName;

            pProcess.StartInfo.CreateNoWindow = true;

            pProcess.StartInfo.UseShellExecute = false;

            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.StartInfo.RedirectStandardOutput = true;
            
            //Set output of program to be written to pProcess output stream
            pProcess.StartInfo.RedirectStandardOutput = true;

            //Get program output
            string stdError = null;

            var stdOutput = new StringBuilder();
            pProcess.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            try
            {
                pProcess.Start();
                pProcess.BeginOutputReadLine();
                stdError = pProcess.StandardError.ReadToEnd();
                pProcess.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing : " + e.Message, e);
                    
            }

            if (pProcess.ExitCode == 0)
            {
                stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                Debug.WriteLine(message);

                throw new Exception("mgcb finished with exit code = " + pProcess.ExitCode + ": " + message);
                    
            }

            //if(loadedTexture!=null)
            //loadedTexture.Dispose();
                loadedTexture = _contentManager.Load<Texture2D>("Runtime/Textures/" + shortFileName);
                
            File.Delete(copiedFilePath);
            });
        }


        public void Update(GameTime gameTime)
        {
            if(!GameStats.UIWasClicked)
            MouseEvents(_camera);      
        }

        public void Draw(GameTime gameTime)
        {
            
        }

        public void Unload()
        {
            string MGCBpathDirectory = Application.StartupPath + "/Content/Runtime";
            DirectoryInfo di = new DirectoryInfo(MGCBpathDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}

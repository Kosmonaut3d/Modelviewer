using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelperSuite.HelperSuite.ContentLoader;
using HelperSuite.HelperSuite.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.Logic
{
    public class MainLogic
    {
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        private Task _task;

        private string _monogameLocation;

        public Texture2D loadedTexture;
        private Texture2D rollTexture2D;
        private SpriteBatch _spriteBatch;

        struct TextureStruct
        {
            public Texture2D tex;
            public string path;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public void Load(ContentManager contentManager)
        {
            _contentManager = new ThreadSafeContentManager(contentManager.ServiceProvider);
            _contentManager.RootDirectory = "Content";

            rollTexture2D = _contentManager.Load<Texture2D>("backarrow02");
        }



        public void TestFunction()
        {

            string completeFilePath = null;
            string copiedFilePath = null;

            string fileName = null;
            string shortFileName = null;
            string fileEnding = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Application.StartupPath; //"c:\\";
            openFileDialog1.Filter = "image files (*.png, .jpg, .jpeg, .bmp, .gif)|*.png;*.jpg;*.bmp;*.jpeg;*.gif|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                completeFilePath = openFileDialog1.FileName;
                if (openFileDialog1.SafeFileName != null)
                    fileName = openFileDialog1.SafeFileName;

                //Make it test instead of test.jpg;
                string[] split = fileName.Split(new []{'.'});

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
            Process pProcess = new Process();

            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = MGCBpathExe;

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

                return;
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

                return;
            }

            //if(loadedTexture!=null)
            //loadedTexture.Dispose();
                loadedTexture = _contentManager.Load<Texture2D>("Runtime/Textures/" + shortFileName);

                GameSettings.testvaluer = 1;
                GameSettings.testvalueg = 1;
                GameSettings.testvalueb = 1;
            

            
            
            
            File.Delete(copiedFilePath);
            });
        }


        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (loadedTexture != null)
            {
                _spriteBatch.Draw(loadedTexture, new Rectangle(0,0,GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), new Color(GameSettings.testvaluer, GameSettings.testvalueg, GameSettings.testvalueb));
                
            }

            _spriteBatch.Draw(rollTexture2D, new Rectangle(200, 600, 100,100), null, Color.White, (float) -gameTime.TotalGameTime.TotalSeconds*3, new Vector2(rollTexture2D.Width/2, rollTexture2D.Height), SpriteEffects.None, 0);

            _spriteBatch.End();

        }

        public void Unload()
        {
            string MGCBpathDirectory = Application.StartupPath + "/Content/Runtime";
            System.IO.DirectoryInfo di = new DirectoryInfo(MGCBpathDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}

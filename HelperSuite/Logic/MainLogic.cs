using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        private string _monogameLocation;

        public Texture2D loadedTexture;
        private SpriteBatch _spriteBatch;

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public void Load(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }



        public void TestFunction()
        {

            string filePath = null;
            string copiedFilePath = null;
            string shortFileName = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Application.StartupPath; //"c:\\";
            openFileDialog1.Filter = "image files (*.png, .jpg, .jpeg, .bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                shortFileName = openFileDialog1.SafeFileName;
                copiedFilePath = Application.StartupPath + "/process." + filePath.Substring(filePath.Length - 3, 3);
                
                try
                {
                    File.Copy(filePath, copiedFilePath);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
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
            
            Debug.Assert(shortFileName != null, "shortFileName != null");
            pProcess.StartInfo.Arguments = "/@:Content/mgcb/runtimepipeline.txt /build:process."+ shortFileName.Substring(shortFileName.Length - 3, 3);

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

            loadedTexture = _contentManager.Load<Texture2D>("Runtime/Textures/process");
            
            File.Delete(copiedFilePath);
        }


        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw()
        {
            if (loadedTexture != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(loadedTexture, new Rectangle(0,0,GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);
                _spriteBatch.End();
            }
                
        }

        public void Unload()
        {
            string MGCBpathDirectory = Application.StartupPath + "/Content";
            
            //We must delete our temporary textures
            //if (loadedTexture != null)
            //{
            //   string test = loadedTexture.Name;
            //}
        }
    }
}

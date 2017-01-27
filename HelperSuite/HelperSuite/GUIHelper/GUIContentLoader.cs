using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelperSuite.HelperSuite.ContentLoader;
using HelperSuite.HelperSuite.Static;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.HelperSuite.GUIHelper
{
    public class GUIContentLoader
    {

        private ContentManager _contentManager;
        private Task _task;

        public readonly List<object> ContentArray = new List<object>();

        public void Load(ContentManager contentManager)
        {
            _contentManager = new ThreadSafeContentManager(contentManager.ServiceProvider);
            _contentManager.RootDirectory = "Content";
        }

        public void LoadContentFile<T>(out Task loadTaskOut, ref int pointerPositionInOut, out string filenameOut/*, string path*/)
        {
            string dialogFilter = "All files(*.*) | *.*";
            string pipeLineFile = "runtimepipeline.txt";
            //Switch the content pipeline parameters depending on the content type
            
            if (typeof(T) == typeof(Texture2D))
            {
                dialogFilter =
                    "image files (*.png, .jpg, .jpeg, .bmp, .gif)|*.png;*.jpg;*.bmp;*.jpeg;*.gif|All files (*.*)|*.*";
                pipeLineFile = "runtimepipeline.txt";
            }
            else
            {
                throw new Exception("Content type not supported!");
            }

            filenameOut = "...";

            string completeFilePath = null;

            string fileName = null;
            //string shortFileName = null;
            //string fileEnding = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Application.StartupPath; //"c:\\";
            openFileDialog1.Filter = dialogFilter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                completeFilePath = openFileDialog1.FileName;
                if (openFileDialog1.SafeFileName != null)
                    fileName = openFileDialog1.SafeFileName;

                //Make it test instead of test.jpg;
                //string[] split = fileName.Split(new[] { '.' });

                //shortFileName = split[0];
                //fileEnding = split[1];
                
                filenameOut = fileName;
            }
            else
            {
                loadTaskOut = null;
                return;
            }


            if (pointerPositionInOut == -1)
            {
                pointerPositionInOut = ContentArray.Count;
                ContentArray.Add(null);
            }
            else
            {
                if (pointerPositionInOut >= ContentArray.Count)
                    throw new NotImplementedException("Shouldn't be possible, pointer is greater than list.count");
            }

            int position = pointerPositionInOut;

            loadTaskOut = Task.Factory.StartNew(() =>
            {
                //string MGCBpathDirectory = Application.StartupPath + "/Content/MGCB/";
                string mgcbPathExe = Application.StartupPath + "/Content/MGCB/mgcb.exe";

                completeFilePath = completeFilePath.Replace("\\", "/");
                
                //Create pProcess
                Process pProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = mgcbPathExe,
                        Arguments = "/@:\"Content/mgcb/" + pipeLineFile +
                                    "\" /build:\"" + completeFilePath + "\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
                };
                
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
                
                if (typeof(T) == typeof(Texture2D))
                {
                    if(ContentArray[position]!=null)
                        ((Texture2D)ContentArray[position]).Dispose();
                }
                string path = completeFilePath.Split('.')[0];
                ContentArray[position] = _contentManager.Load<T>(path);
                //_contentManager.Load<T>("Runtime/Textures/" + shortFileName);

                //We should delete the generated .xnb file in the directory now
                File.Delete(path+".xnb");
            });

        }
    }
}

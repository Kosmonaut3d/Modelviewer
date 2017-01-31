using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelperSuite.HelperSuite.ContentLoader;
using HelperSuite.HelperSuite.Static;
using HelperSuite.Logic;
using HelperSuite.Renderer.ShaderModules.Helper;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HelperSuite.HelperSuite.GUIHelper
{
    public class GUIContentLoader
    {

        private ContentManager _contentManager;

        public readonly List<object> ContentArray = new List<object>();

        public void Load(ContentManager contentManager)
        {
            _contentManager = new ThreadSafeContentManager(contentManager.ServiceProvider) {RootDirectory = "Content"};
        }

        public void LoadContentFile<T>(out Task loadTaskOut, ref int pointerPositionInOut, out string filenameOut/*, string path*/)
        {
            string dialogFilter = "All files(*.*) | *.*";
            string pipeLineFile = "runtime.txt";
            //Switch the content pipeline parameters depending on the content type
            
            if (typeof(T) == typeof(Texture2D))
            {
                dialogFilter =
                    "image files (*.png, .jpg, .jpeg, .bmp, .gif)|*.png;*.jpg;*.bmp;*.jpeg;*.gif|All files (*.*)|*.*";
                pipeLineFile = "runtimetexture.txt";
            }
            else if (typeof(T) == typeof(AnimatedModel))
            {
                dialogFilter =
                    "model file (*.fbx)|*.fbx|All files (*.*)|*.*";
                pipeLineFile = "runtimeanimatedmodel.txt";
            }
            else
            {
                throw new Exception("Content type not supported!");
            }

            filenameOut = "...";

            string completeFilePath = null;

            string fileName = null;
            string copiedFilePath = null;
            string shortFileName;
            
            //string fileEnding = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                //InitialDirectory = Application.StartupPath,
                Filter = dialogFilter,
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = false
            };

            Input.mouseLastState = Mouse.GetState();
            Input.mouseState = Mouse.GetState();

            //"c:\\";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                completeFilePath = openFileDialog1.FileName;
                if (openFileDialog1.SafeFileName != null)
                    fileName = openFileDialog1.SafeFileName;

                //Make it test instead of test.jpg;
                string[] split = fileName.Split(new[] { '.' });

                shortFileName = split[0];
                //fileEnding = split[1];

                if (fileName != null)
                    copiedFilePath = Application.StartupPath + "/" + fileName;

                filenameOut = fileName;
            }
            else
            {
                loadTaskOut = null;
                if(pointerPositionInOut!=-1)
                    ContentArray[pointerPositionInOut] = null;
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
                //Copy file to directory
                {
                    try
                    {
                        if (copiedFilePath != null)
                            File.Copy(completeFilePath, copiedFilePath);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    if (!File.Exists(copiedFilePath)) return;
                }

                //string MGCBpathDirectory = Application.StartupPath + "/Content/MGCB/";
                string mgcbPathExe = Application.StartupPath + "/Content/MGCB/mgcb.exe";

                completeFilePath = completeFilePath.Replace("\\", "/");

                int tries = 0;

                while (tries<2)
                {
                    if (tries>0 /*&& typeof(T) == typeof(AnimatedModel)*/)
                    {
                        pipeLineFile = "runtimemodel.txt";
                    }

                    //Create pProcess
                    Process pProcess = new Process
                    {
                        StartInfo =
                        {
                            FileName = mgcbPathExe,
                            Arguments = "/@:\"Content/mgcb/" + pipeLineFile +
                                        "\" /build:\"" + fileName /*completeFilePath*/+ "\"",
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

                        tries = 10;
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

                        tries++;
                        //Debug.WriteLine(message);

                        //throw new Exception("mgcb finished with exit code = " + pProcess.ExitCode + ": " + message);
                    }
                }

                //if (typeof(T) == typeof(Texture2D))
                //{
                //    if(ContentArray[position]!=null)
                //        ((Texture2D)ContentArray[position]).Dispose();
                //}

                //string path = completeFilePath.Split('.')[0];
                //ContentArray[position] = _contentManager.Load<T>(path);
                //File.Delete(path + ".xnb");

                if (typeof(T) == typeof(AnimatedModel))
                {
                    //Have to load normal model
                    if (tries != 10)
                    {
                        ContentArray[position] = _contentManager.Load<Model>("Runtime/Textures/" + shortFileName);
                    }
                    else
                    {
                        ContentArray[position] = new AnimatedModel("Runtime/Textures/" + shortFileName);
                    ((AnimatedModel)ContentArray[position]).
                        LoadContent(_contentManager);
                        //if (!((AnimatedModel) ContentArray[position]).LoadContent(_contentManager))
                        //{

                        //    ContentArray[position] = _contentManager.Load<Model>("Runtime/Textures/" + shortFileName);
                        //}

                    }
                    
                    //_contentManager.Load<T>("Runtime/Textures/" + shortFileName);
                }
                else
                {
                    ContentArray[position] = _contentManager.Load<T>("Runtime/Textures/" + shortFileName);
                }
                string path = Application.StartupPath + "\\Content\\Runtime\\Textures\\" + shortFileName;
                File.Delete(path + ".xnb");

                //We should delete the generated .xnb file in the directory now

                if (copiedFilePath != null)
                    File.Delete(copiedFilePath);

            });

        }
    }
}

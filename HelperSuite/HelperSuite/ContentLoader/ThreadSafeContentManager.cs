using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace HelperSuite.HelperSuite.ContentLoader
{
    public class ThreadSafeContentManager : ContentManager
    {
        static object loadLock = new object();

        public ThreadSafeContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public ThreadSafeContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        public override T Load<T>(string assetName)
        {
            lock (loadLock)
            {
                return base.Load<T>(assetName);
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMPIHelper
{
    class Loader
    {
        public Loader(LoaderCallback DataLoaderFunction)
        {
            this.dataloader = DataLoaderFunction;
        }

        private bool contentRendered = false;
        private bool dbInitialized = false;
        private LoaderCallback dataloader;

        public void ContentRendered()
        {
            contentRendered = true;
            LoadMMPIData();
        }

        public void DatabaseInitialized()
        {
            dbInitialized = true;
            LoadMMPIData();
        }

        public void LoadMMPIData()
        {
            if (contentRendered && dbInitialized) dataloader();
        }
    }
}

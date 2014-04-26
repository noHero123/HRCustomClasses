using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    class Settings
    {

        public string path="";
        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }


        private Settings()
        {
        }
        
        public void setFilePath(string path)
        {
            this.path=path;
        }
    }
}

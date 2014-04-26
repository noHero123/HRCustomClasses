using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace HREngine.Bots
{


    public class Helpfunctions
    {
        public static IEnumerable<T> Take<T>(IEnumerable<T> source, int limit)
        {
            foreach (T item in source)
            {
                if (limit-- <= 0)
                    yield break;

                yield return item;
            }
        }

        public bool runningbot = false;

        private static Helpfunctions instance;

        public static Helpfunctions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Helpfunctions();
                }
                return instance;
            }
        }

        string path = Settings.Instance.path;


        private Helpfunctions()
        {

            System.IO.File.WriteAllText(path + "Logg.txt", "");
        }

        private bool writelogg = true;
        public void loggonoff(bool onoff)
        {
            //writelogg = onoff;
        }

        public void logg(string s)
        {


            if (!writelogg) return;
            try
            {
                using (StreamWriter sw = File.AppendText(path + "Logg.txt"))
                {
                    sw.WriteLine(s);
                }
            }
            catch { }
        }

        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

    public class BattleField
    {

        public class tagpair
        {
            public int Name = 0;
            public int Value = 0;
        }


        public class HrtUnit
        {

            public string CardID = "";

            public int entitiyID = 0;

            public List<tagpair> tags = new List<tagpair>();

            public int getTag(GAME_TAG gt)
            {
                foreach (tagpair t in tags)
                {
                    if ((GAME_TAG)t.Name == gt)
                    {
                        return t.Value;
                    }
                }
                return 0;
            }

        }

        private static BattleField instance;

        public static BattleField Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleField();
                }
                return instance;
            }
        }

        private BattleField()
        {
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    class Probabilitymaker
    {
        Dictionary<CardDB.cardIDEnum, int> ownCardsPlayed = new Dictionary<CardDB.cardIDEnum, int>();
        Dictionary<CardDB.cardIDEnum, int> enemyCardsPlayed = new Dictionary<CardDB.cardIDEnum, int>();
        List<CardDB.Card> ownDeckGuessed = new List<CardDB.Card>();
        List<CardDB.Card> enemyDeckGuessed = new List<CardDB.Card>();

        private static Probabilitymaker instance;
        public static Probabilitymaker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Probabilitymaker();
                }
                return instance;
            }
        }

        private Probabilitymaker()
        {
 
        }

        public void setOwnCards(List<CardDB.cardIDEnum> list)
        {
            setupDeck(list, ownDeckGuessed, ownCardsPlayed);
        }

        public void setEnemyCards(List<CardDB.cardIDEnum> list)
        {
            setupDeck(list, enemyDeckGuessed, enemyCardsPlayed);
        }

        private void setupDeck(List<CardDB.cardIDEnum> cardsPlayed, List<CardDB.Card> deckGuessed, Dictionary<CardDB.cardIDEnum, int> knownCards)
        {
            deckGuessed.Clear();
            knownCards.Clear();
            foreach (CardDB.cardIDEnum crdidnm in cardsPlayed)
            {
                if (crdidnm == CardDB.cardIDEnum.GAME_005) continue; //(im sure, he has no coins in his deck :D)
                if (knownCards.ContainsKey(crdidnm))
                {
                    knownCards[crdidnm]++;
                }
                else 
                {
                    if (CardDB.Instance.getCardDataFromID(crdidnm).rarity == 5)
                    {
                        //you cant own rare ones more than once!
                        knownCards.Add(crdidnm, 2);
                        continue;
                    }
                    knownCards.Add(crdidnm, 1);
                }
            }

            foreach (KeyValuePair<CardDB.cardIDEnum, int> kvp in knownCards)
            {
                if (kvp.Value == 1) deckGuessed.Add(CardDB.Instance.getCardDataFromID(kvp.Key));
            }
        }


    }
}

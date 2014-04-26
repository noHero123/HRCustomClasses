using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{

    public class Enchantment
    {
        public bool cantBeDispelled = false;
        public string CARDID = "";
        public int creator = 0;
        public int angrbuff = 0;
        public int hpbuff = 0;
        public int weaponAttack = 0;
        public int weapondurability = 0;
        public int angrfaktor = 1;
        public int hpfaktor = 1;
        public bool charge = false;
        public bool divineshild = false;
        public bool taunt = false;
        public bool cantLowerHPbelowONE = false;
        public bool angrEqualLife = false;
        public bool imune = false;
        public bool setHPtoOne = false;
        public bool setANGRtoOne = false;
        public bool cardDrawOnAngr = false;
        public bool windfury = false;
        public int zauberschaden = 0;
        public int controllerOfCreator = 0;
    }

    public class Minion
    {
        public int id = -1;
        public int Posix = 0;
        public int Posiy = 0;
        public int Hp = 0;
        public int maxHp = 0;
        public int Angr = 0;
        public bool Ready = false;
        public bool taunt = false;
        public bool wounded = false;//hp red?
        public string name = "";
        public CardDB.Card card;
        public bool divineshild = false;
        public bool windfury = false;
        public bool frozen = false;
        public int zonepos = 0;
        public bool stealth = false;
        public bool immune = false;
        public bool exhausted = false;
        public int numAttacksThisTurn = 0;
        public bool playedThisTurn = false;
        public bool charge = false;
        public bool poisonous = false;
        public bool silenced = false;
        public int entitiyID = -1;
        public List<Enchantment> enchantments = new List<Enchantment>();

        public Minion()
        {
        }

        public Minion(Minion m)
        {
            this.id = m.id;
            this.Posix = m.Posix;
            this.Posiy = m.Posiy;
            this.Hp = m.Hp;
            this.maxHp = m.maxHp;
            this.Angr = m.Angr;
            this.Ready = m.Ready;
            this.taunt = m.taunt;
            this.wounded = m.wounded;
            this.name = m.name;
            this.card = m.card;
            this.divineshild = m.divineshild;
            this.windfury = m.windfury;
            this.frozen = m.frozen;
            this.zonepos = m.zonepos;
            this.stealth = m.stealth;
            this.immune = m.immune;
            this.exhausted = m.exhausted;
            this.numAttacksThisTurn = m.numAttacksThisTurn;
            this.playedThisTurn = m.playedThisTurn;
            this.charge = m.charge;
            this.poisonous = m.poisonous;
            this.silenced = m.silenced;
            this.entitiyID = m.entitiyID;
            this.enchantments.AddRange(m.enchantments);
        }
    }
}

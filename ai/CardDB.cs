using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HREngine.Bots
{
    public class targett
    {
        public int target = -1;
        public int targetEntity = -1;

        public targett(int targ, int ent)
        {
            this.target = targ;
            this.targetEntity = ent;
        }
    }

    public class CardDB
    {
        // Data is stored in hearthstone-folder -> data->win cardxml0
        //(data-> cardxml0 seems outdated (blutelfkleriker has 3hp there >_>)
        public enum cardtype
        {
            NONE,
            MOB,
            SPELL,
            WEAPON,
            HEROPWR,
            ENCHANTMENT,

        }

        public enum cardIDEnum
        {
            None,
            XXX_040,
            CS2_188o,
            NEW1_007b,
            TU4c_003,
            XXX_024,
            EX1_613,
            EX1_295o,
            CS2_059o,
            EX1_133,
            NEW1_018,
            EX1_012,
            EX1_178a,
            CS2_231,
            EX1_019e,
            CRED_12,
            CS2_179,
            CS2_045e,
            EX1_244,
            EX1_178b,
            XXX_030,
            EX1_573b,
            TU4d_001,
            NEW1_007a,
            EX1_345t,
            EX1_025,
            EX1_396,
            NEW1_017,
            NEW1_008a,
            EX1_587e,
            EX1_533,
            EX1_522,
            NEW1_026,
            EX1_398,
            EX1_007,
            CS1_112,
            CRED_17,
            NEW1_036,
            EX1_355e,
            EX1_258,
            HERO_01,
            XXX_009,
            CS2_087,
            DREAM_05,
            NEW1_036e,
            CS2_092,
            CS2_022,
            EX1_046,
            XXX_005,
            PRO_001b,
            XXX_022,
            PRO_001a,
            CS2_103,
            NEW1_041,
            EX1_360,
            NEW1_038,
            CS2_009,
            EX1_010,
            CS2_024,
            EX1_565,
            CS2_076,
            CS2_046e,
            CS2_162,
            EX1_110t,
            CS2_104e,
            CS2_181,
            EX1_309,
            EX1_354,
            TU4f_001,
            XXX_018,
            EX1_023,
            NEW1_034,
            CS2_003,
            HERO_06,
            CS2_201,
            EX1_508,
            EX1_259,
            EX1_341,
            DREAM_05e,
            CRED_09,
            EX1_103,
            EX1_411,
            CS2_053,
            CS2_182,
            CS2_008,
            CS2_233,
            EX1_626,
            EX1_059,
            EX1_334,
            EX1_619,
            NEW1_032,
            EX1_158t,
            EX1_006,
            NEW1_031,
            DREAM_04,
            CS2_022e,
            EX1_611e,
            EX1_004,
            EX1_014te,
            EX1_095,
            NEW1_007,
            EX1_275,
            EX1_245,
            EX1_383,
            EX1_016t,
            CS2_125,
            EX1_137,
            EX1_178ae,
            DS1_185,
            EX1_598,
            EX1_304,
            EX1_302,
            XXX_017,
            CS2_011o,
            EX1_614t,
            TU4a_006,
            Mekka3e,
            CS2_108,
            CS2_046,
            EX1_014t,
            NEW1_005,
            EX1_062,
            EX1_366e,
            Mekka1,
            XXX_007,
            tt_010a,
            CS2_017o,
            CS2_072,
            EX1_tk28,
            EX1_604o,
            EX1_084e,
            EX1_409t,
            CRED_07,
            TU4e_002,
            EX1_507,
            EX1_144,
            CS2_038,
            EX1_093,
            CS2_080,
            CS1_129e,
            XXX_013,
            EX1_005,
            EX1_382,
            EX1_603e,
            CS2_028,
            TU4f_002,
            EX1_538,
            GAME_003e,
            DREAM_02,
            EX1_581,
            EX1_131t,
            CS2_147,
            CS1_113,
            CS2_161,
            CS2_031,
            EX1_166b,
            EX1_066,
            TU4c_007,
            EX1_355,
            EX1_507e,
            EX1_534,
            EX1_162,
            TU4a_004,
            EX1_363,
            EX1_164a,
            CS2_188,
            EX1_016,
            EX1_tk31,
            EX1_603,
            EX1_238,
            EX1_166,
            DS1h_292,
            DS1_183,
            CRED_11,
            XXX_019,
            EX1_076,
            EX1_048,
            CS2_038e,
            CS2_074,
            EX1_323w,
            EX1_129,
            NEW1_024o,
            EX1_405,
            EX1_317,
            EX1_606,
            EX1_590e,
            XXX_044,
            CS2_074e,
            TU4a_005,
            EX1_258e,
            TU4f_004o,
            NEW1_008,
            CS2_119,
            NEW1_017e,
            EX1_334e,
            TU4e_001,
            CS2_121,
            CS1h_001,
            EX1_tk34,
            NEW1_020,
            CS2_196,
            EX1_312,
            EX1_160b,
            EX1_563,
            XXX_039,
            CS2_087e,
            EX1_613e,
            NEW1_029,
            CS1_129,
            HERO_03,
            Mekka4t,
            EX1_158,
            XXX_010,
            NEW1_025,
            EX1_083,
            EX1_295,
            EX1_407,
            NEW1_004,
            PRO_001at,
            EX1_625t,
            EX1_014,
            CRED_04,
            CS2_097,
            EX1_558,
            EX1_tk29,
            CS2_186,
            EX1_084,
            NEW1_012,
            EX1_623e,
            EX1_578,
            CS2_073e2,
            CS2_221,
            EX1_019,
            EX1_132,
            EX1_284,
            EX1_105,
            NEW1_011,
            EX1_017,
            EX1_249,
            EX1_162o,
            EX1_313,
            EX1_549o,
            EX1_091o,
            CS2_084e,
            EX1_155b,
            NEW1_033,
            CS2_106,
            XXX_002,
            NEW1_036e2,
            XXX_004,
            CS2_122e,
            DS1_233,
            DS1_175,
            NEW1_024,
            CS2_189,
            CRED_10,
            NEW1_037,
            EX1_414,
            EX1_538t,
            EX1_586,
            EX1_310,
            NEW1_010,
            CS2_103e,
            EX1_080o,
            CS2_005o,
            EX1_363e2,
            EX1_534t,
            EX1_604,
            EX1_160,
            EX1_165t1,
            CS2_062,
            CS2_155,
            CS2_213,
            TU4f_007,
            GAME_004,
            XXX_020,
            CS2_004,
            EX1_561e,
            CS2_023,
            EX1_164,
            EX1_009,
            EX1_345,
            EX1_116,
            EX1_399,
            EX1_587,
            XXX_026,
            EX1_571,
            EX1_335,
            TU4e_004,
            HERO_08,
            EX1_166a,
            EX1_finkle,
            EX1_164b,
            EX1_283,
            EX1_339,
            CRED_13,
            EX1_178be,
            EX1_531,
            EX1_134,
            EX1_350,
            EX1_308,
            CS2_197,
            skele21,
            CS2_222o,
            XXX_015,
            NEW1_006,
            EX1_399e,
            EX1_509,
            EX1_612,
            EX1_021,
            CS2_041e,
            CS2_226,
            EX1_608,
            TU4c_008,
            EX1_624,
            EX1_616,
            EX1_008,
            PlaceholderCard,
            XXX_016,
            EX1_045,
            EX1_015,
            GAME_003,
            CS2_171,
            CS2_041,
            EX1_128,
            CS2_112,
            HERO_07,
            EX1_412,
            EX1_612o,
            CS2_117,
            XXX_009e,
            EX1_562,
            EX1_055,
            TU4e_007,
            EX1_317t,
            EX1_004e,
            EX1_278,
            CS2_tk1,
            EX1_590,
            CS1_130,
            NEW1_008b,
            EX1_365,
            CS2_141,
            PRO_001,
            CS2_173,
            CS2_017,
            CRED_16,
            EX1_392,
            EX1_593,
            TU4d_002,
            CRED_15,
            EX1_049,
            EX1_002,
            TU4f_005,
            NEW1_029t,
            TU4a_001,
            CS2_056,
            EX1_596,
            EX1_136,
            EX1_323,
            CS2_073,
            EX1_246e,
            EX1_244e,
            EX1_001,
            EX1_607e,
            EX1_044,
            EX1_573ae,
            XXX_025,
            CRED_06,
            Mekka4,
            CS2_142,
            TU4f_004,
            EX1_411e2,
            EX1_573,
            CS2_050,
            CS2_063e,
            EX1_390,
            EX1_610,
            hexfrog,
            CS2_181e,
            XXX_027,
            CS2_082,
            NEW1_040,
            DREAM_01,
            EX1_595,
            CS2_013,
            CS2_077,
            NEW1_014,
            CRED_05,
            GAME_002,
            EX1_165,
            CS2_013t,
            EX1_tk11,
            EX1_591,
            EX1_549,
            CS2_045,
            CS2_237,
            CS2_027,
            EX1_508o,
            CS2_101t,
            CS2_063,
            EX1_145,
            EX1_110,
            EX1_408,
            EX1_544,
            TU4c_006,
            CS2_151,
            CS2_073e,
            XXX_006,
            CS2_088,
            EX1_057,
            CS2_169,
            EX1_573t,
            EX1_323h,
            EX1_tk9,
            NEW1_018e,
            CS2_037,
            CS2_007,
            EX1_059e2,
            CS2_227,
            EX1_570e,
            NEW1_003,
            GAME_006,
            EX1_320,
            EX1_097,
            tt_004,
            EX1_360e,
            EX1_096,
            DS1_175o,
            EX1_596e,
            XXX_014,
            EX1_158e,
            CRED_01,
            CRED_08,
            EX1_126,
            EX1_577,
            EX1_319,
            EX1_611,
            CS2_146,
            EX1_154b,
            skele11,
            EX1_165t2,
            CS2_172,
            CS2_114,
            CS1_069,
            XXX_003,
            XXX_042,
            EX1_173,
            CS1_042,
            EX1_506a,
            EX1_298,
            CS2_104,
            HERO_02,
            EX1_316e,
            EX1_044e,
            CS2_051,
            NEW1_016,
            EX1_304e,
            EX1_033,
            EX1_028,
            XXX_011,
            EX1_621,
            EX1_554,
            EX1_091,
            EX1_409,
            EX1_363e,
            EX1_410,
            TU4e_005,
            CS2_039,
            EX1_557,
            CS2_105e,
            EX1_128e,
            XXX_021,
            DS1_070,
            CS2_033,
            EX1_536,
            TU4a_003,
            EX1_559,
            XXX_023,
            NEW1_033o,
            CS2_004e,
            CS2_052,
            EX1_539,
            EX1_575,
            CS2_083b,
            CS2_061,
            NEW1_021,
            DS1_055,
            EX1_625,
            EX1_382e,
            CS2_092e,
            CS2_026,
            NEW1_012o,
            EX1_619e,
            EX1_294,
            EX1_287,
            EX1_509e,
            EX1_625t2,
            CS2_118,
            CS2_124,
            Mekka3,
            EX1_112,
            CS2_009e,
            HERO_04,
            EX1_607,
            DREAM_03,
            EX1_103e,
            CS2_105,
            TU4c_002,
            CRED_14,
            EX1_567,
            TU4c_004,
            DS1_184,
            CS2_029,
            TU4e_006,
            GAME_005,
            CS2_187,
            EX1_020,
            EX1_011,
            CS2_057,
            EX1_274,
            EX1_306,
            NEW1_038o,
            EX1_170,
            EX1_617,
            CS1_113e,
            CS2_101,
            CS2_005,
            EX1_537,
            EX1_384,
            TU4a_002,
            EX1_362,
            TU4c_005,
            EX1_301,
            CS2_235,
            EX1_029,
            CS2_042,
            EX1_155a,
            CS2_102,
            EX1_609,
            NEW1_027,
            CS2_236e,
            CS2_083e,
            EX1_165a,
            EX1_570,
            EX1_131,
            EX1_556,
            EX1_543,
            TU4c_008e,
            EX1_379e,
            NEW1_009,
            EX1_100,
            EX1_274e,
            CRED_02,
            EX1_573a,
            CS2_084,
            EX1_582,
            EX1_043,
            EX1_050,
            TU4b_001,
            EX1_620,
            EX1_303,
            HERO_09,
            EX1_067,
            XXX_028,
            EX1_277,
            Mekka2,
            CS2_221e,
            EX1_178,
            CS2_222,
            EX1_409e,
            tt_004o,
            EX1_155ae,
            EX1_160a,
            NEW1_025e,
            CS2_012,
            EX1_246,
            EX1_572,
            EX1_089,
            CS2_059,
            EX1_279,
            CS2_168,
            tt_010,
            NEW1_023,
            CS2_075,
            EX1_316,
            CS2_025,
            CS2_234,
            XXX_043,
            GAME_001,
            EX1_130,
            EX1_584e,
            CS2_064,
            EX1_161,
            CS2_049,
            EX1_154,
            EX1_080,
            NEW1_022,
            EX1_160be,
            EX1_251,
            EX1_371,
            CS2_mirror,
            EX1_594,
            TU4c_006e,
            EX1_560,
            CS2_236,
            TU4f_006,
            EX1_402,
            EX1_506,
            NEW1_027e,
            DS1_070o,
            XXX_045,
            XXX_029,
            DS1_178,
            EX1_315,
            CS2_094,
            TU4e_002t,
            EX1_046e,
            NEW1_040t,
            GAME_005e,
            CS2_131,
            XXX_008,
            EX1_531e,
            CS2_226e,
            XXX_022e,
            DS1_178e,
            CS2_226o,
            Mekka4e,
            EX1_082,
            CS2_093,
            EX1_411e,
            EX1_145o,
            CS2_boar,
            NEW1_019,
            EX1_289,
            EX1_025t,
            EX1_398t,
            EX1_055o,
            CS2_091,
            EX1_241,
            EX1_085,
            CS2_200,
            CS2_034,
            EX1_583,
            EX1_584,
            EX1_155,
            EX1_622,
            CS2_203,
            EX1_124,
            EX1_379,
            CS2_053e,
            EX1_032,
            TU4e_003,
            CS2_146o,
            XXX_041,
            EX1_391,
            EX1_366,
            EX1_059e,
            XXX_012,
            EX1_565o,
            EX1_001e,
            TU4f_003,
            EX1_400,
            EX1_614,
            EX1_561,
            EX1_332,
            HERO_05,
            CS2_065,
            ds1_whelptoken,
            EX1_536e,
            CS2_032,
            CS2_120,
            EX1_155be,
            EX1_247,
            EX1_154a,
            EX1_554t,
            CS2_103e2,
            TU4d_003,
            NEW1_026t,
            EX1_623,
            EX1_383t,
            EX1_597,
            TU4f_006o,
            EX1_130a,
            CS2_011,
            EX1_169,
            EX1_tk33,
            EX1_250,
            EX1_564,
            EX1_043e,
            EX1_349,
            EX1_102,
            EX1_058,
            EX1_243,
            PRO_001c,
            EX1_116t,
            CS2_089,
            TU4c_001,
            EX1_248,
            NEW1_037e,
            CS2_122,
            EX1_393,
            CS2_232,
            EX1_165b,
            NEW1_030,
            EX1_161o,
            EX1_093e,
            CS2_150,
            CS2_152,
            EX1_160t,
            CS2_127,
            CRED_03,
            DS1_188,
            XXX_001,
            EX1_Avenge,//fake
            EX1_duplicate, //fake
        }
        
        public cardIDEnum cardIdstringToEnum(string s)
        {
            if (s == "XXX_040") return CardDB.cardIDEnum.XXX_040;
            if (s == "CS2_188o") return CardDB.cardIDEnum.CS2_188o;
            if (s == "NEW1_007b") return CardDB.cardIDEnum.NEW1_007b;
            if (s == "TU4c_003") return CardDB.cardIDEnum.TU4c_003;
            if (s == "XXX_024") return CardDB.cardIDEnum.XXX_024;
            if (s == "EX1_613") return CardDB.cardIDEnum.EX1_613;
            if (s == "EX1_295o") return CardDB.cardIDEnum.EX1_295o;
            if (s == "CS2_059o") return CardDB.cardIDEnum.CS2_059o;
            if (s == "EX1_133") return CardDB.cardIDEnum.EX1_133;
            if (s == "NEW1_018") return CardDB.cardIDEnum.NEW1_018;
            if (s == "EX1_012") return CardDB.cardIDEnum.EX1_012;
            if (s == "EX1_178a") return CardDB.cardIDEnum.EX1_178a;
            if (s == "CS2_231") return CardDB.cardIDEnum.CS2_231;
            if (s == "EX1_019e") return CardDB.cardIDEnum.EX1_019e;
            if (s == "CRED_12") return CardDB.cardIDEnum.CRED_12;
            if (s == "CS2_179") return CardDB.cardIDEnum.CS2_179;
            if (s == "CS2_045e") return CardDB.cardIDEnum.CS2_045e;
            if (s == "EX1_244") return CardDB.cardIDEnum.EX1_244;
            if (s == "EX1_178b") return CardDB.cardIDEnum.EX1_178b;
            if (s == "XXX_030") return CardDB.cardIDEnum.XXX_030;
            if (s == "EX1_573b") return CardDB.cardIDEnum.EX1_573b;
            if (s == "TU4d_001") return CardDB.cardIDEnum.TU4d_001;
            if (s == "NEW1_007a") return CardDB.cardIDEnum.NEW1_007a;
            if (s == "EX1_345t") return CardDB.cardIDEnum.EX1_345t;
            if (s == "EX1_025") return CardDB.cardIDEnum.EX1_025;
            if (s == "EX1_396") return CardDB.cardIDEnum.EX1_396;
            if (s == "NEW1_017") return CardDB.cardIDEnum.NEW1_017;
            if (s == "NEW1_008a") return CardDB.cardIDEnum.NEW1_008a;
            if (s == "EX1_587e") return CardDB.cardIDEnum.EX1_587e;
            if (s == "EX1_533") return CardDB.cardIDEnum.EX1_533;
            if (s == "EX1_522") return CardDB.cardIDEnum.EX1_522;
            if (s == "NEW1_026") return CardDB.cardIDEnum.NEW1_026;
            if (s == "EX1_398") return CardDB.cardIDEnum.EX1_398;
            if (s == "EX1_007") return CardDB.cardIDEnum.EX1_007;
            if (s == "CS1_112") return CardDB.cardIDEnum.CS1_112;
            if (s == "CRED_17") return CardDB.cardIDEnum.CRED_17;
            if (s == "NEW1_036") return CardDB.cardIDEnum.NEW1_036;
            if (s == "EX1_355e") return CardDB.cardIDEnum.EX1_355e;
            if (s == "EX1_258") return CardDB.cardIDEnum.EX1_258;
            if (s == "HERO_01") return CardDB.cardIDEnum.HERO_01;
            if (s == "XXX_009") return CardDB.cardIDEnum.XXX_009;
            if (s == "CS2_087") return CardDB.cardIDEnum.CS2_087;
            if (s == "DREAM_05") return CardDB.cardIDEnum.DREAM_05;
            if (s == "NEW1_036e") return CardDB.cardIDEnum.NEW1_036e;
            if (s == "CS2_092") return CardDB.cardIDEnum.CS2_092;
            if (s == "CS2_022") return CardDB.cardIDEnum.CS2_022;
            if (s == "EX1_046") return CardDB.cardIDEnum.EX1_046;
            if (s == "XXX_005") return CardDB.cardIDEnum.XXX_005;
            if (s == "PRO_001b") return CardDB.cardIDEnum.PRO_001b;
            if (s == "XXX_022") return CardDB.cardIDEnum.XXX_022;
            if (s == "PRO_001a") return CardDB.cardIDEnum.PRO_001a;
            if (s == "CS2_103") return CardDB.cardIDEnum.CS2_103;
            if (s == "NEW1_041") return CardDB.cardIDEnum.NEW1_041;
            if (s == "EX1_360") return CardDB.cardIDEnum.EX1_360;
            if (s == "NEW1_038") return CardDB.cardIDEnum.NEW1_038;
            if (s == "CS2_009") return CardDB.cardIDEnum.CS2_009;
            if (s == "EX1_010") return CardDB.cardIDEnum.EX1_010;
            if (s == "CS2_024") return CardDB.cardIDEnum.CS2_024;
            if (s == "EX1_565") return CardDB.cardIDEnum.EX1_565;
            if (s == "CS2_076") return CardDB.cardIDEnum.CS2_076;
            if (s == "CS2_046e") return CardDB.cardIDEnum.CS2_046e;
            if (s == "CS2_162") return CardDB.cardIDEnum.CS2_162;
            if (s == "EX1_110t") return CardDB.cardIDEnum.EX1_110t;
            if (s == "CS2_104e") return CardDB.cardIDEnum.CS2_104e;
            if (s == "CS2_181") return CardDB.cardIDEnum.CS2_181;
            if (s == "EX1_309") return CardDB.cardIDEnum.EX1_309;
            if (s == "EX1_354") return CardDB.cardIDEnum.EX1_354;
            if (s == "TU4f_001") return CardDB.cardIDEnum.TU4f_001;
            if (s == "XXX_018") return CardDB.cardIDEnum.XXX_018;
            if (s == "EX1_023") return CardDB.cardIDEnum.EX1_023;
            if (s == "NEW1_034") return CardDB.cardIDEnum.NEW1_034;
            if (s == "CS2_003") return CardDB.cardIDEnum.CS2_003;
            if (s == "HERO_06") return CardDB.cardIDEnum.HERO_06;
            if (s == "CS2_201") return CardDB.cardIDEnum.CS2_201;
            if (s == "EX1_508") return CardDB.cardIDEnum.EX1_508;
            if (s == "EX1_259") return CardDB.cardIDEnum.EX1_259;
            if (s == "EX1_341") return CardDB.cardIDEnum.EX1_341;
            if (s == "DREAM_05e") return CardDB.cardIDEnum.DREAM_05e;
            if (s == "CRED_09") return CardDB.cardIDEnum.CRED_09;
            if (s == "EX1_103") return CardDB.cardIDEnum.EX1_103;
            if (s == "EX1_411") return CardDB.cardIDEnum.EX1_411;
            if (s == "CS2_053") return CardDB.cardIDEnum.CS2_053;
            if (s == "CS2_182") return CardDB.cardIDEnum.CS2_182;
            if (s == "CS2_008") return CardDB.cardIDEnum.CS2_008;
            if (s == "CS2_233") return CardDB.cardIDEnum.CS2_233;
            if (s == "EX1_626") return CardDB.cardIDEnum.EX1_626;
            if (s == "EX1_059") return CardDB.cardIDEnum.EX1_059;
            if (s == "EX1_334") return CardDB.cardIDEnum.EX1_334;
            if (s == "EX1_619") return CardDB.cardIDEnum.EX1_619;
            if (s == "NEW1_032") return CardDB.cardIDEnum.NEW1_032;
            if (s == "EX1_158t") return CardDB.cardIDEnum.EX1_158t;
            if (s == "EX1_006") return CardDB.cardIDEnum.EX1_006;
            if (s == "NEW1_031") return CardDB.cardIDEnum.NEW1_031;
            if (s == "DREAM_04") return CardDB.cardIDEnum.DREAM_04;
            if (s == "CS2_022e") return CardDB.cardIDEnum.CS2_022e;
            if (s == "EX1_611e") return CardDB.cardIDEnum.EX1_611e;
            if (s == "EX1_004") return CardDB.cardIDEnum.EX1_004;
            if (s == "EX1_014te") return CardDB.cardIDEnum.EX1_014te;
            if (s == "EX1_095") return CardDB.cardIDEnum.EX1_095;
            if (s == "NEW1_007") return CardDB.cardIDEnum.NEW1_007;
            if (s == "EX1_275") return CardDB.cardIDEnum.EX1_275;
            if (s == "EX1_245") return CardDB.cardIDEnum.EX1_245;
            if (s == "EX1_383") return CardDB.cardIDEnum.EX1_383;
            if (s == "EX1_016t") return CardDB.cardIDEnum.EX1_016t;
            if (s == "CS2_125") return CardDB.cardIDEnum.CS2_125;
            if (s == "EX1_137") return CardDB.cardIDEnum.EX1_137;
            if (s == "EX1_178ae") return CardDB.cardIDEnum.EX1_178ae;
            if (s == "DS1_185") return CardDB.cardIDEnum.DS1_185;
            if (s == "EX1_598") return CardDB.cardIDEnum.EX1_598;
            if (s == "EX1_304") return CardDB.cardIDEnum.EX1_304;
            if (s == "EX1_302") return CardDB.cardIDEnum.EX1_302;
            if (s == "XXX_017") return CardDB.cardIDEnum.XXX_017;
            if (s == "CS2_011o") return CardDB.cardIDEnum.CS2_011o;
            if (s == "EX1_614t") return CardDB.cardIDEnum.EX1_614t;
            if (s == "TU4a_006") return CardDB.cardIDEnum.TU4a_006;
            if (s == "Mekka3e") return CardDB.cardIDEnum.Mekka3e;
            if (s == "CS2_108") return CardDB.cardIDEnum.CS2_108;
            if (s == "CS2_046") return CardDB.cardIDEnum.CS2_046;
            if (s == "EX1_014t") return CardDB.cardIDEnum.EX1_014t;
            if (s == "NEW1_005") return CardDB.cardIDEnum.NEW1_005;
            if (s == "EX1_062") return CardDB.cardIDEnum.EX1_062;
            if (s == "EX1_366e") return CardDB.cardIDEnum.EX1_366e;
            if (s == "Mekka1") return CardDB.cardIDEnum.Mekka1;
            if (s == "XXX_007") return CardDB.cardIDEnum.XXX_007;
            if (s == "tt_010a") return CardDB.cardIDEnum.tt_010a;
            if (s == "CS2_017o") return CardDB.cardIDEnum.CS2_017o;
            if (s == "CS2_072") return CardDB.cardIDEnum.CS2_072;
            if (s == "EX1_tk28") return CardDB.cardIDEnum.EX1_tk28;
            if (s == "EX1_604o") return CardDB.cardIDEnum.EX1_604o;
            if (s == "EX1_084e") return CardDB.cardIDEnum.EX1_084e;
            if (s == "EX1_409t") return CardDB.cardIDEnum.EX1_409t;
            if (s == "CRED_07") return CardDB.cardIDEnum.CRED_07;
            if (s == "TU4e_002") return CardDB.cardIDEnum.TU4e_002;
            if (s == "EX1_507") return CardDB.cardIDEnum.EX1_507;
            if (s == "EX1_144") return CardDB.cardIDEnum.EX1_144;
            if (s == "CS2_038") return CardDB.cardIDEnum.CS2_038;
            if (s == "EX1_093") return CardDB.cardIDEnum.EX1_093;
            if (s == "CS2_080") return CardDB.cardIDEnum.CS2_080;
            if (s == "CS1_129e") return CardDB.cardIDEnum.CS1_129e;
            if (s == "XXX_013") return CardDB.cardIDEnum.XXX_013;
            if (s == "EX1_005") return CardDB.cardIDEnum.EX1_005;
            if (s == "EX1_382") return CardDB.cardIDEnum.EX1_382;
            if (s == "EX1_603e") return CardDB.cardIDEnum.EX1_603e;
            if (s == "CS2_028") return CardDB.cardIDEnum.CS2_028;
            if (s == "TU4f_002") return CardDB.cardIDEnum.TU4f_002;
            if (s == "EX1_538") return CardDB.cardIDEnum.EX1_538;
            if (s == "GAME_003e") return CardDB.cardIDEnum.GAME_003e;
            if (s == "DREAM_02") return CardDB.cardIDEnum.DREAM_02;
            if (s == "EX1_581") return CardDB.cardIDEnum.EX1_581;
            if (s == "EX1_131t") return CardDB.cardIDEnum.EX1_131t;
            if (s == "CS2_147") return CardDB.cardIDEnum.CS2_147;
            if (s == "CS1_113") return CardDB.cardIDEnum.CS1_113;
            if (s == "CS2_161") return CardDB.cardIDEnum.CS2_161;
            if (s == "CS2_031") return CardDB.cardIDEnum.CS2_031;
            if (s == "EX1_166b") return CardDB.cardIDEnum.EX1_166b;
            if (s == "EX1_066") return CardDB.cardIDEnum.EX1_066;
            if (s == "TU4c_007") return CardDB.cardIDEnum.TU4c_007;
            if (s == "EX1_355") return CardDB.cardIDEnum.EX1_355;
            if (s == "EX1_507e") return CardDB.cardIDEnum.EX1_507e;
            if (s == "EX1_534") return CardDB.cardIDEnum.EX1_534;
            if (s == "EX1_162") return CardDB.cardIDEnum.EX1_162;
            if (s == "TU4a_004") return CardDB.cardIDEnum.TU4a_004;
            if (s == "EX1_363") return CardDB.cardIDEnum.EX1_363;
            if (s == "EX1_164a") return CardDB.cardIDEnum.EX1_164a;
            if (s == "CS2_188") return CardDB.cardIDEnum.CS2_188;
            if (s == "EX1_016") return CardDB.cardIDEnum.EX1_016;
            if (s == "EX1_tk31") return CardDB.cardIDEnum.EX1_tk31;
            if (s == "EX1_603") return CardDB.cardIDEnum.EX1_603;
            if (s == "EX1_238") return CardDB.cardIDEnum.EX1_238;
            if (s == "EX1_166") return CardDB.cardIDEnum.EX1_166;
            if (s == "DS1h_292") return CardDB.cardIDEnum.DS1h_292;
            if (s == "DS1_183") return CardDB.cardIDEnum.DS1_183;
            if (s == "CRED_11") return CardDB.cardIDEnum.CRED_11;
            if (s == "XXX_019") return CardDB.cardIDEnum.XXX_019;
            if (s == "EX1_076") return CardDB.cardIDEnum.EX1_076;
            if (s == "EX1_048") return CardDB.cardIDEnum.EX1_048;
            if (s == "CS2_038e") return CardDB.cardIDEnum.CS2_038e;
            if (s == "CS2_074") return CardDB.cardIDEnum.CS2_074;
            if (s == "EX1_323w") return CardDB.cardIDEnum.EX1_323w;
            if (s == "EX1_129") return CardDB.cardIDEnum.EX1_129;
            if (s == "NEW1_024o") return CardDB.cardIDEnum.NEW1_024o;
            if (s == "EX1_405") return CardDB.cardIDEnum.EX1_405;
            if (s == "EX1_317") return CardDB.cardIDEnum.EX1_317;
            if (s == "EX1_606") return CardDB.cardIDEnum.EX1_606;
            if (s == "EX1_590e") return CardDB.cardIDEnum.EX1_590e;
            if (s == "XXX_044") return CardDB.cardIDEnum.XXX_044;
            if (s == "CS2_074e") return CardDB.cardIDEnum.CS2_074e;
            if (s == "TU4a_005") return CardDB.cardIDEnum.TU4a_005;
            if (s == "EX1_258e") return CardDB.cardIDEnum.EX1_258e;
            if (s == "TU4f_004o") return CardDB.cardIDEnum.TU4f_004o;
            if (s == "NEW1_008") return CardDB.cardIDEnum.NEW1_008;
            if (s == "CS2_119") return CardDB.cardIDEnum.CS2_119;
            if (s == "NEW1_017e") return CardDB.cardIDEnum.NEW1_017e;
            if (s == "EX1_334e") return CardDB.cardIDEnum.EX1_334e;
            if (s == "TU4e_001") return CardDB.cardIDEnum.TU4e_001;
            if (s == "CS2_121") return CardDB.cardIDEnum.CS2_121;
            if (s == "CS1h_001") return CardDB.cardIDEnum.CS1h_001;
            if (s == "EX1_tk34") return CardDB.cardIDEnum.EX1_tk34;
            if (s == "NEW1_020") return CardDB.cardIDEnum.NEW1_020;
            if (s == "CS2_196") return CardDB.cardIDEnum.CS2_196;
            if (s == "EX1_312") return CardDB.cardIDEnum.EX1_312;
            if (s == "EX1_160b") return CardDB.cardIDEnum.EX1_160b;
            if (s == "EX1_563") return CardDB.cardIDEnum.EX1_563;
            if (s == "XXX_039") return CardDB.cardIDEnum.XXX_039;
            if (s == "CS2_087e") return CardDB.cardIDEnum.CS2_087e;
            if (s == "EX1_613e") return CardDB.cardIDEnum.EX1_613e;
            if (s == "NEW1_029") return CardDB.cardIDEnum.NEW1_029;
            if (s == "CS1_129") return CardDB.cardIDEnum.CS1_129;
            if (s == "HERO_03") return CardDB.cardIDEnum.HERO_03;
            if (s == "Mekka4t") return CardDB.cardIDEnum.Mekka4t;
            if (s == "EX1_158") return CardDB.cardIDEnum.EX1_158;
            if (s == "XXX_010") return CardDB.cardIDEnum.XXX_010;
            if (s == "NEW1_025") return CardDB.cardIDEnum.NEW1_025;
            if (s == "EX1_083") return CardDB.cardIDEnum.EX1_083;
            if (s == "EX1_295") return CardDB.cardIDEnum.EX1_295;
            if (s == "EX1_407") return CardDB.cardIDEnum.EX1_407;
            if (s == "NEW1_004") return CardDB.cardIDEnum.NEW1_004;
            if (s == "PRO_001at") return CardDB.cardIDEnum.PRO_001at;
            if (s == "EX1_625t") return CardDB.cardIDEnum.EX1_625t;
            if (s == "EX1_014") return CardDB.cardIDEnum.EX1_014;
            if (s == "CRED_04") return CardDB.cardIDEnum.CRED_04;
            if (s == "CS2_097") return CardDB.cardIDEnum.CS2_097;
            if (s == "EX1_558") return CardDB.cardIDEnum.EX1_558;
            if (s == "EX1_tk29") return CardDB.cardIDEnum.EX1_tk29;
            if (s == "CS2_186") return CardDB.cardIDEnum.CS2_186;
            if (s == "EX1_084") return CardDB.cardIDEnum.EX1_084;
            if (s == "NEW1_012") return CardDB.cardIDEnum.NEW1_012;
            if (s == "EX1_623e") return CardDB.cardIDEnum.EX1_623e;
            if (s == "EX1_578") return CardDB.cardIDEnum.EX1_578;
            if (s == "CS2_073e2") return CardDB.cardIDEnum.CS2_073e2;
            if (s == "CS2_221") return CardDB.cardIDEnum.CS2_221;
            if (s == "EX1_019") return CardDB.cardIDEnum.EX1_019;
            if (s == "EX1_132") return CardDB.cardIDEnum.EX1_132;
            if (s == "EX1_284") return CardDB.cardIDEnum.EX1_284;
            if (s == "EX1_105") return CardDB.cardIDEnum.EX1_105;
            if (s == "NEW1_011") return CardDB.cardIDEnum.NEW1_011;
            if (s == "EX1_017") return CardDB.cardIDEnum.EX1_017;
            if (s == "EX1_249") return CardDB.cardIDEnum.EX1_249;
            if (s == "EX1_162o") return CardDB.cardIDEnum.EX1_162o;
            if (s == "EX1_313") return CardDB.cardIDEnum.EX1_313;
            if (s == "EX1_549o") return CardDB.cardIDEnum.EX1_549o;
            if (s == "EX1_091o") return CardDB.cardIDEnum.EX1_091o;
            if (s == "CS2_084e") return CardDB.cardIDEnum.CS2_084e;
            if (s == "EX1_155b") return CardDB.cardIDEnum.EX1_155b;
            if (s == "NEW1_033") return CardDB.cardIDEnum.NEW1_033;
            if (s == "CS2_106") return CardDB.cardIDEnum.CS2_106;
            if (s == "XXX_002") return CardDB.cardIDEnum.XXX_002;
            if (s == "NEW1_036e2") return CardDB.cardIDEnum.NEW1_036e2;
            if (s == "XXX_004") return CardDB.cardIDEnum.XXX_004;
            if (s == "CS2_122e") return CardDB.cardIDEnum.CS2_122e;
            if (s == "DS1_233") return CardDB.cardIDEnum.DS1_233;
            if (s == "DS1_175") return CardDB.cardIDEnum.DS1_175;
            if (s == "NEW1_024") return CardDB.cardIDEnum.NEW1_024;
            if (s == "CS2_189") return CardDB.cardIDEnum.CS2_189;
            if (s == "CRED_10") return CardDB.cardIDEnum.CRED_10;
            if (s == "NEW1_037") return CardDB.cardIDEnum.NEW1_037;
            if (s == "EX1_414") return CardDB.cardIDEnum.EX1_414;
            if (s == "EX1_538t") return CardDB.cardIDEnum.EX1_538t;
            if (s == "EX1_586") return CardDB.cardIDEnum.EX1_586;
            if (s == "EX1_310") return CardDB.cardIDEnum.EX1_310;
            if (s == "NEW1_010") return CardDB.cardIDEnum.NEW1_010;
            if (s == "CS2_103e") return CardDB.cardIDEnum.CS2_103e;
            if (s == "EX1_080o") return CardDB.cardIDEnum.EX1_080o;
            if (s == "CS2_005o") return CardDB.cardIDEnum.CS2_005o;
            if (s == "EX1_363e2") return CardDB.cardIDEnum.EX1_363e2;
            if (s == "EX1_534t") return CardDB.cardIDEnum.EX1_534t;
            if (s == "EX1_604") return CardDB.cardIDEnum.EX1_604;
            if (s == "EX1_160") return CardDB.cardIDEnum.EX1_160;
            if (s == "EX1_165t1") return CardDB.cardIDEnum.EX1_165t1;
            if (s == "CS2_062") return CardDB.cardIDEnum.CS2_062;
            if (s == "CS2_155") return CardDB.cardIDEnum.CS2_155;
            if (s == "CS2_213") return CardDB.cardIDEnum.CS2_213;
            if (s == "TU4f_007") return CardDB.cardIDEnum.TU4f_007;
            if (s == "GAME_004") return CardDB.cardIDEnum.GAME_004;
            if (s == "XXX_020") return CardDB.cardIDEnum.XXX_020;
            if (s == "CS2_004") return CardDB.cardIDEnum.CS2_004;
            if (s == "EX1_561e") return CardDB.cardIDEnum.EX1_561e;
            if (s == "CS2_023") return CardDB.cardIDEnum.CS2_023;
            if (s == "EX1_164") return CardDB.cardIDEnum.EX1_164;
            if (s == "EX1_009") return CardDB.cardIDEnum.EX1_009;
            if (s == "EX1_345") return CardDB.cardIDEnum.EX1_345;
            if (s == "EX1_116") return CardDB.cardIDEnum.EX1_116;
            if (s == "EX1_399") return CardDB.cardIDEnum.EX1_399;
            if (s == "EX1_587") return CardDB.cardIDEnum.EX1_587;
            if (s == "XXX_026") return CardDB.cardIDEnum.XXX_026;
            if (s == "EX1_571") return CardDB.cardIDEnum.EX1_571;
            if (s == "EX1_335") return CardDB.cardIDEnum.EX1_335;
            if (s == "TU4e_004") return CardDB.cardIDEnum.TU4e_004;
            if (s == "HERO_08") return CardDB.cardIDEnum.HERO_08;
            if (s == "EX1_166a") return CardDB.cardIDEnum.EX1_166a;
            if (s == "EX1_finkle") return CardDB.cardIDEnum.EX1_finkle;
            if (s == "EX1_164b") return CardDB.cardIDEnum.EX1_164b;
            if (s == "EX1_283") return CardDB.cardIDEnum.EX1_283;
            if (s == "EX1_339") return CardDB.cardIDEnum.EX1_339;
            if (s == "CRED_13") return CardDB.cardIDEnum.CRED_13;
            if (s == "EX1_178be") return CardDB.cardIDEnum.EX1_178be;
            if (s == "EX1_531") return CardDB.cardIDEnum.EX1_531;
            if (s == "EX1_134") return CardDB.cardIDEnum.EX1_134;
            if (s == "EX1_350") return CardDB.cardIDEnum.EX1_350;
            if (s == "EX1_308") return CardDB.cardIDEnum.EX1_308;
            if (s == "CS2_197") return CardDB.cardIDEnum.CS2_197;
            if (s == "skele21") return CardDB.cardIDEnum.skele21;
            if (s == "CS2_222o") return CardDB.cardIDEnum.CS2_222o;
            if (s == "XXX_015") return CardDB.cardIDEnum.XXX_015;
            if (s == "NEW1_006") return CardDB.cardIDEnum.NEW1_006;
            if (s == "EX1_399e") return CardDB.cardIDEnum.EX1_399e;
            if (s == "EX1_509") return CardDB.cardIDEnum.EX1_509;
            if (s == "EX1_612") return CardDB.cardIDEnum.EX1_612;
            if (s == "EX1_021") return CardDB.cardIDEnum.EX1_021;
            if (s == "CS2_041e") return CardDB.cardIDEnum.CS2_041e;
            if (s == "CS2_226") return CardDB.cardIDEnum.CS2_226;
            if (s == "EX1_608") return CardDB.cardIDEnum.EX1_608;
            if (s == "TU4c_008") return CardDB.cardIDEnum.TU4c_008;
            if (s == "EX1_624") return CardDB.cardIDEnum.EX1_624;
            if (s == "EX1_616") return CardDB.cardIDEnum.EX1_616;
            if (s == "EX1_008") return CardDB.cardIDEnum.EX1_008;
            if (s == "PlaceholderCard") return CardDB.cardIDEnum.PlaceholderCard;
            if (s == "XXX_016") return CardDB.cardIDEnum.XXX_016;
            if (s == "EX1_045") return CardDB.cardIDEnum.EX1_045;
            if (s == "EX1_015") return CardDB.cardIDEnum.EX1_015;
            if (s == "GAME_003") return CardDB.cardIDEnum.GAME_003;
            if (s == "CS2_171") return CardDB.cardIDEnum.CS2_171;
            if (s == "CS2_041") return CardDB.cardIDEnum.CS2_041;
            if (s == "EX1_128") return CardDB.cardIDEnum.EX1_128;
            if (s == "CS2_112") return CardDB.cardIDEnum.CS2_112;
            if (s == "HERO_07") return CardDB.cardIDEnum.HERO_07;
            if (s == "EX1_412") return CardDB.cardIDEnum.EX1_412;
            if (s == "EX1_612o") return CardDB.cardIDEnum.EX1_612o;
            if (s == "CS2_117") return CardDB.cardIDEnum.CS2_117;
            if (s == "XXX_009e") return CardDB.cardIDEnum.XXX_009e;
            if (s == "EX1_562") return CardDB.cardIDEnum.EX1_562;
            if (s == "EX1_055") return CardDB.cardIDEnum.EX1_055;
            if (s == "TU4e_007") return CardDB.cardIDEnum.TU4e_007;
            if (s == "EX1_317t") return CardDB.cardIDEnum.EX1_317t;
            if (s == "EX1_004e") return CardDB.cardIDEnum.EX1_004e;
            if (s == "EX1_278") return CardDB.cardIDEnum.EX1_278;
            if (s == "CS2_tk1") return CardDB.cardIDEnum.CS2_tk1;
            if (s == "EX1_590") return CardDB.cardIDEnum.EX1_590;
            if (s == "CS1_130") return CardDB.cardIDEnum.CS1_130;
            if (s == "NEW1_008b") return CardDB.cardIDEnum.NEW1_008b;
            if (s == "EX1_365") return CardDB.cardIDEnum.EX1_365;
            if (s == "CS2_141") return CardDB.cardIDEnum.CS2_141;
            if (s == "PRO_001") return CardDB.cardIDEnum.PRO_001;
            if (s == "CS2_173") return CardDB.cardIDEnum.CS2_173;
            if (s == "CS2_017") return CardDB.cardIDEnum.CS2_017;
            if (s == "CRED_16") return CardDB.cardIDEnum.CRED_16;
            if (s == "EX1_392") return CardDB.cardIDEnum.EX1_392;
            if (s == "EX1_593") return CardDB.cardIDEnum.EX1_593;
            if (s == "TU4d_002") return CardDB.cardIDEnum.TU4d_002;
            if (s == "CRED_15") return CardDB.cardIDEnum.CRED_15;
            if (s == "EX1_049") return CardDB.cardIDEnum.EX1_049;
            if (s == "EX1_002") return CardDB.cardIDEnum.EX1_002;
            if (s == "TU4f_005") return CardDB.cardIDEnum.TU4f_005;
            if (s == "NEW1_029t") return CardDB.cardIDEnum.NEW1_029t;
            if (s == "TU4a_001") return CardDB.cardIDEnum.TU4a_001;
            if (s == "CS2_056") return CardDB.cardIDEnum.CS2_056;
            if (s == "EX1_596") return CardDB.cardIDEnum.EX1_596;
            if (s == "EX1_136") return CardDB.cardIDEnum.EX1_136;
            if (s == "EX1_323") return CardDB.cardIDEnum.EX1_323;
            if (s == "CS2_073") return CardDB.cardIDEnum.CS2_073;
            if (s == "EX1_246e") return CardDB.cardIDEnum.EX1_246e;
            if (s == "EX1_244e") return CardDB.cardIDEnum.EX1_244e;
            if (s == "EX1_001") return CardDB.cardIDEnum.EX1_001;
            if (s == "EX1_607e") return CardDB.cardIDEnum.EX1_607e;
            if (s == "EX1_044") return CardDB.cardIDEnum.EX1_044;
            if (s == "EX1_573ae") return CardDB.cardIDEnum.EX1_573ae;
            if (s == "XXX_025") return CardDB.cardIDEnum.XXX_025;
            if (s == "CRED_06") return CardDB.cardIDEnum.CRED_06;
            if (s == "Mekka4") return CardDB.cardIDEnum.Mekka4;
            if (s == "CS2_142") return CardDB.cardIDEnum.CS2_142;
            if (s == "TU4f_004") return CardDB.cardIDEnum.TU4f_004;
            if (s == "EX1_411e2") return CardDB.cardIDEnum.EX1_411e2;
            if (s == "EX1_573") return CardDB.cardIDEnum.EX1_573;
            if (s == "CS2_050") return CardDB.cardIDEnum.CS2_050;
            if (s == "CS2_063e") return CardDB.cardIDEnum.CS2_063e;
            if (s == "EX1_390") return CardDB.cardIDEnum.EX1_390;
            if (s == "EX1_610") return CardDB.cardIDEnum.EX1_610;
            if (s == "hexfrog") return CardDB.cardIDEnum.hexfrog;
            if (s == "CS2_181e") return CardDB.cardIDEnum.CS2_181e;
            if (s == "XXX_027") return CardDB.cardIDEnum.XXX_027;
            if (s == "CS2_082") return CardDB.cardIDEnum.CS2_082;
            if (s == "NEW1_040") return CardDB.cardIDEnum.NEW1_040;
            if (s == "DREAM_01") return CardDB.cardIDEnum.DREAM_01;
            if (s == "EX1_595") return CardDB.cardIDEnum.EX1_595;
            if (s == "CS2_013") return CardDB.cardIDEnum.CS2_013;
            if (s == "CS2_077") return CardDB.cardIDEnum.CS2_077;
            if (s == "NEW1_014") return CardDB.cardIDEnum.NEW1_014;
            if (s == "CRED_05") return CardDB.cardIDEnum.CRED_05;
            if (s == "GAME_002") return CardDB.cardIDEnum.GAME_002;
            if (s == "EX1_165") return CardDB.cardIDEnum.EX1_165;
            if (s == "CS2_013t") return CardDB.cardIDEnum.CS2_013t;
            if (s == "EX1_tk11") return CardDB.cardIDEnum.EX1_tk11;
            if (s == "EX1_591") return CardDB.cardIDEnum.EX1_591;
            if (s == "EX1_549") return CardDB.cardIDEnum.EX1_549;
            if (s == "CS2_045") return CardDB.cardIDEnum.CS2_045;
            if (s == "CS2_237") return CardDB.cardIDEnum.CS2_237;
            if (s == "CS2_027") return CardDB.cardIDEnum.CS2_027;
            if (s == "EX1_508o") return CardDB.cardIDEnum.EX1_508o;
            if (s == "CS2_101t") return CardDB.cardIDEnum.CS2_101t;
            if (s == "CS2_063") return CardDB.cardIDEnum.CS2_063;
            if (s == "EX1_145") return CardDB.cardIDEnum.EX1_145;
            if (s == "EX1_110") return CardDB.cardIDEnum.EX1_110;
            if (s == "EX1_408") return CardDB.cardIDEnum.EX1_408;
            if (s == "EX1_544") return CardDB.cardIDEnum.EX1_544;
            if (s == "TU4c_006") return CardDB.cardIDEnum.TU4c_006;
            if (s == "CS2_151") return CardDB.cardIDEnum.CS2_151;
            if (s == "CS2_073e") return CardDB.cardIDEnum.CS2_073e;
            if (s == "XXX_006") return CardDB.cardIDEnum.XXX_006;
            if (s == "CS2_088") return CardDB.cardIDEnum.CS2_088;
            if (s == "EX1_057") return CardDB.cardIDEnum.EX1_057;
            if (s == "CS2_169") return CardDB.cardIDEnum.CS2_169;
            if (s == "EX1_573t") return CardDB.cardIDEnum.EX1_573t;
            if (s == "EX1_323h") return CardDB.cardIDEnum.EX1_323h;
            if (s == "EX1_tk9") return CardDB.cardIDEnum.EX1_tk9;
            if (s == "NEW1_018e") return CardDB.cardIDEnum.NEW1_018e;
            if (s == "CS2_037") return CardDB.cardIDEnum.CS2_037;
            if (s == "CS2_007") return CardDB.cardIDEnum.CS2_007;
            if (s == "EX1_059e2") return CardDB.cardIDEnum.EX1_059e2;
            if (s == "CS2_227") return CardDB.cardIDEnum.CS2_227;
            if (s == "EX1_570e") return CardDB.cardIDEnum.EX1_570e;
            if (s == "NEW1_003") return CardDB.cardIDEnum.NEW1_003;
            if (s == "GAME_006") return CardDB.cardIDEnum.GAME_006;
            if (s == "EX1_320") return CardDB.cardIDEnum.EX1_320;
            if (s == "EX1_097") return CardDB.cardIDEnum.EX1_097;
            if (s == "tt_004") return CardDB.cardIDEnum.tt_004;
            if (s == "EX1_360e") return CardDB.cardIDEnum.EX1_360e;
            if (s == "EX1_096") return CardDB.cardIDEnum.EX1_096;
            if (s == "DS1_175o") return CardDB.cardIDEnum.DS1_175o;
            if (s == "EX1_596e") return CardDB.cardIDEnum.EX1_596e;
            if (s == "XXX_014") return CardDB.cardIDEnum.XXX_014;
            if (s == "EX1_158e") return CardDB.cardIDEnum.EX1_158e;
            if (s == "CRED_01") return CardDB.cardIDEnum.CRED_01;
            if (s == "CRED_08") return CardDB.cardIDEnum.CRED_08;
            if (s == "EX1_126") return CardDB.cardIDEnum.EX1_126;
            if (s == "EX1_577") return CardDB.cardIDEnum.EX1_577;
            if (s == "EX1_319") return CardDB.cardIDEnum.EX1_319;
            if (s == "EX1_611") return CardDB.cardIDEnum.EX1_611;
            if (s == "CS2_146") return CardDB.cardIDEnum.CS2_146;
            if (s == "EX1_154b") return CardDB.cardIDEnum.EX1_154b;
            if (s == "skele11") return CardDB.cardIDEnum.skele11;
            if (s == "EX1_165t2") return CardDB.cardIDEnum.EX1_165t2;
            if (s == "CS2_172") return CardDB.cardIDEnum.CS2_172;
            if (s == "CS2_114") return CardDB.cardIDEnum.CS2_114;
            if (s == "CS1_069") return CardDB.cardIDEnum.CS1_069;
            if (s == "XXX_003") return CardDB.cardIDEnum.XXX_003;
            if (s == "XXX_042") return CardDB.cardIDEnum.XXX_042;
            if (s == "EX1_173") return CardDB.cardIDEnum.EX1_173;
            if (s == "CS1_042") return CardDB.cardIDEnum.CS1_042;
            if (s == "EX1_506a") return CardDB.cardIDEnum.EX1_506a;
            if (s == "EX1_298") return CardDB.cardIDEnum.EX1_298;
            if (s == "CS2_104") return CardDB.cardIDEnum.CS2_104;
            if (s == "HERO_02") return CardDB.cardIDEnum.HERO_02;
            if (s == "EX1_316e") return CardDB.cardIDEnum.EX1_316e;
            if (s == "EX1_044e") return CardDB.cardIDEnum.EX1_044e;
            if (s == "CS2_051") return CardDB.cardIDEnum.CS2_051;
            if (s == "NEW1_016") return CardDB.cardIDEnum.NEW1_016;
            if (s == "EX1_304e") return CardDB.cardIDEnum.EX1_304e;
            if (s == "EX1_033") return CardDB.cardIDEnum.EX1_033;
            if (s == "EX1_028") return CardDB.cardIDEnum.EX1_028;
            if (s == "XXX_011") return CardDB.cardIDEnum.XXX_011;
            if (s == "EX1_621") return CardDB.cardIDEnum.EX1_621;
            if (s == "EX1_554") return CardDB.cardIDEnum.EX1_554;
            if (s == "EX1_091") return CardDB.cardIDEnum.EX1_091;
            if (s == "EX1_409") return CardDB.cardIDEnum.EX1_409;
            if (s == "EX1_363e") return CardDB.cardIDEnum.EX1_363e;
            if (s == "EX1_410") return CardDB.cardIDEnum.EX1_410;
            if (s == "TU4e_005") return CardDB.cardIDEnum.TU4e_005;
            if (s == "CS2_039") return CardDB.cardIDEnum.CS2_039;
            if (s == "EX1_557") return CardDB.cardIDEnum.EX1_557;
            if (s == "CS2_105e") return CardDB.cardIDEnum.CS2_105e;
            if (s == "EX1_128e") return CardDB.cardIDEnum.EX1_128e;
            if (s == "XXX_021") return CardDB.cardIDEnum.XXX_021;
            if (s == "DS1_070") return CardDB.cardIDEnum.DS1_070;
            if (s == "CS2_033") return CardDB.cardIDEnum.CS2_033;
            if (s == "EX1_536") return CardDB.cardIDEnum.EX1_536;
            if (s == "TU4a_003") return CardDB.cardIDEnum.TU4a_003;
            if (s == "EX1_559") return CardDB.cardIDEnum.EX1_559;
            if (s == "XXX_023") return CardDB.cardIDEnum.XXX_023;
            if (s == "NEW1_033o") return CardDB.cardIDEnum.NEW1_033o;
            if (s == "CS2_004e") return CardDB.cardIDEnum.CS2_004e;
            if (s == "CS2_052") return CardDB.cardIDEnum.CS2_052;
            if (s == "EX1_539") return CardDB.cardIDEnum.EX1_539;
            if (s == "EX1_575") return CardDB.cardIDEnum.EX1_575;
            if (s == "CS2_083b") return CardDB.cardIDEnum.CS2_083b;
            if (s == "CS2_061") return CardDB.cardIDEnum.CS2_061;
            if (s == "NEW1_021") return CardDB.cardIDEnum.NEW1_021;
            if (s == "DS1_055") return CardDB.cardIDEnum.DS1_055;
            if (s == "EX1_625") return CardDB.cardIDEnum.EX1_625;
            if (s == "EX1_382e") return CardDB.cardIDEnum.EX1_382e;
            if (s == "CS2_092e") return CardDB.cardIDEnum.CS2_092e;
            if (s == "CS2_026") return CardDB.cardIDEnum.CS2_026;
            if (s == "NEW1_012o") return CardDB.cardIDEnum.NEW1_012o;
            if (s == "EX1_619e") return CardDB.cardIDEnum.EX1_619e;
            if (s == "EX1_294") return CardDB.cardIDEnum.EX1_294;
            if (s == "EX1_287") return CardDB.cardIDEnum.EX1_287;
            if (s == "EX1_509e") return CardDB.cardIDEnum.EX1_509e;
            if (s == "EX1_625t2") return CardDB.cardIDEnum.EX1_625t2;
            if (s == "CS2_118") return CardDB.cardIDEnum.CS2_118;
            if (s == "CS2_124") return CardDB.cardIDEnum.CS2_124;
            if (s == "Mekka3") return CardDB.cardIDEnum.Mekka3;
            if (s == "EX1_112") return CardDB.cardIDEnum.EX1_112;
            if (s == "CS2_009e") return CardDB.cardIDEnum.CS2_009e;
            if (s == "HERO_04") return CardDB.cardIDEnum.HERO_04;
            if (s == "EX1_607") return CardDB.cardIDEnum.EX1_607;
            if (s == "DREAM_03") return CardDB.cardIDEnum.DREAM_03;
            if (s == "EX1_103e") return CardDB.cardIDEnum.EX1_103e;
            if (s == "CS2_105") return CardDB.cardIDEnum.CS2_105;
            if (s == "TU4c_002") return CardDB.cardIDEnum.TU4c_002;
            if (s == "CRED_14") return CardDB.cardIDEnum.CRED_14;
            if (s == "EX1_567") return CardDB.cardIDEnum.EX1_567;
            if (s == "TU4c_004") return CardDB.cardIDEnum.TU4c_004;
            if (s == "DS1_184") return CardDB.cardIDEnum.DS1_184;
            if (s == "CS2_029") return CardDB.cardIDEnum.CS2_029;
            if (s == "TU4e_006") return CardDB.cardIDEnum.TU4e_006;
            if (s == "GAME_005") return CardDB.cardIDEnum.GAME_005;
            if (s == "CS2_187") return CardDB.cardIDEnum.CS2_187;
            if (s == "EX1_020") return CardDB.cardIDEnum.EX1_020;
            if (s == "EX1_011") return CardDB.cardIDEnum.EX1_011;
            if (s == "CS2_057") return CardDB.cardIDEnum.CS2_057;
            if (s == "EX1_274") return CardDB.cardIDEnum.EX1_274;
            if (s == "EX1_306") return CardDB.cardIDEnum.EX1_306;
            if (s == "NEW1_038o") return CardDB.cardIDEnum.NEW1_038o;
            if (s == "EX1_170") return CardDB.cardIDEnum.EX1_170;
            if (s == "EX1_617") return CardDB.cardIDEnum.EX1_617;
            if (s == "CS1_113e") return CardDB.cardIDEnum.CS1_113e;
            if (s == "CS2_101") return CardDB.cardIDEnum.CS2_101;
            if (s == "CS2_005") return CardDB.cardIDEnum.CS2_005;
            if (s == "EX1_537") return CardDB.cardIDEnum.EX1_537;
            if (s == "EX1_384") return CardDB.cardIDEnum.EX1_384;
            if (s == "TU4a_002") return CardDB.cardIDEnum.TU4a_002;
            if (s == "EX1_362") return CardDB.cardIDEnum.EX1_362;
            if (s == "TU4c_005") return CardDB.cardIDEnum.TU4c_005;
            if (s == "EX1_301") return CardDB.cardIDEnum.EX1_301;
            if (s == "CS2_235") return CardDB.cardIDEnum.CS2_235;
            if (s == "EX1_029") return CardDB.cardIDEnum.EX1_029;
            if (s == "CS2_042") return CardDB.cardIDEnum.CS2_042;
            if (s == "EX1_155a") return CardDB.cardIDEnum.EX1_155a;
            if (s == "CS2_102") return CardDB.cardIDEnum.CS2_102;
            if (s == "EX1_609") return CardDB.cardIDEnum.EX1_609;
            if (s == "NEW1_027") return CardDB.cardIDEnum.NEW1_027;
            if (s == "CS2_236e") return CardDB.cardIDEnum.CS2_236e;
            if (s == "CS2_083e") return CardDB.cardIDEnum.CS2_083e;
            if (s == "EX1_165a") return CardDB.cardIDEnum.EX1_165a;
            if (s == "EX1_570") return CardDB.cardIDEnum.EX1_570;
            if (s == "EX1_131") return CardDB.cardIDEnum.EX1_131;
            if (s == "EX1_556") return CardDB.cardIDEnum.EX1_556;
            if (s == "EX1_543") return CardDB.cardIDEnum.EX1_543;
            if (s == "TU4c_008e") return CardDB.cardIDEnum.TU4c_008e;
            if (s == "EX1_379e") return CardDB.cardIDEnum.EX1_379e;
            if (s == "NEW1_009") return CardDB.cardIDEnum.NEW1_009;
            if (s == "EX1_100") return CardDB.cardIDEnum.EX1_100;
            if (s == "EX1_274e") return CardDB.cardIDEnum.EX1_274e;
            if (s == "CRED_02") return CardDB.cardIDEnum.CRED_02;
            if (s == "EX1_573a") return CardDB.cardIDEnum.EX1_573a;
            if (s == "CS2_084") return CardDB.cardIDEnum.CS2_084;
            if (s == "EX1_582") return CardDB.cardIDEnum.EX1_582;
            if (s == "EX1_043") return CardDB.cardIDEnum.EX1_043;
            if (s == "EX1_050") return CardDB.cardIDEnum.EX1_050;
            if (s == "TU4b_001") return CardDB.cardIDEnum.TU4b_001;
            if (s == "EX1_620") return CardDB.cardIDEnum.EX1_620;
            if (s == "EX1_303") return CardDB.cardIDEnum.EX1_303;
            if (s == "HERO_09") return CardDB.cardIDEnum.HERO_09;
            if (s == "EX1_067") return CardDB.cardIDEnum.EX1_067;
            if (s == "XXX_028") return CardDB.cardIDEnum.XXX_028;
            if (s == "EX1_277") return CardDB.cardIDEnum.EX1_277;
            if (s == "Mekka2") return CardDB.cardIDEnum.Mekka2;
            if (s == "CS2_221e") return CardDB.cardIDEnum.CS2_221e;
            if (s == "EX1_178") return CardDB.cardIDEnum.EX1_178;
            if (s == "CS2_222") return CardDB.cardIDEnum.CS2_222;
            if (s == "EX1_409e") return CardDB.cardIDEnum.EX1_409e;
            if (s == "tt_004o") return CardDB.cardIDEnum.tt_004o;
            if (s == "EX1_155ae") return CardDB.cardIDEnum.EX1_155ae;
            if (s == "EX1_160a") return CardDB.cardIDEnum.EX1_160a;
            if (s == "NEW1_025e") return CardDB.cardIDEnum.NEW1_025e;
            if (s == "CS2_012") return CardDB.cardIDEnum.CS2_012;
            if (s == "EX1_246") return CardDB.cardIDEnum.EX1_246;
            if (s == "EX1_572") return CardDB.cardIDEnum.EX1_572;
            if (s == "EX1_089") return CardDB.cardIDEnum.EX1_089;
            if (s == "CS2_059") return CardDB.cardIDEnum.CS2_059;
            if (s == "EX1_279") return CardDB.cardIDEnum.EX1_279;
            if (s == "CS2_168") return CardDB.cardIDEnum.CS2_168;
            if (s == "tt_010") return CardDB.cardIDEnum.tt_010;
            if (s == "NEW1_023") return CardDB.cardIDEnum.NEW1_023;
            if (s == "CS2_075") return CardDB.cardIDEnum.CS2_075;
            if (s == "EX1_316") return CardDB.cardIDEnum.EX1_316;
            if (s == "CS2_025") return CardDB.cardIDEnum.CS2_025;
            if (s == "CS2_234") return CardDB.cardIDEnum.CS2_234;
            if (s == "XXX_043") return CardDB.cardIDEnum.XXX_043;
            if (s == "GAME_001") return CardDB.cardIDEnum.GAME_001;
            if (s == "EX1_130") return CardDB.cardIDEnum.EX1_130;
            if (s == "EX1_584e") return CardDB.cardIDEnum.EX1_584e;
            if (s == "CS2_064") return CardDB.cardIDEnum.CS2_064;
            if (s == "EX1_161") return CardDB.cardIDEnum.EX1_161;
            if (s == "CS2_049") return CardDB.cardIDEnum.CS2_049;
            if (s == "EX1_154") return CardDB.cardIDEnum.EX1_154;
            if (s == "EX1_080") return CardDB.cardIDEnum.EX1_080;
            if (s == "NEW1_022") return CardDB.cardIDEnum.NEW1_022;
            if (s == "EX1_160be") return CardDB.cardIDEnum.EX1_160be;
            if (s == "EX1_251") return CardDB.cardIDEnum.EX1_251;
            if (s == "EX1_371") return CardDB.cardIDEnum.EX1_371;
            if (s == "CS2_mirror") return CardDB.cardIDEnum.CS2_mirror;
            if (s == "EX1_594") return CardDB.cardIDEnum.EX1_594;
            if (s == "TU4c_006e") return CardDB.cardIDEnum.TU4c_006e;
            if (s == "EX1_560") return CardDB.cardIDEnum.EX1_560;
            if (s == "CS2_236") return CardDB.cardIDEnum.CS2_236;
            if (s == "TU4f_006") return CardDB.cardIDEnum.TU4f_006;
            if (s == "EX1_402") return CardDB.cardIDEnum.EX1_402;
            if (s == "EX1_506") return CardDB.cardIDEnum.EX1_506;
            if (s == "NEW1_027e") return CardDB.cardIDEnum.NEW1_027e;
            if (s == "DS1_070o") return CardDB.cardIDEnum.DS1_070o;
            if (s == "XXX_045") return CardDB.cardIDEnum.XXX_045;
            if (s == "XXX_029") return CardDB.cardIDEnum.XXX_029;
            if (s == "DS1_178") return CardDB.cardIDEnum.DS1_178;
            if (s == "EX1_315") return CardDB.cardIDEnum.EX1_315;
            if (s == "CS2_094") return CardDB.cardIDEnum.CS2_094;
            if (s == "TU4e_002t") return CardDB.cardIDEnum.TU4e_002t;
            if (s == "EX1_046e") return CardDB.cardIDEnum.EX1_046e;
            if (s == "NEW1_040t") return CardDB.cardIDEnum.NEW1_040t;
            if (s == "GAME_005e") return CardDB.cardIDEnum.GAME_005e;
            if (s == "CS2_131") return CardDB.cardIDEnum.CS2_131;
            if (s == "XXX_008") return CardDB.cardIDEnum.XXX_008;
            if (s == "EX1_531e") return CardDB.cardIDEnum.EX1_531e;
            if (s == "CS2_226e") return CardDB.cardIDEnum.CS2_226e;
            if (s == "XXX_022e") return CardDB.cardIDEnum.XXX_022e;
            if (s == "DS1_178e") return CardDB.cardIDEnum.DS1_178e;
            if (s == "CS2_226o") return CardDB.cardIDEnum.CS2_226o;
            if (s == "Mekka4e") return CardDB.cardIDEnum.Mekka4e;
            if (s == "EX1_082") return CardDB.cardIDEnum.EX1_082;
            if (s == "CS2_093") return CardDB.cardIDEnum.CS2_093;
            if (s == "EX1_411e") return CardDB.cardIDEnum.EX1_411e;
            if (s == "EX1_145o") return CardDB.cardIDEnum.EX1_145o;
            if (s == "CS2_boar") return CardDB.cardIDEnum.CS2_boar;
            if (s == "NEW1_019") return CardDB.cardIDEnum.NEW1_019;
            if (s == "EX1_289") return CardDB.cardIDEnum.EX1_289;
            if (s == "EX1_025t") return CardDB.cardIDEnum.EX1_025t;
            if (s == "EX1_398t") return CardDB.cardIDEnum.EX1_398t;
            if (s == "EX1_055o") return CardDB.cardIDEnum.EX1_055o;
            if (s == "CS2_091") return CardDB.cardIDEnum.CS2_091;
            if (s == "EX1_241") return CardDB.cardIDEnum.EX1_241;
            if (s == "EX1_085") return CardDB.cardIDEnum.EX1_085;
            if (s == "CS2_200") return CardDB.cardIDEnum.CS2_200;
            if (s == "CS2_034") return CardDB.cardIDEnum.CS2_034;
            if (s == "EX1_583") return CardDB.cardIDEnum.EX1_583;
            if (s == "EX1_584") return CardDB.cardIDEnum.EX1_584;
            if (s == "EX1_155") return CardDB.cardIDEnum.EX1_155;
            if (s == "EX1_622") return CardDB.cardIDEnum.EX1_622;
            if (s == "CS2_203") return CardDB.cardIDEnum.CS2_203;
            if (s == "EX1_124") return CardDB.cardIDEnum.EX1_124;
            if (s == "EX1_379") return CardDB.cardIDEnum.EX1_379;
            if (s == "CS2_053e") return CardDB.cardIDEnum.CS2_053e;
            if (s == "EX1_032") return CardDB.cardIDEnum.EX1_032;
            if (s == "TU4e_003") return CardDB.cardIDEnum.TU4e_003;
            if (s == "CS2_146o") return CardDB.cardIDEnum.CS2_146o;
            if (s == "XXX_041") return CardDB.cardIDEnum.XXX_041;
            if (s == "EX1_391") return CardDB.cardIDEnum.EX1_391;
            if (s == "EX1_366") return CardDB.cardIDEnum.EX1_366;
            if (s == "EX1_059e") return CardDB.cardIDEnum.EX1_059e;
            if (s == "XXX_012") return CardDB.cardIDEnum.XXX_012;
            if (s == "EX1_565o") return CardDB.cardIDEnum.EX1_565o;
            if (s == "EX1_001e") return CardDB.cardIDEnum.EX1_001e;
            if (s == "TU4f_003") return CardDB.cardIDEnum.TU4f_003;
            if (s == "EX1_400") return CardDB.cardIDEnum.EX1_400;
            if (s == "EX1_614") return CardDB.cardIDEnum.EX1_614;
            if (s == "EX1_561") return CardDB.cardIDEnum.EX1_561;
            if (s == "EX1_332") return CardDB.cardIDEnum.EX1_332;
            if (s == "HERO_05") return CardDB.cardIDEnum.HERO_05;
            if (s == "CS2_065") return CardDB.cardIDEnum.CS2_065;
            if (s == "ds1_whelptoken") return CardDB.cardIDEnum.ds1_whelptoken;
            if (s == "EX1_536e") return CardDB.cardIDEnum.EX1_536e;
            if (s == "CS2_032") return CardDB.cardIDEnum.CS2_032;
            if (s == "CS2_120") return CardDB.cardIDEnum.CS2_120;
            if (s == "EX1_155be") return CardDB.cardIDEnum.EX1_155be;
            if (s == "EX1_247") return CardDB.cardIDEnum.EX1_247;
            if (s == "EX1_154a") return CardDB.cardIDEnum.EX1_154a;
            if (s == "EX1_554t") return CardDB.cardIDEnum.EX1_554t;
            if (s == "CS2_103e2") return CardDB.cardIDEnum.CS2_103e2;
            if (s == "TU4d_003") return CardDB.cardIDEnum.TU4d_003;
            if (s == "NEW1_026t") return CardDB.cardIDEnum.NEW1_026t;
            if (s == "EX1_623") return CardDB.cardIDEnum.EX1_623;
            if (s == "EX1_383t") return CardDB.cardIDEnum.EX1_383t;
            if (s == "EX1_597") return CardDB.cardIDEnum.EX1_597;
            if (s == "TU4f_006o") return CardDB.cardIDEnum.TU4f_006o;
            if (s == "EX1_130a") return CardDB.cardIDEnum.EX1_130a;
            if (s == "CS2_011") return CardDB.cardIDEnum.CS2_011;
            if (s == "EX1_169") return CardDB.cardIDEnum.EX1_169;
            if (s == "EX1_tk33") return CardDB.cardIDEnum.EX1_tk33;
            if (s == "EX1_250") return CardDB.cardIDEnum.EX1_250;
            if (s == "EX1_564") return CardDB.cardIDEnum.EX1_564;
            if (s == "EX1_043e") return CardDB.cardIDEnum.EX1_043e;
            if (s == "EX1_349") return CardDB.cardIDEnum.EX1_349;
            if (s == "EX1_102") return CardDB.cardIDEnum.EX1_102;
            if (s == "EX1_058") return CardDB.cardIDEnum.EX1_058;
            if (s == "EX1_243") return CardDB.cardIDEnum.EX1_243;
            if (s == "PRO_001c") return CardDB.cardIDEnum.PRO_001c;
            if (s == "EX1_116t") return CardDB.cardIDEnum.EX1_116t;
            if (s == "CS2_089") return CardDB.cardIDEnum.CS2_089;
            if (s == "TU4c_001") return CardDB.cardIDEnum.TU4c_001;
            if (s == "EX1_248") return CardDB.cardIDEnum.EX1_248;
            if (s == "NEW1_037e") return CardDB.cardIDEnum.NEW1_037e;
            if (s == "CS2_122") return CardDB.cardIDEnum.CS2_122;
            if (s == "EX1_393") return CardDB.cardIDEnum.EX1_393;
            if (s == "CS2_232") return CardDB.cardIDEnum.CS2_232;
            if (s == "EX1_165b") return CardDB.cardIDEnum.EX1_165b;
            if (s == "NEW1_030") return CardDB.cardIDEnum.NEW1_030;
            if (s == "EX1_161o") return CardDB.cardIDEnum.EX1_161o;
            if (s == "EX1_093e") return CardDB.cardIDEnum.EX1_093e;
            if (s == "CS2_150") return CardDB.cardIDEnum.CS2_150;
            if (s == "CS2_152") return CardDB.cardIDEnum.CS2_152;
            if (s == "EX1_160t") return CardDB.cardIDEnum.EX1_160t;
            if (s == "CS2_127") return CardDB.cardIDEnum.CS2_127;
            if (s == "CRED_03") return CardDB.cardIDEnum.CRED_03;
            if (s == "DS1_188") return CardDB.cardIDEnum.DS1_188;
            if (s == "XXX_001") return CardDB.cardIDEnum.XXX_001;
            return CardDB.cardIDEnum.None;
        }

        public enum cardName
        {
            unknown,
            shadeofnaxxramas,//naxx
            deathsbite,//naxx
            nerubian,//naxx
            reincarnation,//naxx
            poisonseeds,//naxx
            baronrivendare,//naxx
            nerubianegg,//naxx
            dancingswords,//naxx
            anubarambusher,//naxx
            voidcaller,//naxx
            darkcultist,//naxx
            webspinner,//naxx
            undertaker,//naxx
            hogger,
            starfall,
            barrel,
            damagereflector,
            edwinvancleef,
            perditionsblade,
            bloodsailraider,
            bloodmagethalnos,
            rooted,
            wisp,
            rachelledavis,
            senjinshieldmasta,
            totemicmight,
            uproot,
            opponentdisconnect,
            shandoslesson,
            hemetnesingwary,
            mindgameswhiffedyouropponenthadnominions,
            dragonlingmechanic,
            mogushanwarden,
            hungrycrab,
            ancientteachings,
            misdirection,
            patientassassin,
            violetteacher,
            arathiweaponsmith,
            acolyteofpain,
            holynova,
            robpardo,
            commandingshout,
            unboundelemental,
            garroshhellscream,
            enchant,
            blessingofmight,
            nightmare,
            blessingofkings,
            polymorph,
            darkirondwarf,
            destroy,
            roguesdoit,
            freecards,
            iammurloc,
            charge,
            stampedingkodo,
            humility,
            gruul,
            markofthewild,
            worgeninfiltrator,
            frostbolt,
            flametonguetotem,
            assassinate,
            lordofthearena,
            bainebloodhoof,
            injuredblademaster,
            siphonsoul,
            layonhands,
            lorewalkercho,
            destroyallminions,
            silvermoonguardian,
            huffer,
            mindvision,
            malfurionstormrage,
            corehound,
            grimscaleoracle,
            lightningstorm,
            lightwell,
            benthompson,
            coldlightseer,
            gorehowl,
            farsight,
            chillwindyeti,
            moonfire,
            bladeflurry,
            massdispel,
            crazedalchemist,
            shadowmadness,
            equality,
            misha,
            treant,
            alarmobot,
            animalcompanion,
            dream,
            youngpriestess,
            gadgetzanauctioneer,
            coneofcold,
            earthshock,
            tirionfordring,
            skeleton,
            ironfurgrizzly,
            headcrack,
            arcaneshot,
            imp,
            voidterror,
            mortalcoil,
            draw3cards,
            flameofazzinoth,
            jainaproudmoore,
            execute,
            bloodlust,
            bananas,
            kidnapper,
            oldmurkeye,
            homingchicken,
            enableforattack,
            spellbender,
            backstab,
            squirrel,
            heavyaxe,
            zwick,
            flamesofazzinoth,
            murlocwarleader,
            shadowstep,
            ancestralspirit,
            defenderofargus,
            assassinsblade,
            discard,
            biggamehunter,
            aldorpeacekeeper,
            blizzard,
            pandarenscout,
            unleashthehounds,
            yseraawakens,
            sap,
            defiasbandit,
            gnomishinventor,
            mindcontrol,
            ravenholdtassassin,
            icelance,
            dispel,
            acidicswampooze,
            muklasbigbrother,
            blessedchampion,
            savannahhighmane,
            direwolfalpha,
            hoggersmash,
            blessingofwisdom,
            nourish,
            abusivesergeant,
            sylvanaswindrunner,
            crueltaskmaster,
            lightningbolt,
            keeperofthegrove,
            steadyshot,
            multishot,
            jaybaxter,
            molasses,
            pintsizedsummoner,
            spellbreaker,
            deadlypoison,
            bloodfury,
            fanofknives,
            shieldbearer,
            sensedemons,
            shieldblock,
            handswapperminion,
            massivegnoll,
            ancientoflore,
            oasissnapjaw,
            illidanstormrage,
            frostwolfgrunt,
            lesserheal,
            infernal,
            wildpyromancer,
            razorfenhunter,
            twistingnether,
            leaderofthepack,
            malygos,
            becomehogger,
            millhousemanastorm,
            innerfire,
            valeerasanguinar,
            chicken,
            souloftheforest,
            silencedebug,
            bloodsailcorsair,
            tinkmasteroverspark,
            iceblock,
            brawl,
            vanish,
            murloc,
            mindspike,
            kingmukla,
            stevengabriel,
            truesilverchampion,
            harrisonjones,
            devilsaur,
            wargolem,
            warsongcommander,
            manawyrm,
            savagery,
            spitefulsmith,
            shatteredsuncleric,
            eyeforaneye,
            azuredrake,
            mountaingiant,
            korkronelite,
            junglepanther,
            barongeddon,
            pitlord,
            markofnature,
            leokk,
            fierywaraxe,
            damage5,
            restore5,
            mindblast,
            timberwolf,
            captaingreenskin,
            elvenarcher,
            michaelschweitzer,
            masterswordsmith,
            grommashhellscream,
            hound,
            seagiant,
            doomguard,
            alakirthewindlord,
            hyena,
            frothingberserker,
            powerofthewild,
            druidoftheclaw,
            hellfire,
            archmage,
            recklessrocketeer,
            crazymonkey,
            damageallbut1,
            powerwordshield,
            arcaneintellect,
            angrychicken,
            mindgames,
            leeroyjenkins,
            gurubashiberserker,
            windspeaker,
            enableemotes,
            forceofnature,
            lightspawn,
            warglaiveofazzinoth,
            finkleeinhorn,
            frostelemental,
            thoughtsteal,
            brianschwab,
            scavenginghyena,
            si7agent,
            prophetvelen,
            soulfire,
            ogremagi,
            damagedgolem,
            crash,
            adrenalinerush,
            murloctidecaller,
            kirintormage,
            thrallmarfarseer,
            frostwolfwarlord,
            sorcerersapprentice,
            willofmukla,
            holyfire,
            manawraith,
            argentsquire,
            placeholdercard,
            snakeball,
            ancientwatcher,
            noviceengineer,
            stonetuskboar,
            ancestralhealing,
            conceal,
            arcanitereaper,
            guldan,
            ragingworgen,
            earthenringfarseer,
            onyxia,
            manaaddict,
            dualwarglaives,
            worthlessimp,
            shiv,
            sheep,
            bloodknight,
            holysmite,
            ancientsecrets,
            holywrath,
            ironforgerifleman,
            elitetaurenchieftain,
            bluegillwarrior,
            shapeshift,
            hamiltonchu,
            battlerage,
            nightblade,
            crazedhunter,
            andybrock,
            youthfulbrewmaster,
            theblackknight,
            brewmaster,
            lifetap,
            demonfire,
            redemption,
            lordjaraxxus,
            coldblood,
            lightwarden,
            questingadventurer,
            donothing,
            dereksakamoto,
            poultryizer,
            koboldgeomancer,
            legacyoftheemperor,
            cenarius,
            searingtotem,
            taurenwarrior,
            explosivetrap,
            frog,
            servercrash,
            wickedknife,
            laughingsister,
            cultmaster,
            wildgrowth,
            sprint,
            masterofdisguise,
            kyleharrison,
            avatarofthecoin,
            excessmana,
            spiritwolf,
            auchenaisoulpriest,
            bestialwrath,
            rockbiterweapon,
            starvingbuzzard,
            mirrorimage,
            silverhandrecruit,
            corruption,
            preparation,
            cairnebloodhoof,
            mortalstrike,
            flare,
            silverhandknight,
            breakweapon,
            guardianofkings,
            ancientbrewmaster,
            youngdragonhawk,
            frostshock,
            healingtouch,
            venturecomercenary,
            sacrificialpact,
            noooooooooooo,
            baneofdoom,
            abomination,
            flesheatingghoul,
            loothoarder,
            mill10,
            jasonchayes,
            benbrode,
            betrayal,
            thebeast,
            flameimp,
            freezingtrap,
            southseadeckhand,
            wrath,
            bloodfenraptor,
            cleave,
            fencreeper,
            restore1,
            handtodeck,
            starfire,
            goldshirefootman,
            murlocscout,
            ragnarosthefirelord,
            rampage,
            thrall,
            stoneclawtotem,
            captainsparrot,
            windfuryharpy,
            stranglethorntiger,
            summonarandomsecret,
            circleofhealing,
            snaketrap,
            cabalshadowpriest,
            upgrade,
            shieldslam,
            flameburst,
            windfury,
            natpagle,
            restoreallhealth,
            houndmaster,
            waterelemental,
            eaglehornbow,
            gnoll,
            archmageantonidas,
            destroyallheroes,
            wrathofairtotem,
            killcommand,
            manatidetotem,
            daggermastery,
            drainlife,
            doomsayer,
            darkscalehealer,
            shadowform,
            frostnova,
            mirrorentity,
            counterspell,
            mindshatter,
            magmarager,
            wolfrider,
            emboldener3000,
            gelbinmekkatorque,
            utherlightbringer,
            innerrage,
            emeralddrake,
            heroicstrike,
            barreltoss,
            yongwoo,
            doomhammer,
            stomp,
            tracking,
            fireball,
            metamorphosis,
            thecoin,
            bootybaybodyguard,
            scarletcrusader,
            voodoodoctor,
            shadowbolt,
            etherealarcanist,
            succubus,
            emperorcobra,
            deadlyshot,
            reinforce,
            claw,
            explosiveshot,
            avengingwrath,
            riverpawgnoll,
            argentprotector,
            hiddengnome,
            felguard,
            northshirecleric,
            lepergnome,
            fireelemental,
            armorup,
            snipe,
            southseacaptain,
            catform,
            bite,
            defiasringleader,
            harvestgolem,
            kingkrush,
            healingtotem,
            ericdodds,
            demigodsfavor,
            huntersmark,
            dalaranmage,
            twilightdrake,
            coldlightoracle,
            moltengiant,
            shadowflame,
            anduinwrynn,
            argentcommander,
            revealhand,
            arcanemissiles,
            repairbot,
            ancientofwar,
            stormwindchampion,
            summonapanther,
            swipe,
            hex,
            ysera,
            arcanegolem,
            bloodimp,
            pyroblast,
            murlocraider,
            faeriedragon,
            sinisterstrike,
            poweroverwhelming,
            arcaneexplosion,
            shadowwordpain,
            mill30,
            noblesacrifice,
            dreadinfernal,
            naturalize,
            totemiccall,
            secretkeeper,
            dreadcorsair,
            forkedlightning,
            handofprotection,
            vaporize,
            nozdormu,
            divinespirit,
            transcendence,
            armorsmith,
            murloctidehunter,
            stealcard,
            opponentconcede,
            tundrarhino,
            summoningportal,
            hammerofwrath,
            stormwindknight,
            freeze,
            madbomber,
            consecration,
            boar,
            knifejuggler,
            icebarrier,
            mechanicaldragonling,
            battleaxe,
            lightsjustice,
            lavaburst,
            mindcontroltech,
            boulderfistogre,
            fireblast,
            priestessofelune,
            ancientmage,
            shadowworddeath,
            ironbeakowl,
            eviscerate,
            repentance,
            sunwalker,
            nagamyrmidon,
            destoryheropower,
            slam,
            swordofjustice,
            bounce,
            shadopanmonk,
            whirlwind,
            alexstrasza,
            silence,
            rexxar,
            voidwalker,
            whelp,
            flamestrike,
            rivercrocolisk,
            stormforgedaxe,
            snake,
            shotgunblast,
            violetapprentice,
            templeenforcer,
            ashbringer,
            impmaster,
            defender,
            savageroar,
            innervate,
            inferno,
            earthelemental,
            facelessmanipulator,
            divinefavor,
            demolisher,
            sunfuryprotector,
            dustdevil,
            powerofthehorde,
            holylight,
            feralspirit,
            raidleader,
            amaniberserker,
            ironbarkprotector,
            bearform,
            deathwing,
            stormpikecommando,
            squire,
            panther,
            silverbackpatriarch,
            bobfitch,
            gladiatorslongbow,
            damage1,
        }
        
        public cardName cardNamestringToEnum(string s)
        {
            if (s == "unknown") return CardDB.cardName.unknown;
            if (s == "hogger") return CardDB.cardName.hogger;
            if (s == "starfall") return CardDB.cardName.starfall;
            if (s == "barrel") return CardDB.cardName.barrel;
            if (s == "damagereflector") return CardDB.cardName.damagereflector;
            if (s == "edwinvancleef") return CardDB.cardName.edwinvancleef;
            if (s == "perditionsblade") return CardDB.cardName.perditionsblade;
            if (s == "bloodsailraider") return CardDB.cardName.bloodsailraider;
            if (s == "bloodmagethalnos") return CardDB.cardName.bloodmagethalnos;
            if (s == "rooted") return CardDB.cardName.rooted;
            if (s == "wisp") return CardDB.cardName.wisp;
            if (s == "rachelledavis") return CardDB.cardName.rachelledavis;
            if (s == "senjinshieldmasta") return CardDB.cardName.senjinshieldmasta;
            if (s == "totemicmight") return CardDB.cardName.totemicmight;
            if (s == "uproot") return CardDB.cardName.uproot;
            if (s == "opponentdisconnect") return CardDB.cardName.opponentdisconnect;
            if (s == "shandoslesson") return CardDB.cardName.shandoslesson;
            if (s == "hemetnesingwary") return CardDB.cardName.hemetnesingwary;
            if (s == "mindgameswhiffedyouropponenthadnominions") return CardDB.cardName.mindgameswhiffedyouropponenthadnominions;
            if (s == "dragonlingmechanic") return CardDB.cardName.dragonlingmechanic;
            if (s == "mogushanwarden") return CardDB.cardName.mogushanwarden;
            if (s == "hungrycrab") return CardDB.cardName.hungrycrab;
            if (s == "ancientteachings") return CardDB.cardName.ancientteachings;
            if (s == "misdirection") return CardDB.cardName.misdirection;
            if (s == "patientassassin") return CardDB.cardName.patientassassin;
            if (s == "violetteacher") return CardDB.cardName.violetteacher;
            if (s == "arathiweaponsmith") return CardDB.cardName.arathiweaponsmith;
            if (s == "acolyteofpain") return CardDB.cardName.acolyteofpain;
            if (s == "holynova") return CardDB.cardName.holynova;
            if (s == "robpardo") return CardDB.cardName.robpardo;
            if (s == "commandingshout") return CardDB.cardName.commandingshout;
            if (s == "unboundelemental") return CardDB.cardName.unboundelemental;
            if (s == "garroshhellscream") return CardDB.cardName.garroshhellscream;
            if (s == "enchant") return CardDB.cardName.enchant;
            if (s == "blessingofmight") return CardDB.cardName.blessingofmight;
            if (s == "nightmare") return CardDB.cardName.nightmare;
            if (s == "blessingofkings") return CardDB.cardName.blessingofkings;
            if (s == "polymorph") return CardDB.cardName.polymorph;
            if (s == "darkirondwarf") return CardDB.cardName.darkirondwarf;
            if (s == "destroy") return CardDB.cardName.destroy;
            if (s == "roguesdoit") return CardDB.cardName.roguesdoit;
            if (s == "freecards") return CardDB.cardName.freecards;
            if (s == "iammurloc") return CardDB.cardName.iammurloc;
            if (s == "charge") return CardDB.cardName.charge;
            if (s == "stampedingkodo") return CardDB.cardName.stampedingkodo;
            if (s == "humility") return CardDB.cardName.humility;
            if (s == "gruul") return CardDB.cardName.gruul;
            if (s == "markofthewild") return CardDB.cardName.markofthewild;
            if (s == "worgeninfiltrator") return CardDB.cardName.worgeninfiltrator;
            if (s == "frostbolt") return CardDB.cardName.frostbolt;
            if (s == "flametonguetotem") return CardDB.cardName.flametonguetotem;
            if (s == "assassinate") return CardDB.cardName.assassinate;
            if (s == "lordofthearena") return CardDB.cardName.lordofthearena;
            if (s == "bainebloodhoof") return CardDB.cardName.bainebloodhoof;
            if (s == "injuredblademaster") return CardDB.cardName.injuredblademaster;
            if (s == "siphonsoul") return CardDB.cardName.siphonsoul;
            if (s == "layonhands") return CardDB.cardName.layonhands;
            if (s == "lorewalkercho") return CardDB.cardName.lorewalkercho;
            if (s == "destroyallminions") return CardDB.cardName.destroyallminions;
            if (s == "silvermoonguardian") return CardDB.cardName.silvermoonguardian;
            if (s == "huffer") return CardDB.cardName.huffer;
            if (s == "mindvision") return CardDB.cardName.mindvision;
            if (s == "malfurionstormrage") return CardDB.cardName.malfurionstormrage;
            if (s == "corehound") return CardDB.cardName.corehound;
            if (s == "grimscaleoracle") return CardDB.cardName.grimscaleoracle;
            if (s == "lightningstorm") return CardDB.cardName.lightningstorm;
            if (s == "lightwell") return CardDB.cardName.lightwell;
            if (s == "benthompson") return CardDB.cardName.benthompson;
            if (s == "coldlightseer") return CardDB.cardName.coldlightseer;
            if (s == "gorehowl") return CardDB.cardName.gorehowl;
            if (s == "farsight") return CardDB.cardName.farsight;
            if (s == "chillwindyeti") return CardDB.cardName.chillwindyeti;
            if (s == "moonfire") return CardDB.cardName.moonfire;
            if (s == "bladeflurry") return CardDB.cardName.bladeflurry;
            if (s == "massdispel") return CardDB.cardName.massdispel;
            if (s == "crazedalchemist") return CardDB.cardName.crazedalchemist;
            if (s == "shadowmadness") return CardDB.cardName.shadowmadness;
            if (s == "equality") return CardDB.cardName.equality;
            if (s == "misha") return CardDB.cardName.misha;
            if (s == "treant") return CardDB.cardName.treant;
            if (s == "alarmobot") return CardDB.cardName.alarmobot;
            if (s == "animalcompanion") return CardDB.cardName.animalcompanion;
            if (s == "dream") return CardDB.cardName.dream;
            if (s == "youngpriestess") return CardDB.cardName.youngpriestess;
            if (s == "gadgetzanauctioneer") return CardDB.cardName.gadgetzanauctioneer;
            if (s == "coneofcold") return CardDB.cardName.coneofcold;
            if (s == "earthshock") return CardDB.cardName.earthshock;
            if (s == "tirionfordring") return CardDB.cardName.tirionfordring;
            if (s == "skeleton") return CardDB.cardName.skeleton;
            if (s == "ironfurgrizzly") return CardDB.cardName.ironfurgrizzly;
            if (s == "headcrack") return CardDB.cardName.headcrack;
            if (s == "arcaneshot") return CardDB.cardName.arcaneshot;
            if (s == "imp") return CardDB.cardName.imp;
            if (s == "voidterror") return CardDB.cardName.voidterror;
            if (s == "mortalcoil") return CardDB.cardName.mortalcoil;
            if (s == "draw3cards") return CardDB.cardName.draw3cards;
            if (s == "flameofazzinoth") return CardDB.cardName.flameofazzinoth;
            if (s == "jainaproudmoore") return CardDB.cardName.jainaproudmoore;
            if (s == "execute") return CardDB.cardName.execute;
            if (s == "bloodlust") return CardDB.cardName.bloodlust;
            if (s == "bananas") return CardDB.cardName.bananas;
            if (s == "kidnapper") return CardDB.cardName.kidnapper;
            if (s == "oldmurkeye") return CardDB.cardName.oldmurkeye;
            if (s == "homingchicken") return CardDB.cardName.homingchicken;
            if (s == "enableforattack") return CardDB.cardName.enableforattack;
            if (s == "spellbender") return CardDB.cardName.spellbender;
            if (s == "backstab") return CardDB.cardName.backstab;
            if (s == "squirrel") return CardDB.cardName.squirrel;
            if (s == "heavyaxe") return CardDB.cardName.heavyaxe;
            if (s == "zwick") return CardDB.cardName.zwick;
            if (s == "flamesofazzinoth") return CardDB.cardName.flamesofazzinoth;
            if (s == "murlocwarleader") return CardDB.cardName.murlocwarleader;
            if (s == "shadowstep") return CardDB.cardName.shadowstep;
            if (s == "ancestralspirit") return CardDB.cardName.ancestralspirit;
            if (s == "defenderofargus") return CardDB.cardName.defenderofargus;
            if (s == "assassinsblade") return CardDB.cardName.assassinsblade;
            if (s == "discard") return CardDB.cardName.discard;
            if (s == "biggamehunter") return CardDB.cardName.biggamehunter;
            if (s == "aldorpeacekeeper") return CardDB.cardName.aldorpeacekeeper;
            if (s == "blizzard") return CardDB.cardName.blizzard;
            if (s == "pandarenscout") return CardDB.cardName.pandarenscout;
            if (s == "unleashthehounds") return CardDB.cardName.unleashthehounds;
            if (s == "yseraawakens") return CardDB.cardName.yseraawakens;
            if (s == "sap") return CardDB.cardName.sap;
            if (s == "defiasbandit") return CardDB.cardName.defiasbandit;
            if (s == "gnomishinventor") return CardDB.cardName.gnomishinventor;
            if (s == "mindcontrol") return CardDB.cardName.mindcontrol;
            if (s == "ravenholdtassassin") return CardDB.cardName.ravenholdtassassin;
            if (s == "icelance") return CardDB.cardName.icelance;
            if (s == "dispel") return CardDB.cardName.dispel;
            if (s == "acidicswampooze") return CardDB.cardName.acidicswampooze;
            if (s == "muklasbigbrother") return CardDB.cardName.muklasbigbrother;
            if (s == "blessedchampion") return CardDB.cardName.blessedchampion;
            if (s == "savannahhighmane") return CardDB.cardName.savannahhighmane;
            if (s == "direwolfalpha") return CardDB.cardName.direwolfalpha;
            if (s == "hoggersmash") return CardDB.cardName.hoggersmash;
            if (s == "blessingofwisdom") return CardDB.cardName.blessingofwisdom;
            if (s == "nourish") return CardDB.cardName.nourish;
            if (s == "abusivesergeant") return CardDB.cardName.abusivesergeant;
            if (s == "sylvanaswindrunner") return CardDB.cardName.sylvanaswindrunner;
            if (s == "crueltaskmaster") return CardDB.cardName.crueltaskmaster;
            if (s == "lightningbolt") return CardDB.cardName.lightningbolt;
            if (s == "keeperofthegrove") return CardDB.cardName.keeperofthegrove;
            if (s == "steadyshot") return CardDB.cardName.steadyshot;
            if (s == "multishot") return CardDB.cardName.multishot;
            if (s == "jaybaxter") return CardDB.cardName.jaybaxter;
            if (s == "molasses") return CardDB.cardName.molasses;
            if (s == "pintsizedsummoner") return CardDB.cardName.pintsizedsummoner;
            if (s == "spellbreaker") return CardDB.cardName.spellbreaker;
            if (s == "deadlypoison") return CardDB.cardName.deadlypoison;
            if (s == "bloodfury") return CardDB.cardName.bloodfury;
            if (s == "fanofknives") return CardDB.cardName.fanofknives;
            if (s == "shieldbearer") return CardDB.cardName.shieldbearer;
            if (s == "sensedemons") return CardDB.cardName.sensedemons;
            if (s == "shieldblock") return CardDB.cardName.shieldblock;
            if (s == "handswapperminion") return CardDB.cardName.handswapperminion;
            if (s == "massivegnoll") return CardDB.cardName.massivegnoll;
            if (s == "ancientoflore") return CardDB.cardName.ancientoflore;
            if (s == "oasissnapjaw") return CardDB.cardName.oasissnapjaw;
            if (s == "illidanstormrage") return CardDB.cardName.illidanstormrage;
            if (s == "frostwolfgrunt") return CardDB.cardName.frostwolfgrunt;
            if (s == "lesserheal") return CardDB.cardName.lesserheal;
            if (s == "infernal") return CardDB.cardName.infernal;
            if (s == "wildpyromancer") return CardDB.cardName.wildpyromancer;
            if (s == "razorfenhunter") return CardDB.cardName.razorfenhunter;
            if (s == "twistingnether") return CardDB.cardName.twistingnether;
            if (s == "leaderofthepack") return CardDB.cardName.leaderofthepack;
            if (s == "malygos") return CardDB.cardName.malygos;
            if (s == "becomehogger") return CardDB.cardName.becomehogger;
            if (s == "millhousemanastorm") return CardDB.cardName.millhousemanastorm;
            if (s == "innerfire") return CardDB.cardName.innerfire;
            if (s == "valeerasanguinar") return CardDB.cardName.valeerasanguinar;
            if (s == "chicken") return CardDB.cardName.chicken;
            if (s == "souloftheforest") return CardDB.cardName.souloftheforest;
            if (s == "silencedebug") return CardDB.cardName.silencedebug;
            if (s == "bloodsailcorsair") return CardDB.cardName.bloodsailcorsair;
            if (s == "tinkmasteroverspark") return CardDB.cardName.tinkmasteroverspark;
            if (s == "iceblock") return CardDB.cardName.iceblock;
            if (s == "brawl") return CardDB.cardName.brawl;
            if (s == "vanish") return CardDB.cardName.vanish;
            if (s == "murloc") return CardDB.cardName.murloc;
            if (s == "mindspike") return CardDB.cardName.mindspike;
            if (s == "kingmukla") return CardDB.cardName.kingmukla;
            if (s == "stevengabriel") return CardDB.cardName.stevengabriel;
            if (s == "truesilverchampion") return CardDB.cardName.truesilverchampion;
            if (s == "harrisonjones") return CardDB.cardName.harrisonjones;
            if (s == "devilsaur") return CardDB.cardName.devilsaur;
            if (s == "wargolem") return CardDB.cardName.wargolem;
            if (s == "warsongcommander") return CardDB.cardName.warsongcommander;
            if (s == "manawyrm") return CardDB.cardName.manawyrm;
            if (s == "savagery") return CardDB.cardName.savagery;
            if (s == "spitefulsmith") return CardDB.cardName.spitefulsmith;
            if (s == "shatteredsuncleric") return CardDB.cardName.shatteredsuncleric;
            if (s == "eyeforaneye") return CardDB.cardName.eyeforaneye;
            if (s == "azuredrake") return CardDB.cardName.azuredrake;
            if (s == "mountaingiant") return CardDB.cardName.mountaingiant;
            if (s == "korkronelite") return CardDB.cardName.korkronelite;
            if (s == "junglepanther") return CardDB.cardName.junglepanther;
            if (s == "barongeddon") return CardDB.cardName.barongeddon;
            if (s == "pitlord") return CardDB.cardName.pitlord;
            if (s == "markofnature") return CardDB.cardName.markofnature;
            if (s == "leokk") return CardDB.cardName.leokk;
            if (s == "fierywaraxe") return CardDB.cardName.fierywaraxe;
            if (s == "damage5") return CardDB.cardName.damage5;
            if (s == "restore5") return CardDB.cardName.restore5;
            if (s == "mindblast") return CardDB.cardName.mindblast;
            if (s == "timberwolf") return CardDB.cardName.timberwolf;
            if (s == "captaingreenskin") return CardDB.cardName.captaingreenskin;
            if (s == "elvenarcher") return CardDB.cardName.elvenarcher;
            if (s == "michaelschweitzer") return CardDB.cardName.michaelschweitzer;
            if (s == "masterswordsmith") return CardDB.cardName.masterswordsmith;
            if (s == "grommashhellscream") return CardDB.cardName.grommashhellscream;
            if (s == "hound") return CardDB.cardName.hound;
            if (s == "seagiant") return CardDB.cardName.seagiant;
            if (s == "doomguard") return CardDB.cardName.doomguard;
            if (s == "alakirthewindlord") return CardDB.cardName.alakirthewindlord;
            if (s == "hyena") return CardDB.cardName.hyena;
            if (s == "frothingberserker") return CardDB.cardName.frothingberserker;
            if (s == "powerofthewild") return CardDB.cardName.powerofthewild;
            if (s == "druidoftheclaw") return CardDB.cardName.druidoftheclaw;
            if (s == "hellfire") return CardDB.cardName.hellfire;
            if (s == "archmage") return CardDB.cardName.archmage;
            if (s == "recklessrocketeer") return CardDB.cardName.recklessrocketeer;
            if (s == "crazymonkey") return CardDB.cardName.crazymonkey;
            if (s == "damageallbut1") return CardDB.cardName.damageallbut1;
            if (s == "powerwordshield") return CardDB.cardName.powerwordshield;
            if (s == "arcaneintellect") return CardDB.cardName.arcaneintellect;
            if (s == "angrychicken") return CardDB.cardName.angrychicken;
            if (s == "mindgames") return CardDB.cardName.mindgames;
            if (s == "leeroyjenkins") return CardDB.cardName.leeroyjenkins;
            if (s == "gurubashiberserker") return CardDB.cardName.gurubashiberserker;
            if (s == "windspeaker") return CardDB.cardName.windspeaker;
            if (s == "enableemotes") return CardDB.cardName.enableemotes;
            if (s == "forceofnature") return CardDB.cardName.forceofnature;
            if (s == "lightspawn") return CardDB.cardName.lightspawn;
            if (s == "warglaiveofazzinoth") return CardDB.cardName.warglaiveofazzinoth;
            if (s == "finkleeinhorn") return CardDB.cardName.finkleeinhorn;
            if (s == "frostelemental") return CardDB.cardName.frostelemental;
            if (s == "thoughtsteal") return CardDB.cardName.thoughtsteal;
            if (s == "brianschwab") return CardDB.cardName.brianschwab;
            if (s == "scavenginghyena") return CardDB.cardName.scavenginghyena;
            if (s == "si7agent") return CardDB.cardName.si7agent;
            if (s == "prophetvelen") return CardDB.cardName.prophetvelen;
            if (s == "soulfire") return CardDB.cardName.soulfire;
            if (s == "ogremagi") return CardDB.cardName.ogremagi;
            if (s == "damagedgolem") return CardDB.cardName.damagedgolem;
            if (s == "crash") return CardDB.cardName.crash;
            if (s == "adrenalinerush") return CardDB.cardName.adrenalinerush;
            if (s == "murloctidecaller") return CardDB.cardName.murloctidecaller;
            if (s == "kirintormage") return CardDB.cardName.kirintormage;
            if (s == "thrallmarfarseer") return CardDB.cardName.thrallmarfarseer;
            if (s == "frostwolfwarlord") return CardDB.cardName.frostwolfwarlord;
            if (s == "sorcerersapprentice") return CardDB.cardName.sorcerersapprentice;
            if (s == "willofmukla") return CardDB.cardName.willofmukla;
            if (s == "holyfire") return CardDB.cardName.holyfire;
            if (s == "manawraith") return CardDB.cardName.manawraith;
            if (s == "argentsquire") return CardDB.cardName.argentsquire;
            if (s == "placeholdercard") return CardDB.cardName.placeholdercard;
            if (s == "snakeball") return CardDB.cardName.snakeball;
            if (s == "ancientwatcher") return CardDB.cardName.ancientwatcher;
            if (s == "noviceengineer") return CardDB.cardName.noviceengineer;
            if (s == "stonetuskboar") return CardDB.cardName.stonetuskboar;
            if (s == "ancestralhealing") return CardDB.cardName.ancestralhealing;
            if (s == "conceal") return CardDB.cardName.conceal;
            if (s == "arcanitereaper") return CardDB.cardName.arcanitereaper;
            if (s == "guldan") return CardDB.cardName.guldan;
            if (s == "ragingworgen") return CardDB.cardName.ragingworgen;
            if (s == "earthenringfarseer") return CardDB.cardName.earthenringfarseer;
            if (s == "onyxia") return CardDB.cardName.onyxia;
            if (s == "manaaddict") return CardDB.cardName.manaaddict;
            if (s == "dualwarglaives") return CardDB.cardName.dualwarglaives;
            if (s == "worthlessimp") return CardDB.cardName.worthlessimp;
            if (s == "shiv") return CardDB.cardName.shiv;
            if (s == "sheep") return CardDB.cardName.sheep;
            if (s == "bloodknight") return CardDB.cardName.bloodknight;
            if (s == "holysmite") return CardDB.cardName.holysmite;
            if (s == "ancientsecrets") return CardDB.cardName.ancientsecrets;
            if (s == "holywrath") return CardDB.cardName.holywrath;
            if (s == "ironforgerifleman") return CardDB.cardName.ironforgerifleman;
            if (s == "elitetaurenchieftain") return CardDB.cardName.elitetaurenchieftain;
            if (s == "bluegillwarrior") return CardDB.cardName.bluegillwarrior;
            if (s == "shapeshift") return CardDB.cardName.shapeshift;
            if (s == "hamiltonchu") return CardDB.cardName.hamiltonchu;
            if (s == "battlerage") return CardDB.cardName.battlerage;
            if (s == "nightblade") return CardDB.cardName.nightblade;
            if (s == "crazedhunter") return CardDB.cardName.crazedhunter;
            if (s == "andybrock") return CardDB.cardName.andybrock;
            if (s == "youthfulbrewmaster") return CardDB.cardName.youthfulbrewmaster;
            if (s == "theblackknight") return CardDB.cardName.theblackknight;
            if (s == "brewmaster") return CardDB.cardName.brewmaster;
            if (s == "lifetap") return CardDB.cardName.lifetap;
            if (s == "demonfire") return CardDB.cardName.demonfire;
            if (s == "redemption") return CardDB.cardName.redemption;
            if (s == "lordjaraxxus") return CardDB.cardName.lordjaraxxus;
            if (s == "coldblood") return CardDB.cardName.coldblood;
            if (s == "lightwarden") return CardDB.cardName.lightwarden;
            if (s == "questingadventurer") return CardDB.cardName.questingadventurer;
            if (s == "donothing") return CardDB.cardName.donothing;
            if (s == "dereksakamoto") return CardDB.cardName.dereksakamoto;
            if (s == "poultryizer") return CardDB.cardName.poultryizer;
            if (s == "koboldgeomancer") return CardDB.cardName.koboldgeomancer;
            if (s == "legacyoftheemperor") return CardDB.cardName.legacyoftheemperor;
            if (s == "cenarius") return CardDB.cardName.cenarius;
            if (s == "searingtotem") return CardDB.cardName.searingtotem;
            if (s == "taurenwarrior") return CardDB.cardName.taurenwarrior;
            if (s == "explosivetrap") return CardDB.cardName.explosivetrap;
            if (s == "frog") return CardDB.cardName.frog;
            if (s == "servercrash") return CardDB.cardName.servercrash;
            if (s == "wickedknife") return CardDB.cardName.wickedknife;
            if (s == "laughingsister") return CardDB.cardName.laughingsister;
            if (s == "cultmaster") return CardDB.cardName.cultmaster;
            if (s == "wildgrowth") return CardDB.cardName.wildgrowth;
            if (s == "sprint") return CardDB.cardName.sprint;
            if (s == "masterofdisguise") return CardDB.cardName.masterofdisguise;
            if (s == "kyleharrison") return CardDB.cardName.kyleharrison;
            if (s == "avatarofthecoin") return CardDB.cardName.avatarofthecoin;
            if (s == "excessmana") return CardDB.cardName.excessmana;
            if (s == "spiritwolf") return CardDB.cardName.spiritwolf;
            if (s == "auchenaisoulpriest") return CardDB.cardName.auchenaisoulpriest;
            if (s == "bestialwrath") return CardDB.cardName.bestialwrath;
            if (s == "rockbiterweapon") return CardDB.cardName.rockbiterweapon;
            if (s == "starvingbuzzard") return CardDB.cardName.starvingbuzzard;
            if (s == "mirrorimage") return CardDB.cardName.mirrorimage;
            if (s == "silverhandrecruit") return CardDB.cardName.silverhandrecruit;
            if (s == "corruption") return CardDB.cardName.corruption;
            if (s == "preparation") return CardDB.cardName.preparation;
            if (s == "cairnebloodhoof") return CardDB.cardName.cairnebloodhoof;
            if (s == "mortalstrike") return CardDB.cardName.mortalstrike;
            if (s == "flare") return CardDB.cardName.flare;
            if (s == "silverhandknight") return CardDB.cardName.silverhandknight;
            if (s == "breakweapon") return CardDB.cardName.breakweapon;
            if (s == "guardianofkings") return CardDB.cardName.guardianofkings;
            if (s == "ancientbrewmaster") return CardDB.cardName.ancientbrewmaster;
            if (s == "youngdragonhawk") return CardDB.cardName.youngdragonhawk;
            if (s == "frostshock") return CardDB.cardName.frostshock;
            if (s == "healingtouch") return CardDB.cardName.healingtouch;
            if (s == "venturecomercenary") return CardDB.cardName.venturecomercenary;
            if (s == "sacrificialpact") return CardDB.cardName.sacrificialpact;
            if (s == "noooooooooooo") return CardDB.cardName.noooooooooooo;
            if (s == "baneofdoom") return CardDB.cardName.baneofdoom;
            if (s == "abomination") return CardDB.cardName.abomination;
            if (s == "flesheatingghoul") return CardDB.cardName.flesheatingghoul;
            if (s == "loothoarder") return CardDB.cardName.loothoarder;
            if (s == "mill10") return CardDB.cardName.mill10;
            if (s == "jasonchayes") return CardDB.cardName.jasonchayes;
            if (s == "benbrode") return CardDB.cardName.benbrode;
            if (s == "betrayal") return CardDB.cardName.betrayal;
            if (s == "thebeast") return CardDB.cardName.thebeast;
            if (s == "flameimp") return CardDB.cardName.flameimp;
            if (s == "freezingtrap") return CardDB.cardName.freezingtrap;
            if (s == "southseadeckhand") return CardDB.cardName.southseadeckhand;
            if (s == "wrath") return CardDB.cardName.wrath;
            if (s == "bloodfenraptor") return CardDB.cardName.bloodfenraptor;
            if (s == "cleave") return CardDB.cardName.cleave;
            if (s == "fencreeper") return CardDB.cardName.fencreeper;
            if (s == "restore1") return CardDB.cardName.restore1;
            if (s == "handtodeck") return CardDB.cardName.handtodeck;
            if (s == "starfire") return CardDB.cardName.starfire;
            if (s == "goldshirefootman") return CardDB.cardName.goldshirefootman;
            if (s == "murlocscout") return CardDB.cardName.murlocscout;
            if (s == "ragnarosthefirelord") return CardDB.cardName.ragnarosthefirelord;
            if (s == "rampage") return CardDB.cardName.rampage;
            if (s == "thrall") return CardDB.cardName.thrall;
            if (s == "stoneclawtotem") return CardDB.cardName.stoneclawtotem;
            if (s == "captainsparrot") return CardDB.cardName.captainsparrot;
            if (s == "windfuryharpy") return CardDB.cardName.windfuryharpy;
            if (s == "stranglethorntiger") return CardDB.cardName.stranglethorntiger;
            if (s == "summonarandomsecret") return CardDB.cardName.summonarandomsecret;
            if (s == "circleofhealing") return CardDB.cardName.circleofhealing;
            if (s == "snaketrap") return CardDB.cardName.snaketrap;
            if (s == "cabalshadowpriest") return CardDB.cardName.cabalshadowpriest;
            if (s == "upgrade") return CardDB.cardName.upgrade;
            if (s == "shieldslam") return CardDB.cardName.shieldslam;
            if (s == "flameburst") return CardDB.cardName.flameburst;
            if (s == "windfury") return CardDB.cardName.windfury;
            if (s == "natpagle") return CardDB.cardName.natpagle;
            if (s == "restoreallhealth") return CardDB.cardName.restoreallhealth;
            if (s == "houndmaster") return CardDB.cardName.houndmaster;
            if (s == "waterelemental") return CardDB.cardName.waterelemental;
            if (s == "eaglehornbow") return CardDB.cardName.eaglehornbow;
            if (s == "gnoll") return CardDB.cardName.gnoll;
            if (s == "archmageantonidas") return CardDB.cardName.archmageantonidas;
            if (s == "destroyallheroes") return CardDB.cardName.destroyallheroes;
            if (s == "wrathofairtotem") return CardDB.cardName.wrathofairtotem;
            if (s == "killcommand") return CardDB.cardName.killcommand;
            if (s == "manatidetotem") return CardDB.cardName.manatidetotem;
            if (s == "daggermastery") return CardDB.cardName.daggermastery;
            if (s == "drainlife") return CardDB.cardName.drainlife;
            if (s == "doomsayer") return CardDB.cardName.doomsayer;
            if (s == "darkscalehealer") return CardDB.cardName.darkscalehealer;
            if (s == "shadowform") return CardDB.cardName.shadowform;
            if (s == "frostnova") return CardDB.cardName.frostnova;
            if (s == "mirrorentity") return CardDB.cardName.mirrorentity;
            if (s == "counterspell") return CardDB.cardName.counterspell;
            if (s == "mindshatter") return CardDB.cardName.mindshatter;
            if (s == "magmarager") return CardDB.cardName.magmarager;
            if (s == "wolfrider") return CardDB.cardName.wolfrider;
            if (s == "emboldener3000") return CardDB.cardName.emboldener3000;
            if (s == "gelbinmekkatorque") return CardDB.cardName.gelbinmekkatorque;
            if (s == "utherlightbringer") return CardDB.cardName.utherlightbringer;
            if (s == "innerrage") return CardDB.cardName.innerrage;
            if (s == "emeralddrake") return CardDB.cardName.emeralddrake;
            if (s == "heroicstrike") return CardDB.cardName.heroicstrike;
            if (s == "barreltoss") return CardDB.cardName.barreltoss;
            if (s == "yongwoo") return CardDB.cardName.yongwoo;
            if (s == "doomhammer") return CardDB.cardName.doomhammer;
            if (s == "stomp") return CardDB.cardName.stomp;
            if (s == "tracking") return CardDB.cardName.tracking;
            if (s == "fireball") return CardDB.cardName.fireball;
            if (s == "metamorphosis") return CardDB.cardName.metamorphosis;
            if (s == "thecoin") return CardDB.cardName.thecoin;
            if (s == "bootybaybodyguard") return CardDB.cardName.bootybaybodyguard;
            if (s == "scarletcrusader") return CardDB.cardName.scarletcrusader;
            if (s == "voodoodoctor") return CardDB.cardName.voodoodoctor;
            if (s == "shadowbolt") return CardDB.cardName.shadowbolt;
            if (s == "etherealarcanist") return CardDB.cardName.etherealarcanist;
            if (s == "succubus") return CardDB.cardName.succubus;
            if (s == "emperorcobra") return CardDB.cardName.emperorcobra;
            if (s == "deadlyshot") return CardDB.cardName.deadlyshot;
            if (s == "reinforce") return CardDB.cardName.reinforce;
            if (s == "claw") return CardDB.cardName.claw;
            if (s == "explosiveshot") return CardDB.cardName.explosiveshot;
            if (s == "avengingwrath") return CardDB.cardName.avengingwrath;
            if (s == "riverpawgnoll") return CardDB.cardName.riverpawgnoll;
            if (s == "argentprotector") return CardDB.cardName.argentprotector;
            if (s == "hiddengnome") return CardDB.cardName.hiddengnome;
            if (s == "felguard") return CardDB.cardName.felguard;
            if (s == "northshirecleric") return CardDB.cardName.northshirecleric;
            if (s == "lepergnome") return CardDB.cardName.lepergnome;
            if (s == "fireelemental") return CardDB.cardName.fireelemental;
            if (s == "armorup") return CardDB.cardName.armorup;
            if (s == "snipe") return CardDB.cardName.snipe;
            if (s == "southseacaptain") return CardDB.cardName.southseacaptain;
            if (s == "catform") return CardDB.cardName.catform;
            if (s == "bite") return CardDB.cardName.bite;
            if (s == "defiasringleader") return CardDB.cardName.defiasringleader;
            if (s == "harvestgolem") return CardDB.cardName.harvestgolem;
            if (s == "kingkrush") return CardDB.cardName.kingkrush;
            if (s == "healingtotem") return CardDB.cardName.healingtotem;
            if (s == "ericdodds") return CardDB.cardName.ericdodds;
            if (s == "demigodsfavor") return CardDB.cardName.demigodsfavor;
            if (s == "huntersmark") return CardDB.cardName.huntersmark;
            if (s == "dalaranmage") return CardDB.cardName.dalaranmage;
            if (s == "twilightdrake") return CardDB.cardName.twilightdrake;
            if (s == "coldlightoracle") return CardDB.cardName.coldlightoracle;
            if (s == "moltengiant") return CardDB.cardName.moltengiant;
            if (s == "shadowflame") return CardDB.cardName.shadowflame;
            if (s == "anduinwrynn") return CardDB.cardName.anduinwrynn;
            if (s == "argentcommander") return CardDB.cardName.argentcommander;
            if (s == "revealhand") return CardDB.cardName.revealhand;
            if (s == "arcanemissiles") return CardDB.cardName.arcanemissiles;
            if (s == "repairbot") return CardDB.cardName.repairbot;
            if (s == "ancientofwar") return CardDB.cardName.ancientofwar;
            if (s == "stormwindchampion") return CardDB.cardName.stormwindchampion;
            if (s == "summonapanther") return CardDB.cardName.summonapanther;
            if (s == "swipe") return CardDB.cardName.swipe;
            if (s == "hex") return CardDB.cardName.hex;
            if (s == "ysera") return CardDB.cardName.ysera;
            if (s == "arcanegolem") return CardDB.cardName.arcanegolem;
            if (s == "bloodimp") return CardDB.cardName.bloodimp;
            if (s == "pyroblast") return CardDB.cardName.pyroblast;
            if (s == "murlocraider") return CardDB.cardName.murlocraider;
            if (s == "faeriedragon") return CardDB.cardName.faeriedragon;
            if (s == "sinisterstrike") return CardDB.cardName.sinisterstrike;
            if (s == "poweroverwhelming") return CardDB.cardName.poweroverwhelming;
            if (s == "arcaneexplosion") return CardDB.cardName.arcaneexplosion;
            if (s == "shadowwordpain") return CardDB.cardName.shadowwordpain;
            if (s == "mill30") return CardDB.cardName.mill30;
            if (s == "noblesacrifice") return CardDB.cardName.noblesacrifice;
            if (s == "dreadinfernal") return CardDB.cardName.dreadinfernal;
            if (s == "naturalize") return CardDB.cardName.naturalize;
            if (s == "totemiccall") return CardDB.cardName.totemiccall;
            if (s == "secretkeeper") return CardDB.cardName.secretkeeper;
            if (s == "dreadcorsair") return CardDB.cardName.dreadcorsair;
            if (s == "forkedlightning") return CardDB.cardName.forkedlightning;
            if (s == "handofprotection") return CardDB.cardName.handofprotection;
            if (s == "vaporize") return CardDB.cardName.vaporize;
            if (s == "nozdormu") return CardDB.cardName.nozdormu;
            if (s == "divinespirit") return CardDB.cardName.divinespirit;
            if (s == "transcendence") return CardDB.cardName.transcendence;
            if (s == "armorsmith") return CardDB.cardName.armorsmith;
            if (s == "murloctidehunter") return CardDB.cardName.murloctidehunter;
            if (s == "stealcard") return CardDB.cardName.stealcard;
            if (s == "opponentconcede") return CardDB.cardName.opponentconcede;
            if (s == "tundrarhino") return CardDB.cardName.tundrarhino;
            if (s == "summoningportal") return CardDB.cardName.summoningportal;
            if (s == "hammerofwrath") return CardDB.cardName.hammerofwrath;
            if (s == "stormwindknight") return CardDB.cardName.stormwindknight;
            if (s == "freeze") return CardDB.cardName.freeze;
            if (s == "madbomber") return CardDB.cardName.madbomber;
            if (s == "consecration") return CardDB.cardName.consecration;
            if (s == "boar") return CardDB.cardName.boar;
            if (s == "knifejuggler") return CardDB.cardName.knifejuggler;
            if (s == "icebarrier") return CardDB.cardName.icebarrier;
            if (s == "mechanicaldragonling") return CardDB.cardName.mechanicaldragonling;
            if (s == "battleaxe") return CardDB.cardName.battleaxe;
            if (s == "lightsjustice") return CardDB.cardName.lightsjustice;
            if (s == "lavaburst") return CardDB.cardName.lavaburst;
            if (s == "mindcontroltech") return CardDB.cardName.mindcontroltech;
            if (s == "boulderfistogre") return CardDB.cardName.boulderfistogre;
            if (s == "fireblast") return CardDB.cardName.fireblast;
            if (s == "priestessofelune") return CardDB.cardName.priestessofelune;
            if (s == "ancientmage") return CardDB.cardName.ancientmage;
            if (s == "shadowworddeath") return CardDB.cardName.shadowworddeath;
            if (s == "ironbeakowl") return CardDB.cardName.ironbeakowl;
            if (s == "eviscerate") return CardDB.cardName.eviscerate;
            if (s == "repentance") return CardDB.cardName.repentance;
            if (s == "sunwalker") return CardDB.cardName.sunwalker;
            if (s == "nagamyrmidon") return CardDB.cardName.nagamyrmidon;
            if (s == "destoryheropower") return CardDB.cardName.destoryheropower;
            if (s == "slam") return CardDB.cardName.slam;
            if (s == "swordofjustice") return CardDB.cardName.swordofjustice;
            if (s == "bounce") return CardDB.cardName.bounce;
            if (s == "shadopanmonk") return CardDB.cardName.shadopanmonk;
            if (s == "whirlwind") return CardDB.cardName.whirlwind;
            if (s == "alexstrasza") return CardDB.cardName.alexstrasza;
            if (s == "silence") return CardDB.cardName.silence;
            if (s == "rexxar") return CardDB.cardName.rexxar;
            if (s == "voidwalker") return CardDB.cardName.voidwalker;
            if (s == "whelp") return CardDB.cardName.whelp;
            if (s == "flamestrike") return CardDB.cardName.flamestrike;
            if (s == "rivercrocolisk") return CardDB.cardName.rivercrocolisk;
            if (s == "stormforgedaxe") return CardDB.cardName.stormforgedaxe;
            if (s == "snake") return CardDB.cardName.snake;
            if (s == "shotgunblast") return CardDB.cardName.shotgunblast;
            if (s == "violetapprentice") return CardDB.cardName.violetapprentice;
            if (s == "templeenforcer") return CardDB.cardName.templeenforcer;
            if (s == "ashbringer") return CardDB.cardName.ashbringer;
            if (s == "impmaster") return CardDB.cardName.impmaster;
            if (s == "defender") return CardDB.cardName.defender;
            if (s == "savageroar") return CardDB.cardName.savageroar;
            if (s == "innervate") return CardDB.cardName.innervate;
            if (s == "inferno") return CardDB.cardName.inferno;
            if (s == "earthelemental") return CardDB.cardName.earthelemental;
            if (s == "facelessmanipulator") return CardDB.cardName.facelessmanipulator;
            if (s == "divinefavor") return CardDB.cardName.divinefavor;
            if (s == "demolisher") return CardDB.cardName.demolisher;
            if (s == "sunfuryprotector") return CardDB.cardName.sunfuryprotector;
            if (s == "dustdevil") return CardDB.cardName.dustdevil;
            if (s == "powerofthehorde") return CardDB.cardName.powerofthehorde;
            if (s == "holylight") return CardDB.cardName.holylight;
            if (s == "feralspirit") return CardDB.cardName.feralspirit;
            if (s == "raidleader") return CardDB.cardName.raidleader;
            if (s == "amaniberserker") return CardDB.cardName.amaniberserker;
            if (s == "ironbarkprotector") return CardDB.cardName.ironbarkprotector;
            if (s == "bearform") return CardDB.cardName.bearform;
            if (s == "deathwing") return CardDB.cardName.deathwing;
            if (s == "stormpikecommando") return CardDB.cardName.stormpikecommando;
            if (s == "squire") return CardDB.cardName.squire;
            if (s == "panther") return CardDB.cardName.panther;
            if (s == "silverbackpatriarch") return CardDB.cardName.silverbackpatriarch;
            if (s == "bobfitch") return CardDB.cardName.bobfitch;
            if (s == "gladiatorslongbow") return CardDB.cardName.gladiatorslongbow;
            if (s == "damage1") return CardDB.cardName.damage1;
            return CardDB.cardName.unknown;
        }

        public enum ErrorType2
        {
            NONE,//=0
            REQ_MINION_TARGET,//=1
            REQ_FRIENDLY_TARGET,//=2
            REQ_ENEMY_TARGET,//=3
            REQ_DAMAGED_TARGET,//=4
            REQ_ENCHANTED_TARGET,
            REQ_FROZEN_TARGET,
            REQ_CHARGE_TARGET,
            REQ_TARGET_MAX_ATTACK,//=8
            REQ_NONSELF_TARGET,//=9
            REQ_TARGET_WITH_RACE,//=10
            REQ_TARGET_TO_PLAY,//=11 
            REQ_NUM_MINION_SLOTS,//=12 
            REQ_WEAPON_EQUIPPED,//=13
            REQ_ENOUGH_MANA,//=14
            REQ_YOUR_TURN,
            REQ_NONSTEALTH_ENEMY_TARGET,
            REQ_HERO_TARGET,//17
            REQ_SECRET_CAP,
            REQ_MINION_CAP_IF_TARGET_AVAILABLE,//19
            REQ_MINION_CAP,
            REQ_TARGET_ATTACKED_THIS_TURN,
            REQ_TARGET_IF_AVAILABLE,//=22
            REQ_MINIMUM_ENEMY_MINIONS,//=23 /like spalen :D
            REQ_TARGET_FOR_COMBO,//=24
            REQ_NOT_EXHAUSTED_ACTIVATE,
            REQ_UNIQUE_SECRET,
            REQ_TARGET_TAUNTER,
            REQ_CAN_BE_ATTACKED,
            REQ_ACTION_PWR_IS_MASTER_PWR,
            REQ_TARGET_MAGNET,
            REQ_ATTACK_GREATER_THAN_0,
            REQ_ATTACKER_NOT_FROZEN,
            REQ_HERO_OR_MINION_TARGET,
            REQ_CAN_BE_TARGETED_BY_SPELLS,
            REQ_SUBCARD_IS_PLAYABLE,
            REQ_TARGET_FOR_NO_COMBO,
            REQ_NOT_MINION_JUST_PLAYED,
            REQ_NOT_EXHAUSTED_HERO_POWER,
            REQ_CAN_BE_TARGETED_BY_OPPONENTS,
            REQ_ATTACKER_CAN_ATTACK,
            REQ_TARGET_MIN_ATTACK,//=41
            REQ_CAN_BE_TARGETED_BY_HERO_POWERS,
            REQ_ENEMY_TARGET_NOT_IMMUNE,
            REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY,//44 (totemic call)
            REQ_MINIMUM_TOTAL_MINIONS,//45 (scharmuetzel)
            REQ_MUST_TARGET_TAUNTER,//=46
            REQ_UNDAMAGED_TARGET//=47
        }

        public class Card
        {
            //public string CardID = "";
            public cardName name = cardName.unknown;
            public int race = 0;
            public int rarity = 0;
            public int cost = 0;
            public int crdtype = 0;
            public cardtype type = CardDB.cardtype.NONE;
            //public string description = "";
            public int carddraw = 0;

            public bool hasEffect = false;// has the minion an effect, but not battlecry

            public int Attack = 0;
            public int Health = 0;
            public int Durability = 0;//for weapons
            public bool target = false;
            //public string targettext = "";
            public bool tank = false;
            public bool Silence = false;
            public bool choice = false;
            public bool windfury = false;
            public bool poisionous = false;
            public bool deathrattle = false;
            public bool battlecry = false;
            public bool oneTurnEffect = false;
            public bool Enrage = false;
            public bool Aura = false;
            public bool Elite = false;
            public bool Combo = false;
            public bool Recall = false;
            public int recallValue = 0;
            public bool immuneWhileAttacking = false;
            public bool immuneToSpellpowerg = false;
            public bool Stealth = false;
            public bool Freeze = false;
            public bool AdjacentBuff = false;
            public bool Shield = false;
            public bool Charge = false;
            public bool Secret = false;
            public bool Morph = false;
            public bool Spellpower = false;
            public bool GrantCharge = false;
            public bool HealTarget = false;
            //playRequirements, reqID= siehe PlayErrors->ErrorType
            public int needEmptyPlacesForPlaying = 0;
            public int needWithMinAttackValueOf = 0;
            public int needWithMaxAttackValueOf = 0;
            public int needRaceForPlaying = 0;
            public int needMinNumberOfEnemy = 0;
            public int needMinTotalMinions = 0;
            public int needMinionsCapIfAvailable = 0;
            
            public int spellpowervalue = 0;
            public cardIDEnum cardIDenum = cardIDEnum.None;
            public List<ErrorType2> playrequires;

            public Card()
            {
                playrequires = new List<ErrorType2>();
            }

            public Card(Card c)
            {
                //this.entityID = c.entityID;
                this.hasEffect = c.hasEffect;
                this.rarity = c.rarity;
                this.AdjacentBuff = c.AdjacentBuff;
                this.Attack = c.Attack;
                this.Aura = c.Aura;
                this.battlecry = c.battlecry;
                this.carddraw = c.carddraw;
                //this.CardID = c.CardID;
                this.Charge = c.Charge;
                this.choice = c.choice;
                this.Combo = c.Combo;
                this.cost = c.cost;
                this.crdtype = c.crdtype;
                this.deathrattle = c.deathrattle;
                //this.description = c.description;
                this.Durability = c.Durability;
                this.Elite = c.Elite;
                this.Enrage = c.Enrage;
                this.Freeze = c.Freeze;
                this.GrantCharge = c.GrantCharge;
                this.HealTarget = c.HealTarget;
                this.Health = c.Health;
                this.immuneToSpellpowerg = c.immuneToSpellpowerg;
                this.immuneWhileAttacking = c.immuneWhileAttacking;
                this.Morph = c.Morph;
                this.name = c.name;
                this.needEmptyPlacesForPlaying = c.needEmptyPlacesForPlaying;
                this.needMinionsCapIfAvailable = c.needMinionsCapIfAvailable;
                this.needMinNumberOfEnemy = c.needMinNumberOfEnemy;
                this.needMinTotalMinions = c.needMinTotalMinions;
                this.needRaceForPlaying = c.needRaceForPlaying;
                this.needWithMaxAttackValueOf = c.needWithMaxAttackValueOf;
                this.needWithMinAttackValueOf = c.needWithMinAttackValueOf;
                this.oneTurnEffect = c.oneTurnEffect;
                this.playrequires =  new List<ErrorType2>(c.playrequires);
                this.poisionous = c.poisionous;
                this.race = c.race;
                this.Recall = c.Recall;
                this.recallValue = c.recallValue;
                this.Secret = c.Secret;
                this.Shield = c.Shield;
                this.Silence = c.Silence;
                this.Spellpower = c.Spellpower;
                this.spellpowervalue = c.spellpowervalue;
                this.Stealth = c.Stealth;
                this.tank = c.tank;
                this.target = c.target;
                //this.targettext = c.targettext;
                this.type = c.type;
                this.windfury = c.windfury;
            }

            public bool isRequirementInList(CardDB.ErrorType2 et)
            {
                if (this.playrequires.Contains(et)) return true;
                return false;
            }

            public List<targett> getTargetsForCard(Playfield p)
            {
                List<targett> retval = new List<targett>();

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_FOR_COMBO) && p.cardsPlayedThisTurn == 0) return retval;

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_TO_PLAY) || isRequirementInList(CardDB.ErrorType2.REQ_NONSELF_TARGET) || isRequirementInList(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE) || isRequirementInList(CardDB.ErrorType2.REQ_TARGET_FOR_COMBO))
                {
                    retval.Add(new targett(100, p.ownHeroEntity));//ownhero
                    retval.Add(new targett(200, p.enemyHeroEntity));//enemyhero
                    foreach (Minion m in p.ownMinions)
                    {
                        if ((this.type == cardtype.SPELL || this.type == cardtype.HEROPWR) && (m.name == CardDB.cardName.faeriedragon || m.name == CardDB.cardName.laughingsister)) continue;
                        retval.Add(new targett(m.id, m.entitiyID));
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (((this.type == cardtype.SPELL || this.type == cardtype.HEROPWR) && (m.name == CardDB.cardName.faeriedragon || m.name == CardDB.cardName.laughingsister)) || m.stealth) continue;
                        retval.Add(new targett(m.id + 10, m.entitiyID));
                    }

                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_HERO_TARGET))
                {
                    retval.RemoveAll(x => (x.target <= 30));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINION_TARGET))
                {
                    retval.RemoveAll(x => (x.target == 100) || (x.target == 200));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_FRIENDLY_TARGET))
                {
                    retval.RemoveAll(x => (x.target >= 10 && x.target <= 20) || (x.target == 200));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_ENEMY_TARGET))
                {
                    retval.RemoveAll(x => (x.target <= 9 || (x.target == 100)));
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_DAMAGED_TARGET))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_UNDAMAGED_TARGET))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.wounded)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_MAX_ATTACK))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Angr > this.needWithMaxAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.Angr > this.needWithMaxAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_MIN_ATTACK))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Angr < this.needWithMinAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.Angr < this.needWithMinAttackValueOf)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_WITH_RACE))
                {
                    retval.RemoveAll(x => (x.target == 100) || (x.target == 200));
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!(m.handcard.card.race == this.needRaceForPlaying))
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!(m.handcard.card.race == this.needRaceForPlaying))
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MUST_TARGET_TAUNTER))
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (!m.taunt)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.taunt)
                        {
                            retval.RemoveAll(x => x.targetEntity == m.entitiyID);
                        }
                    }
                }
                return retval;

            }

            public int calculateManaCost(Playfield p)//calculates the mana from orginal mana, needed for back-to hand effects
            {
                int retval = this.cost;
                int offset = 0;

                if (this.type == cardtype.MOB)
                {
                    offset += (p.soeldnerDerVenture) * 3;

                    offset += (p.managespenst);

                    int temp = -(p.startedWithbeschwoerungsportal) * 2;
                    if (retval + temp <= 0) temp = -retval + 1;
                    offset = offset + temp;

                    if (p.mobsplayedThisTurn == 0 )
                    { 
                        offset -= p.winzigebeschwoererin;
                    }

                }

                if (this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset -= (p.zauberlehrling);
                    if (p.playedPreparation)
                    { //if the number of zauberlehrlings change
                        offset -= 3;
                    }

                }

                switch (this.name)
                {
                    case CardDB.cardName.dreadcorsair:
                        retval = retval + offset - p.ownWeaponAttack;
                        break;
                    case CardDB.cardName.seagiant:
                        retval = retval + offset - p.ownMinions.Count - p.enemyMinions.Count;
                        break;
                    case CardDB.cardName.mountaingiant:
                        retval = retval + offset - p.owncards.Count;
                        break;
                    case CardDB.cardName.moltengiant:
                        retval = retval + offset - p.ownHeroHp;
                        break;
                    default:
                        retval = retval + offset;
                        break;
                }

                if (this.Secret && p.playedmagierinderkirintor)
                {
                    retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            public int getManaCost(Playfield p, int currentcost)//calculates mana from current mana
            {
                int retval = currentcost;


                int offset = 0; // if offset < 0 costs become lower, if >0 costs are higher at the end

                // CARDS that increase the manacosts of others ##############################
                //Manacosts changes with soeldner der venture co.
                if (p.soeldnerDerVenture != p.startedWithsoeldnerDerVenture && this.type == cardtype.MOB)
                {
                    offset += (p.soeldnerDerVenture - p.startedWithsoeldnerDerVenture) * 3;
                }

                //Manacosts changes with mana-ghost
                if (p.managespenst != p.startedWithManagespenst && this.type == cardtype.MOB)
                {
                    offset += (p.managespenst - p.startedWithManagespenst);
                }


                // CARDS that decrease the manacosts of others ##############################

                //Manacosts changes with the summoning-portal >_>
                if (p.startedWithbeschwoerungsportal != p.beschwoerungsportal && this.type == cardtype.MOB)
                { //cant lower the mana to 0
                    int temp = (p.startedWithbeschwoerungsportal - p.beschwoerungsportal) * 2;
                    if (retval + temp <= 0) temp = -retval + 1;
                    offset = offset + temp;
                }

                //Manacosts changes with the pint-sized summoner
                if (p.winzigebeschwoererin >= 1 && p.mobsplayedThisTurn >= 1 && p.startedWithMobsPlayedThisTurn == 0 && this.type == cardtype.MOB)
                { // if we start oure calculations with 0 mobs played, then the cardcost are 1 mana to low in the further calculations (with the little summoner on field)
                    offset += p.winzigebeschwoererin;
                }
                if (p.mobsplayedThisTurn == 0 && p.winzigebeschwoererin <= p.startedWithWinzigebeschwoererin && this.type == cardtype.MOB)
                { // one pint-sized summoner got killed, before we played the first mob -> the manacost are higher of all mobs
                    offset += (p.startedWithWinzigebeschwoererin - p.winzigebeschwoererin);
                }

                //Manacosts changes with the zauberlehrling summoner
                if (p.zauberlehrling != p.startedWithZauberlehrling && this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset += (p.startedWithZauberlehrling - p.zauberlehrling);
                }



                //manacosts are lowered, after we played preparation
                if (p.playedPreparation && this.type == cardtype.SPELL)
                { //if the number of zauberlehrlings change
                    offset -= 3;
                }





                switch (this.name)
                {
                    case CardDB.cardName.dreadcorsair:
                        retval = retval + offset - p.ownWeaponAttack + p.ownWeaponAttackStarted; // if weapon attack change we change manacost
                        break;
                    case CardDB.cardName.seagiant:
                        retval = retval + offset - p.ownMinions.Count - p.enemyMinions.Count + p.ownMobsCountStarted;
                        break;
                    case CardDB.cardName.mountaingiant:
                        retval = retval + offset - p.owncards.Count + p.ownCardsCountStarted;
                        break;
                    case CardDB.cardName.moltengiant:
                        retval = retval + offset - p.ownHeroHp + p.ownHeroHpStarted;
                        break;
                    default:
                        retval = retval + offset;
                        break;
                }

                if (this.Secret && p.playedmagierinderkirintor)
                {
                    retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            public bool canplayCard(Playfield p, int manacost)
            {
                //is playrequirement?
                bool haveToDoRequires = isRequirementInList(CardDB.ErrorType2.REQ_TARGET_TO_PLAY);
                bool retval = true;
                // cant play if i have to few mana

                if (p.mana < this.getManaCost(p, manacost)) return false;

                // cant play mob, if i have allready 7 mininos
                if (this.type == CardDB.cardtype.MOB && p.ownMinions.Count >= 7) return false;

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS))
                {
                    if (p.enemyMinions.Count < this.needMinNumberOfEnemy) return false;
                }
                if (isRequirementInList(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS))
                {
                    if (p.ownMinions.Count > 7 - this.needEmptyPlacesForPlaying) return false;
                }
                
                if (isRequirementInList(CardDB.ErrorType2.REQ_WEAPON_EQUIPPED))
                {
                    if (p.ownWeaponName == CardDB.cardName.unknown) return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_MINIMUM_TOTAL_MINIONS))
                {
                    if (this.needMinTotalMinions > p.ownMinions.Count + p.enemyMinions.Count) return false;
                }

                if (haveToDoRequires)
                {
                    if (this.getTargetsForCard(p).Count == 0) return false;

                    //it requires a target-> return false if 
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE) && isRequirementInList(CardDB.ErrorType2.REQ_MINION_CAP_IF_TARGET_AVAILABLE))
                {
                    if (this.getTargetsForCard(p).Count >= 1 && p.ownMinions.Count > 7-this.needMinionsCapIfAvailable ) return false;
                }

                if (isRequirementInList(CardDB.ErrorType2.REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY))
                {
                    int difftotem = 0;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.name == CardDB.cardName.healingtotem || m.name == CardDB.cardName.wrathofairtotem || m.name == CardDB.cardName.searingtotem || m.name == CardDB.cardName.stoneclawtotem) difftotem++;
                    }
                    if (difftotem == 4) return false;
                }
                

                if (this.Secret)
                {
                    if (p.ownSecretsIDList.Contains(this.cardIDenum)) return false;
                    if (p.ownSecretsIDList.Count >= 5) return false;
                }


                return true;
            }



        }

        List<Card> cardlist = new List<Card>();
        Dictionary<cardIDEnum, Card> cardidToCardList = new Dictionary<cardIDEnum, Card>();
        List<string> allCardIDS = new List<string>();

        private static CardDB instance;

        public static CardDB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardDB();
                    //instance.enumCreator();// only call this to get latest cardids
                }
                return instance;
            }
        }

        private CardDB()
        {
            string[] lines = new string[0] { };
            try
            {
                string path = Settings.Instance.path;
                lines = System.IO.File.ReadAllLines(path + "_carddb.txt");
            }
            catch
            {
                Helpfunctions.Instance.logg("cant find _carddb.txt");
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                Helpfunctions.Instance.ErrorLog("cant find _carddb.txt");
            }
            cardlist.Clear();
            this.cardidToCardList.Clear();
            Card c = new Card();
            int de = 0;
            bool targettext = false;
            //placeholdercard
            Card plchldr = new Card();
            plchldr.name = CardDB.cardName.unknown;
            plchldr.cost = 1000;
            this.cardlist.Add(plchldr);

            foreach (string s in lines)
            {
                if (s.Contains("/Entity"))
                {
                    if (c.type == cardtype.ENCHANTMENT)
                    {
                        //Helpfunctions.Instance.logg(c.CardID);
                        //Helpfunctions.Instance.logg(c.name);
                        //Helpfunctions.Instance.logg(c.description);
                        continue;
                    }

                    if (c.name != CardDB.cardName.unknown)
                    {
                        //Helpfunctions.Instance.logg(c.name);
                        this.cardlist.Add(c);
                        if (!this.cardidToCardList.ContainsKey(c.cardIDenum))
                        {
                            this.cardidToCardList.Add(c.cardIDenum, c);
                        }
                    }

                }
                if (s.Contains("<Entity version=\"2\" CardID=\""))
                {
                    c = new Card();
                    de = 0;
                    targettext = false;
                    string temp = s.Replace("<Entity version=\"2\" CardID=\"", "");
                    temp = temp.Replace("\">", "");
                    //c.CardID = temp;
                    allCardIDS.Add(temp);
                    c.cardIDenum = this.cardIdstringToEnum(temp);
                    continue;
                }
                if (s.Contains("<Entity version=\"1\" CardID=\""))
                {
                    c = new Card();
                    de = 0;
                    targettext = false;
                    string temp = s.Replace("<Entity version=\"1\" CardID=\"", "");
                    temp = temp.Replace("\">", "");
                    //c.CardID = temp;
                    allCardIDS.Add(temp);
                    c.cardIDenum = this.cardIdstringToEnum(temp);
                    continue;
                }

                if (s.Contains("<Tag name=\"Health\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Health = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Atk\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Attack = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Race\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.race = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Rarity\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.rarity = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Cost\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.cost = Convert.ToInt32(temp);
                    continue;
                }

                if (s.Contains("<Tag name=\"CardType\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    if (c.name != CardDB.cardName.unknown)
                    {
                        //Helpfunctions.Instance.logg(temp);
                    }

                    c.crdtype = Convert.ToInt32(temp);
                    if (c.crdtype == 10)
                    {
                        c.type = CardDB.cardtype.HEROPWR;
                    }
                    if (c.crdtype == 4)
                    {
                        c.type = CardDB.cardtype.MOB;
                    }
                    if (c.crdtype == 5)
                    {
                        c.type = CardDB.cardtype.SPELL;
                    }
                    if (c.crdtype == 6)
                    {
                        c.type = CardDB.cardtype.ENCHANTMENT;
                    }
                    if (c.crdtype == 7)
                    {
                        c.type = CardDB.cardtype.WEAPON;
                    }
                    continue;
                }

                if (s.Contains("<enUS>"))
                {
                    string temp = s.Replace("<enUS>", "");

                    temp = temp.Replace("</enUS>", "");
                    temp = temp.Replace("&lt;", "");
                    temp = temp.Replace("b&gt;", "");
                    temp = temp.Replace("/b&gt;", "");
                    temp = temp.ToLower();
                    if (de == 0)
                    {
                        temp = temp.Replace("'", "");
                        temp = temp.Replace(" ", "");
                        temp = temp.Replace(":", "");
                        temp = temp.Replace(".", "");
                        temp = temp.Replace("!", "");
                        temp = temp.Replace("-", "");
                        //temp = temp.Replace("ß", "ss");
                        //temp = temp.Replace("ü", "ue");
                        //temp = temp.Replace("ä", "ae");
                        //temp = temp.Replace("ö", "oe");

                        //Helpfunctions.Instance.logg(temp);
                        c.name = this.cardNamestringToEnum(temp);

                        if (PenalityManager.Instance.specialMinions.ContainsKey(this.cardNamestringToEnum(temp))) c.hasEffect = true;

                    }
                    if (de == 1)
                    {
                        //c.description = temp;
                        //if (c.description.Contains("choose one"))
                        if (temp.Contains("choose one"))
                        {
                            c.choice = true;
                            //Helpfunctions.Instance.logg(c.name + " is choice");
                        }
                    }
                    if (targettext)
                    {
                        //c.targettext = temp;
                        targettext = false;
                    }

                    de++;
                    continue;
                }
                if (s.Contains("<Tag name=\"Poisonous\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.poisionous = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Enrage\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Enrage = true;
                    continue;
                }

                if (s.Contains("<Tag name=\"OneTurnEffect\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.oneTurnEffect = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Aura\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Aura = true;
                    continue;
                }


                if (s.Contains("<Tag name=\"Taunt\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.tank = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Battlecry\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.battlecry = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Windfury\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.windfury = true;
                    continue;
                }

                if (s.Contains("<Tag name=\"Deathrattle\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.deathrattle = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Durability\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.Durability = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("<Tag name=\"Elite\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Elite = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Combo\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Combo = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Recall\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Recall = true;
                    c.recallValue = 1;
                    if (c.name == CardDB.cardName.forkedlightning) c.recallValue = 2;
                    if (c.name == CardDB.cardName.dustdevil) c.recallValue = 2;
                    if (c.name == CardDB.cardName.lightningstorm) c.recallValue = 2;
                    if (c.name == CardDB.cardName.lavaburst) c.recallValue = 2;
                    if (c.name == CardDB.cardName.feralspirit) c.recallValue = 2;
                    if (c.name == CardDB.cardName.doomhammer) c.recallValue = 2;
                    if (c.name == CardDB.cardName.earthelemental) c.recallValue = 3;
                    continue;
                }

                if (s.Contains("<Tag name=\"ImmuneToSpellpower\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.immuneToSpellpowerg = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Stealth\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Stealth = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Secret\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Secret = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Freeze\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Freeze = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"AdjacentBuff\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.AdjacentBuff = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Divine Shield\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Shield = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Charge\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Charge = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Silence\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Silence = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Morph\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Morph = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"Spellpower\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.Spellpower = true;
                    c.spellpowervalue = 1;
                    if (c.name == CardDB.cardName.ancientmage) c.spellpowervalue = 0;
                    if (c.name == CardDB.cardName.malygos) c.spellpowervalue = 5;
                    continue;
                }
                if (s.Contains("<Tag name=\"GrantCharge\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.GrantCharge = true;
                    continue;
                }
                if (s.Contains("<Tag name=\"HealTarget\""))
                {
                    string temp = s.Split(new string[] { "value=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    int ti = Convert.ToInt32(temp);
                    if (ti == 1) c.HealTarget = true;
                    continue;
                }

                if (s.Contains("TargetingArrowText"))
                {
                    c.target = true;
                    targettext = true;
                    continue;
                }

                if (s.Contains("<PlayRequirement"))
                {
                    string temp = s.Split(new string[] { "reqID=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    ErrorType2 et2 = (ErrorType2)Convert.ToInt32(temp);
                    c.playrequires.Add(et2);
                }


                if (s.Contains("<PlayRequirement reqID=\"12\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needEmptyPlacesForPlaying = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"41\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needWithMinAttackValueOf = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"8\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needWithMaxAttackValueOf = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"10\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needRaceForPlaying = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"23\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinNumberOfEnemy = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"45\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinTotalMinions = Convert.ToInt32(temp);
                    continue;
                }
                if (s.Contains("PlayRequirement reqID=\"19\" param=\""))
                {
                    string temp = s.Split(new string[] { "param=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    c.needMinionsCapIfAvailable = Convert.ToInt32(temp);
                    continue;
                }



                if (s.Contains("<Tag name="))
                {
                    string temp = s.Split(new string[] { "<Tag name=\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    temp = temp.Split('\"')[0];
                    /*
                    if (temp != "DevState" && temp != "FlavorText" && temp != "ArtistName" && temp != "Cost" && temp != "EnchantmentIdleVisual" && temp != "EnchantmentBirthVisual" && temp != "Collectible" && temp != "CardSet" && temp != "AttackVisualType" && temp != "CardName" && temp != "Class" && temp != "CardTextInHand" && temp != "Rarity" && temp != "TriggerVisual" && temp != "Faction" && temp != "HowToGetThisGoldCard" && temp != "HowToGetThisCard" && temp != "CardTextInPlay")
                        Helpfunctions.Instance.logg(s);*/
                }


            }


        }


        public Card getCardData(CardDB.cardName cardname)
        {
            Card c = new Card();

            foreach (Card ca in this.cardlist)
            {
                if (ca.name == cardname)
                {
                    return ca;
                }
            }

            return new Card(c);
        }

        public Card getCardDataFromID(cardIDEnum id)
        {
            if (this.cardidToCardList.ContainsKey(id))
            {
                return new Card(cardidToCardList[id]);
            }

            return new Card();
        }

        private void enumCreator()
        {
            //call this, if carddb.txt was changed, to get latest public enum cardIDEnum
            Helpfunctions.Instance.logg("public enum cardIDEnum");
            Helpfunctions.Instance.logg("{");
            Helpfunctions.Instance.logg( "None,");
            foreach (string cardid in this.allCardIDS)
            {
                Helpfunctions.Instance.logg(cardid+",");
            }
            Helpfunctions.Instance.logg("}");



            Helpfunctions.Instance.logg("public cardIDEnum cardIdstringToEnum(string s)");
            Helpfunctions.Instance.logg("{");
            foreach (string cardid in this.allCardIDS)
            {
                Helpfunctions.Instance.logg("if(s==\"" + cardid + "\") return CardDB.cardIDEnum." + cardid + ";");
            }
            Helpfunctions.Instance.logg("return CardDB.cardIDEnum.None;");
            Helpfunctions.Instance.logg("}");

            List<string> namelist = new List<string>();

            foreach (Card cardid in this.cardlist)
            {
                if (namelist.Contains(cardid.name.ToString())) continue;
                namelist.Add(cardid.name.ToString());
            }


            Helpfunctions.Instance.logg("public enum cardName");
            Helpfunctions.Instance.logg("{");
            foreach (string cardid in namelist)
            {
                Helpfunctions.Instance.logg(cardid + ",");
            }
            Helpfunctions.Instance.logg("}");

            Helpfunctions.Instance.logg("public cardName cardNamestringToEnum(string s)");
            Helpfunctions.Instance.logg("{");
            foreach (string cardid in namelist)
            {
                Helpfunctions.Instance.logg("if(s==\"" + cardid + "\") return CardDB.cardName." + cardid + ";");
            }
            Helpfunctions.Instance.logg("return CardDB.cardName.unknown;");
            Helpfunctions.Instance.logg("}");




        }

        public static Enchantment getEnchantmentFromCardID(cardIDEnum cardID)
        {
            Enchantment retval = new Enchantment();
            retval.CARDID = cardID;

            if (cardID == cardIDEnum.CS2_188o)//insiriert  dieser diener hat +2 angriff in diesem zug. (ruchloser unteroffizier)
            {
                retval.angrbuff = 2;
            }

            if (cardID == cardIDEnum.CS2_059o)//blutpakt (blutwichtel)
            {
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_019e)//Segen der Klerikerin (blutelfenklerikerin)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }

            if (cardID == cardIDEnum.CS2_045e)//waffedesfelsbeissers
            {
                retval.angrbuff = 3;
            }
            if (cardID == cardIDEnum.EX1_587e)//windfury
            {
                retval.windfury = true;
            }
            if (cardID == cardIDEnum.EX1_355e)//urteildestemplers   granted by blessed champion
            {
                retval.angrfaktor = 2;
            }
            if (cardID == cardIDEnum.NEW1_036e)//befehlsruf
            {
                retval.cantLowerHPbelowONE = true;
            }
            if (cardID == cardIDEnum.CS2_046e)// kampfrausch
            {
                retval.angrbuff = 3;
            }

            if (cardID == cardIDEnum.CS2_104e)// toben
            {
                retval.angrbuff = 3;
                retval.hpbuff = 3;
            }
            if (cardID == cardIDEnum.DREAM_05e)// alptraum
            {
                retval.angrbuff = 5;
                retval.hpbuff = 5;
            }
            if (cardID == cardIDEnum.CS2_022e)// verwandlung
            {
                retval.angrbuff = 3;
            }
            if (cardID == cardIDEnum.EX1_611e)// gefangen
            {
                //icetrap?
            }

            if (cardID == cardIDEnum.EX1_014te)// banane
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_178ae)// festgewurzelt
            {
                retval.hpbuff = 5;
                retval.taunt = true;
            }
            if (cardID == cardIDEnum.CS2_011o)// wildesbruellen
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_366e)// rechtschaffen
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_017o)// klauen (ownhero +1angr)
            {
            }
            if (cardID == cardIDEnum.EX1_604o)// rasend
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_084e)// sturmangriff
            {
                retval.charge = true;
            }
            if (cardID == cardIDEnum.CS1_129e)// inneresfeuer // angr = live
            {
                retval.angrEqualLife = true;
            }
            if (cardID == cardIDEnum.EX1_603e)// aufzackgebracht (fieser zuchtmeister)
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_507e)// mrgglaargl! der murlocanführer verleiht +2/+1.
            {
                retval.angrbuff = 2;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_038e)// geistderahnen : todesröcheln: dieser diener kehrt aufs schlachtfeld zurück.
            {

            }
            if (cardID == cardIDEnum.NEW1_024o)// gruenhauts befehl +1/+1.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_590e)// schattenvonmuru : angriff und leben durch aufgezehrte gottesschilde erhöht. (blutritter)
            {
                retval.angrbuff = 3;
                retval.hpbuff = 3;
            }
            if (cardID == cardIDEnum.CS2_074e)// toedlichesgift
            {
            }
            if (cardID == cardIDEnum.EX1_258e)// ueberladen von entfesselnder elementar
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.TU4f_004o)// vermaechtnisdeskaisers von cho
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == cardIDEnum.NEW1_017e)// gefuellterbauch randvoll mit murloc. (hungrigekrabbe)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == cardIDEnum.EX1_334e)// dunklerbefehl von dunkler Wahnsin
            {
            }

            if (cardID == cardIDEnum.CS2_087e)// segendermacht von segendermacht
            {
                retval.angrbuff = 3;
            }
            if (cardID == cardIDEnum.EX1_613e)// vancleefsrache dieser diener hat erhöhten angriff und erhöhtes leben.
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_623e)// infusion
            {
                retval.hpbuff = 3;
            }
            if (cardID == cardIDEnum.CS2_073e2)// kaltbluetigkeit +4
            {
                retval.angrbuff = 4;
            }
            if (cardID == cardIDEnum.EX1_162o)// staerkedesrudels der terrorwolfalpha verleiht diesem diener +1 angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_549o)// zorndeswildtiers +2 angriff und immun/ in diesem zug.
            {
                retval.angrbuff = 2;
                retval.imune = true;
            }

            if (cardID == cardIDEnum.EX1_091o)//  kontrollederkabale  dieser diener wurde von einer kabaleschattenpriesterin gestohlen.
            {
            }

            if (cardID == cardIDEnum.CS2_084e)//  maldesjaegers
            {
                retval.setHPtoOne = true;
            }
            if (cardID == cardIDEnum.NEW1_036e2)//  befehlsruf2 ? das leben eurer diener kann in diesem zug nicht unter 1 fallen.
            {
                retval.cantLowerHPbelowONE = true;
            }
            if (cardID == cardIDEnum.CS2_122e)// angespornt der schlachtzugsleiter verleiht diesem diener +1 angriff. (schlachtzugsleiter)
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_103e)// charge
            {
                retval.charge = true;
            }
            if (cardID == cardIDEnum.EX1_080o)// geheimnissebewahren    erhöhte werte.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_005o)// klaue +2 angriff in diesem zug.
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_363e2)// segenderweisheit
            {
                retval.cardDrawOnAngr = true;
            }
            if (cardID == cardIDEnum.EX1_178be)//  entwurzelt +5 angr
            {
                retval.angrbuff = 5;
            }
            if (cardID == cardIDEnum.CS2_222o)//  diemachtsturmwinds +1+1 (von champ of sturmwind)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_399e)// amoklauf von gurubashi berserker
            {
                retval.angrbuff = 3;
            }
            if (cardID == cardIDEnum.CS2_041e)// machtderahnen
            {
                retval.taunt = true;
            }
            if (cardID == cardIDEnum.EX1_612o)//  machtderkirintor
            {

            }
            if (cardID == cardIDEnum.EX1_004e)// elunesanmut erhöhtes leben. von junger priesterin
            {
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_246e)// verhext dieser diener wurde verwandelt.
            {

            }
            if (cardID == cardIDEnum.EX1_244e)// machtdertotems (card that buffs hp of totems)
            {
                retval.hpbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_607e)// innerewut (innere wut)
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_573ae)// gunstdeshalbgotts (cenarius?)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_411e2)// schliffbenoetigt angriff verringert.  von waffe blutschrei
            {
                retval.angrbuff = -1;
            }
            if (cardID == cardIDEnum.CS2_063e)// verderbnis  wird zu beginn des zuges des verderbenden spielers vernichtet.
            {

            }
            if (cardID == cardIDEnum.CS2_181e)// vollekraft +2 angr ka von wem
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_508o)// mlarggragllabl! dieser murloc hat +1 angriff. (grimmschuppenorakel)
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_073e)// kaltbluetigkeit +2 angriff.
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.NEW1_018e)// goldrausch von blutsegelraeuberin
            {

            }
            if (cardID == cardIDEnum.EX1_059e2)// experimente! der verrückte alchemist hat angriff und leben vertauscht.
            {

            }
            if (cardID == cardIDEnum.EX1_570e)// biss (only hero)
            {
                retval.angrbuff = 4;
            }
            if (cardID == cardIDEnum.EX1_360e)//  demut  angriff wurde auf 1 gesetzt.
            {
                retval.setANGRtoOne = true;
            }
            if (cardID == cardIDEnum.DS1_175o)// wutgeheul durch waldwolf
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_596e)// daemonenfeuer
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }

            if (cardID == cardIDEnum.EX1_158e)// seeledeswaldes todesröcheln: ruft einen treant (2/2) herbei.
            {

            }
            if (cardID == cardIDEnum.EX1_316e)// ueberwaeltigendemacht
            {
                retval.angrbuff = 4;
                retval.hpbuff = 4;
            }
            if (cardID == cardIDEnum.EX1_044e)// stufenaufstieg erhöhter angriff und erhöhtes leben. (rastloser abenteuer)
            {

            }
            if (cardID == cardIDEnum.EX1_304e)// verzehren  erhöhte werte. (hexer)
            {

            }
            if (cardID == cardIDEnum.EX1_363e)// segenderweisheit der segnende spieler zieht eine karte, wenn dieser diener angreift.
            {

            }
            if (cardID == cardIDEnum.CS2_105e)// heldenhafterstoss
            {

            }
            if (cardID == cardIDEnum.EX1_128e)// verhuellt bleibt bis zu eurem nächsten zug verstohlen.
            {

            }
            if (cardID == cardIDEnum.NEW1_033o)// himmelsauge leokk verleiht diesem diener +1 angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_004e)// machtwortschild
            {
                retval.hpbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_382e)// waffenniederlegen! angriff auf 1 gesetzt.
            {
                retval.setANGRtoOne = true;
            }
            if (cardID == cardIDEnum.CS2_092e)// segenderkoenige
            {
                retval.angrbuff = 4;
                retval.hpbuff = 4;
            }
            if (cardID == cardIDEnum.NEW1_012o)// manasaettigung  erhöhter angriff.
            {

            }
            if (cardID == cardIDEnum.EX1_619e)//  gleichheit  leben auf 1 gesetzt.
            {
                retval.setHPtoOne = true;
            }
            if (cardID == cardIDEnum.EX1_509e)// blarghghl    erhöhter angriff.
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_009e)// malderwildnis
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
                retval.taunt = true;
            }
            if (cardID == cardIDEnum.EX1_103e)// mrghlglhal +2 leben.
            {
                retval.hpbuff = 2;
            }
            if (cardID == cardIDEnum.NEW1_038o)// wachstum  gruul wächst ...
            {

            }
            if (cardID == cardIDEnum.CS1_113e)//  gedankenkontrolle
            {

            }
            if (cardID == cardIDEnum.CS2_236e)//  goettlicherwille  dieser diener hat doppeltes leben.
            {

            }
            if (cardID == cardIDEnum.CS2_083e)// geschaerft +1 angriff in diesem zug.
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.TU4c_008e)// diemachtmuklas
            {
                retval.angrbuff = 8;
            }
            if (cardID == cardIDEnum.EX1_379e)//  busse 
            {
                retval.setHPtoOne = true;
            }
            if (cardID == cardIDEnum.EX1_274e)// puremacht! (astraler arkanist)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
            }
            if (cardID == cardIDEnum.CS2_221e)// vorsicht!scharf! +2 angriff von hasserfüllte schmiedin. 
            {
                retval.weaponAttack = 2;
            }
            if (cardID == cardIDEnum.EX1_409e)// aufgewertet +1 angriff und +1 haltbarkeit.
            {
                retval.weaponAttack = 1;
                retval.weapondurability = 1;
            }
            if (cardID == cardIDEnum.tt_004o)//kannibalismus (fleischfressender ghul)
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_155ae)// maldernatur
            {
                retval.angrbuff = 4;
            }
            if (cardID == cardIDEnum.NEW1_025e)// verstaerkt (by emboldener 3000)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_584e)// lehrenderkirintor zauberschaden+1 (by uralter magier)
            {
                retval.zauberschaden = 1;
            }
            if (cardID == cardIDEnum.EX1_160be)// rudelfuehrer +1/+1. (macht der wildnis)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.TU4c_006e)//  banane
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.NEW1_027e)// yarrr!   der südmeerkapitän verleiht +1/+1.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.DS1_070o)// praesenzdesmeisters +2/+2 und spott/. (hundemeister)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 2;
                retval.taunt = true;
            }
            if (cardID == cardIDEnum.EX1_046e)// gehaertet +2 angriff in diesem zug. (dunkeleisenzwerg)
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_531e)// satt    erhöhter angriff und erhöhtes leben. (aasfressende Hyaene)
            {
                retval.angrbuff = 2;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.CS2_226e)// bannerderfrostwoelfe    erhöhte werte. (frostwolfkriegsfuerst)
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.DS1_178e)//  sturmangriff tundranashorn verleiht ansturm.
            {
                retval.charge = true;
            }
            if (cardID == cardIDEnum.CS2_226o)//befehlsgewalt der kriegsfürst der frostwölfe hat erhöhten angriff und erhöhtes leben.
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.Mekka4e)// verwandelt wurde in ein huhn verwandelt!
            {

            }
            if (cardID == cardIDEnum.EX1_411e)// blutrausch kein haltbarkeitsverlust. (blutschrei)
            {

            }
            if (cardID == cardIDEnum.EX1_145o)// vorbereitung    der nächste zauber, den ihr in diesem zug wirkt, kostet (3) weniger.
            {

            }
            if (cardID == cardIDEnum.EX1_055o)// gestaerkt    die manasüchtige hat erhöhten angriff.
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.CS2_053e)// fernsicht   eine eurer karten kostet (3) weniger.
            {

            }
            if (cardID == cardIDEnum.CS2_146o)//  geschaerft +1 haltbarkeit.
            {
                retval.weapondurability = 1;
            }
            if (cardID == cardIDEnum.EX1_059e)//  experimente! der verrückte alchemist hat angriff und leben vertauscht.
            {

            }
            if (cardID == cardIDEnum.EX1_565o)// flammenzunge +2 angriff von totem der flammenzunge.
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_001e)// wachsam    erhöhter angriff. (lichtwaechterin)
            {
                retval.angrbuff = 2;
            }
            if (cardID == cardIDEnum.EX1_536e)// aufgewertet   erhöhte haltbarkeit.
            {
                retval.weaponAttack = 1;
                retval.weapondurability = 1;
            }
            if (cardID == cardIDEnum.EX1_155be)// maldernatur   dieser diener hat +4 leben und spott/.
            {
                retval.hpbuff = 4;
                retval.taunt = true;
            }
            if (cardID == cardIDEnum.CS2_103e2)// sturmangriff    +2 angriff und ansturm/.
            {
                retval.angrbuff = 2;
                retval.charge = true;
            }
            if (cardID == cardIDEnum.TU4f_006o)// transzendenz    cho kann nicht angegriffen werden, bevor ihr seine diener erledigt habt.
            {

            }
            if (cardID == cardIDEnum.EX1_043e)// stundedeszwielichts    erhöhtes leben. (zwielichtdrache)
            {
                retval.hpbuff = 1;
            }
            if (cardID == cardIDEnum.NEW1_037e)// bewaffnet   erhöhter angriff. meisterschwertschmied
            {
                retval.angrbuff = 1;
            }
            if (cardID == cardIDEnum.EX1_161o)// demoralisierendesgebruell    dieser diener hat -3 angriff in diesem zug.
            {

            }
            if (cardID == cardIDEnum.EX1_093e)// handvonargus
            {
                retval.angrbuff = 1;
                retval.hpbuff = 1;
                retval.taunt = true;
            }


            return retval;
        }



    }

}

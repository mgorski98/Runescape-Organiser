using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;



namespace RunescapeOrganiser {

    public enum SlayerMonsters : byte {
        None,
        Airut,
        Dark_Beast,
        Acheron_Mammoth,
        Abyssal_Demon,
        Nechryael,
        Automaton,
        Seeker,
        Gargoyle,
        Muspah,
        Blood_Nihil,
        Smoke_Nihil,
        Shadow_Nihil,
        Ice_Nihil,
        Nightmare,
        Mutated_Jadinko_Baby,
        Mutated_Jadinko_Male,
        Mutated_Jadinko_Guard,
        Rorarius,
        Gladius,
        Corrupted_Scorpion,
        Edimmu,
        Corrupted_Scarab,
        Ice_Strykewyrm,
        Lava_Strykewyrm,
        Corrupted_Lizard,
        Dragonstone_Dragon,
        Ganodermic_Beast,
        Living_Wyvern,
        Ripper_Demon,
        Camel_Warrior,
        Corrupted_Dust_Devil,
        Onyx_Dragon,
        Soulgazer,
        Corrupted_Kalphite,
        Hydrix_Dragon,
        Corrupted_Worker,
        Vinecrawler,
        Salawa_Akh,
        Bulbous_Crawler,
        Feline_Akh,
        Moss_Golem,
        Scarab_Akh,
        Crocodile_Akh,
        Gorilla_Akh,
        Imperial_Akh,
        Dagannoth,
        Adamant_Dragon,
        Rune_Dragon,
        Black_Demon,
        Celestial_Dragon,
        Chaos_Giant,
        Crystal_Shapeshifter,
        Glacor,
        Steel_Dragon,
        Iron_Dragon,
        Kal_Gerion_Demon,
        Kalphite,
        Thruthful_Shadow,
        Blissful_Shadow,
        Manifest_Shadow,
        Tormented_Demon,
        Vyrewatch,
        Waterfiend,
        Elves,
        Aviansie,
        Black_Dragon,
        Desert_Strykewyrm,
        Greater_Demon,
        Mithril_Dragon,
        Aquanite,
        Grotworm,
    }

    public enum BossSlayerMonsters : byte {
        None = 0,
        QBD,
        The_Magister,
        Kalphite_King,
        Exiled_Kalphite_Queen,
        Kalphite_Queen,
        KBD,
        Kril_Tsutsaroth,
        KreeArra,
        Legiones,
        Dagannoth_Kings
    }

    public enum SlayerBonuses : byte {
        SlayerCodexNone,
        SlayerCodexTier1,
        SlayerCodexTier2,
        SlayerCodexTier3,
        SlayerCodexTier4,
        SlayerCodexTier5,
        SlayerContract
    }

    public struct Slayer { //TODO: add a lookup table of monsters with Slayer Contract available
        public static Dictionary<SlayerMonsters, KeyValuePair<string, decimal>> SlayerLookUpTable
            = new Dictionary<SlayerMonsters, KeyValuePair<string, decimal>>();
        public static Dictionary<BossSlayerMonsters, KeyValuePair<string, decimal>> BossSlayerLookUpTable
            = new Dictionary<BossSlayerMonsters, KeyValuePair<string, decimal>>();
        public static Dictionary<SlayerBonuses, decimal> SlayerBonusesLookUpTable = new Dictionary<SlayerBonuses, decimal>();
        public static Dictionary<BossSlayerMonsters, SlayerMonsters> BossMonsterTypesLookUpTable = new Dictionary<BossSlayerMonsters, SlayerMonsters>();
        public static List<SlayerMonsters> SlayerMonstersWithContractAvailableLookUpTable = new List<SlayerMonsters>() {
        SlayerMonsters.Abyssal_Demon,
        SlayerMonsters.Gargoyle,
        SlayerMonsters.Nechryael
    };
        public static Dictionary<BossSlayerMonsters, List<SlayerMonsters>> BossMonsterGroups = new Dictionary<BossSlayerMonsters, List<SlayerMonsters>>(){
        {BossSlayerMonsters.Dagannoth_Kings, new List<SlayerMonsters>() { SlayerMonsters.Dagannoth } },
        {BossSlayerMonsters.Exiled_Kalphite_Queen, new List<SlayerMonsters>() { SlayerMonsters.Kalphite } },
        {BossSlayerMonsters.Kalphite_King, new List<SlayerMonsters>() { SlayerMonsters.Kalphite } },
        {BossSlayerMonsters.Kalphite_Queen, new List<SlayerMonsters>() { SlayerMonsters.Kalphite } },
        {BossSlayerMonsters.KBD, new List<SlayerMonsters>() { SlayerMonsters.Black_Dragon } },
        {BossSlayerMonsters.KreeArra, new List<SlayerMonsters>() { SlayerMonsters.Aviansie } },
        {BossSlayerMonsters.Kril_Tsutsaroth, new List<SlayerMonsters>() { SlayerMonsters.Greater_Demon } },
        {BossSlayerMonsters.Legiones, new List<SlayerMonsters>() { SlayerMonsters.Rorarius, SlayerMonsters.Gladius } },
        {BossSlayerMonsters.QBD, new List<SlayerMonsters>() { SlayerMonsters.Black_Dragon } },
        {BossSlayerMonsters.The_Magister, new List<SlayerMonsters>() { SlayerMonsters.Salawa_Akh, SlayerMonsters.Feline_Akh, SlayerMonsters.Gorilla_Akh, SlayerMonsters.Scarab_Akh, SlayerMonsters.Imperial_Akh, SlayerMonsters.Crocodile_Akh } }
    };

        private const string SlayerLookUpTableFilePath = @"../../JsonFiles/SlayerMonsters.json";
        private const string BossSlayerLookUpTableFilePath = @"../../JsonFiles/BossSlayerMonsters.json";
        private const string SlayerBonusesLookUpTableFilePath = @"../../JsonFiles/SlayerBonuses.json";
        private const string BossMonsterTypesLookUpTableFilePath = @"../../JsonFiles/BossMonsterTypes.json";
        //private const string SlayerMonstersWithContractAvailableLookUpTableFilePath = @"../../JsonFiles/SlayerContracts.json";

        public static void InitSlayerTables() {
            using (var reader = new StreamReader(SlayerLookUpTableFilePath)) {
                SlayerLookUpTable = JsonConvert.DeserializeObject<Dictionary<SlayerMonsters, KeyValuePair<string, decimal>>>(reader.ReadToEnd());
            }
            using (var reader1 = new StreamReader(BossSlayerLookUpTableFilePath)) {
                BossSlayerLookUpTable = JsonConvert.DeserializeObject<Dictionary<BossSlayerMonsters, KeyValuePair<string, decimal>>>(reader1.ReadToEnd());
            }
            using (var reader2 = new StreamReader(SlayerBonusesLookUpTableFilePath)) {
                SlayerBonusesLookUpTable = JsonConvert.DeserializeObject<Dictionary<SlayerBonuses, decimal>>(reader2.ReadToEnd());
            }
            using (var reader3 = new StreamReader(BossMonsterTypesLookUpTableFilePath)) {
                BossMonsterTypesLookUpTable = JsonConvert.DeserializeObject<Dictionary<BossSlayerMonsters, SlayerMonsters>>(reader3.ReadToEnd());
            }
        }

        [System.Obsolete]
        public static void DumpToDisk() {
            Dictionary<SlayerMonsters, KeyValuePair<string, decimal>> temp1 = new Dictionary<SlayerMonsters, KeyValuePair<string, decimal>>() {
            {SlayerMonsters.Grotworm,               new KeyValuePair<string, decimal>("Grotworms (Mature)", (decimal)343.6)},
            {SlayerMonsters.Aquanite,               new KeyValuePair<string, decimal>("Aquanites", (decimal)212.6) },
            {SlayerMonsters.Airut,                  new KeyValuePair<string, decimal>("Airuts", (decimal)800.2) },
            {SlayerMonsters.Dark_Beast,             new KeyValuePair<string, decimal>("Dark Beasts", (decimal)331.4) },
            {SlayerMonsters.Acheron_Mammoth,        new KeyValuePair<string, decimal>("Acheron Mammoths", (decimal)4215.6) },
            {SlayerMonsters.Abyssal_Demon,          new KeyValuePair<string, decimal>("Abyssal Demons", (decimal)278) },
            {SlayerMonsters.Nechryael,              new KeyValuePair<string, decimal>("Nechryaels", (decimal)251.6) },
            {SlayerMonsters.Automaton,              new KeyValuePair<string, decimal>("Automatons", (decimal)624) },
            {SlayerMonsters.Seeker,                 new KeyValuePair<string, decimal>("Seekers", (decimal)440.6) },
            {SlayerMonsters.Gargoyle,               new KeyValuePair<string, decimal>("Gargoyles", (decimal)197.4) },
            {SlayerMonsters.Muspah,                 new KeyValuePair<string, decimal>("Muspah", (decimal)469) },
            {SlayerMonsters.Blood_Nihil,            new KeyValuePair<string, decimal>("Blood Nihils", (decimal)705.4) },
            {SlayerMonsters.Smoke_Nihil,            new KeyValuePair<string, decimal>("Smoke Nihils", (decimal)564) },
            {SlayerMonsters.Shadow_Nihil,           new KeyValuePair<string, decimal>("Shadow Nihils", (decimal)423) },
            {SlayerMonsters.Ice_Nihil,              new KeyValuePair<string, decimal>("Ice Nihils", (decimal)564) },
            {SlayerMonsters.Nightmare,              new KeyValuePair<string, decimal>("Nightmares", (decimal)1540) },
            {SlayerMonsters.Mutated_Jadinko_Baby,   new KeyValuePair<string, decimal>("Mutated Jadinko Babies", (decimal)98.6) },
            {SlayerMonsters.Mutated_Jadinko_Male,   new KeyValuePair<string, decimal>("Mutated Jadinko Males", (decimal)209.6) },
            {SlayerMonsters.Mutated_Jadinko_Guard,  new KeyValuePair<string, decimal>("Mutated Jadinko Guards", (decimal)188.4) },
            {SlayerMonsters.Rorarius,               new KeyValuePair<string, decimal>("Rorarii", (decimal)140) },
            {SlayerMonsters.Gladius,                new KeyValuePair<string, decimal>("Gladii", (decimal)220.2) },
            {SlayerMonsters.Corrupted_Scorpion,     new KeyValuePair<string, decimal>("Corrupted Scorpions", (decimal)353.2) },
            {SlayerMonsters.Edimmu,                 new KeyValuePair<string, decimal>("Edimmus", (decimal)880.2) },
            {SlayerMonsters.Corrupted_Scarab,       new KeyValuePair<string, decimal>("Corrupted Scarabs", (decimal)389) },
            {SlayerMonsters.Ice_Strykewyrm,         new KeyValuePair<string, decimal>("Ice Strykewyrms", (decimal)693.2) },
            {SlayerMonsters.Lava_Strykewyrm,        new KeyValuePair<string, decimal>("Lava Strykewyrms", (decimal)1872) },
            {SlayerMonsters.Corrupted_Lizard,       new KeyValuePair<string, decimal>("Corrupted Lizards", (decimal)533.4) },
            {SlayerMonsters.Dragonstone_Dragon,     new KeyValuePair<string, decimal>("Dragonstone Dragons", (decimal)1448.4) },
            {SlayerMonsters.Ganodermic_Beast,       new KeyValuePair<string, decimal>("Ganodermic Beasts", (decimal)565) },
            {SlayerMonsters.Living_Wyvern,          new KeyValuePair<string, decimal>("Living Wyverns", (decimal)1878.8) },
            {SlayerMonsters.Ripper_Demon,           new KeyValuePair<string, decimal>("Ripper Demons", (decimal)2721.5) },
            {SlayerMonsters.Camel_Warrior,          new KeyValuePair<string, decimal>("Camel Warriors", (decimal)4768.8) },
            {SlayerMonsters.Corrupted_Dust_Devil,   new KeyValuePair<string, decimal>("Corrupted Dust Devils", (decimal)679.8) },
            {SlayerMonsters.Onyx_Dragon,            new KeyValuePair<string, decimal>("Onyx Dragons", (decimal)1858.8) },
            {SlayerMonsters.Soulgazer,              new KeyValuePair<string, decimal>("Soulgazers", (decimal)1950) },
            {SlayerMonsters.Corrupted_Kalphite,     new KeyValuePair<string, decimal>("Corrupted Kalphites", (decimal)494) },
            {SlayerMonsters.Hydrix_Dragon,          new KeyValuePair<string, decimal>("Hydrix Dragons",(decimal)4768.8) },
            {SlayerMonsters.Corrupted_Worker,       new KeyValuePair<string, decimal>("Corrupted Workers",(decimal)653) },
            {SlayerMonsters.Vinecrawler,            new KeyValuePair<string, decimal>("Vinecrawlers",(decimal)2086) },
            {SlayerMonsters.Salawa_Akh,             new KeyValuePair<string, decimal>("Salawa Akhs",(decimal)504) },
            {SlayerMonsters.Bulbous_Crawler,        new KeyValuePair<string, decimal>("Bulbous Crawlers",(decimal)2360) },
            {SlayerMonsters.Feline_Akh,             new KeyValuePair<string, decimal>("Feline Akhs",(decimal)536.2) },
            {SlayerMonsters.Moss_Golem,             new KeyValuePair<string, decimal>("Moss Golems",(decimal)2408.8) },
            {SlayerMonsters.Scarab_Akh,             new KeyValuePair<string, decimal>("Scarab Akhs",(decimal)714.8) },
            {SlayerMonsters.Crocodile_Akh,          new KeyValuePair<string, decimal>("Crocodile Akhs",(decimal)761) },
            {SlayerMonsters.Gorilla_Akh,            new KeyValuePair<string, decimal>("Gorilla Akhs",(decimal)705) },
            {SlayerMonsters.Imperial_Akh,           new KeyValuePair<string, decimal>("Imperial Akhs",(decimal)1780.6) },
            {SlayerMonsters.Dagannoth,              new KeyValuePair<string, decimal>("Dagannoth",(decimal)57.6) },
            {SlayerMonsters.Adamant_Dragon,         new KeyValuePair<string, decimal>("Adamant Dragons",(decimal)655.6) },
            {SlayerMonsters.Rune_Dragon,            new KeyValuePair<string, decimal>("Rune Dragons",(decimal)2051) },
            {SlayerMonsters.Black_Demon,            new KeyValuePair<string, decimal>("Black Demons",(decimal)294.4) },
            {SlayerMonsters.Celestial_Dragon,       new KeyValuePair<string, decimal>("Celestial Dragons",(decimal)976.3) },
            {SlayerMonsters.Chaos_Giant,            new KeyValuePair<string, decimal>("Chaos Giants",(decimal)929.3) },
            {SlayerMonsters.Crystal_Shapeshifter,   new KeyValuePair<string, decimal>("Crystal Shapeshifters", (decimal)1269.8) },
            {SlayerMonsters.Glacor,                 new KeyValuePair<string, decimal>("Glacors", (decimal)1881) },
            {SlayerMonsters.Steel_Dragon,           new KeyValuePair<string, decimal>("Steel Dragons", (decimal)350) },
            {SlayerMonsters.Iron_Dragon,            new KeyValuePair<string, decimal>("Iron Dragons", (decimal)245) },
            {SlayerMonsters.Kal_Gerion_Demon,       new KeyValuePair<string, decimal>("Kal'Gerion Demons", (decimal)1858.8) },
            {SlayerMonsters.Kalphite,               new KeyValuePair<string, decimal>("Kalphites", (decimal)147) },
            {SlayerMonsters.Thruthful_Shadow,       new KeyValuePair<string, decimal>("Thrutful Shadows", (decimal)343) },
            {SlayerMonsters.Blissful_Shadow,        new KeyValuePair<string, decimal>("Blissful Shadows", (decimal)565) },
            {SlayerMonsters.Manifest_Shadow,        new KeyValuePair<string, decimal>("Manifest Shadows", (decimal)930) },
            {SlayerMonsters.Tormented_Demon,        new KeyValuePair<string, decimal>("Tormented Demons", (decimal)1136) },
            {SlayerMonsters.Vyrewatch,              new KeyValuePair<string, decimal>("Vyrewatch", (decimal)89.4) },
            {SlayerMonsters.Waterfiend,             new KeyValuePair<string, decimal>("Waterfiends", (decimal)335) },
            {SlayerMonsters.Elves,                  new KeyValuePair<string, decimal>("Elves", (decimal)608) },
            {SlayerMonsters.Aviansie,               new KeyValuePair<string, decimal>("Aviansies", (decimal)187) },
            {SlayerMonsters.Black_Dragon,           new KeyValuePair<string, decimal>("Black Dragons", (decimal)245) },
            {SlayerMonsters.Desert_Strykewyrm,      new KeyValuePair<string, decimal>("Desert Strykewyrms", (decimal)376.5) },
            {SlayerMonsters.Greater_Demon,          new KeyValuePair<string, decimal>("Greater Demons", (decimal)135.4) },
            {SlayerMonsters.Mithril_Dragon,         new KeyValuePair<string, decimal>("Mithril Dragons", (decimal)564.4) },
        };
            Dictionary<BossSlayerMonsters, KeyValuePair<string, decimal>> temp2 = new Dictionary<BossSlayerMonsters, KeyValuePair<string, decimal>>() {
            {BossSlayerMonsters.QBD,                    new KeyValuePair<string, decimal>("Queen Black Dragon", (decimal)1693.5)},
            {BossSlayerMonsters.The_Magister,           new KeyValuePair<string, decimal>("The Magister", (decimal)8000)},
            {BossSlayerMonsters.Legiones,               new KeyValuePair<string, decimal>("Legiones", (decimal)1829.2)},
            {BossSlayerMonsters.Kril_Tsutsaroth,        new KeyValuePair<string, decimal>("K'ril Tsutsaroth", (decimal)2151.5)},
            {BossSlayerMonsters.KreeArra,               new KeyValuePair<string, decimal>("Kree'arra", (decimal)2934)},
            {BossSlayerMonsters.KBD,                    new KeyValuePair<string, decimal>("King Black Dragon", (decimal)1050.6)},
            {BossSlayerMonsters.Kalphite_Queen,         new KeyValuePair<string, decimal>("Kalphite Queen", (decimal)1309.4)},
            {BossSlayerMonsters.Kalphite_King,          new KeyValuePair<string, decimal>("Kalphite King", (decimal)3963)},
            {BossSlayerMonsters.Exiled_Kalphite_Queen,  new KeyValuePair<string, decimal>("Exiled Kalphite Queen", (decimal)2055)},
            {BossSlayerMonsters.Dagannoth_Kings,        new KeyValuePair<string, decimal>("Dagannoth Kings", (decimal)1068)},
        };
            Dictionary<SlayerBonuses, decimal> temp3 = new Dictionary<SlayerBonuses, decimal>() {
            { SlayerBonuses.SlayerCodexTier1, (decimal)0.01},
            { SlayerBonuses.SlayerCodexTier2, (decimal)0.02},
            { SlayerBonuses.SlayerCodexTier3, (decimal)0.03},
            { SlayerBonuses.SlayerCodexTier4, (decimal)0.04},
            { SlayerBonuses.SlayerCodexTier5, (decimal)0.05},
            { SlayerBonuses.SlayerContract, (decimal)0.20 }
        };
            Dictionary<BossSlayerMonsters, SlayerMonsters> temp4 = new Dictionary<BossSlayerMonsters, SlayerMonsters>() {
            {BossSlayerMonsters.Dagannoth_Kings, SlayerMonsters.Dagannoth},
            {BossSlayerMonsters.QBD, SlayerMonsters.Black_Dragon},
            {BossSlayerMonsters.Exiled_Kalphite_Queen, SlayerMonsters.Kalphite},
            {BossSlayerMonsters.Kalphite_King, SlayerMonsters.Kalphite },
            {BossSlayerMonsters.Kalphite_Queen, SlayerMonsters.Kalphite },
            {BossSlayerMonsters.KBD, SlayerMonsters.Black_Dragon },
            {BossSlayerMonsters.Legiones, SlayerMonsters.Rorarius},
            {BossSlayerMonsters.The_Magister, SlayerMonsters.Imperial_Akh},
            {BossSlayerMonsters.Kril_Tsutsaroth, SlayerMonsters.Greater_Demon },
            {BossSlayerMonsters.KreeArra, SlayerMonsters.Aviansie }
        };

            using (var fs = new FileStream(SlayerLookUpTableFilePath, FileMode.OpenOrCreate)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(temp1, Formatting.Indented));
                }
            }

            using (var fs = new FileStream(BossSlayerLookUpTableFilePath, FileMode.OpenOrCreate)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(temp2, Formatting.Indented));
                }
            }

            using (var fs = new FileStream(SlayerBonusesLookUpTableFilePath, FileMode.OpenOrCreate)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(temp3, Formatting.Indented));
                }
            }

            using (var fs = new FileStream(BossMonsterTypesLookUpTableFilePath, FileMode.OpenOrCreate)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(temp4, Formatting.Indented));
                }
            }
        }
    }

    public struct Earnings {//item names will be filled continuously with added items

        private const string ItemNamesFilePath = @"../../JsonFiles/Items.json";

        public static HashSet<string> ItemNames = new HashSet<string>();

        [System.Obsolete]
        public static void DumpToDisk() {
            ItemNames.Add("High Level Alchemy");
            using (var fs = new FileStream(ItemNamesFilePath, FileMode.Create)) {
                using (var writer = new StreamWriter(fs)) {
                    writer.Write(JsonConvert.SerializeObject(ItemNames, Formatting.Indented));
                }
            }
        }

        public static void InitItemNames() {
            using (var reader = new StreamReader(ItemNamesFilePath)) {
                ItemNames = JsonConvert.DeserializeObject<HashSet<string>>(reader.ReadToEnd());
            }
        }
    }

}
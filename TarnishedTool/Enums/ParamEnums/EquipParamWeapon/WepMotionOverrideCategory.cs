// 

using System.ComponentModel;

namespace TarnishedTool.Enums.ParamEnums.EquipParamWeapon;

public enum WepMotionOverrideCategory : ushort
{
    None = 0,
    [Description("Great Knife/Ivory Sickle/Celebrant's Sickle")]
    GreatKnifeIvorySickleCelebrantsSickle = 100,
    [Description("Misericorde/Scorpion's Stinger/Glintstone Kris")]
    MisericordeScorpionsStingerGlintstoneKris = 101,
    [Description("Erdsteel Dagger/Blade of Calling/Black Knife")]
    ErdsteelDaggerBladeOfCallingBlackKnife = 103,
    Wakizashi = 104,
    [Description("Broadsword/Cane Sword")]
    BroadswordCaneSword = 110,
    [Description("Short Sword")]
    ShortSword = 111,
    [Description("Miquellan Knight's Sword")]
    MiquellanKnightsSword = 112,
    [Description("Carian Knight's Sword/Lazuli Glintstone Sword")]
    CarianKnightsSwordLazuliGlintstoneSword = 113,
    [Description("Warhawk's Talon")]
    WarhawksTalon = 117,
    [Description("Eleonora's Poleblade")]
    EleonorasPoleblade = 120,
    [Description("Godskin Peeler")]
    GodskinPeeler = 121,
    Claymore = 125,
    Flamberge = 126,
    [Description("Sword of Milos/Death's Poker")]
    SwordOfMilosDeathsPoker = 127,
    [Description("Knight's Greatsword/Banished Knight's Greatsword/Inseparable Sword")]
    KnightsGreatswordBanishedKnightsGreatswordInseparableSword = 128,
    [Description("Dark Moon Greatsword")]
    DarkMoonGreatsword = 129,
    [Description("Marais Executioner's Sword")]
    MaraisExecutionersSword = 130,
    [Description("Zweihander/Troll Knight's Sword")]
    ZweihanderTrollKnightsSword = 135,
    Greatsword = 136,
    [Description("Godslayer's Greatsword")]
    GodslayersGreatsword = 137,
    [Description("Ruins Greatsword")]
    RuinsGreatsword = 138,
    [Description("Estoc/Noble's Estoc")]
    EstocNoblesEstoc = 145,
    [Description("Rogier's Rapier")]
    RogiersRapier = 146,
    [Description("Frozen Needle")]
    FrozenNeedle = 147,
    [Description("Godskin Stitcher")]
    GodskinStitcher = 150,
    [Description("Great Epee")]
    GreatEpee = 151,
    [Description("Shotel/Eclipse Shotel/Nox Flowing Sword")]
    ShotelEclipseShotelNoxFlowingSword = 155,
    [Description("Scimitar/Shamshir")]
    ScimitarShamshir = 156,
    [Description("Flowing Curved Sword")]
    FlowingCurvedSword = 157,
    [Description("Mantis Blade")]
    MantisBlade = 158,
    [Description("Wing of Astel")]
    WingOfAstel = 159,
    [Description("Beastman's Curved Sword")]
    BeastmansCurvedSword = 161,
    [Description("Serpentbone Blade")]
    SerpentboneBlade = 165,
    [Description("Meteoric Ore Blade")]
    MeteoricOreBlade = 166,
    Nagakiba = 167,
    [Description("Hand Axe/Icerind Hatchet/Forked Hatchet")]
    HandAxeIcerindHatchetForkedHatchet = 170,
    [Description("Warped Axe/Ripple Blade")]
    WarpedAxeRippleBlade = 171,
    [Description("Iron Cleaver/Celebrant's Cleaver")]
    IronCleaverCelebrantsCleaver = 172,
    [Description("Butchering Knife")]
    ButcheringKnife = 175,
    Pickaxe = 176,
    [Description("Axe of Godrick/Crescent Moon Axe")]
    AxeOfGodrickCrescentMoonAxe = 177,
    [Description("Club/Stone Club")]
    ClubStoneClub = 180,
    [Description("Spiked Club")]
    SpikedClub = 181,
    [Description("Morning Star/Scepter of the All-Knowing")]
    MorningStarScepterOfTheAllKnowing = 182,
    Mace = 183,
    [Description("Monk's Flamemace")]
    MonksFlamemace = 184,
    [Description("Great Club")]
    GreatClub = 195,
    [Description("Prelate's Inferno Crozier")]
    PrelatesInfernoCrozier = 196,
    [Description("Giant-Crusher")]
    GiantCrusher = 197,
    [Description("Golem's Halberd")]
    GolemsHalberd = 198,
    [Description("Partisan/Spiked Spear/Death Ritual Spear")]
    PartisanSpikedSpearDeathRitualSpear = 200,
    Pike = 201,
    [Description("Cross-Naginata")]
    CrossNaginata = 202,
    [Description("Short Spear/Cleanrot Spear")]
    ShortSpearCleanrotSpear = 203,
    [Description("Vyke's War Spear/Siluria's Tree")]
    VykesWarSpearSiluriasTree = 205,
    Treespear = 206,
    [Description("Serpent-Hunter")]
    SerpentHunter = 207,
    [Description("Halberd/Commander's Standard/Banished Knight's Halberd/Dragon Halberd")]
    HalberdCommandersStandardBanishedKnightsHalberdDragonHalberd = 210,
    [Description("Lucerne/Golden Halberd/Nightrider Glaive")]
    LucerneGoldenHalberdNightriderGlaive = 211,
    [Description("Guardian's Swordspear/Loretta's War Sickle")]
    GuardiansSwordspearLorettasWarSickle = 212,
    [Description("Zamor Curved Sword")]
    ZamorCurvedSword = 215,
    [Description("Omen Cleaver/Magma Wyrm's Scalesword")]
    OmenCleaverMagmaWyrmsScalesword = 216,
    [Description("Katar/Veteran's Prosthesis")]
    KatarVeteransProsthesis = 220,
    Caestus = 221,
    Scythe = 225,
    Urumi = 226,
    [Description("Raptor Talons")]
    RaptorTalons = 227,
    [Description("Albinauric Bow")]
    AlbinauricBow = 230,
    [Description("Harp Bow")]
    HarpBow = 232,
    [Description("Pulley Crossbow")]
    PulleyCrossbow = 233,
    [Description("Full Moon Crossbow")]
    FullMoonCrossbow = 236,
    Seal = 240,
    [Description("Sword Lance")]
    SwordLance = 241,
    [Description("Dane's Footwork")]
    DanesFootwork = 246,
    [Description("Smithscript Spear")]
    SmithscriptSpear = 247,
    [Description("Smithscript Axe")]
    SmithscriptAxe = 248,
    [Description("Rellana's Twin Blades")]
    RellanaTwinBlades = 249,
    [Description("Smithscript Greathammer")]
    SmithscriptGreathammer = 250,
    [Description("Smithscript Cirque")]
    SmithscriptCirque = 251,
    [Description("Swift Spear")]
    SwiftSpear = 252,
    [Description("Black Steel Greathammer")]
    BlackSteelGreathammer = 253,
    [Description("Claws of Night")]
    ClawsOfNight = 254,
    [Description("Falx/Horned Warrior's Sword")]
    FalxHornedWarriorsSword = 255,
    [Description("Dancing Blade of Ranah")]
    DancingBladeOfRanah = 257,
    [Description("Death Knight's Twin Axes")]
    DeathKnightsTwinAxes = 258,
    [Description("Golem Fist")]
    GolemFist = 259,
    [Description("Rabbath's Cannon")]
    RabbathsCannon = 261,
    [Description("Main-gauche")]
    MainGauche = 262,
    [Description("Fire Knight's Greatsword")]
    FireKnightsGreatsword = 263,
    [Description("Lizard Greatsword")]
    LizardGreatsword = 264,
    [Description("Curseblade's Cirque")]
    CursebladesCirque = 265,
    [Description("Spear of the Impaler")]
    SpearOfTheImpaler = 266,
    [Description("Bloodfiend's Arm")]
    BloodfiendsArm = 267,
    [Description("Putrescence Cleaver")]
    PutrescenceCleaver = 268,
    [Description("Nanaya's Torch")]
    NanayasTorch = 288,
    [Description("Lamenting Visage")]
    LamentingVisage = 289,
    [Description("Staff of the Great Beyond")]
    StaffOfTheGreatBeyond = 290,
    [Description("Ghostflame Torch")]
    GhostflameTorch = 291,
    [Description("St. Trina's Torch")]
    StTrinasTorch = 292,
    [Description("Carian Sorcery Sword")]
    CarianSorcerySword = 293,
    Unarmed = 522,
    [Description("Axe of Godfrey")]
    AxeOfGodfrey = 831,
    [Description("Starscourge Greatsword")]
    StarscourgeGreatsword = 832,
    [Description("Ghiza's Wheel")]
    GhizasWheel = 839,
    [Description("Ornamental Straight Sword")]
    OrnamentalStraightSword = 852
}
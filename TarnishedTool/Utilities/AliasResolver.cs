using System;
using System.Collections.Generic;
using System.Linq;
using TarnishedTool.Models;

namespace TarnishedTool.Utilities;

public class AliasResolver // I heckin love fuzzy search!
{
    private static AliasEntry WeaponAlias(string plural, string singular = null) =>
        new() { Names = singular != null ? [plural, singular] : [plural], Targets = [plural] };

    private static readonly List<AliasEntry> WeaponAliases = new()
    {
        WeaponAlias("Daggers", "Dagger"),
        WeaponAlias("Straight Swords", "Straight Sword"),
        WeaponAlias("Greatswords", "Greatsword"),
        WeaponAlias("Colossal Swords", "Colossal Sword"),
        WeaponAlias("Curved Swords", "Curved Sword"),
        WeaponAlias("Curved Great Swords", "Curved Great Sword"),
        WeaponAlias("Katanas", "Katana"),
        WeaponAlias("Twinblades", "Twinblade"),
        WeaponAlias("Thrusting Swords", "Thrusting Sword"),
        WeaponAlias("Heavy Thrusting Swords", "Heavy Thrusting Sword"),
        WeaponAlias("Axes", "Axe"),
        WeaponAlias("Great Axes", "Great Axe"),
        WeaponAlias("Hammers", "Hammer"),
        WeaponAlias("Great Hammers", "Great Hammer"),
        WeaponAlias("Flails", "Flail"),
        WeaponAlias("Spears", "Spear"),
        WeaponAlias("Great Spears", "Great Spear"),
        WeaponAlias("Halberds", "Halberd"),
        WeaponAlias("Reapers", "Reaper"),
        WeaponAlias("Fists", "Fist"),
        WeaponAlias("Claws", "Claw"),
        WeaponAlias("Whips", "Whip"),
        WeaponAlias("Colossal Weapons", "Colossal Weapon"),
        WeaponAlias("Short Bows", "Short Bow"),
        WeaponAlias("Bows", "Bow"),
        WeaponAlias("Great Bows", "Great Bow"),
        WeaponAlias("Crossbows", "Crossbow"),
        WeaponAlias("Ballistas", "Ballista"),
        WeaponAlias("Staves", "Stave"),
        WeaponAlias("Seals", "Seal"),
        WeaponAlias("Small Shields", "Small Shield"),
        WeaponAlias("Medium Shields", "Medium Shield"),
        WeaponAlias("Great Shields", "Great Shield"),
        WeaponAlias("Torches", "Torch"),
        WeaponAlias("Hand-To-Hand Arts", "Hand-To-Hand Art"),
        WeaponAlias("Perfume Bottles", "Perfume Bottle"),
        WeaponAlias("Thrusting Shields", "Thrusting Shield"),
        WeaponAlias("Throwing Blades", "Throwing Blade"),
        WeaponAlias("Backhand Blades", "Backhand Blade"),
        WeaponAlias("Light Greatswords", "Light Greatsword"),
        WeaponAlias("Great Katanas", "Great Katana"),
        WeaponAlias("Beast Claws", "Beast Claw"),
    };

    private static readonly List<AliasEntry> BuiltIn = new List<AliasEntry>
        {
            new()
            {
                Names = ["godfrey", "hoarah loux"],
                Targets =
                [
                    "Godfrey First Elden Lord", "Godfrey the First Elden Lord", "Hoarah Loux", "Hoarah Loux Warrior"
                ]
            },

            new() { Names = ["bofa"], Targets = ["Beastman of Farum Azula"] },
            new() { Names = ["moose"], Targets = ["Ancestor Spirit", "Regal Ancestor Spirit"] },
            new() { Names = ["bbh"], Targets = ["Bell Bearing Hunter"] },
            new() { Names = ["bbk"], Targets = ["Black Blade Kindred"] },
            new()
            {
                Names = ["bka"], Targets =
                [
                    "Black Knife Assassin", "Black Knife Assassin (Sage's Cave)",
                    "Black Knife Assassin (Sainted Hero's Grave)",
                    "Alecto Black Knife Ringleader"
                ]
            },
            new() { Names = ["death bird"], Targets = ["Deathbird", "Death Rite Bird"] },
            new()
            {
                Names = ["dragon"], Targets =
                [
                    "Ancient Dragon Lansseax", "Borealis the Freezing Fog", "Decaying Ekzykes", "Dragonkin Soldier",
                    "Dragonlord Placidusax", "Flying Dragon Agheel", "Flying Dragon Greyll", "Glintstone Dragon Adula",
                    "Glintstone Dragon Smarag", "Lichdragon Fortissax", "Ancient Dragon Senessax", "Ghostflame Dragon",
                    "Jagged Peak Drake", "Jagged Peak Drake (Duo Encounter)", "Bayle the Dread"
                ]
            },
            new() { Names = ["gduo"], Targets = ["Godskin Duo"] },
            new() { Names = ["boatman"], Targets = ["Tibia Mariner"] },
            new() { Names = ["bug"], Targets = ["Romina Saint of the Bud"] },

            new() { Names = ["adm"], Targets = ["Ancient Dragon-Man"] },
            new() { Names = ["Knight of the Solitary Gaol"], Targets = ["Blackgaol Knight"] },

            // Items
            new() { Names = ["magic", "sorcery", "sorceries"], Targets = ["Sorceries"] },
            new() { Names = ["spells", "spell"], Targets = ["Sorceries", "Incantations"] },
            new() { Names = ["incants", "incantation", "incantations"], Targets = ["Incantations"] },
            new() { Names = ["upgrades", "upgrade mats", "upgrade materials"], Targets = ["Upgrade Materials"] },
            new() { Names = ["weapons"], Targets = ["Weapons"] },
            new() { Names = ["key items"], Targets = ["Key Items"] },
            new() { Names = ["summons", "spirit ash", "ash summons"], Targets = ["Spirit Ashes"] },
            new() { Names = ["aow", "ash of war", "ashes of war"], Targets = ["Ash of War"] },
            new() { Names = ["tears", "crystal tears"], Targets = ["Crystal Tears"] },
            new() { Names = ["crafting books", "cookbooks"], Targets = ["Cookbooks"] },
            new() { Names = ["bell bearings"], Targets = ["Bell Bearings"] },
            new() { Names = ["tali", "talis", "talismans", "talismen"], Targets = ["Talismans"] },
            new() { Names = ["Armor", "Armour"], Targets = ["Armor"] },
            new() { Names = ["Arrow", "Arrows"], Targets = ["Arrows"] },
            new() { Names = ["Greatarrow", "Greatarrows"], Targets = ["Radahn's Spear", "Igon's Harpoon"] },
            new() { Names = ["Greatbolt", "Greatbolts"], Targets = ["Ballista Bolt", "Bone Ballista Bolt"] },


            new() { Names = ["base game"], Targets = ["Base Game"] },
            new()
            {
                Names = ["dlc"], Targets =
                [
                    "DLC", "Spectral Steed Regalia: Tree Sentinel",
                    "Spectral Steed Regalia: Carian Silver", "Spectral Steed Regalia: Funeral Night",
                ]
            },

            new()
            {
                Names = ["horse", "torrent"], Targets =
                [
                    "Spectral Steed Whistle", "Spectral Steed Regalia: Tree Sentinel",
                    "Spectral Steed Regalia: Carian Silver", "Spectral Steed Regalia: Funeral Night"
                ]
            },
            new()
            {
                Names = ["buff", "buffs"], Targets =
                [
                    // Weapon
                    "Sacred Blade", "Chilling Mist", "Poisonous Mist", "Lightning Slash", "Barbaric Roar",
                    "Determination",
                    "Royal Knight's Resolve", "Seppuku", "Flaming Strike", "War Cry", "Cragblade", "Sacred Order",
                    "Braggart's Roar",
                    "Freezing Grease", "Drawstring Freezing Grease", "Soporific Grease", "Drawstring Soporific Grease",
                    "Fire Grease",
                    "Drawstring Fire Grease", "Lightning Grease", "Drawstring Lightning Grease", "Magic Grease",
                    "Drawstring Magic Grease",
                    "Poison Grease", "Drawstring Poison Grease", "Holy Grease", "Drawstring Holy Grease",
                    "Blood Grease",
                    "Drawstring Blood Grease", "Dragonwound Grease", "Rot Grease", "Drawstring Rot Grease",
                    "Eternal Sleep Grease",
                    "Drawstring Eternal Sleep Grease", "Shield Grease", "Scholar's Armament", "Frozen Armament",
                    "Unseen Blade",
                    "Black Flame Blade", "Bloodflame Blade", "Order's Blade", "Electrify Armament", "Vyke's Dragonbolt",
                    "Poison Armament", "Flame Skewer", "Shriek of Sorrow", "Flame Spear", "Messmerfire Grease",
                    "Drawstring Messmerfire Grease", "Dragonbolt Grease", "Drawstring Dragonbolt Grease",
                    "Royal Magic Grease",
                    "Drawstring Royal Magic Grease", "Golden Grease", "Drawstring Golden Grease",
                    "Dragon Communion Grease",
                    "Festive Grease",

                    // Body
                    "Spellproof Dried Liver", "Fireproof Dried Liver", "Lightningproof Dried Liver",
                    "Holyproof Dried Liver",
                    "Exalted Flesh", "Immunizing Cured Meat", "Invigorating Cured Meat", "Clarifying Cured Meat",
                    "Dappled Cured Meat",
                    "Cleanrot Knight Finlay", "Boiled Crab", "Boiled Prawn", "Immunizing White Cured Meat",
                    "Invigorating White Cured Meat",
                    "Clarifying White Cured Meat", "Dappled White Cured Meat", "Baldachin's Blessing",
                    "Radiant Baldachin's Blessing",
                    "Ironjar Aromatic", "Bloodboil Aromatic", "Flame, Grant Me Strength", "Flame, Protect Me",
                    "Black Flame's Protection",
                    "Barrier of Gold", "Protection of the Erdtree", "Flame Fortification", "Magic Fortification",
                    "Lightning Fortification", "Divine Fortification", "Lord's Divine Fortification",
                    "Vyke's Dragonbolt",
                    "Dragonbolt Blessing", "Howl of Shabriri", "Fire's Deadly Sin", "Golden Lightning Fortification",
                    "Dragon Communion Flesh", "Dragonscale Flesh", "Fingerprint Nostrum", "Spellproof Pickled Liver",
                    "Fireproof Pickled Liver", "Lightningproof Pickled Liver", "Holyproof Pickled Liver",
                    "Opaline Pickled Liver",
                    "Scorpion Stew", "Gourmet Scorpion Stew", "Sacred Bloody Flesh", "Electrocharge", "Sword of Light",

                    // Aura
                    "Golden Vow", "Golden Great Arrow", "Commander's Standard", "Golden Epitaph", "Shared Order",
                    "Stormhawk Deenh",
                    "Perfumer Tricia", "Uplifting Aromatic", "Ancient Dragon's Blessing", "Dragonbolt of Florissax",

                    // HP Regen
                    "Cleanrot Knight Finlay", "Crimsonburst Crystal Tear", "Blessing's Boon", "Blessing of the Erdtree",
                    "Bestial Vitality", "Scorpion Stew", "Gourmet Scorpion Stew",

                    // Variant Buffs
                    "Rock Heart ", "Priestess Heart", "Lamenter's Mask", "Soft Cotton", "Assassin's Approach",
                    "Pickled Turtle Neck", "Well-Pickled Turtle Neck", "Gold-Pickled Fowl Foot", "Golden Horn Tender",
                    "Silver-Pickled Fowl Foot", "Silver Horn Tender",

                    // Unique
                    "Jellyfish Shield", "Holy Ground", "Endure", "Assassin's Gambit", "Grafted Blade Greatsword",
                    "Starlight Shard",
                    "Terra Magica", "Unseen Form", "Lamenting Visage", "Lulling Branch", "Sacred Bloody Flesh",
                    "Mantle of Thorns",
                ]
            },

            new()
            {
                Names = ["weapon buff", "weapon buffs"], Targets =
                [
                    "Sacred Blade", "Chilling Mist", "Poisonous Mist", "Lightning Slash", "Barbaric Roar","Milos Roar",
                    "Determination",
                    "Royal Knight's Resolve", "Seppuku", "Flaming Strike", "War Cry", "Cragblade", "Sacred Order",
                    "Braggart's Roar",
                    "Freezing Grease", "Drawstring Freezing Grease", "Soporific Grease", "Drawstring Soporific Grease",
                    "Fire Grease",
                    "Drawstring Fire Grease", "Lightning Grease", "Drawstring Lightning Grease", "Magic Grease",
                    "Drawstring Magic Grease",
                    "Poison Grease", "Drawstring Poison Grease", "Holy Grease", "Drawstring Holy Grease",
                    "Blood Grease",
                    "Drawstring Blood Grease", "Dragonwound Grease", "Rot Grease", "Drawstring Rot Grease",
                    "Eternal Sleep Grease",
                    "Drawstring Eternal Sleep Grease", "Shield Grease", "Scholar's Armament", "Frozen Armament",
                    "Unseen Blade",
                    "Black Flame Blade", "Bloodflame Blade", "Order's Blade", "Electrify Armament", "Vyke's Dragonbolt",
                    "Poison Armament", "Flame Skewer", "Shriek of Sorrow", "Flame Spear", "Messmerfire Grease",
                    "Drawstring Messmerfire Grease", "Dragonbolt Grease", "Drawstring Dragonbolt Grease",
                    "Royal Magic Grease",
                    "Drawstring Royal Magic Grease", "Golden Grease", "Drawstring Golden Grease",
                    "Dragon Communion Grease",
                    "Festive Grease"
                ]
            },

            new()
            {
                Names = ["aura buff", "aura buffs"], Targets =
                [
                    "Golden Vow", "Golden Great Arrow", "Commander's Standard", "Golden Epitaph", "Shared Order",
                    "Stormhawk Deenh",
                    "Perfumer Tricia", "Uplifting Aromatic", "Ancient Dragon's Blessing", "Dragonbolt of Florissax",
                ]
            },

            new()
            {
                Names = ["body buff", "body buffs"], Targets =
                [
                    "Spellproof Dried Liver", "Fireproof Dried Liver", "Lightningproof Dried Liver",
                    "Holyproof Dried Liver",
                    "Exalted Flesh", "Immunizing Cured Meat", "Invigorating Cured Meat", "Clarifying Cured Meat",
                    "Dappled Cured Meat",
                    "Cleanrot Knight Finlay", "Boiled Crab", "Boiled Prawn", "Immunizing White Cured Meat",
                    "Invigorating White Cured Meat",
                    "Clarifying White Cured Meat", "Dappled White Cured Meat", "Baldachin's Blessing",
                    "Radiant Baldachin's Blessing",
                    "Ironjar Aromatic", "Bloodboil Aromatic", "Flame, Grant Me Strength", "Flame, Protect Me",
                    "Black Flame's Protection",
                    "Barrier of Gold", "Protection of the Erdtree", "Flame Fortification", "Magic Fortification",
                    "Lightning Fortification", "Divine Fortification", "Lord's Divine Fortification",
                    "Vyke's Dragonbolt",
                    "Dragonbolt Blessing", "Howl of Shabriri", "Fire's Deadly Sin", "Golden Lightning Fortification",
                    "Dragon Communion Flesh", "Dragonscale Flesh", "Fingerprint Nostrum", "Spellproof Pickled Liver",
                    "Fireproof Pickled Liver", "Lightningproof Pickled Liver", "Holyproof Pickled Liver",
                    "Opaline Pickled Liver",
                    "Scorpion Stew", "Gourmet Scorpion Stew", "Sacred Bloody Flesh", "Electrocharge", "Sword of Light",
                ]
            },

            new()
            {
                Names = ["unique buff", "unique buffs"], Targets =
                [
                    "Jellyfish Shield", "Holy Ground", "Endure", "Assassin's Gambit", "Grafted Blade Greatsword",
                    "Starlight Shard",
                    "Frenzyflame Stone", "Terra Magica", "Unseen Form", "Lamenting Visage", "Lulling Branch",
                    "Sacred Bloody Flesh",
                    "Mantle of Thorns", "Minor Erdtree",
                ]
            },

            new()
            {
                Names = ["stealth"], Targets =
                [
                    "Assassin's Gambit", "Unseen Form", "Lamenting Visage", "Soft Cotton", "Assassin's Approach",
                    "Black Knife Armor", "Concealing Veil", "Crepus's Vial"
                ]
            },

            // adding just for the funny
            new()
            {
                Names = ["lank"], Targets =
                [
                    "Assassin's Gambit", "Howl of Shabriri", "Golden Vow", "reduvia"
                ]
            },

            new()
            {
                Names = ["hp regen", "health regen", "health restoration", "hp restoration"], Targets =
                [
                    "Cleanrot Knight Finlay", "Crimsonburst Crystal Tear", "Blessing's Boon", "Blessing of the Erdtree",
                    "Bestial Vitality", "Scorpion Stew", "Gourmet Scorpion Stew", "Warming Stone", "Sunwarmth Stone",
                    "Blessed Dew Talisman", "Assassin's Crimson Dagger", "Crimson Seed Talisman", "Taker's Cameo",
                    "Blasphemous Blade", "Serpent-God's Curved Sword", "Greathorn Hammer",
                    "Rykard's Great Rune (Deactivated)", "Rykard's Great Rune (Activated)", "Royal Remains",
                    "Icon Shield", "Godskin Swaddling Cloth", "Malenia's Great Rune (Deactivated)",
                    "Malenia's Great Rune (Activated)",
                    "Butchering Knife", "Great Stars", "Devourer's Scepter", "Blood Tax", "Lifesteal Fist",
                    "Prayful Strike", "Holy Ground"
                ]
            },

            new()
            {
                Names = ["raya", "academy", "raya lucaria", "academy of raya lucaria"], Targets =
                    ["Main Academy Gate", "South Raya Lucaria Gate"]
            },

            // Talis abbrevs
            new() { Names = ["bgt"], Targets = ["bullgoat talisman"] },
            new() { Names = ["bom"], Targets = ["blade of mercy"] },
            new() { Names = ["dcgs"], Targets = ["dragoncrest greatshield"] },
            new() { Names = ["jarsenal"], Targets = ["great jar's arsenal"] },
            new() { Names = ["kore"], Targets = ["kindred of rot’s exultation"] },
            new() { Names = ["lobe"], Targets = ["lord of blood’s exultation"] },
            new() { Names = ["mp"], Targets = ["millicent’s prosthesis"] },
            new() { Names = ["soa"], Targets = ["shard of alexander"] },
            new() { Names = ["rwsi"], Targets = ["rotten winged sword insignia"] },
            new() { Names = ["2hst"], Targets = ["two-handed sword talisman"] },
            new() { Names = ["bdc"], Targets = ["blue dancer charm"] },

            // Spells/AoWs abbrevs
            new()
            {
                Names = ["aotc"], Targets =
                [
                    "aspects of the crucible: wings", "aspects of the crucible: bloom",
                    "aspects of the crucible: thorns",
                    "aspects of the crucible: breath", "aspects of the crucible: horns", "aspects of the crucible: tail"
                ]
            },
            new() { Names = ["bfb"], Targets = ["black flame blade", "bloodflame blade"] },
            new() { Names = ["bfp"], Targets = ["black flame's protection"] },
            new() { Names = ["bft"], Targets = ["black flame tornado"] },
            new() { Names = ["bote"], Targets = ["blessing of the erdtree"] },
            new() { Names = ["fgms"], Targets = ["flame, grant me strength"] },
            new() { Names = ["gv"], Targets = ["golden vow"] },
            new() { Names = ["kls"], Targets = ["knight's lightning spear"] },
            new() { Names = ["offsac"], Targets = ["offhand sacred order"] },
            new() { Names = ["rkr"], Targets = ["royal knight's resolve"] },
            new() { Names = ["sgt"], Targets = ["spinning gravity thrust"] },
            new() { Names = ["tpfbt", "tpfbx2"], Targets = ["the poison flower blooms twice"] },
            new() { Names = ["rfbs", "rtsr"], Targets = ["red-feathered branchsword"] },

            
            // Unique weapon arts to weapons, and some common ways to refer to them, because names are hard

            new() { Names = ["gof", "Causality's Wrath", "Golden Order Flail"], Targets = ["Golden Order Flail"] },
            new() { Names = ["Red Bear Hunt", "Red Bear's Claw"], Targets = ["Red Bear's Claw"] },
            new()
            {
                Names = ["Dragonwound Slash", "Dragon Hunter's Great Katana"],
                Targets = ["Dragon Hunter's Great Katana"]
            },
            new() { Names = ["Needle Piercer", "Leda's Sword"], Targets = ["Leda's Sword"] },
            new() { Names = ["Light", "Sword of Light"], Targets = ["Sword of Light"] },
            new() { Names = ["Darkness", "Sword of Darkness"], Targets = ["Sword of Darkness"] },
            new() { Names = ["Onze's Line of Stars", "Star-Lined Sword"], Targets = ["Star-Lined Sword"] },
            new() { Names = ["Guillotine", "Spinning Guillotine", "Putrescence Cleaver"], Targets = ["Putrescence Cleaver"] },
            new() { Names = ["Unending Dance", "Dancing Blade of Ranah"], Targets = ["Dancing Blade of Ranah"] },
            new() { Names = ["Revenger's Blade", "Falx"], Targets = ["Falx"] },
            new()
            {
                Names = ["Mists of Eternal Sleep", "Velvet Sword of St Trina"], Targets = ["Velvet Sword of St Trina"]
            },
            new() { Names = ["Dynastic Sickleplay", "Obsidian Lamina"], Targets = ["Obsidian Lamina"] },
            new()
            {
                Names = ["Blinkbolt: Twinaxe", "Death Knight's Twin Axes"], Targets = ["Death Knight's Twin Axes"]
            },
            new()
            {
                Names = ["Blinkbolt: Long-hafted Axe", "Death Knight's Longhaft Axe"],
                Targets = ["Death Knight's Longhaft Axe"]
            },
            new()
            {
                Names = ["Promised Consort", "Greatsword of Radahn (Lord)"], Targets = ["Greatsword of Radahn (Lord)"]
            },
            new()
            {
                Names = ["Shadow Sunflower Headbutt", "Shadow Sunflower Blossom"],
                Targets = ["Shadow Sunflower Blossom"]
            },
            new() { Names = ["Moon-and-Fire Stance", "Rellana's Twin Blades"], Targets = ["Rellana's Twin Blades"] },
            new() { Names = ["Devonia's Vortex", "Devonia's Hammer"], Targets = ["Devonia's Hammer"] },
            new() { Names = ["Messmer's Assault", "Spear of the Impaler"], Targets = ["Spear of the Impaler"] },
            new() { Names = ["Sleep Evermore", "Thiollier's Hidden Needle"], Targets = ["Thiollier's Hidden Needle"] },
            new() { Names = ["Golden Crux", "Greatsword of Damnation"], Targets = ["Greatsword of Damnation"] },
            new() { Names = ["Moore's Charge", "Verdigris Greatshield"], Targets = ["Verdigris Greatshield"] },
            new()
            {
                Names = ["White Light Charge", "Ancient Meteoric Ore Greatsword"],
                Targets = ["Ancient Meteoric Ore Greatsword"]
            },
            new()
            {
                Names = ["Rancor Slash", "Spirit Glaive", "Spirit Sword"], Targets = ["Spirit Glaive", "Spirit Sword"]
            },
            new() { Names = ["Witching Hour Slash", "Sword of Night"], Targets = ["Sword of Night"] },
            new() { Names = ["Euporia Vortex", "Euporia"], Targets = ["Euporia"] },
            new() { Names = ["Smithing Art Spears", "Anvil Hammer"], Targets = ["Anvil Hammer"] },
            new() { Names = ["Romina's Purification", "Poleblade of the Bud"], Targets = ["Poleblade of the Bud"] },
            new() { Names = ["Poison Spear Hand Strike", "Poisoned Hand"], Targets = ["Poisoned Hand"] },
            new() { Names = ["Madding Spear Hand Strike", "Madding Hand"], Targets = ["Madding Hand"] },
            new() { Names = ["Feeble Lord's Frenzied Flame", "Nanaya's Torch"], Targets = ["Nanaya's Torch"] },
            new() { Names = ["Repeating Fire", "Repeating Crossbow"], Targets = ["Repeating Crossbow"] },
            new() { Names = ["Deadly Dance", "Curseblade's Cirque"], Targets = ["Curseblade's Cirque"] },
            new() { Names = ["Fan Shot", "Ansbach's Longbow"], Targets = ["Ansbach's Longbow"] },
            new() { Names = ["Discus Hurl", "Smithscript Shield"], Targets = ["Smithscript Shield"] },
            new() { Names = ["Flower Dragonbolt", "Flowerstone Gavel"], Targets = ["Flowerstone Gavel"] },
            new() { Names = ["Kowtower's Resentment", "Gazing Finger"], Targets = ["Gazing Finger"] },
            new() { Names = ["Painful Strike", "Tooth Whip"], Targets = ["Tooth Whip"] },
            new() { Names = ["Solitary Moon Slash", "Greatsword of Solitude"], Targets = ["Greatsword of Solitude"] },
            new() { Names = ["Revenge of the Night", "Shield of Night"], Targets = ["Shield of Night"] },
            new() { Names = ["Weed Cutter", "Rakshasa's Great Katana"], Targets = ["Rakshasa's Great Katana"] },
            new() { Names = ["Blindfold of Happiness", "Lamenting Visage"], Targets = ["Lamenting Visage"] },
            new() { Names = ["Jori's Inquisition", "Barbed Staff-Spear"], Targets = ["Barbed Staff-Spear"] },
            new() { Names = ["Roaring Bash", "Golden Lion Shield"], Targets = ["Golden Lion Shield"] },
            new()
            {
                Names = ["Deadly Poison Spray", "Deadly Poison Perfume Bottle"],
                Targets = ["Deadly Poison Perfume Bottle"]
            },
            new() { Names = ["Flare, O Serpent", "Serpent Flail"], Targets = ["Serpent Flail"] },
            new() { Names = ["Scattershot", "Claws of Night"], Targets = ["Claws of Night"] },
            new() { Names = ["Hone Blade", "Bonny Butchering Knife"], Targets = ["Bonny Butchering Knife"] },
            new() { Names = ["Horn Calling", "Horned Warrior's Sword"], Targets = ["Horned Warrior's Sword"] },
            new()
            {
                Names = ["Horn Calling: Storm", "Horned Warrior's Greatsword"],
                Targets = ["Horned Warrior's Greatsword"]
            },
            new()
            {
                Names = ["Phalanx", "Tremendous Phalanx", "Moonrithyll's Knights Sword"], Targets = ["Moonrithyll's Knights Sword"]
            },
            new()
            {
                Names = ["Bloodboon", "Bloodfiends' Bloodboon", "Bloodfiend's Sacred Spear"], Targets = ["Bloodfiend's Sacred Spear"]
            },
            new() { Names = ["Dragonform Flame", "Forked-Tongue Hatchet"], Targets = ["Forked-Tongue Hatchet"] },
            new()
            {
                Names = ["Lightspeed Slash", "Greatsword of Radahn (Light)"], Targets = ["Greatsword of Radahn (Light)"]
            },
            new() { Names = ["Rancor Shot", "Bone Bow"], Targets = ["Bone Bow"] },
            new() { Names = ["Torch Attack", "Torches"], Targets = ["Torches"] },
            new() { Names = ["Firebreather", "Steel-Wire Torch"], Targets = ["Steel-Wire Torch"] },
            new() { Names = ["Buckler Parry", "Buckler"], Targets = ["Buckler"] },
            new()
            {
                Names = ["Surge of Faith", "Cranial Vessel Candlestand"], Targets = ["Cranial Vessel Candlestand"]
            },
            new() { Names = ["Flame Spit", "One-Eyed Shield"], Targets = ["One-Eyed Shield"] },
            new() { Names = ["Tongues of Fire", "Visage Shield"], Targets = ["Visage Shield"] },
            new() { Names = ["Oracular Bubble", "Envoy's Horn"], Targets = ["Envoy's Horn"] },
            new() { Names = ["Bubble Shower", "Envoy's Long Horn"], Targets = ["Envoy's Long Horn"] },
            new() { Names = ["Great Oracular Bubble", "Envoy's Greathorn"], Targets = ["Envoy's Greathorn"] },
            new() { Names = ["Sea of Magma", "Magma Whip Candlestick"], Targets = ["Magma Whip Candlestick"] },
            new() { Names = ["Viper Bite", "Coil Shield"], Targets = ["Coil Shield"] },
            new() { Names = ["Moonlight Greatsword", "Dark Moon Greatsword"], Targets = ["Dark Moon Greatsword"] },
            new() { Names = ["Siluria's Woe", "Siluria's Tree"], Targets = ["Siluria's Tree"] },
            new() { Names = ["Rallying Standard", "Commander's Standard"], Targets = ["Commander's Standard"] },
            new() { Names = ["Bear Witness!", "Grafted Dragon"], Targets = ["Grafted Dragon"] },
            new()
            {
                Names = ["Eochaid's Dancing Blade", "Marais Executioner's Sword", "Regalia of Eochaid"],
                Targets = ["Marais Executioner's Sword", "Regalia of Eochaid"]
            },
            new() { Names = ["Soul Stifler", "Winged Greathorn"], Targets = ["Winged Greathorn"] },
            new() { Names = ["Taker's Flames", "Blasphemous Blade"], Targets = ["Blasphemous Blade"] },
            new() { Names = ["Shriek of Milos", "Sword of Milos"], Targets = ["Sword of Milos"] },
            new() { Names = ["Reduvia Blood Blade", "Reduvia"], Targets = ["Reduvia"] },
            new() { Names = ["Glintstone Dart", "Glintstone Kris"], Targets = ["Glintstone Kris"] },
            new()
            {
                Names = ["Night-and-Flame Stance", "Sword of Night and Flame"], Targets = ["Sword of Night and Flame"]
            },
            new() { Names = ["Wave of Gold", "Sacred Relic Sword"], Targets = ["Sacred Relic Sword"] },
            new() { Names = ["Ruinous Ghostflame", "Helphen's Steeple"], Targets = ["Helphen's Steeple"] },
            new() { Names = ["Establish Order", "Golden Order Greatsword"], Targets = ["Golden Order Greatsword"] },
            new() { Names = ["Mists of Slumber", "Sword of St Trina"], Targets = ["Sword of St Trina"] },
            new() { Names = ["Spearcall Ritual", "Death Ritual Spear"], Targets = ["Death Ritual Spear"] },
            new() { Names = ["Wolf's Assault", "Royal Greatsword"], Targets = ["Royal Greatsword"] },
            new() { Names = ["Thundercloud Form", "Dragon King's Cragblade"], Targets = ["Dragon King's Cragblade"] },
            new() { Names = ["Cursed-Blood Slice", "Morgott's Cursed Sword"], Targets = ["Morgott's Cursed Sword"] },
            new() { Names = ["Waterfowl Dance", "Hand of Malenia"], Targets = ["Hand of Malenia"] },
            new() { Names = ["Gold Breaker", "Marika's Hammer"], Targets = ["Marika's Hammer"] },
            new() { Names = ["I Command Thee, Kneel!", "Axe of Godrick"], Targets = ["Axe of Godrick"] },
            new() { Names = ["Regal Roar", "Axe of Godfrey"], Targets = ["Axe of Godfrey"] },
            new() { Names = ["Starcaller Cry", "Starscourge Greatsword"], Targets = ["Starscourge Greatsword"] },
            new() { Names = ["Wave of Destruction", "Ruins Greatsword"], Targets = ["Ruins Greatsword"] },
            new() { Names = ["Bloodboon Ritual", "Mohgwyn's Sacred Spear"], Targets = ["Mohgwyn's Sacred Spear"] },
            new() { Names = ["Flowing Form", "Nox Flowing Sword"], Targets = ["Nox Flowing Sword"] },
            new() { Names = ["Flowing Form", "Nox Flowing Hammer"], Targets = ["Nox Flowing Hammer"] },
            new() { Names = ["Blade of Death", "Black Knife"], Targets = ["Black Knife"] },
            new() { Names = ["Blade of Gold", "Blade of Calling"], Targets = ["Blade of Calling"] },
            new() { Names = ["Destined Death", "Maliketh's Black Blade"], Targets = ["Maliketh's Black Blade"] },
            new() { Names = ["Pizza", "Spinning Wheel", "Ghiza's Wheel"], Targets = ["Ghiza's Wheel"] },
            new() { Names = ["Alabaster Lord's Pull", "Alabaster Lord's Sword"], Targets = ["Alabaster Lord's Sword"] },
            new() { Names = ["Onyx Lord's Repulsion", "Onyx Lord's Greatsword"], Targets = ["Onyx Lord's Greatsword"] },
            new() { Names = ["Oath of Vengeance", "Grafted Blade Greatsword"], Targets = ["Grafted Blade Greatsword"] },
            new() { Names = ["Ice Lightning Sword", "Dragonscale Blade"], Targets = ["Dragonscale Blade"] },
            new() { Names = ["Regal Beastclaw", "Beastclaw Greathammer"], Targets = ["Beastclaw Greathammer"] },
            new() { Names = ["Flame Dance", "Giant's Red Braid"], Targets = ["Giant's Red Braid"] },
            new() { Names = ["Claw Flick", "Ringed Finger"], Targets = ["Ringed Finger"] },
            new()
            {
                Names = ["Nebula", "Bastard's Stars", "Wing of Astel"], Targets = ["Bastard's Stars", "Wing of Astel"]
            },
            new() { Names = ["Ghostflame Ignition", "Death's Poker"], Targets = ["Death's Poker"] },
            new() { Names = ["Ancient Lightning Spear", "Bolt of Gransax"], Targets = ["Bolt of Gransax"] },
            new() { Names = ["Frenzyflame Thrust", "Vyke's War Spear"], Targets = ["Vyke's War Spear"] },
            new() { Names = ["Ring of light", "Miquella's Ring of Light", "Halo Scythe"], Targets = ["Halo Scythe"] },
            new()
            {
                Names = ["Golden Tempering", "Ornamental Straight Sword"], Targets = ["Ornamental Straight Sword"]
            },
            new() { Names = ["Last Rites", "Golden Epitaph"], Targets = ["Golden Epitaph"] },
            new()
            {
                Names = ["Unblockable Blade", "Cipher Pata", "Coded Sword"], Targets = ["Cipher Pata", "Coded Sword"]
            },
            new() { Names = ["Corpse Wax Cutter", "Gargoyle's Blackblade"], Targets = ["Gargoyle's Blackblade"] },
            new() { Names = ["Zamor Ice Storm", "Zamor Curved Sword"], Targets = ["Zamor Curved Sword"] },
            new() { Names = ["Radahn's Rain", "Lion Greatbow"], Targets = ["Lion Greatbow"] },
            new() { Names = ["Black Flame", "The Queen's Black Flame", "Godslayer Greatsword"], Targets = ["Godslayer Greatsword"] },
            new() { Names = ["Dynast's Finesse", "Bloody Helice"], Targets = ["Bloody Helice"] },
            new() { Names = ["Magma Shower", "Magma Blade"], Targets = ["Magma Blade"] },
            new() { Names = ["Death Flare", "Eclipse Shotel"], Targets = ["Eclipse Shotel"] },
            new() { Names = ["Bloodhound's Finesse", "Bloodhound's Fang"], Targets = ["Bloodhound's Fang"] },
            new() { Names = ["Magma Guillotine", "Magma Wyrm's Scalesword"], Targets = ["Magma Wyrm's Scalesword"] },
            new() { Names = ["Corpse Piler", "Rivers of Blood"], Targets = ["Rivers of Blood"] },
            new() { Names = ["Transient Moonlight", "Moonveil"], Targets = ["Moonveil"] },
            new() { Names = ["Bloodblade Dance", "Eleonora's Poleblade"], Targets = ["Eleonora's Poleblade"] },
            new()
            {
                Names = ["Knowledge Above All", "Scepter of the All-Knowing"], Targets = ["Scepter of the All-Knowing"]
            },
            new() { Names = ["Devourer of Worlds", "Devourer's Scepter"], Targets = ["Devourer's Scepter"] },
            new() { Names = ["Familial Rancor", "Family Heads"], Targets = ["Family Heads"] },
            new() { Names = ["Rosus's Summons", "Rosus' Axe"], Targets = ["Rosus' Axe"] },
            new() { Names = ["Thunderstorm", "Stormhawk Axe"], Targets = ["Stormhawk Axe"] },
            new() { Names = ["Sacred Phalanx", "Cleanrot Spear"], Targets = ["Cleanrot Spear"] },
            new() { Names = ["Great-Serpent Hunt", "Serpent-Hunter"], Targets = ["Serpent-Hunter"] },
            new() { Names = ["Angel's Wings", "Winged Scythe"], Targets = ["Winged Scythe"] },
            new() { Names = ["Storm Kick", "Veteran's Prosthesis"], Targets = ["Veteran's Prosthesis"] },
            new() { Names = ["Sorcery of the Crozier", "Watchdog's Staff"], Targets = ["Watchdog's Staff"] },
            new()
            {
                Names = ["Erdtree Slam", "Staff of the Avatar", "Rotten Staff"],
                Targets = ["Staff of the Avatar", "Rotten Staff"]
            },
            new() { Names = ["Gravity Bolt", "Fallingstar Beast Jaw"], Targets = ["Fallingstar Beast Jaw"] },
            new() { Names = ["Fires of Slumber", "St Trina's Torch"], Targets = ["St Trina's Torch"] },
            new() { Names = ["Golden Retaliation", "Erdtree Greatshield"], Targets = ["Erdtree Greatshield"] },
            new() { Names = ["Contagious Fury", "Jellyfish Shield"], Targets = ["Jellyfish Shield"] },
            new() { Names = ["Ordovis's Vortex", "Ordovis's Greatsword"], Targets = ["Ordovis's Greatsword"] },
            
            // More Weapons abbrevs
            new() { Names = ["als"], Targets = ["alabaster lord's sword"] },
            new() { Names = ["msa"], Targets = ["messmer soldier's axe"] },
            new() { Names = ["amogs"], Targets = ["ancient meteoric ore greatsword"] },
            new() { Names = ["bb"], Targets = ["blasphemous blade"] },
            new() { Names = ["bfa"], Targets = ["bloodfiend's arm"] },
            new() { Names = ["bfss"], Targets = ["bloodfiend's sacred spear"] },
            new() { Names = ["bhb"], Targets = ["backhand blade"] },
            new() { Names = ["bhc"], Targets = ["bloodhound claws"] },
            new() { Names = ["bhf"], Targets = ["bloodhound’s fang"] },
            new() { Names = ["bog"], Targets = ["bolt of gransax"] },
            new() { Names = ["con"], Targets = ["claws of night"] },
            new() { Names = ["cts"], Targets = ["carian thrusting shield "] },
            new() { Names = ["dkla"], Targets = ["death knight's longhaft axe"] },
            new() { Names = ["dkt"], Targets = ["death knight's twin axes"] },
            new() { Names = ["dmgs"], Targets = ["dark moon greatsword"] },
            new() { Names = ["gkat"], Targets = ["great katana"] },
            new() { Names = ["gsos"], Targets = ["greatsword of solitude"] },
            new() { Names = ["gsgs"], Targets = ["godslayer's greatsword"] },
            new() { Names = ["gss"], Targets = ["guardian's swordspear"] },
            new() { Names = ["gugs"], Targets = ["greatsword"] },
            new() { Names = ["fkgs"], Targets = ["fire knight's greatsword"] },
            new() { Names = ["fkss"], Targets = ["fire knight's shortsword"] },
            new() { Names = ["hom"], Targets = ["hand of malenia"] },
            new() { Names = ["rob"], Targets = ["rivers of blood"] },
            new() { Names = ["sonaf"], Targets = ["sword of night and flame"] },
            new() { Names = ["son"], Targets = ["sword of night"] },
            new() { Names = ["mss"], Targets = ["messmer soldier spear"] },
            new() { Names = ["xnagi"], Targets = ["cross-naginata"] },
        }
        .Concat(WeaponAliases)
        .ToList();

    

    private static readonly List<(string[] Names, string[] NormalizedTargets)> Compiled =
        BuiltIn.Select(e => (e.Names.ToArray(),
                             e.Targets.Select(t => t.NormalizeForAlias()).ToArray()))
               .ToList();
    
    public static HashSet<string> ResolveMatches(string searchText)
    {
        var matches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(searchText)) return matches;

        foreach (var (names, targets) in Compiled)
        {
            if (!names.Any(n => n.ContainsFuzzy(searchText))) continue;
            foreach (var t in targets) matches.Add(t);
        }
        return matches;
    }
}
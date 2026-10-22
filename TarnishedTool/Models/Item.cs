// 

using System.Collections.Generic;

namespace TarnishedTool.Models;

public class Item
{
    public bool IsDlc { get; set; }
    public bool IsTarnishedPack { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int StackSize { get; set; } 
    public int MaxStorage { get; set; }
    public string CategoryName { get; set; }
}
// Stuff for fuzzy search

public static class ItemAliasTags
{
    public static IEnumerable<string> ForItem(Item item)
    {
        yield return item.Name;
        yield return item.CategoryName;
        yield return item.IsDlc ? "DLC" : "Base Game";

        if (item is Weapon weapon)
        {
            yield return weapon.UpgradeType switch
            {
                0 => "Standard",
                1 => "Somber",
                2 => "Special"
            };

            if (WeaponTypeNames.TryGetValue(weapon.WeaponType, out var typeName))
                yield return typeName;
        }
    }


    private static readonly Dictionary<ushort, string> WeaponTypeNames = new()
    {
        { 1, "Daggers" },
        { 3, "Straight Swords" },
        { 5, "Greatswords" },
        { 7, "Colossal Swords" },
        { 9, "Curved Swords" },
        { 11, "Curved Great Swords" },
        { 13, "Katanas" },
        { 14, "Twinblades" },
        { 15, "Thrusting Swords" },
        { 16, "Heavy Thrusting Swords" },
        { 17, "Axes" },
        { 19, "Great Axes" },
        { 21, "Hammers" },
        { 23, "Great Hammers" },
        { 24, "Flails" },
        { 25, "Spears" },
        { 28, "Great Spears" },
        { 29, "Halberds" },
        { 31, "Reapers" },
        { 35, "Fists" },
        { 37, "Claws" },
        { 39, "Whips" },
        { 41, "Colossal Weapons" },
        { 50, "Short Bows" },
        { 51, "Bows" },
        { 53, "Great Bows" },
        { 55, "Crossbows"},
        { 56, "Ballistas"},
        { 57, "Staves"},
        { 61, "Seals"},
        { 65, "Small Shields"},
        { 67, "Medium Shields"},
        { 69, "Great Shields"},
        { 87, "Torches"},
        { 88, "Hand-To-Hand Arts"},
        { 89, "Perfume Bottles"},
        { 90, "Thrusting Shields"},
        { 91, "Throwing Blades"},
        { 92, "Backhand Blades"},
        { 93, "Light Greatswords"},
        { 94, "Great Katanas"},
        { 95, "Beast Claws"},
        
    };
}
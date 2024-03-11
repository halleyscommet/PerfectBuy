using System.Collections.Generic;
using System.Linq;

namespace PerfectBuy;

public class ItemDetails
{
    public int Price { get; init; }
    public int Rating { get; init; }
    public int Weight { get; init; }
    public string? Needed { get; init; }
}

public class Item
{
    public string? Name { get; init; }
    public ItemDetails? Details { get; init; }
}

public class ItemChooser
{
    private Dictionary<string, ItemDetails> GetHardCodedValues()
    {
        return new Dictionary<string, ItemDetails>
        {
            {
                "boombox", new ItemDetails
                {
                    Price = 60,
                    Rating = 5,
                    Weight = 16,
                    Needed = "1"
                }
            },
            {
                "extensionLadder", new ItemDetails
                {
                    Price = 60,
                    Rating = 25,
                    Weight = 0,
                    Needed = "1"
                }
            },
            {
                "flashlight", new ItemDetails
                {
                    Price = 15,
                    Rating = 10,
                    Weight = 0,
                    Needed = "Crew Size"
                }
            },
            {
                "jetpack", new ItemDetails
                {
                    Price = 700,
                    Rating = 95,
                    Weight = 52,
                    Needed = "Crew Size / 2"
                }
            },
            {
                "proFlashlight", new ItemDetails
                {
                    Price = 25,
                    Rating = 95,
                    Weight = 5,
                    Needed = "Crew Size"
                }
            },
            {
                "radarBooster", new ItemDetails
                {
                    Price = 60,
                    Rating = 15,
                    Weight = 19,
                    Needed = "1"
                }
            },
            {
                "shovel", new ItemDetails
                {
                    Price = 30,
                    Rating = 60,
                    Weight = 8,
                    Needed = "Crew Size / 2"
                }
            },
            {
                "sprayPaint", new ItemDetails
                {
                    Price = 50,
                    Rating = 70,
                    Weight = 0,
                    Needed = "2"
                }
            },
            {
                "stunGrenade", new ItemDetails
                {
                    Price = 30,
                    Rating = 40,
                    Weight = 5,
                    Needed = "4"
                }
            },
            {
                "tzpInhalent", new ItemDetails
                {
                    Price = 120,
                    Rating = 10,
                    Weight = 0,
                    Needed = "1"
                }
            },
            {
                "walkieTalkie", new ItemDetails
                {
                    Price = 12,
                    Rating = 85,
                    Weight = 0,
                    Needed = "Crew Size"
                }
            },
            {
                "zapGun", new ItemDetails
                {
                    Price = 400,
                    Rating = 5,
                    Weight = 11,
                    Needed = "1"
                }
            },
            {
                "teleporter", new ItemDetails
                {
                    Price = 375,
                    Rating = 100,
                    Weight = 0,
                    Needed = "1"
                }
            },
            {
                "inverseTeleporter", new ItemDetails
                {
                    Price = 425,
                    Rating = 100,
                    Weight = 0,
                    Needed = "1"
                }
            },
            {
                "loudHorn", new ItemDetails
                {
                    Price = 100,
                    Rating = 80,
                    Weight = 0,
                    Needed = "1"
                }
            },
            {
                "signalTranslator", new ItemDetails
                {
                    Price = 255,
                    Rating = 45,
                    Weight = 0,
                    Needed = "1"
                }
            }
        };
    }

    public (List<(string, int)>, int) ChooseItems(Dictionary<string, ItemDetails> values, int crewmates, int money,
        bool flashlights, bool minimizeSpending, bool jetpacks, bool walkie)
    {
        var itemsToBuy = new List<(string, int)>();
        var itemsCount = 0;
        var proFlashlightBought = false;

        var sortedItems = values
            .Select(x => new Item { Name = x.Key, Details = x.Value })
            .OrderByDescending(x => x.Details!.Rating) // No need to parse
            .ThenBy(x => x.Details!.Weight == 0 ? 0 : x.Details.Weight) // Compare with 0 instead of "N/A"
            .ToList();

        var minimizeItems = new List<string>
        {
            "shovel", "extensionLadder", "radarBooster", "boombox", "lockpicker", "zapGun", "stunGrenade",
            "tzpInhalent", "inverseTeleporter"
        };

        var neededCalculation = new Dictionary<string, int>
        {
            { "Crew Size", crewmates },
            { "Crew Size / 2", crewmates > 1 ? crewmates / 2 : 1 }
        };

        foreach (var item in sortedItems)
        {
            if (minimizeItems.Contains(item.Name!) && minimizeSpending)
                continue;
            if (item.Name == "flashlight" && (!flashlights || proFlashlightBought))
                continue;
            if (item.Name == "proFlashlight" && !flashlights)
                continue;
            if (item.Name == "jetpack" && !jetpacks)
                continue;
            if (item.Name == "walkieTalkie" && !walkie)
                continue;
            if (item.Name == "signalTranslator" && walkie)
                continue;

            var needed = neededCalculation.ContainsKey(item.Details!.Needed!)
                ? neededCalculation[item.Details.Needed!]
                : int.Parse(item.Details.Needed!); // Handle conversion here
            var totalPrice = needed * item.Details.Price;

            if (totalPrice <= money)
            {
                itemsToBuy.Add((item.Name, needed)!);
                money -= totalPrice;
                itemsCount += 1;

                if (item.Name == "proFlashlight")
                    proFlashlightBought = true;
            }

            if ((minimizeSpending && money <= 0) || itemsCount >= 10)
                break;
        }

        return (itemsToBuy, money);
    }

    public Dictionary<string, ItemDetails> LoadJson()
    {
        // using (StreamReader r = new StreamReader("values.json"))
        // {
        //     string json = r.ReadToEnd();
        //     return JsonConvert.DeserializeObject<Dictionary<string, ItemDetails>>(json);
        // }
        return GetHardCodedValues();
    }
}
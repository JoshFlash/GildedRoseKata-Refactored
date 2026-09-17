using Xunit;
using System.Collections.Generic;
using GildedRoseKata;

namespace GildedRoseTests;

public class GildedRoseTest
{
    private class DefaultItemData : TheoryData<Item>
    {
        public DefaultItemData() => AddRange(
            new Item { Name = "TestItem", SellIn = 1, Quality = 1 },
            new Item { Name = "Pitchfork", SellIn = 0, Quality = 2 },
            new Item { Name = "A Frayed Knot", SellIn = 0, Quality = 0 },
            new Item { Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20 }
        );
    }
    
    private class AppreciatingItemData : TheoryData<Item>
    {
        public AppreciatingItemData() => AddRange(
            new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 },
            new Item { Name = "Aged Brie", SellIn = -3, Quality = 49 },
            new Item { Name = "Aged Brie", SellIn = 2, Quality = 50 },
            new Item { Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 15, Quality = 20 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 49 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 49 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 0, Quality = 50 }
        );
    }
    
    private class LegendaryItemData : TheoryData<Item>
    {
        public LegendaryItemData() => AddRange(
            new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
            new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80 }
        );
    }
    
    [Theory]
    [ClassData(typeof(DefaultItemData))]
    [ClassData(typeof(AppreciatingItemData))]
    [ClassData(typeof(LegendaryItemData))]
    public void WhenUpdatingQuality_ItemNameIsUnaltered(Item item)
    {
        IList<Item> items = new List<Item> { item };
        GildedRose app = new GildedRose(items);
        app.UpdateQuality();
        Assert.Equal(item.Name, items[0].Name);
    }
    
    [Theory]
    [ClassData(typeof(DefaultItemData))]
    [ClassData(typeof(AppreciatingItemData))]
    [ClassData(typeof(LegendaryItemData))]
    public void WhenUpdatingQuality_ItemSellInIsCorrectlyDecremented(Item item)
    {
        int expectedSellIn = item.Name.Equals("Sulfuras, Hand of Ragnaros") ? item.SellIn : item.SellIn - 1;

        IList<Item> items = new List<Item> { item };
        GildedRose app = new GildedRose(items);
        app.UpdateQuality();
        Assert.Equal(expectedSellIn, items[0].SellIn);
    }
    
    [Theory]
    [ClassData(typeof(DefaultItemData))]
    [ClassData(typeof(AppreciatingItemData))]
    [ClassData(typeof(LegendaryItemData))]
    public void WhenUpdatingQuality_ItemQualityIsCorrectlyDecremented(Item item)
    {
        int expectedQuality = GetExpectedQuality(item);
        bool unconstrainedQuality = IsLegendary(item) && item.Quality >= 0;
        if (!unconstrainedQuality)
        {
            expectedQuality = System.Math.Clamp(expectedQuality, 0, 50);
        }
        
        IList<Item> items = new List<Item> { item };
        GildedRose app = new GildedRose(items);
        app.UpdateQuality();
        Assert.Equal(expectedQuality, items[0].Quality);
    }
    
    private int GetExpectedQuality(Item item)
    {
        if (IsLegendary(item))
        {
            return item.Quality < 0 ? 0 : item.Quality;
        }
        if (IsAppreciating(item))
        {
            switch (item.Name)
            {
                case "Aged Brie":
                    return item.Quality + 1;
                case "Backstage passes to a TAFKAL80ETC concert":
                    return item.SellIn <= 0 ? 0 
                        : item.SellIn > 10 ? item.Quality + 1 
                        : item.SellIn > 5 ? item.Quality + 2 
                        : item.Quality + 3;
                default:
                    return item.Quality + 1;
            }
        }
        
        return item.SellIn > 0 ? item.Quality - 1 : item.Quality - 2;
    }
    
    private static bool IsLegendary(Item item) => item.Name.Equals("Sulfuras, Hand of Ragnaros");
    
    private static bool IsAppreciating(Item item) => 
        item.Name.Equals("Aged Brie") || item.Name.ToLowerInvariant().Contains("backstage pass");

}
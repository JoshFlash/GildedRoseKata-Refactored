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
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 49 }
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
        int expectedSellIn = item.SellIn - 1;
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
        int expectedQuality = item.Quality > 0 ? item.Quality - 1 : 0;
        IList<Item> items = new List<Item> { item };
        GildedRose app = new GildedRose(items);
        app.UpdateQuality();
        Assert.Equal(expectedQuality, items[0].Quality);
    }
}
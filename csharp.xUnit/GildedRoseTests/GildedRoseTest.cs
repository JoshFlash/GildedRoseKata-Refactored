using Xunit;
using System.Collections.Generic;
using GildedRoseKata;

namespace GildedRoseTests;

public class GildedRoseTest
{
    private class DefaultItemData : TheoryData<Item>
    {
        public DefaultItemData() => AddRange(
            new Item { Name = "TestItem", SellIn = 1, Quality = 1}
        );
    }
    
    [Theory]
    [ClassData(typeof(DefaultItemData))]
    public void WhenUpdatingQuality_ItemNameIsUnaltered(Item item)
    {
        IList<Item> items = new List<Item> { item };
        GildedRose app = new GildedRose(items);
        app.UpdateQuality();
        Assert.Equal(item.Name, items[0].Name);
    }
    
    [Theory]
    [ClassData(typeof(DefaultItemData))]
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
    public void WhenUpdatingQuality_ItemQualityIsCorrectlyDecremented(Item item)
    {
        int expectedQuality = item.Quality > 0 ? item.Quality - 1 : 0;
        IList<Item> items = new List<Item> { item };
        GildedRose app = new GildedRose(items);
        app.UpdateQuality();
        Assert.Equal(expectedQuality, items[0].Quality);
    }
}
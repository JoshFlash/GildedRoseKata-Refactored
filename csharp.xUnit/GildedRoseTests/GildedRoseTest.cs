using Xunit;
using System.Collections.Generic;
using GildedRoseKata;

namespace GildedRoseTests;

public class GildedRoseTest
{
    private const string TestItemName = "TestItem";
    
    [Fact]
    public void WhenUpdatingQuality_ItemNameIsUnaltered()
    {
        IList<Item> Items = new List<Item> { new Item { Name = TestItemName, SellIn = 0, Quality = 0 } };
        GildedRose app = new GildedRose(Items);
        app.UpdateQuality();
        Assert.Equal(TestItemName, Items[0].Name);
    }
}
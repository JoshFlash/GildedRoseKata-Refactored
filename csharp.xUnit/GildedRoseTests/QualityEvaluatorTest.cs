using Xunit;
using System;
using System.Collections.Generic;
using GildedRoseKata;

namespace GildedRoseTests;

/// <summary>
/// This class contains exhaustive tests for the Quality Evaluator to ensure correct values are being calculated.
/// The test logic and test cases were generated with the use of AI and reviewed by a human.
/// </summary>
public class QualityEvaluatorTest
{
    private const int MaxQuality = 50;
    private const int MinQuality = 0;
    
    private static int Evaluate(string itemName, int sellIn, int quality)
    {
        var item = new Item { Name = itemName, SellIn = sellIn, Quality = quality };
        var assetTypeMap = new AssetTypeMap(new List<Item> { item });
        return QualityEvaluator.GetExpectedQuality(item, assetTypeMap);
    }
    
    #region Depreciating items
    
    [Theory]
    // Before the sell in passes a depreciating item loses exactly one quality.
    [InlineData(10, 20, 19)]
    [InlineData(5, 7, 6)]
    [InlineData(1, 1, 0)]
    [InlineData(1, 50, 49)]
    [InlineData(int.MaxValue, 30, 29)]
    // On and after the sell in passes a depreciating item loses exactly two quality.
    [InlineData(0, 20, 18)]
    [InlineData(-1, 20, 18)]
    [InlineData(-100, 3, 1)]
    [InlineData(int.MinValue, 3, 1)]
    public void GetExpectedQuality_DepreciatingItem_DegradesByExpectedAmount(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.DefaultVest, sellIn, quality));
    }
    
    [Theory]
    // Quality is never negative, regardless of how far past the sell in passes the item is.
    [InlineData(1, 0, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(-5, 1, 0)]
    [InlineData(-5, 2, 0)]
    public void GetExpectedQuality_DepreciatingItem_NeverDropsBelowMinimumQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.DefaultVest, sellIn, quality));
    }
    
    [Theory]
    [InlineData(5, 51, 50)]
    [InlineData(5, 60, 50)]
    [InlineData(0, 60, 50)]
    [InlineData(5, int.MaxValue, 50)]
    [InlineData(5, -10, 0)]
    [InlineData(0, -10, 0)]
    public void GetExpectedQuality_DepreciatingItem_ClampsOutOfRangeQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.DefaultVest, sellIn, quality));
    }
    
    [Theory]
    [InlineData("TestItem")]
    [InlineData("Pitchfork")]
    [InlineData("A Frayed Knot")]
    [InlineData("Elixir of the Mongoose")]
    [InlineData("")]
    public void GetExpectedQuality_AnyUnclassifiedName_IsTreatedAsDepreciating(string itemName)
    {
        Assert.Equal(9, Evaluate(itemName, 4, 10));
        Assert.Equal(8, Evaluate(itemName, 0, 10));
    }
    
    [Fact]
    public void GetExpectedQuality_NameMissingFromAssetTypeMap_FallsBackToDepreciating()
    {
        var mappedItems = new List<Item> { new Item { Name = ItemNames.AgedBrie, SellIn = 5, Quality = 10 } };
        var assetTypeMap = new AssetTypeMap(mappedItems);
        var unmappedItem = new Item { Name = "Unmapped Trinket", SellIn = 5, Quality = 10 };
        
        Assert.Equal(9, QualityEvaluator.GetExpectedQuality(unmappedItem, assetTypeMap));
    }
    
    #endregion
    
    #region Appreciating items
    
    [Theory]
    // Aged Brie gains one quality before sell-in passes.
    [InlineData(10, 0, 1)]
    [InlineData(2, 0, 1)]
    [InlineData(2, 10, 11)]
    [InlineData(1, 48, 49)]
    [InlineData(0, 10, 12)]
    // Aged Brie gains two quality after sell-in passes.
    [InlineData(-1, 10, 12)]
    [InlineData(-100, 10, 12)]
    [InlineData(int.MinValue, 49, 50)]
    public void GetExpectedQuality_AgedBrie_AppreciatesByExpectedAmout(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.AgedBrie, sellIn, quality));
    }
    
    [Theory]
    // Aged Brie quality never exceeds the maximum and is never negative.
    [InlineData(2, 49, 50)]
    [InlineData(2, 50, 50)]
    [InlineData(-3, 50, 50)]
    [InlineData(2, 51, 50)]
    [InlineData(2, 100, 50)]
    [InlineData(2, -1, 0)]
    [InlineData(2, -20, 0)]
    public void GetExpectedQuality_AgedBrie_ClampsToLegalQualityRange(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.AgedBrie, sellIn, quality));
    }
    
    #endregion
    
    #region Legendary items
    
    [Theory]
    // Sulfuras quality never changes.
    [InlineData(5, 80, 80)]
    [InlineData(0, 80, 80)]
    [InlineData(-1, 80, 80)]
    [InlineData(int.MinValue, 80, 80)]
    [InlineData(int.MaxValue, 80, 80)]
    [InlineData(0, 50, 50)]
    [InlineData(0, 0, 0)]
    [InlineData(0, int.MaxValue, int.MaxValue)]
    public void GetExpectedQuality_Sulfuras_IsStatic(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.Sulfuras, sellIn, quality));
    }
    
    [Theory]
    // A negative legendary quality is still raised to the minimum.
    [InlineData(0, -1, 0)]
    [InlineData(-10, -80, 0)]
    public void GetExpectedQuality_Sulfuras_NeverReportsNegativeQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.Sulfuras, sellIn, quality));
    }
    
    #endregion
    
    #region Limited time only items
    
    [Theory]
    // More than ten days left: one quality gained.
    [InlineData(int.MaxValue, 20, 21)]
    [InlineData(15, 20, 21)]
    [InlineData(12, 20, 21)]
    [InlineData(11, 20, 21)]
    // Ten to six days left: two quality gained.
    [InlineData(10, 20, 22)]
    [InlineData(9, 20, 22)]
    [InlineData(6, 20, 22)]
    // Five to one days left: three quality gained.
    [InlineData(5, 20, 23)]
    [InlineData(3, 20, 23)]
    [InlineData(1, 20, 23)]
    public void GetExpectedQuality_BackstagePasses_AppreciateByTierBeforeTheConcert(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.BackstagePasses, sellIn, quality));
    }
    
    [Theory]
    // Once the concert has happened the passes are worthless.
    [InlineData(0, 50)]
    [InlineData(0, 20)]
    [InlineData(0, 0)]
    [InlineData(-1, 49)]
    [InlineData(-100, 50)]
    [InlineData(int.MinValue, 50)]
    public void GetExpectedQuality_BackstagePasses_AreWorthlessAfterTheConcert(int sellIn, int quality)
    {
        Assert.Equal(0, Evaluate(ItemNames.BackstagePasses, sellIn, quality));
    }
    
    [Theory]
    // Each appreciation tier is still capped at the maximum quality.
    [InlineData(11, 50, 50)]
    [InlineData(11, 49, 50)]
    [InlineData(10, 49, 50)]
    [InlineData(10, 50, 50)]
    [InlineData(5, 48, 50)]
    [InlineData(5, 49, 50)]
    [InlineData(5, 50, 50)]
    [InlineData(5, 100, 50)]
    public void GetExpectedQuality_BackstagePasses_ClampToMaximumQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.BackstagePasses, sellIn, quality));
    }
    
    [Theory]
    [InlineData(11, -5, 0)]
    [InlineData(8, -5, 0)]
    [InlineData(2, -5, 0)]
    [InlineData(2, -3, 0)]
    public void GetExpectedQuality_BackstagePasses_ClampToMinimumQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.BackstagePasses, sellIn, quality));
    }
    
    [Fact]
    public void GetExpectedQuality_BackstagePasses_TierBoundariesAreExact()
    {
        Assert.Equal(Evaluate(ItemNames.BackstagePasses, 11, 20) + 1, Evaluate(ItemNames.BackstagePasses, 10, 20));
        Assert.Equal(Evaluate(ItemNames.BackstagePasses, 6, 20) + 1, Evaluate(ItemNames.BackstagePasses, 5, 20));
        Assert.Equal(0, Evaluate(ItemNames.BackstagePasses, 0, 20));
        Assert.Equal(23, Evaluate(ItemNames.BackstagePasses, 1, 20));
    }
    
    #endregion
    
    #region Conjured items
    
    [Theory]
    // Before the sell in passes a conjured item loses exactly two quality.
    [InlineData(3, 6, 4)]
    [InlineData(10, 20, 18)]
    [InlineData(5, 7, 5)]
    [InlineData(1, 50, 48)]
    [InlineData(1, 2, 0)]
    [InlineData(int.MaxValue, 30, 28)]
    // On and after the sell in passes a conjured item loses exactly four quality.
    [InlineData(0, 20, 16)]
    [InlineData(-1, 20, 16)]
    [InlineData(0, 6, 2)]
    [InlineData(-100, 10, 6)]
    [InlineData(int.MinValue, 10, 6)]
    public void GetExpectedQuality_ConjuredItem_DegradesTwiceAsFastAsANormalItem(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.Conjured, sellIn, quality));
    }
    
    [Theory]
    // Quality is never negative, however little is left to lose.
    [InlineData(5, 0, 0)]
    [InlineData(5, 1, 0)]
    [InlineData(5, 2, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 3, 0)]
    [InlineData(0, 4, 0)]
    [InlineData(-5, 3, 0)]
    public void GetExpectedQuality_ConjuredItem_NeverDropsBelowMinimumQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.Conjured, sellIn, quality));
    }
    
    [Theory]
    // An out of range quality is still pulled back into the legal range.
    [InlineData(5, 51, 49)]
    [InlineData(5, 60, 50)]
    [InlineData(0, 60, 50)]
    [InlineData(5, int.MaxValue, 50)]
    [InlineData(5, -10, 0)]
    [InlineData(0, -10, 0)]
    public void GetExpectedQuality_ConjuredItem_ClampsOutOfRangeQuality(int sellIn, int quality, int expected)
    {
        Assert.Equal(expected, Evaluate(ItemNames.Conjured, sellIn, quality));
    }
    
    [Theory]
    // The degradation is exactly double the equivalent depreciating item, before and after the sell in passes.
    [InlineData(5, 20)]
    [InlineData(1, 40)]
    [InlineData(0, 20)]
    [InlineData(-4, 40)]
    public void GetExpectedQuality_ConjuredItem_LosesExactlyDoubleADepreciatingItem(int sellIn, int quality)
    {
        var depreciatingLoss = quality - Evaluate(ItemNames.DefaultVest, sellIn, quality);
        var conjuredLoss = quality - Evaluate(ItemNames.Conjured, sellIn, quality);
        
        Assert.Equal(depreciatingLoss * 2, conjuredLoss);
    }
    
    [Fact]
    public void GetExpectedQuality_ConjuredItem_SellInBoundaryIsExact()
    {
        Assert.Equal(18, Evaluate(ItemNames.Conjured, 1, 20));
        Assert.Equal(16, Evaluate(ItemNames.Conjured, 0, 20));
        Assert.Equal(16, Evaluate(ItemNames.Conjured, -1, 20));
    }
    
    #endregion
}

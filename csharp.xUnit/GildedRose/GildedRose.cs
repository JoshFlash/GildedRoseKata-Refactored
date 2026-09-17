using System.Collections.Generic;

namespace GildedRoseKata;

public class GildedRose
{
    IList<Item> Items;
    AssetTypeMap AssetTypeMap;

    public GildedRose(IList<Item> items)
    {
        Items = items;
        AssetTypeMap = new AssetTypeMap(items);
    }
    
    public void UpdateQuality()
    {
        foreach (var item in Items)
        {
            item.Quality = QualityEvaluator.GetExpectedQuality(item, AssetTypeMap);
            UpdateSellIn(item);
        }
    }

    private void UpdateSellIn(Item item)
    {
        if (AssetTypeMap[item.Name].Equals(AssetType.Legendary))
            return;
        
        item.SellIn--;
    }
}
using System.Collections.Generic;

namespace GildedRoseKata;

public enum AssetType
{
    Depreciating = 0,
    Appreciating,
    Legendary,
    LimitedTimeOnly,
    Conjured
}

public class AssetTypeMap
{
    private readonly Dictionary<string, AssetType> _assetTypeByItemName;
    
    public AssetType this[string itemName]
    {
        get => _assetTypeByItemName.GetValueOrDefault(itemName, AssetType.Depreciating);
        set => _assetTypeByItemName[itemName] = value;
    }
    
    private AssetType AssignAssetType(Item item)
    {
        return item.Name switch
        {
            ItemNames.Sulfuras => AssetType.Legendary,
            ItemNames.AgedBrie => AssetType.Appreciating,
            ItemNames.BackstagePasses => AssetType.LimitedTimeOnly,
            ItemNames.Conjured => AssetType.Conjured,
            _ => AssetType.Depreciating
        };
    }
    
    public AssetTypeMap(IList<Item> items)
    {
        // TODO Next: Load asset type data for each item from a catalogue data-source instead
        _assetTypeByItemName = new Dictionary<string, AssetType>();
        foreach (var item in items)
        {
            _assetTypeByItemName.TryAdd(item.Name, AssignAssetType(item));
        }
    }
}
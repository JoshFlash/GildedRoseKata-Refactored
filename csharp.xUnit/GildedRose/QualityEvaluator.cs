using System;

namespace GildedRoseKata;

public static class QualityEvaluator
{
    private const int MaxQuality = 50;
    private const int MinQuality = 0;
    
    public delegate int QualityEvaluation(int sellIn, int quality);
    
    public static int GetExpectedQuality(Item item, AssetTypeMap assetTypeMap)
    {
        var EvaluationMethod = GetEvaluationFromAssetType(assetTypeMap[item.Name]);
        return EvaluationMethod(item.SellIn, item.Quality);
    }

    private static QualityEvaluation GetEvaluationFromAssetType(AssetType assetType)
    {
        return assetType switch
        {
            AssetType.Depreciating => DefaultDepreciation,
            AssetType.Appreciating => DefaultAppreciation,
            AssetType.Legendary => Static,
            AssetType.LimitedTimeOnly => LimitedTimeOnly,
            AssetType.Conjured => ConjuredDepreciation,
            _ => DefaultDepreciation
        };
    }
    
    private static readonly QualityEvaluation DefaultDepreciation = (sellIn, quality) => Math.Clamp(sellIn > 0 ? quality - 1 : quality - 2, MinQuality, MaxQuality);
    private static readonly QualityEvaluation Static = (_, quality) => Math.Max(MinQuality, quality);
    private static readonly QualityEvaluation DefaultAppreciation = (_, quality) => Math.Clamp(quality + 1, MinQuality, MaxQuality);
    private static readonly QualityEvaluation LimitedTimeOnly = (sellIn, quality) =>
    {
        var result = sellIn <= 0 ? 0
            : sellIn > 10 ? quality + 1
            : sellIn > 5 ? quality + 2
            : quality + 3;

        return Math.Clamp(result, MinQuality, MaxQuality);
    };
    
    public static QualityEvaluation ConjuredDepreciation = (sellIn, quality) => throw new NotImplementedException();
}
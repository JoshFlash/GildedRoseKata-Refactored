using System;

namespace GildedRoseKata;

public static class QualityEvaluator
{
    private const int MaxQuality = 50;
    private const int MinQuality = 0;

    public delegate int QualityEvaluation(int sellIn, int quality);

    public static QualityEvaluation DefaultDepreciation = (sellIn, quality) =>
        Math.Clamp(sellIn > 0 ? quality - 1 : quality - 2, MinQuality, MaxQuality);

    public static QualityEvaluation Static = (_, quality) => Math.Max(MinQuality, quality);

    public static QualityEvaluation DefaultAppreciation =
        (_, quality) => Math.Clamp(quality + 1, MinQuality, MaxQuality);

    public static QualityEvaluation LimitedTimeOnly = (sellIn, quality) =>
    {
        var result = sellIn <= 0 ? 0
            : sellIn > 10 ? quality + 1
            : sellIn > 5 ? quality + 2
            : quality + 3;

        return Math.Clamp(result, MinQuality, MaxQuality);
    };

    public static QualityEvaluation ConjuredDepreciation = (sellIn, quality) => throw new NotImplementedException();
}
namespace GildedRoseKata;

public static class QualityEvaluator
{
    public delegate int QualityEvaluation(int sellIn, int quality);
    
    public static QualityEvaluation DefaultDepreciation = (sellIn, quality) => sellIn > 0 ? quality - 1 : quality - 2;
}
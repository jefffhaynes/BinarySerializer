namespace BinarySerialization.Test
{
    public class Ingredients
    {
        [FieldOrder(0)]
        public byte MainIngredientIndicator { get; set; }

        [FieldOrder(1)]
        [Subtype(nameof(MainIngredientIndicator), 1, typeof (CalciumCarbonate))]
        [Subtype(nameof(MainIngredientIndicator), 2, typeof (Iron))]
        [Subtype(nameof(MainIngredientIndicator), 3, typeof (Zinc))]
        public Chemical MainIngredient { get; set; }
    }
}
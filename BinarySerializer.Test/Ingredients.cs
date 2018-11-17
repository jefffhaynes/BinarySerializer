namespace BinarySerialization.Test
{
    public class Ingredients
    {
        [FieldOrder(0)]
        public byte MainIngredientIndicator { get; set; }

        [FieldOrder(1)]
        [Subtype("MainIngredientIndicator", 1, typeof (CalciumCarbonate))]
        [Subtype("MainIngredientIndicator", 2, typeof (Iron))]
        [Subtype("MainIngredientIndicator", 3, typeof (Zinc))]
        public Chemical MainIngredient { get; set; }
    }
}
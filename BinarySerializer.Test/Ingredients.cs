using BinarySerialization;

namespace BinarySerializer.Test
{
    public class Ingredients
    {
        public byte MainIngredientIndicator { get; set; }

        [Subtype("MainIngredientIndicator", 1, typeof(CalciumCarbonate))]
        [Subtype("MainIngredientIndicator", 2, typeof(Iron))]
        [Subtype("MainIngredientIndicator", 3, typeof(Zinc))]
        public Chemical MainIngredient { get; set; }
    }
}

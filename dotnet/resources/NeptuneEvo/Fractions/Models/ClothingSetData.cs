using NeptuneEvo.Chars;

namespace NeptuneEvo.Fractions.Models
{
    public class ClothingSetData
    {
        public sbyte Gender { get; }
        public Fractions Fraction { get; }
        public ClothesComponent Component { get; }
        public int Variation { get; }
        public int Color { get; }
        
        public ClothingSetData(sbyte gender, Fractions fraction, ClothesComponent component, int variation, int color)
        {
            Gender = gender;
            Fraction = fraction;
            Component = component;
            Variation = variation;
            Color = color;
        } 
    }
    
    public class TempEditableClothingData 
    {
        public string NewName { get; set; } = null;
        public bool Gender { get; set; } = true;
    }
}
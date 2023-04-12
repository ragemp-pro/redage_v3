namespace NeptuneEvo.Character.Models
{
    public class ComponentData
    {
        public int Drawable;
        public int Texture;
        public bool IsBlock;

        public ComponentData(int drawable, int texture)
        {
            this.Drawable = drawable;
            this.Texture = texture;
            this.IsBlock = false;
        }

        public ComponentData(int drawable, int texture, bool isBlock)
        {
            this.Drawable = drawable;
            this.Texture = texture;
            this.IsBlock = isBlock;
        }
    }
}
using System.Drawing;

namespace Converter.Model.Model
{
    public abstract class ExtendedModel : BaseModel
    {
        //TODO HH: needed?
        public Color Color { get; set; }

        public bool Visible { get; set; }
    }
}

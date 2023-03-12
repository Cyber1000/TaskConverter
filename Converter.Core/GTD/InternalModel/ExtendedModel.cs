using System.Drawing;

namespace Converter.Core.GTD.InternalModel
{
    public abstract class ExtendedModel : BaseModel
    {
        public Color Color { get; set; }

        public bool Visible { get; set; }
    }
}

using System.Drawing;

namespace TaskConverter.Model.Model;

public abstract class ExtendedModel : BaseModel
{
    public Color Color { get; set; }

    public bool Visible { get; set; }
}

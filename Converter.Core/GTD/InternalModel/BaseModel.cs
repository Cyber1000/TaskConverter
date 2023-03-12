using NodaTime;

namespace Converter.Core.GTD.InternalModel
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public Instant Created { get; set; }

        public Instant Modified { get; set; }

        public string? Title { get; set; }
    }
}

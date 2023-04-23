using NodaTime;

namespace Converter.Model.Model
{
    public abstract class BaseModel
    {
        //TODO HH: foreignid?
        public int Id { get; set; }

        public Instant Created { get; set; }

        public Instant Modified { get; set; }

        public string? Title { get; set; }
    }
}

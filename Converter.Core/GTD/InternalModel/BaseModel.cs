namespace Converter.Core.GTD.InternalModel
{
    public class BaseModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string? Title { get; set; }
    }
}

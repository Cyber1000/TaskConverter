using AutoMapper;

namespace TaskConverter.Plugin.GTD.ConversionHelper
{
    //TODO HH: move to common class?
    public class StringToIntConverter : IValueConverter<string, int>
    {
        public int Convert(string sourceMember, ResolutionContext context)
        {
            return int.TryParse(sourceMember, out var id) ? id : Math.Abs(sourceMember?.GetHashCode() ?? 0);
        }
    }
}

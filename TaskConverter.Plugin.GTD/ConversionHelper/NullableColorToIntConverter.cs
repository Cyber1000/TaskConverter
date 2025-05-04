using System.Drawing;
using AutoMapper;

namespace TaskConverter.Plugin.GTD.ConversionHelper
{
    //TODO HH: move to common class?
    public class NullableColorToIntConverter : IValueConverter<Color?, int>
    {
        public int Convert(Color? sourceMember, ResolutionContext context)
        {
            return sourceMember.HasValue ? sourceMember.Value.ToArgb() : 0;
        }
    }
}

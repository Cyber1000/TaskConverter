using System.Drawing;
using AutoMapper;

namespace TaskConverter.Plugin.GTD.ConversionHelper
{
    //TODO HH: move to common class?
    public class NullableStringToBoolConverter : IValueConverter<string, bool>
    {
        public bool Convert(string sourceMember, ResolutionContext context)
        {
            return bool.TryParse(sourceMember, out var boolConvert) && boolConvert;
        }
    }
}

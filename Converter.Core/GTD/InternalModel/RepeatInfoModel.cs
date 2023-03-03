using Converter.Core.GTD.Model;

namespace Converter.Core.GTD.InternalModel
{
    public struct RepeatInfoModel
    {
        public int Interval { get; }

        public Period Period { get; }

        public RepeatFrom RepeatFrom { get; set; }

        public RepeatInfoModel(int interval, Period period, RepeatFrom repeatFrom)
        {
            Interval = interval;
            Period = period;
            RepeatFrom = repeatFrom;
        }
    }
}

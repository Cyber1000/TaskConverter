using System.Text.RegularExpressions;

namespace Converter.Core.GTD.Model
{
    public enum Period
    {
        Day,
        Week,
        Month,
        Year
    }

    public struct RepeatInfo
    {
        private Func<string, (bool Success, int Interval, Period Period)>[] searchFunctions = new Func<
            string,
            (bool Success, int Interval, Period Period)
        >[]
        {
            (repeatInfo) => GetIntervalPeriod(repeatInfo, @"every (?<interval>\d+) (?<period>[^s]*)"),
            (repeatInfo) => GetIntervalPeriod(repeatInfo, @"(?<period>[^s]+)ly"),
            (repeatInfo) => GetIntervalPeriod(repeatInfo, @"daily", 1, Period.Day),
            (repeatInfo) => GetIntervalPeriod(repeatInfo, @"bi(?<period>[^s]+)ly", 2),
            (repeatInfo) => GetIntervalPeriod(repeatInfo, @"quarterly", 3, Period.Month),
            (repeatInfo) => GetIntervalPeriod(repeatInfo, @"semiannually", 6, Period.Month)
        };
        public int Interval { get; }

        public Period Period { get; }

        public RepeatInfo(string repeatInfo)
        {
            foreach (var searchFunction in searchFunctions)
            {
                var (success, interval, period) = searchFunction.Invoke(repeatInfo);
                if (success)
                {
                    Interval = interval;
                    Period = period;
                    return;
                }
            }

            throw new NotImplementedException($"Cannot cast \"{repeatInfo}\" to RepeatInfo");
        }

        public RepeatInfo(int interval, Period period)
        {
            Interval = interval;
            Period = period;
        }

        private static (bool Success, int Interval, Period Period) GetIntervalPeriod(
            string repeatInfo,
            string searchPattern,
            int? setInterval = null,
            Period? setPeriod = null
        )
        {
            var match = Regex.Match(repeatInfo, searchPattern, RegexOptions.IgnoreCase);
            var interval = setInterval ?? 1;
            var period = setPeriod ?? Period.Day;
            if (match.Success)
            {
                if (match.Groups["interval"].Success)
                {
                    if (!int.TryParse(match.Groups["interval"].Value, out interval))
                    {
                        return (false, interval, period);
                    }
                }
                if (match.Groups["period"].Success)
                {
                    if (!Enum.TryParse(typeof(Period), match.Groups["period"].Value, true, out var periodObject))
                    {
                        return (false, interval, period);
                    }
                    period = (Period)periodObject;
                }
            }
            return (match.Success, interval, period);
        }

        public override string ToString()
        {
            return $"Every {Interval} {GetPeriodText(Interval, Period)}";

            static string GetPeriodText(int interval, Period period)
            {
                var periodString = interval == 1 ? period.ToString() : $"{period}s";
                return periodString.ToLower();
            }
        }
    }
}

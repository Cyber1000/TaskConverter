namespace TaskConverter.Plugin.GTD.Model;

/// <summary>
/// Raised for a repeat mode the app knows and this converter cannot express. Distinct from
/// <see cref="NotImplementedException"/> on purpose: this is a property of the source data, not
/// a gap someone forgot to close, and the message has to name the value so the user can find the
/// task it came from.
/// </summary>
public class UnsupportedRepeatModeException(string repeatInfo)
    : Exception($"Repeat mode \"{repeatInfo}\" is not supported. Supported are Norepeat, Daily, Weekly, Biweekly, Monthly, Bimonthly, Quarterly, Semiannually, Yearly, BusinessDay, Weekend and \"Every <n> days|weeks|months|years\".")
{
    public string RepeatInfo { get; } = repeatInfo;
}

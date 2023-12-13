namespace CustomDialogs.Models;

public class Step
{
    public object[] SuggestedMessages { get; set; }
    public Question Question { get; set; }
    public Triage Triage { get; set; }
    public Notes Notes { get; set; }

}

using System.Collections.Generic;

namespace CustomDialogs.Models;

public class Triage
{
    // todo: care type enum? is their a clearstep sdk?
    public string BestCareType { get; set; } 
    public List<string> SecondaryCareTypes { get; set; }
    public List<CareDetailContainer> CareDetails { get; set;}
    public string EducationalInfo { get; set; }
    public string PrimaryCareSummary { get; set; }
    public string PrimaryCareDescription { get; set; }
}

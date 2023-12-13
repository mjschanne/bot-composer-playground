namespace CustomDialogs.Models;

public class ConversationStartRequest
{
    public int Years { get; set; }
    public string Sex { get; set; } // todo: enum
    public long ComplaintId { get; set; } // todo: enum? or just leave unsanitized for flexibility to their api
}

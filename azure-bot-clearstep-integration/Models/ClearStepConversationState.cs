using System;

namespace CustomDialogs.Models;

public class ClearStepConversationState
{
    public UserDetails UserDetails { get; set; }
    public Guid ConversationId { get; set; }
    public Step NextStep { get; set; }
}

using System;
using System.Collections.Generic;

namespace CustomDialogs.Models;

public class CustomPromptBotState
{
    public Guid ConversationId { get; set; }
    public Step Step { get; set; }
    public string InitialComplaint { get; set; }
    public List<Complaint> PossibleComplaints { get; set; }
}

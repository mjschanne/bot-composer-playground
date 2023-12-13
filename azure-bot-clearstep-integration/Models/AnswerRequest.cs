using System;
using System.Collections.Generic;

namespace CustomDialogs.Models;

internal class AnswerRequest
{
    public Guid ConversationId { get; set; }
    public string QuestionId { get; set; }
    public List<long> AffirmedOptionIds { get; set; }
}
using System.Collections.Generic;

namespace CustomDialogs.Models;

public class Question
{
    public string QuestionId { get; set; }
    public string Text { get; set; }
    public bool IsMultiSelect { get; set; }
    public List<Options> Options { get; set; }
}

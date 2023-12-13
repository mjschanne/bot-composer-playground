using CustomDialogs.Models;
using CustomDialogs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomDialogs.Dialogs;

public class ClearStepConversationDialog : Dialog
{
    public ClearStepConversationDialog(ClearStepTriageService clearStepTriageService)
        : base(nameof(ClearStepConversationDialog))
    {
        _clearStepTriageService = clearStepTriageService;
    }   

    // The list of slots defines the properties to collect and the dialogs to use to collect them.
    private const string Step = "step";
    private Step _step;
    private readonly ClearStepTriageService _clearStepTriageService;

    // flow: start => pose question => receive answer => check for triage => if triage, end conversation => if no triage, pose next question
    public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
    {
        // if i cant get it out of the state set state equal to options
        var step = ((dialogContext.ActiveDialog.State.TryGetValue(Step, out var stepObj) && stepObj != null) ? stepObj : options) as Step;
        dialogContext.ActiveDialog.State[Step] = step;

        var result = await dialogContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions { Prompt = MessageFactory.Text(step.Question.Text), Choices = step.Question.Options.Select(o => new Choice(o.Value)).ToList() }, cancellationToken);

        Console.WriteLine($"BeginDialogAsync: {dialogContext.Context}");

        return result;
    }

    public async Task<DialogTurnResult> PostQuestionAsync(DialogContext dialogContext, CancellationToken cancellationToken = default)
    {
        var step = dialogContext.ActiveDialog.State[Step] as Step;

        // answer previous question and set state forward
        Console.WriteLine($"Answer: {dialogContext.Context}");

        return await dialogContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions { Prompt = MessageFactory.Text(step.Question.Text), Choices = step.Question.Options.Select(o => new Choice(o.Value)).ToList() }, cancellationToken);
    }

    public async Task<DialogTurnResult> PostTriageAsync(DialogContext dialogContext, CancellationToken cancellationToken = default)
    {
        // this should provide triage result
        var step = dialogContext.ActiveDialog.State[Step] as Step;

        return await dialogContext.EndDialogAsync(step, cancellationToken);
    }

    public async Task<DialogTurnResult> EndConversationAsync(DialogContext dialogContext, CancellationToken cancellationToken = default)
    {
        var step = dialogContext.ActiveDialog.State[Step] as Step;

        // do we really need to send back any info? maybe just triage in case you want to kick off a different flow..
        return await dialogContext.EndDialogAsync(step, cancellationToken);
    }
}

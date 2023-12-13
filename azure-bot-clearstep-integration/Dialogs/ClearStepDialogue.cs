
using CustomDialogs.Models;
using CustomDialogs.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomDialogs.Dialogs;

public class ClearStepDialogue : ComponentDialog
{
    // Define value names for values tracked inside the dialogs.
    private const string CLEAR_STEP_CONTEXT = "value-ClearStepContext";
    private readonly ClearStepTriageService _clearStepTriageService;

    public ClearStepDialogue() : base(nameof(ClearStepDialogue))
    {
        // todo: move to dependency injection/keyvault or something
        // todo: depedency inject clear step triage service

        // this is from other example
        AddDialog(new TextPrompt(nameof(TextPrompt)));
        AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        AddDialog(new ClearStepQuestionPrompt(nameof(ClearStepQuestionPrompt), _clearStepTriageService));

        //AddDialog(new ReviewSelectionDialog());

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
                InitialComplaintStepAsync,
                VerifyComplaintAsync,
                StartConversationAsync
        }));

        AddDialog(new ClearStepConversationDialog(_clearStepTriageService));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private static async Task<DialogTurnResult> InitialComplaintStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        // Create an object in which to collect the user's information within the dialog.
        stepContext.Values[CLEAR_STEP_CONTEXT] = new ClearStepContext();

        var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("What seems to be the problem?") };

        // Ask the user to enter their name.
        return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
    }

    // will there be any issue if this is not static?
    private async Task<DialogTurnResult> VerifyComplaintAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var clearStepContext = stepContext.Values[CLEAR_STEP_CONTEXT] as ClearStepContext;

        clearStepContext.InitialComplaint = (string)stepContext.Result;

        var possibleComplaints = await _clearStepTriageService.EvaluateForComplaintAsync(clearStepContext.InitialComplaint);
        clearStepContext.PossibleComplaints = possibleComplaints;

        var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Which of these more closely describes your ailment?"), Choices = possibleComplaints.Select(c => new Choice(c.Name)).ToList() };

        // Ask the user to enter their age.
        return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
    }

    private async Task<DialogTurnResult> StartConversationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var clearStepContext = stepContext.Values[CLEAR_STEP_CONTEXT] as ClearStepContext;

        var foundChoice = stepContext.Result as FoundChoice;

        clearStepContext.IdentifiedComplaint = clearStepContext.PossibleComplaints[foundChoice.Index].Id;

        // hard coding certain information because i'm just interested in solving conversation flow and these represent easier things to code up
        var conversationStep = await _clearStepTriageService.StartConversationAsync("male", 30, clearStepContext.IdentifiedComplaint);

        var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(conversationStep.NextStep.Question.Text), Choices = conversationStep.NextStep.Question.Options.Select(o => new Choice(o.Value)).ToList() };

        stepContext.Context.TurnState.Add("currentStep", conversationStep);

        //stepContext.Values.Add("currentStep", conversationStep);

        return await stepContext.PromptAsync(nameof(ClearStepQuestionPrompt), promptOptions, cancellationToken);
    }

    //private async Task<DialogTurnResult> ConverseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //{
    //    // Retrieve their selection list, the choice they made, and whether they chose to finish.
    //    var clearStepContext = stepContext.Values[CLEAR_STEP_CONTEXT] as ClearStepContext;

    //    var foundChoice = stepContext.Result as FoundChoice;


    //    if (clearStepContext.CurrentStep.Triage != null) // if triage is available, end conversation
    //    {
    //        return await stepContext.EndDialogAsync(clearStepContext.CurrentStep.Triage, cancellationToken);
    //    }
    //    else // if triage is not available, continue conversation
    //    {
    //        if (clearStepContext.CurrentStep.Question.QuestionId != clearStepContext.CurrentQuestionId) //.if questionIds match; launch this step as question
    //        {


    //            await stepContext.RepromptDialogAsync(cancellationToken);
    //        }
    //        else // otherwise, process answer and ask question
    //        {
    //            await stepContext.RepromptDialogAsync(cancellationToken);
    //        }
    //    }



    //}
}

public class ClearStepContext
{
    public string InitialComplaint { get; set; }
    public List<Complaint> PossibleComplaints { get; set; } // does this need to be a string - will have to convert back and forth...
    public long IdentifiedComplaint { get; set; }
    public string ComplaintList { get; set; }
    public Step CurrentStep { get; set; }
    public string CurrentQuestionId { get; set; }
}
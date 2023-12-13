using CustomDialogs.Models;
using CustomDialogs.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomDialogs.Bots;

// This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
// to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
// each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
public class CustomPromptBot : ActivityHandler
{
    private readonly BotState _conversationState;

    private readonly ClearStepTriageService _clearStepTriageService;

    public CustomPromptBot(ConversationState conversationState, ClearStepTriageService clearStepTriageService)
    {
        _conversationState = conversationState;
        _clearStepTriageService = clearStepTriageService;
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        var stateAccessor = _conversationState.CreateProperty<CustomPromptBotState>("state");
        var state = await stateAccessor.GetAsync(turnContext, () => new CustomPromptBotState(), cancellationToken);

        await AskQuestionAsync(state, turnContext, cancellationToken);

        // Save changes.
        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
    }

    private async Task AskQuestionAsync(CustomPromptBotState state, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        // todo: perform some validation for each stage...
        var input = turnContext.Activity.Text?.Trim();

        var responseMsg = "";

        if (state.Step is null)
        {
            if (state.PossibleComplaints is null)
            {
                state.InitialComplaint = input;

                state.PossibleComplaints = await _clearStepTriageService.EvaluateForComplaintAsync(state.InitialComplaint);

                responseMsg += "Which of these most closely matches what you're experiencing?";

                for (var i = 1; i <= state.PossibleComplaints.Count; i++)
                {
                    responseMsg += Environment.NewLine + $"{i}. {state.PossibleComplaints[i - 1].Name}";
                }

                await turnContext.SendActivityAsync(responseMsg, null, null, cancellationToken);
            }
            else
            {
                // can do validation; might try adaptive card button...
                var choiceIndex = int.Parse(input);

                // not going through the tedium of how to code up getting sex and age
                var conversationState = await _clearStepTriageService.StartConversationAsync("male", 30, state.PossibleComplaints[choiceIndex - 1].Id);

                state.Step = conversationState.NextStep;
                state.ConversationId = conversationState.ConversationId;

                responseMsg += state.Step.Question.Text;

                for (var i = 1; i <= state.Step.Question.Options.Count; i++)
                {
                    responseMsg += Environment.NewLine + $"{i}. {state.Step.Question.Options[i - 1].Value}";
                }

                await turnContext.SendActivityAsync(responseMsg, null, null, cancellationToken);
            }

        }
        else
        {
            // ask question and move forward
            // can do validation; might try adaptive card button...
            var optionIndex = int.Parse(input);

            // not going through the tedium of how to code up getting sex and age
            state.Step = await _clearStepTriageService.AnswerQuestionAsync(state.ConversationId, state.Step.Question.QuestionId, state.Step.Question.Options[optionIndex - 1].OptionId);

            if (state.Step.Triage is not null)
            {
                await turnContext.SendActivityAsync(state.Step.Triage.PrimaryCareSummary, null, null, cancellationToken);
                await turnContext.SendActivityAsync(state.Step.Triage.EducationalInfo, null, null, cancellationToken);
                return;
            }

            responseMsg += state.Step.Question.Text;

            for (var i = 1; i <= state.Step.Question.Options.Count; i++)
            {
                responseMsg += Environment.NewLine + $"{i}. {state.Step.Question.Options[i - 1].Value}";
            }

            await turnContext.SendActivityAsync(responseMsg, null, null, cancellationToken);
        }
    }
}
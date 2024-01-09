using CustomDialogs.Models;
using CustomDialogs.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomDialogs.Bots;

// This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
// to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
// each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
public class CustomPromptBot : ActivityHandler
{
    private readonly AdaptiveCardFactory _adaptiveCardFactory;
    private readonly BotState _conversationState;
    private readonly ClearStepTriageService _clearStepTriageService;

    public CustomPromptBot(AdaptiveCardFactory adaptiveCardFactory, ConversationState conversationState, ClearStepTriageService clearStepTriageService)
    {
        _adaptiveCardFactory = adaptiveCardFactory;
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

                var dict = new Dictionary<string, string>();

                foreach (var complaint in state.PossibleComplaints)
                {
                    // name is both the title and the value since the value when you click the button is what gets submitted as the users response as text
                    // so you want it to be meaingful looking and since our options here are just what symptoms are they experiencing than it makes sense to use it as both
                    // the title and the value of the option
                    dict.Add(complaint.Name, complaint.Name);
                }

                var card = _adaptiveCardFactory.CreateCombinationCard("Which of these most closely matches what you're experiencing?", dict);

                await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
            }
            else
            {
                // you might add some additional validations and checks here to see if the user is trying to exit the flow to do something else
                // or you may decide the triage process should be uniterrupable but you'd still want to check to reprompt them to answer the question
                // you'll also want to handle forgetting this state if you add in a persistence layer with cosmos

                // not going through the tedium of how to code up getting sex and age, you can add that in earlier in the process
                var conversationState = await _clearStepTriageService.StartConversationAsync("male", 30, state.PossibleComplaints.Find(c => c.Name == input).Id);

                state.Step = conversationState.NextStep;
                state.ConversationId = conversationState.ConversationId;

                responseMsg += state.Step.Question.Text;

                var dict = new Dictionary<string, string>();

                foreach (var option in state.Step.Question.Options)
                {
                    // see earlier comment about why the name is both the title and the value
                    dict.Add(option.Value, option.Value);
                }

                var card = _adaptiveCardFactory.CreateCombinationCard(state.Step.Question.Text, dict);

                await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
            }
        }
        else
        {
            // see earlier comments on validations

            // not going through the tedium of how to code up getting sex and age
            state.Step = await _clearStepTriageService.AnswerQuestionAsync(state.ConversationId, state.Step.Question.QuestionId,

                state.Step.Question.Options.Find(o => o.Value == input).OptionId);

            if (state.Step.Triage is not null)
            {
                await turnContext.SendActivityAsync(state.Step.Triage.PrimaryCareSummary, null, null, cancellationToken);
                await turnContext.SendActivityAsync(state.Step.Triage.EducationalInfo, null, null, cancellationToken);
                return;
            }

            var dict = new Dictionary<string, string>();

            foreach (var option in state.Step.Question.Options)
            {
                // see earlier comment about why the name is both the title and the value
                dict.Add(option.Value, option.Value);
            }

            var card = _adaptiveCardFactory.CreateCombinationCard(state.Step.Question.Text, dict);

            await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
        }
    }
}
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using CustomDialogs.Models;
using CustomDialogs.Services;
using System.Linq;

namespace CustomDialogs.Dialogs;

public class ClearStepQuestionPrompt : ChoicePrompt
{
    private readonly ClearStepTriageService _clearStepTriageService;
    private Triage _triage = null;
    private ClearStepConversationState _currentStep = null;

    public ClearStepQuestionPrompt(string dialogId, ClearStepTriageService clearStepTriageService, PromptValidator<FoundChoice> validator = null, string defaultLocale = null) : base(dialogId, validator, defaultLocale)
    {
        _clearStepTriageService = clearStepTriageService;
    }

    public ClearStepQuestionPrompt(string dialogId, ClearStepTriageService clearStepTriageService, Dictionary<string, ChoiceFactoryOptions> choiceDefaults, PromptValidator<FoundChoice> validator = null, string defaultLocale = null) : base(dialogId, choiceDefaults, validator, defaultLocale)
    {
        _clearStepTriageService = clearStepTriageService;
    }

    protected override async Task OnPromptAsync(
            ITurnContext turnContext,
            IDictionary<string, object> state,
            PromptOptions options,
            bool isRetry,
            CancellationToken cancellationToken = default)
    {
        _currentStep ??= turnContext.TurnState.Get<ClearStepConversationState>("currentStep");

        while (_triage is null)
        {
            await base.OnPromptAsync(turnContext, state, options, isRetry);

            var result = await OnRecognizeAsync(turnContext, state, options, CancellationToken.None);

            var conversationId = _currentStep.ConversationId;
            var questionId = _currentStep.NextStep.Question.QuestionId;
            var answer = _currentStep.NextStep.Question.Options.FirstOrDefault(o => o.Value == result.Value.Value).OptionId;

            var conversationStepResponse = await _clearStepTriageService.AnswerQuestionAsync(conversationId, questionId, answer);

            //if (conversationStepResponse.NextStep.Triage is not null)
            //    _triage = conversationStepResponse.NextStep.Triage;
            //else
            //    _currentStep = conversationStepResponse;
        }
    }

    protected override async Task<PromptRecognizerResult<FoundChoice>> OnRecognizeAsync(
            ITurnContext turnContext,
            IDictionary<string, object> state,
            PromptOptions options,
            CancellationToken cancellationToken = default)
    {
        var result = await base.OnRecognizeAsync(turnContext, state, options, cancellationToken);

        var conversationId = _currentStep.ConversationId;
        var questionId = _currentStep.NextStep.Question.QuestionId;
        var answer = _currentStep.NextStep.Question.Options.FirstOrDefault(o => o.Value == result.Value.Value).OptionId;

        var conversationStepResponse = await _clearStepTriageService.AnswerQuestionAsync(conversationId, questionId, answer);

        //if (conversationStepResponse.NextStep.Triage is not null)
        //{
        //    _triage = conversationStepResponse.NextStep.Triage;

        //    //await OnPromptAsync(turnContext, state, conversationStepResponse.NextStep.Question.Options., false, cancellationToken);
        //}

        //else
        //    _currentStep = conversationStepResponse;

        return result;
    }
}

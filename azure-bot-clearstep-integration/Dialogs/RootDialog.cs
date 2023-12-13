// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CustomDialogs.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.BotBuilderSamples;


/// <summary>
/// This is an example root dialog. Replace this with your applications.
/// </summary>
public class RootDialog : ComponentDialog
{
    private readonly IStatePropertyAccessor<JObject> _userStateAccessor;

    public RootDialog(UserState userState)
        : base("root")
    {
        _userStateAccessor = userState.CreateProperty<JObject>("result");

        AddDialog(new ClearStepDialogue());

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
                InitialStepAsync,
                FinalStepAsync,
        }));

        // The initial child Dialog to run.
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.BeginDialogAsync(nameof(ClearStepDialogue), null, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        await stepContext.Context.SendActivityAsync("End of simulation.");

        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
}

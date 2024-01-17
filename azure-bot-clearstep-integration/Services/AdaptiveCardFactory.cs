using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CustomDialogs.Services;

public sealed class AdaptiveCardFactory
{
    //The default schema for the adaptive cards, it tells the client what version of adaptive cards to render
    private static readonly AdaptiveSchemaVersion defaultSchema = new(1, 0);

    /// <summary>
    /// Creates a simple adaptive card with two submit actions.
    /// </summary>
    public Attachment CreateOptionsCard(Dictionary<string, string> options)
    {
        var card_actions = new List<AdaptiveAction>();

        foreach (var option in options)
        {
            card_actions.Add(new AdaptiveSubmitAction
            {
                Title = option.Key,
                Data = option.Value
            });
        }

        AdaptiveCard card = new(defaultSchema)
        {
            Body = new List<AdaptiveElement>()
                {
                    new AdaptiveActionSet
                    {
                        Actions = card_actions
                    }
                }
        };

        return CreateAdaptiveCardAttachment(card.ToJson());
    }

    /// <summary>
    /// Creates an adaptive card that contains a textbox only
    /// </summary>
    //public Microsoft.Bot.Schema.Attachment CreateTextCard()
    //{
    //    var message = ConstantStrings.welcome_message;

    //    AdaptiveCard card = new(defaultSchema)
    //    {
    //        Body = new List<AdaptiveElement>()
    //            {
    //                new AdaptiveTextBlock()
    //                {
    //                    Text = message,
    //                    Size = AdaptiveTextSize.Default,
    //                    Wrap = true
    //                }
    //            }
    //    };

    //    return CreateAdaptiveCardAttachment(card.ToJson());
    //}

    /// <summary>
    /// Creates an adaptive card that has both text and a submit action
    /// </summary>
    public Attachment CreateCombinationCard(string message, IDictionary<string, string> actions)
    {
        AdaptiveCard card = new(defaultSchema)
        {
            Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock()
                    {
                        Text = message,
                        Size = AdaptiveTextSize.Default
                    },
                    new AdaptiveActionSet
                    {
                        Actions = actions.Select(kvp => new AdaptiveSubmitAction
                        {
                            Title = kvp.Value,
                            Data = kvp.Key
                        }).ToList<AdaptiveAction>()
                    }
                }
        };

        return CreateAdaptiveCardAttachment(card.ToJson());
    }

    /// <summary>
    /// Using a JSON representation of the adaptive card return an attachment that can be sent to the client
    /// </summary>
    Attachment CreateAdaptiveCardAttachment(string jsonData)
    {
        var adaptiveCardAttachment = new Attachment()
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JsonConvert.DeserializeObject(jsonData),
        };

        return adaptiveCardAttachment;
    }
}
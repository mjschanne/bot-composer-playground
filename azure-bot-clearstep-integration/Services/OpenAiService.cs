using Azure.AI.OpenAI;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomDialogs.Services;

public class OpenAiService
{
    private const string IS_TRIAGE_PROMPT = "Given the following text, determine if it is related to a person asking for triage of symptoms (Please respond with 'True' if the text is related to a person asking for triage of symptoms, and 'False' if it is not.):";
    private const string GENERAL_PROMPT = """
You are a friendly AI assistant. Your main goal is to assist users by providing accurate information, 
answering their questions, and helping them with tasks in a polite and friendly manner. 
Remember to always maintain a positive and respectful tone in your interactions.
""";


    private readonly OpenAIClient _client;
    private readonly string _deploymentName;

    public OpenAiService(string uri, string deploymentName, string tenantId)
    {
        // I'm doing this because I have access to too many tenants so this will keep default credentials from getting confused
        var options = new DefaultAzureCredentialOptions { TenantId = tenantId };
        var credential = new DefaultAzureCredential(options);

        _client = new OpenAIClient(new Uri(uri), credential);
        _deploymentName = deploymentName;
    }

    public async Task<bool> IsTriageAsync(string userInput)
    {
        var req = new ChatCompletionsOptions(_deploymentName, new List<ChatRequestMessage> {
            new ChatRequestSystemMessage(IS_TRIAGE_PROMPT),
            new ChatRequestUserMessage(userInput)
        });

        var resp = await _client.GetChatCompletionsAsync(req);

        var content = resp.Value.Choices.FirstOrDefault()?.Message?.Content;

        if (!bool.TryParse(content, out var isTriage))
            throw new Exception("Unable to parse response from OpenAI"); // fail loudly in POC stage

        return isTriage;
    }

    public async Task<string> GetGeneralResponseAsync(string userInput)
    {
        var req = new ChatCompletionsOptions(_deploymentName, new List<ChatRequestMessage>
        {
            new ChatRequestSystemMessage(GENERAL_PROMPT),
            new ChatRequestUserMessage(userInput)
        });

        var resp = await _client.GetChatCompletionsAsync(req);

        return resp.Value.Choices.FirstOrDefault()?.Message?.Content;
    }
}

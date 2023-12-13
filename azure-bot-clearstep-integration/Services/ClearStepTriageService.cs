using CustomDialogs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomDialogs.Services;

public class ClearStepTriageService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
    };

    public ClearStepTriageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Complaint>> EvaluateForComplaintAsync(string possibleComplaint)
    {
        var message = CreateHttpRequestMessage(HttpMethod.Get, $"nlp?text={possibleComplaint}");

        var response =  await _httpClient.SendAsync(message);

        return HandleResponse<List<Complaint>>(response);
    }

    public async Task<ClearStepConversationState> StartConversationAsync(string sex, int yrsOld, long complaintId)
    {
        var conversationStartRequest = new ConversationStartRequest
        {
            Sex = sex,
            Years = yrsOld,
            ComplaintId = complaintId
        };

        var requestJson = JsonConvert.SerializeObject(conversationStartRequest, _jsonSerializerSettings);

        var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var message = CreateHttpRequestMessage(HttpMethod.Post, $"start_conversation", stringContent);

        var response = await _httpClient.SendAsync(message);

        return HandleResponse<ClearStepConversationState>(response);
    }

    public async Task<Step> AnswerQuestionAsync(Guid conversationId, string QuestionId, params long[] answers)
    {
        var answerRequest = new AnswerRequest
        {
            ConversationId = conversationId,
            QuestionId = QuestionId,
            AffirmedOptionIds = answers.ToList()
        };

        var requestJson = JsonConvert.SerializeObject(answerRequest, _jsonSerializerSettings);

        var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var message = CreateHttpRequestMessage(HttpMethod.Post, $"answer_question", stringContent);

        var response = await _httpClient.SendAsync(message);

        return HandleResponse<Step>(response);
    }

    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string endpoint, HttpContent content = null)
    {
        // todo: sanitize url; move version to config
        var msg = new HttpRequestMessage(method, "/v1/" + endpoint);

        if (content != null)
            msg.Content = content;

        return msg;
    }

    private T HandleResponse<T>(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);

            return result;
        } catch (Exception ex)
        {
            throw new Exception("Error handling response", ex);
        }   
    }
}

public class ClearStepOptions
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
}

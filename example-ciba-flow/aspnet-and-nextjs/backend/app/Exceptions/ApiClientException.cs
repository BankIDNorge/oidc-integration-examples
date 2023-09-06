using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ExampleAspnet.Models;

namespace ExampleAspnet.Exceptions;

/// <summary>
///  Represents an error received from the API.
/// </summary>
[Serializable]
public class ApiClientException : ExampleAppException
{
    public HttpStatusCode StatusCode { get; init; }

    public string? Name { get; init; }

    public string? Description { get; init; }

    public ApiClientException(HttpResponseMessage response, string? name, string? description) : base(
        new ErrorResponse(name ?? "unknown", description ?? "Unknown error"))
    {
        Name = name;
        Description = description;
        StatusCode = response.StatusCode;
    }

    public static async Task<ApiClientException> From(HttpResponseMessage response)
    {
        string? name = null;
        string? description = null;
        try
        {
            var json = await response.Content.ReadFromJsonAsync<JsonObject>();
            if (json != null)
            {
                name = GetErrorType(json["type"]?.Deserialize<string>()) ?? json["error"]?.Deserialize<string>();
                description = json["detail"]?.Deserialize<string>() ?? json["error_description"]?.Deserialize<string>();
            }
        }
        catch (Exception)
        {
            // Ignore 
        }

        return new ApiClientException(response, name, description);
    }

    private static string? GetErrorType(string? typeUri)
    {
        if (typeUri == null)
        {
            return null;
        }

        var index = typeUri.LastIndexOf('/') + 1;
        return typeUri[index..];
    }
}
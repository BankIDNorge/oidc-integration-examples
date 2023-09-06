using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ExampleAspnet.Exceptions;
using ExampleAspnet.Utils;

namespace ExampleAspnet.Services;

public interface IApiClient
{
    /// <summary>
    /// Performs a GET request to the specified <paramref name="uri"/> and returns the response as an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="uri">The URI to make the request to.</param>
    /// <typeparam name="T">The type to deserialize the JSON response into.</typeparam>
    public Task<T> GetJsonAsync<T>(string uri);

    /// <summary>
    /// Performs a GET request to the specified <paramref name="uri"/> and returns the response as a string.
    /// </summary>
    /// <param name="uri">The URI to make the request to.</param>
    /// <param name="bearerToken">If present, the bearer token will be sent as a Bearer Authorization header.</param>
    public Task<string> GetStringAsync(string uri, string? bearerToken);

    /// <summary>
    /// Performs a POST request to the specified <paramref name="uri"/> and returns the response as an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="uri">The URI to make the request to.</param>
    /// <param name="bodyValues">The body values of the request. The content type is set to application/x-www-form-urlencoded.</param>
    /// <param name="basicCredentials">If present, the credentials will be encoded and sent as a Basic Authorization header.</param>
    /// <typeparam name="T">The type to deserialize the JSON response into.</typeparam>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<T> PostFormUrlEncodedAsync<T>(string uri, HttpContent bodyValues,
        (string user, string pass)? basicCredentials);

    /// <summary>
    /// Performs a POST request to the specified <paramref name="uri"/> and returns the response as an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="uri">The URI to make the request to.</param>
    /// <param name="bodyValues">The body values of the request. The content type is set to application/x-www-form-urlencoded.</param>
    /// <param name="bearerToken">If present, the bearer token will be sent as a Bearer Authorization header.</param>
    /// <typeparam name="T">The type to deserialize the JSON response into.</typeparam>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<T> PostFormUrlEncodedAsync<T>(string uri, HttpContent bodyValues,
        string? bearerToken);

    public Task<T> PostJsonAsync<T>(string uri, object body, string? bearerToken);
}

/// <summary>
/// A wrapper class for <see cref="HttpClient"/>.
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { IncludeFields = true };

    public ApiClient(HttpClient client)
    {
        _client = client;
    }


    public async Task<T> GetJsonAsync<T>(string uri)
    {
        var response = await _client.GetAsync(uri);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions) ??
                   throw new InvalidOperationException("Attempted to deserialize null into a non-nullable type.");
        }

        throw await ApiClientException.From(response);
    }

    public async Task<string> GetStringAsync(string uri, string? bearerToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        if (bearerToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw await ApiClientException.From(response);
    }

    public Task<T> PostFormUrlEncodedAsync<T>(string uri, HttpContent bodyValues,
        (string user, string pass)? basicCredentials)
    {
        AuthenticationHeaderValue? authorization = null;
        if (basicCredentials != null)
        {
            var encodedCredentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(basicCredentials.Value.user + ":" + basicCredentials.Value.pass)
            );
            authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
        }

        return PostFormUrlEncodedAsync<T>(uri, bodyValues, authorization);
    }

    public Task<T> PostFormUrlEncodedAsync<T>(string uri, HttpContent bodyValues, string? bearerToken)
    {
        return PostFormUrlEncodedAsync<T>(uri, bodyValues,
            bearerToken != null ? new AuthenticationHeaderValue("Bearer", bearerToken) : null);
    }

    public async Task<T> PostJsonAsync<T>(string uri, object body, string? bearerToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = JsonContent.Create(body);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeTypes.ApplicationJson);

        if (bearerToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var response = await _client.SendAsync(request);
        // TODO: Handle non-success status codes

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>() ??
                   throw new InvalidOperationException("Attempted to deserialize null into a non-nullable type.");
        }

        throw await ApiClientException.From(response);
    }

    private async Task<T> PostFormUrlEncodedAsync<T>(string uri, HttpContent bodyValues,
        AuthenticationHeaderValue? authorization)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = bodyValues;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeTypes.ApplicationFormUrlEncoded);

        if (authorization != null)
        {
            request.Headers.Authorization = authorization;
        }

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>() ??
                   throw new InvalidOperationException("Attempted to deserialize null into a non-nullable type.");
        }

        throw await ApiClientException.From(response);
    }
}
namespace AndrezOG.Infrastructure.Auth;

using System.Net.Http.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Servicio HTTP que se comunica con Google OAuth 2.0 y Google People API.
/// Responsabilidad unica: intercambiar codigo de autorizacion por token,
/// y obtener los datos del usuario autenticado con Google.
/// </summary>
public class GoogleAuthService
{
    private readonly HttpClient _http;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public GoogleAuthService(HttpClient http, string clientId, string clientSecret)
    {
        _http = http;
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    /// <summary>
    /// Intercambia el authorization_code por un id_token + access_token en Google.
    /// </summary>
    public async Task<GoogleTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["redirect_uri"] = redirectUri,
            ["grant_type"] = "authorization_code"
        });

        var response = await _http.PostAsync("https://oauth2.googleapis.com/token", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Google token exchange failed: {response.StatusCode} — {errorBody}");
        }

        return (await response.Content.ReadFromJsonAsync<GoogleTokenResponse>())!;
    }

    /// <summary>
    /// Obtiene los datos del usuario desde Google usando el access_token
    /// (endpoint userinfo de OpenID Connect).
    /// </summary>
    public async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Google userinfo failed: {response.StatusCode} — {errorBody}");
        }

        return (await response.Content.ReadFromJsonAsync<GoogleUserInfo>())!;
    }
}

// ---------- Modelos internos para deserializar respuestas de Google ----------

public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}

public class GoogleUserInfo
{
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty; // GoogleId

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; } = string.Empty;

    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
}
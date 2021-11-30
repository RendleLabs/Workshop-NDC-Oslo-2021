namespace Frontend.Auth;

public class AuthHelper
{
    private readonly HttpClient _http;
    private string? _token;

    public AuthHelper(HttpClient http)
    {
        _http = http;
    }

    public ValueTask<string> GetTokenAsync()
    {
        if (_token is {Length: > 0})
        {
            return new ValueTask<string>(_token);
        }

        return new ValueTask<string>(GetTokenImpl());
    }

    private async Task<string> GetTokenImpl()
    {
        _token = await _http.GetStringAsync("/generateJwt?name=frontend");
        return _token;
    }
}
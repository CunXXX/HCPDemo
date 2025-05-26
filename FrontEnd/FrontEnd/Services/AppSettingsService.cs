using System.Net.Http.Json;
using System.Text.Json;

namespace FrontEnd.Services;

public class AppSettingsService
{
    private readonly HttpClient _httpClient;
    private Dictionary<string, object>? _settings;

    public AppSettingsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LoadAsync()
    {
        _settings = await _httpClient.GetFromJsonAsync<Dictionary<string, object>>("appsettings.json");
    }

    public T Get<T>(string key)
    {
        if (_settings != null && _settings.TryGetValue(key, out var value))
        {
            if (value is JsonElement element)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)(element.GetString() ?? "");
                }
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)element.GetInt32();
                }
                if (typeof(T) == typeof(bool))
                {
                    return (T)(object)element.GetBoolean();
                }
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        return default!;
    }
}
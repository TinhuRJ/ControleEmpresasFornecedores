using System.Net.Http;
using System.Text.Json;

namespace ControleEmpresasFornecedores.Api.Services;

public interface ICepService
{
    Task<bool> CepValidoAsync(string cep);
}

public class CepService : ICepService
{
    private readonly HttpClient _httpClient;

    public CepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CepValidoAsync(string cep)
    {
        if(string.IsNullOrWhiteSpace(cep))
            return false;

        try
        {
            var response = await _httpClient.GetAsync($"https://cep.la/{cep}");

            if(!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();

            return !string.IsNullOrWhiteSpace(content);
        }
        catch
        {
            // fail soft → não derruba o sistema se a API cair
            return true;
        }
    }
}
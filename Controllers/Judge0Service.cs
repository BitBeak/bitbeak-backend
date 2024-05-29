using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using BitBeakAPI.Models;

public class Judge0Service
{
    private readonly HttpClient _httpClient;
    private readonly string _judge0ApiUrl = "https://judge0-ce.p.rapidapi.com/submissions?base64_encoded=false&wait=true";
    private readonly string _rapidApiKey;

    public Judge0Service(HttpClient httpClient, IOptions<RapidApiConfig> config)
    {
        _httpClient = httpClient;
        _rapidApiKey = config.Value.Key;
    }

    public async Task<bool> ValidarCodigo(string codigoUsuario, List<CasoTeste> casosTeste)
    {
        foreach (var casoTeste in casosTeste)
        {
            var payload = new
            {
                source_code = codigoUsuario,
                language_id = 63, // 63 é o ID para JavaScript no Judge0
                stdin = casoTeste.Entrada,
                expected_output = casoTeste.SaidaEsperada
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            content.Headers.Add("X-RapidAPI-Key", _rapidApiKey);
            content.Headers.Add("X-RapidAPI-Host", "judge0-ce.p.rapidapi.com");

            var response = await _httpClient.PostAsync(_judge0ApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Request failed with status code {response.StatusCode}");
                return false;
            }

            var responseData = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseData);
            var result = JsonSerializer.Deserialize<Judge0Result>(responseData);

            if (result == null || result.Status == null)
            {
                Console.WriteLine("Failed to parse response or status is null.");
                return false;
            }

            if (result.Status.Id != 3) // 3 é o status para "Accepted"
            {
                Console.WriteLine($"Test failed with status id {result.Status.Id} and description {result.Status.Description}");
                return false;
            }
        }

        return true;
    }
}

public class Judge0Result
{
    public Status Status { get; set; }
}

public class Status
{
    public int Id { get; set; }
    public string Description { get; set; }
}

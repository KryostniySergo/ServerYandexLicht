using Microsoft.AspNetCore.Mvc;
using ServerViewYa.Database;
using ServerViewYa.Models;

namespace ServerViewYa.Controllers
{
    public class ApiController : Controller
    {
        private readonly ApplicationContext _context;
        readonly HttpClient client;

        public ApiController(ApplicationContext context)
        {
            _context = context;
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization",
                "Api-Key AQVNy0iP5LuvWAGUERHMukcmZkDeZisazhg-393d");
        }

        public async Task Index(string filename)
        {
            var content = new StringContent(
            "{\n  " +
            "  \"config\": " +
            "{\n" +
            " \"specification\":" +
            " {\n" +
            "  \"languageCode\": \"ru-RU\",\n" +
            "  \"model\": \"general\",\n" +
            "  \"profanityFilter\": \"false\",\n" +
            "  \"sampleRateHertz\": 48000,\n" +
            "  \"audioChannelCount\": 1\n " +
            "  }\n    },\n   " +
            " \"audio\": {\n     " +
            " \"uri\": " +
            $"\"https://storage.yandexcloud.net/audiotost/{filename}\"\n " +
            "  }\n}");

            using var response = await client
                .PostAsync(
                "https://transcribe.api.cloud.yandex.net/speech/stt/v2/longRunningRecognize",
                content);

            var resp = await response.Content.ReadFromJsonAsync<RespStt>();

            await _context.audios.AddAsync(new Audio
            {
                Name = filename,
                AudioId = resp.id,
                Status = false,
                Text = ""
            });

            await _context.SaveChangesAsync();

            await Response.WriteAsync("Good");
        }
    }

    record RespStt(bool done, string id, string createdAt, string createdBy, string modifiedAt);

}

using Mindee;
using Mindee.Http;
using Mindee.Input;
using Mindee.Product.Generated;
using System.Threading.Tasks;

static class MindeeService
{
    private static MindeeClient _mindeeClient = new MindeeClient("MindeeToken");

    public static async Task<string> ParseDocument(string filePath, string type)
    {
        var inputSource = new LocalInputSource(filePath);
        CustomEndpoint endpoint;

        if (type == "Passport")
        {
            endpoint = new CustomEndpoint("id_card", "qenzai", "1");
        }
        else if (type == "RegistrationCertificate")
        {
            endpoint = new CustomEndpoint("_registrationcertificate", "qenzai", "1");
        }
        else
        {
            return "Невідомий тип документа.";
        }

        try
        {
            var response = await _mindeeClient.EnqueueAndParseAsync<GeneratedV1>(inputSource, endpoint);
            var doc = response.Document;
            return doc.ToString();
        }
        catch (Exception ex)
        {
            return $"Помилка розпізнавання: {ex.Message}";
        }
    }
}

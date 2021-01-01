using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http;

namespace Example
{
    namespace Keygen
    {
        public class License
        {
            public string Id { get; set; }
            public string Type { get; set; }
        }

        public class LicenseValidationResult
        {
            public string Constant { get; set; }
            public bool Valid { get; set;  }
        }

        public class ValidateKeyResponse
        {
            public LicenseValidationResult Meta { get; set; }
            public License Data { get; set; }
        }
    }

    class Program
    {
        private static HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://api.keygen.sh/v1/accounts/demo/"),
        };

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var licenseKey = PromptForLicenseKey();
            var validation = await ValidateLicenseKeyAsync(licenseKey);

            if (validation.Meta.Valid)
            {
                Console.WriteLine($"Valid! License ID: {validation.Data.Id}");

                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"Invalid! Validation code: {validation.Meta.Constant}");

                Environment.Exit(1);
            }
        }

        private static string PromptForLicenseKey()
        {
            Console.Write("Please enter a license key: ");

            return Console.ReadLine();
        }

        private static async Task<Keygen.ValidateKeyResponse> ValidateLicenseKeyAsync(string licenseKey)
        {
            var serialized = JsonSerializer.Serialize(new
            {
                meta = new
                {
                    key = licenseKey,
                }
            });

            var content = new StringContent(serialized, Encoding.UTF8, "application/json");
            var res = await client.PostAsync("licenses/actions/validate-key", content);
            var body = await res.Content.ReadAsStringAsync();

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Fatal API error: {body}");
            }

            return JsonSerializer.Deserialize<Keygen.ValidateKeyResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                }
            );
        }
    }
}

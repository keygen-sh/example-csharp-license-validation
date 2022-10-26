using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public record Attributes
{
  public DateTime Expiry { get; set; }
}

public record Data
{
  public string ID { get; set; }
  public Attributes Attributes { get; set; }
}

public record Error
{
  public string Title { get; set; }
  public string Detail { get; set; }
  public string Code { get; set; }
}

public record Meta
{
  public string Detail { get; set; }
  public string Code { get; set; }
  public bool Valid { get; set; }
}

public record Document
{
  public Meta Meta { get; set; }
  public Data Data { get; set; }
  public List<Error> Errors { get; set; }
}

class Program
{
  // This is your Keygen account ID.
  //
  // Available at: https://app.keygen.sh/settings
  const string KEYGEN_ACCOUNT_ID = "demo";

  static void Main()
  {
    MainAsync().Wait();
  }

  static async Task MainAsync()
  {
    var keygen = new RestClient(string.Format("https://api.keygen.sh/v1/accounts/{0}", KEYGEN_ACCOUNT_ID));
    var request = new RestRequest("licenses/actions/validate-key", Method.POST);

    request.AddHeader("Content-Type", "application/vnd.api+json");
    request.AddHeader("Accept", "application/vnd.api+json");
    request.AddJsonBody(new {
      // In a real app, this license key should be dynamic, based on user input.
      meta = new { key = "DEMO-AABCCD-7F6E4A-E64012-340C88-V3" }
    });

    var response = await keygen.ExecuteAsync<Document>(request);
    if (response.Data.Errors != null)
    {
      Console.Write("[ERROR] Status={0}", response.StatusCode);

      response.Data.Errors.ForEach(err =>
        Console.Write(" Title={0} Detail={1} Code={2}", err.Title, err.Detail, err.Code)
      );

      Console.WriteLine();

      Environment.Exit(1);
    }

    if (response.Data.Meta.Valid)
    {
      Console.WriteLine(
        "[INFO] License={0} Expiry={1} Valid={2} Code={3}",
        response.Data.Data.ID,
        response.Data.Data.Attributes.Expiry,
        response.Data.Meta.Valid,
        response.Data.Meta.Code
      );
    }
    else
    {
      Console.WriteLine(
        "[INFO] License={0} Invalid={1} Code={2}",
        response.Data.Data != null ? response.Data.Data.ID : "N/A",
        response.Data.Meta.Detail,
        response.Data.Meta.Code
      );
    }
  }
}

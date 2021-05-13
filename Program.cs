using RestSharp;
using System;
using System.Collections.Generic;

class Program
{
  // This is your Keygen account ID.
  //
  // Available at: https://app.keygen.sh/settings
  const string KEYGEN_ACCOUNT_ID = "demo";

  public static void Main (string[] args)
  {
    var keygen = new RestClient(string.Format("https://api.keygen.sh/v1/accounts/{0}", KEYGEN_ACCOUNT_ID));
    var request = new RestRequest("licenses/actions/validate-key", Method.POST);

    request.AddHeader("Content-Type", "application/vnd.api+json");
    request.AddHeader("Accept", "application/vnd.api+json");
    request.AddJsonBody(new {
      meta = new { key = "DEMO-AABCCD-7F6E4A-E64012-340C88-V3" }
    });

    var response = keygen.Execute<Dictionary<string, object>>(request);
    if (response.Data.ContainsKey("errors"))
    {
      var errors = (RestSharp.JsonArray) response.Data["errors"];
      if (errors != null)
      {
        Console.WriteLine("[ERROR] Status={0} Errors={1}", response.StatusCode, errors);

        Environment.Exit(1);
      }
    }

    var license = (Dictionary<string, object>) response.Data["data"];
    var meta = (Dictionary<string, object>) response.Data["meta"];

    if ((bool) meta["valid"])
    {
      Console.WriteLine("[INFO] License={0} Valid={1} ValidationCode={2}", license["id"], meta["detail"], meta["constant"]);
    }
    else
    {
      Console.WriteLine(
        "[INFO] License={0} Invalid={1} ValidationCode={2}",
        license != null ? license["id"] : "N/A",
        meta["detail"],
        meta["constant"]
      );
    }
  }
}
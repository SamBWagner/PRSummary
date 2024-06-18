using System.Net;
using RestSharp;
using Newtonsoft.Json.Linq;

const string token = "TOKEN HERE";
const string gitHubUserName = "NAME HERE";
const string date = "DATE HERE"; // format is YYYY-MM-DD
const string url = $"https://api.github.com/search/issues?q=is:pr+is:closed+author:{gitHubUserName}+merged:>{date}";

var client = new RestClient(url);
var request = new RestRequest(url);
        
request.AddHeader("Authorization", $"token {token}");
request.AddHeader("User-Agent", "CSharpApp");

var response = client.Execute(request);
        
if (response.StatusCode == HttpStatusCode.OK)
{
    if (response.Content is null)
    {
        Console.WriteLine("No content in response");
        return;
    }
    var data = JObject.Parse(response.Content);
    var items = data["items"];

    var prList = new List<Dictionary<string, string>>();

    if (items is null)
    {
        Console.WriteLine("No items in response");
        return;
    }
    foreach (var item in items)
    {
        if (item["title"] is null)
        {
            Console.WriteLine("Item is null");
            continue;
        }
        
        var pr = new Dictionary<string, string>
        {
            { "title", item["title"]!.ToString() }
        };
        prList.Add(pr);
    }

    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PRs.txt");

    using (var file = new StreamWriter(filePath))
    {
        foreach (var pr in prList)
        {
            file.WriteLine($"Title: {pr["title"]}");
            file.WriteLine(new string('-', 40));
        }
    }

    Console.WriteLine($"PR data has been written to: {filePath}");
}
else
{
    Console.WriteLine($"Error: {response.StatusCode}");
    Console.WriteLine(response.Content);
}
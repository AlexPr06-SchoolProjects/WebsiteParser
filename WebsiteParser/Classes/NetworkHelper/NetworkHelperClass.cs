using System.Net.NetworkInformation;

namespace WebsiteParser.Classes.NetworkHelper;

internal static class NetworkHelperClass
{
    public static async Task<bool> HasConnection()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return false;

        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(2);
            var result = await client.GetAsync("http://www.google.com");
            return result.IsSuccessStatusCode;
        } 
        catch {  return false; }
    }
}

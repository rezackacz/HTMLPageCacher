using System;
using System.Net;
using System.Net.Http;
namespace PageCacher {
  public class Program {
    public static void Main(string[] args) {
      if (args.Length == 0) { Console.WriteLine("Usage: PageCacher SERVER_IP CACHE_PATH CLASSROOM_GROUP_ID"); return; }

      Console.WriteLine(args[0]); string IP = args[0];
      Console.WriteLine(args[1]); string cache_path = args[1];
      Console.WriteLine(args[2]); string clsgroupid = args[2];

      UriBuilder uriBuilderJidelna = new UriBuilder();
      uriBuilderJidelna.Host = IP;
      uriBuilderJidelna.Port = 80;
      uriBuilderJidelna.Path = "jidelna.html";

      UriBuilder uriBuilderTimeTable = new();
      uriBuilderTimeTable.Path = "classroomGroup" + clsgroupid + ".html";
      uriBuilderTimeTable.Host = IP;
      uriBuilderTimeTable.Port = 80;


      HttpClient jidelna = new HttpClient();
      HttpClient clsGroup = new HttpClient();


      while (true) {
        HttpResponseMessage jidelnaResponse;
        HttpResponseMessage clsResponse;

        try {
          jidelnaResponse = jidelna.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderJidelna.Uri));
          clsResponse = clsGroup.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderTimeTable.Uri));
        } catch (Exception e) {
          Console.WriteLine("Download failed :(.");

          goto SLEEP_AND_REPEAT;
        }

        String jidelnaHTML = jidelnaResponse.Content.ReadAsStringAsync().Result;
        String clsHTML = clsResponse.Content.ReadAsStringAsync().Result;

        File.WriteAllText(cache_path + "/jidelna.html", jidelnaHTML);
        File.WriteAllText(cache_path + "/clsGroup.html", clsHTML);


      SLEEP_AND_REPEAT:
        Thread.Sleep(10000);
      }
    }
    private void NoNetwork(string cache_path) {
      if (File.Exists(cache_path + "/jidelna.html") && File.Exists(cache_path + "/clsGroup.html")) {

      } else {

      }



    }

  }
}
}
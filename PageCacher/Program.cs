using System;
using System.Net;
using System.Net.Http;
namespace PageCacher {
  public class Program {
    public static void Main(string[] args) {
      if (args.Length == 0) { Console.WriteLine("Usage: PageCacher SERVER_IP STORAGE_URL"); return; }

      Console.WriteLine(args[0]);
      Console.WriteLine(args[1]);
      Console.WriteLine(args[2]);

      UriBuilder uriBuilderJidelna = new UriBuilder();
      uriBuilderJidelna.Host = args[0];
      uriBuilderJidelna.Port = 80;
      uriBuilderJidelna.Path = "jidelna.html";

      UriBuilder uriBuilderTimeTable = uriBuilderJidelna; uriBuilderTimeTable.Path = "classroomGroup" + args[2] + ".html";


      HttpClient jidelna = new HttpClient();
      HttpClient clsGroup = new HttpClient();


      while (true) {
        HttpResponseMessage jidelnaResponse;
        HttpResponseMessage clsResponse;

        try {
          jidelnaResponse = jidelna.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderJidelna.Uri));
          clsResponse = clsGroup.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderTimeTable.Uri));
        } catch (Exception e) {
          goto SLEEP_AND_REPEAT;
        }

        String jidelnaHTML = jidelnaResponse.Content.ReadAsStringAsync().Result;
        String clsHTML = clsResponse.Content.ReadAsStringAsync().Result;

        File.WriteAllText(args[1] + "/jidelna.html", jidelnaHTML);
        File.WriteAllText(args[1] + "/clsGroup.html", clsHTML);
        

        SLEEP_AND_REPEAT:
        Thread.Sleep(10000);
      }

    }
  }
}
using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Net.Http;
namespace PageCacher {
  public class Program {

    private const string no_net_html = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"><title>NENÍ PŘIPOJENO</title></head><body style=\"display: flex;height: 100%;font-size: xxx-large;flex-flow: column;justify-content: center;font-family: sans-serif;text-align: center;\"><h1 id=\"main\">Připojení k serveru se nepodařilo.</h1><h2 style=\"font-family: monospace;text-align: center;\">Síť nemusí být k dispozici, nebo nastal problém se serverem.</h2><style type=\"text/css\"></style><deepl-input-controller></deepl-input-controller></body></html>";

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
          NetworklessRender(cache_path);
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
    private static void NetworklessRender(string cache_path) {
      if (File.Exists(cache_path + "/jidelna.html") && File.Exists(cache_path + "/clsGroup.html")) {
        string jidelnaHTML = File.ReadAllText(cache_path + "/jidelna.html");
        string clsGroupHTML = File.ReadAllText(cache_path + "/clsGroup.html");

        

      } else {
        File.WriteAllText(cache_path + "/jidelna.html", no_net_html);
        File.WriteAllText(cache_path + "/clsGroup.html", no_net_html);
      }



    }

  }
}
}
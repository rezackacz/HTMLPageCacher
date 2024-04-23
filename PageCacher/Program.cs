using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Web;
using System.Xml.Linq;
namespace PageCacher {
  public class Program {

    private const string no_net_html = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"/><title>NENÍ PŘIPOJENO</title></head><body style=\"display: flex;height: 100%;font-size: xxx-large;flex-flow: column;justify-content: center;font-family: sans-serif;text-align: center;\"><h1 id=\"main\">Připojení k serveru se nepodařilo.</h1><h2 style=\"font-family: monospace;text-align: center;\">Síť nemusí být k dispozici, nebo nastal problém se serverem.</h2><style type=\"text/css\"></style><deepl-input-controller></deepl-input-controller></body></html>";
    private static string svg_link = "";

    public static void Main(string[] args) {
      if (args.Length != 4) { Console.WriteLine("Usage: PageCacher SERVER_IP CACHE_PATH CLASSROOM_GROUP_ID SVG_URL"); return; }

      Console.WriteLine(args[0]); string IP = args[0];
      Console.WriteLine(args[1]); string cache_path = args[1];
      Console.WriteLine(args[2]); string clsgroupid = args[2];
      Console.WriteLine(args[3]); string svg_url = args[3];

      svg_link = "file:///" + cache_path + "/wifierror.svg";

      UriBuilder uriBuilderJidelna = new UriBuilder();
      uriBuilderJidelna.Host = IP;
      uriBuilderJidelna.Port = 80;
      uriBuilderJidelna.Path = "jidelna.html";

      UriBuilder uriBuilderTimeTable = new();
      uriBuilderTimeTable.Path = "classroomGroup" + clsgroupid + ".html";
      uriBuilderTimeTable.Host = IP;
      uriBuilderTimeTable.Port = 80;

      UriBuilder uriBuilderSVG = new();
      uriBuilderSVG.Path = svg_url;
      uriBuilderSVG.Host = IP;
      uriBuilderSVG.Port = 80;


      HttpClient jidelna = new HttpClient();
      HttpClient clsGroup = new HttpClient();
      HttpClient svgClient = new HttpClient();

      while (true) {
        HttpResponseMessage jidelnaResponse;
        HttpResponseMessage clsResponse;


        if (!File.Exists(cache_path + "/wifierror.svg")) {
          HttpResponseMessage svgResponse;
          try {
            svgResponse = svgClient.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderSVG.Uri));
          } catch (Exception ex) {
            NetworklessRender(cache_path);
            svgResponse = null;
          }

          if (svgResponse != null) {
            var svgAsBytes = svgResponse.Content.ReadAsByteArrayAsync().Result;
            File.WriteAllBytes(cache_path + "/wifierror.svg", svgAsBytes);
          }
        }

        try {
          jidelnaResponse = jidelna.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderJidelna.Uri));
          clsResponse = clsGroup.Send(new HttpRequestMessage(HttpMethod.Get, uriBuilderTimeTable.Uri));
        } catch (Exception e) {
          Console.WriteLine("Download failed. :(");
          NetworklessRender(cache_path);
          continue;
        }

        String jidelnaHTML = jidelnaResponse.Content.ReadAsStringAsync().Result;
        String clsHTML = clsResponse.Content.ReadAsStringAsync().Result;


        File.WriteAllText(cache_path + "/jidelna.html", jidelnaHTML);
        File.WriteAllText(cache_path + "/clsGroup.html", clsHTML);
      }
    }
    private static void NetworklessRender(string cache_path) {
      if (File.Exists(cache_path + "/jidelna.html") && File.Exists(cache_path + "/clsGroup.html")) {
        string jidelnaHTML = File.ReadAllText(cache_path + "/jidelna.html");
        string clsGroupHTML = File.ReadAllText(cache_path + "/clsGroup.html");

        string jidelnaHTMLsub = jidelnaHTML.Substring(jidelnaHTML.IndexOf("<body"), jidelnaHTML.IndexOf("</body>") - jidelnaHTML.IndexOf("<body") + "</body>".Length);
        string clsGroupHTMLsub = clsGroupHTML.Substring(clsGroupHTML.IndexOf("<body"), clsGroupHTML.IndexOf("</body>") - clsGroupHTML.IndexOf("<body") + "</body>".Length);

        XDocument jidelnaHtmlAsXDocument = XDocument.Parse(jidelnaHTMLsub);
        XDocument clsGroupAsXDocument = XDocument.Parse(clsGroupHTMLsub);

        XElement jidelnaBody = jidelnaHtmlAsXDocument.Root;

        XAttribute svgsrc = new("src", svg_link);
        XAttribute svgstyle = new("style", "left: 0;bottom: 0;position: absolute;height: 6vw;");
        XElement xESVG = new("img", svgsrc, svgstyle);

        jidelnaBody.Add(xESVG);
        jidelnaHtmlAsXDocument.Root.ReplaceWith(jidelnaBody);

        XElement clsBody = clsGroupAsXDocument.Root;
        clsBody.Add(xESVG);
        clsGroupAsXDocument.Root.ReplaceWith(clsBody);

        string jidelnaHTMLreplaced = jidelnaHTML.Replace(jidelnaHTMLsub, jidelnaBody.ToString());
        string clsGroupHTMLreplaced = clsGroupHTML.Replace(clsGroupHTMLsub, clsBody.ToString());

        File.WriteAllText(cache_path + "/jidelna.html", jidelnaHTMLreplaced);
        File.WriteAllText(cache_path + "/clsGroup.html", clsGroupHTMLreplaced);



        return;

      } else {
        File.WriteAllText(cache_path + "/jidelna.html", no_net_html);
        File.WriteAllText(cache_path + "/clsGroup.html", no_net_html);
      }



    }

  }
}

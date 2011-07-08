using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;


public static class Couch
{
    public static string Database
    {
        get { return ConfigurationManager.AppSettings["Couch.Database"]; }
    }

    public static Uri Uri
    {
        get { return new Uri(Database); }
    }



    public static string Put(this Uri uri, string path, string data)
    {
        var destination = new Uri(uri, path);
        return doRequest(destination, "PUT", data);
    }

    public static string Post(this Uri uri, string path, string data)
    {
        var destination = new Uri(uri, path);
        return doRequest(destination, "POST", data);
    }

    public static string Delete(this Uri uri, string id,string rev)
    {
        var destination = new UriBuilder(new Uri(uri,id));
        destination.Query = String.Format("rev={0}", rev);
        
        return doRequest(destination.Uri, "DELETE", "");
    }

    public static string Get(this Uri uri, string path)
    {
        var destination = new Uri(uri, path);
        var client = new WebClient();
        return client.DownloadString(destination);
    }

    static string doRequest(Uri uri, string method, string data)
    {
        
        var request = WebRequest.Create(uri);
        request.Method = method;

        var bytes = Encoding.UTF8.GetBytes(data);
        request.ContentType = "application/json; charset=utf-8";
        request.ContentLength = bytes.Length;

        var writer = request.GetRequestStream();
        writer.Write(bytes, 0, bytes.Length);
        writer.Close();

        var response = request.GetResponse();

        using (var reader = new StreamReader(response.GetResponseStream()))
        {
            var result = reader.ReadToEnd();
            response.Close();
            return result;
        }
    }
}

public class DocumentCollection<T> where T:class
{
    [JsonProperty("total_rows")]
    public int Count { get; set; }

    [JsonProperty("rows")]
    public IEnumerable<ViewDocument<T>> Items { get; set; }
}

public class ViewDocument<T> where T : class
{
    
    [JsonProperty("doc")]
    public T Item { get; set; }
}

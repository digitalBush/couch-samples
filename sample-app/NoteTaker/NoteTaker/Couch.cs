﻿using System;
using System.Collections.Generic;
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

    public static string Put<T>(this Uri uri, object id, T obj) {
        var json = JsonConvert.SerializeObject(obj);
        var destination = new Uri(uri, id.ToString());
        return doRequest(destination, "PUT", json);
    }


    public static string Delete(this Uri uri, object id,string rev)
    {
        var destination = new UriBuilder(new Uri(uri,id.ToString()));
        destination.Query = String.Format("rev={0}", rev);
        
        return doRequest(destination.Uri, "DELETE", "");
    }

    public static T Get<T>(this Uri uri, object id)
    {
        var destination = new Uri(uri, id.ToString());
        var client = new WebClient();
        var json= client.DownloadString(destination);
        return JsonConvert.DeserializeObject<T>(json);
    }

    static string doRequest(Uri uri, string method, string data)
    {   
        var request = WebRequest.Create(uri);
        request.Method = method;

        var bytes = Encoding.UTF8.GetBytes(data);
        request.ContentType = "application/json";
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
    public IEnumerable<ViewResult<T>> Items { get; set; }
}

public class ViewResult<T> where T : class
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("doc")]
    public T Document { get; set; }
}





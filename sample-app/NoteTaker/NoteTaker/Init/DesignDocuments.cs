using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace NoteTaker.Init
{
    public static class DesignDocuments
    {
        public static void Init()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var views = assembly.GetManifestResourceNames()
                .Where(n => n.EndsWith("json"));

            foreach(var view in views){
                var name = view.Remove(view.Length - 5).Split('.').Last();
                var resource = assembly.GetManifestResourceStream(view);
                string rev = FetchCurrentRevisionFor(name);
                string json = CreateJSON(resource,rev);
                Couch.Uri.Put("_design/" + name, json);   
            }
        }

        private static string FetchCurrentRevisionFor(string name)
        {
            try
            {
                var json=Couch.Uri.Get("_design/" + name);
                dynamic obj = JsonConvert.DeserializeObject(json);
                return obj._rev;
            }
            catch (WebException ex)
            {
                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if (statusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }   
        }

        private static string CreateJSON(Stream resource,string currentRevision)
        {
            var body = new StreamReader(resource).ReadToEnd();
            dynamic obj = JsonConvert.DeserializeObject(body);
            if (!String.IsNullOrEmpty(currentRevision))
                obj._rev = currentRevision;
            return JsonConvert.SerializeObject(obj);
        }
    }
}
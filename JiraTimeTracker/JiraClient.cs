using System;
using System.IO;
using System.Net;
using System.Text;

namespace JiraTimeTracker
{
    class JiraClient
    {
        private string _user;
        private string _pass;
        private string _encodedCredentials;

        private string EncodedCredentials
        {
            get
            {
                if (string.IsNullOrEmpty(_encodedCredentials))
                {
                    string mergedCredentials = string.Format("{0}:{1}", _user, _pass);
                    byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
                    _encodedCredentials = Convert.ToBase64String(byteCredentials);
                }
                return _encodedCredentials;
            }
        }

        public JiraClient(string user, string pass)
        {
            _user = user;
            _pass = pass;
        }

        public string RunQuery(string query, string argument = null, string data = null, string method = "GET")
        {
            var newRequest = WebRequest.Create(query) as HttpWebRequest;
            newRequest.ContentType = "application/json";
            newRequest.Method = method;

            if (data != null)
            {
                using (var writer = new StreamWriter(newRequest.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            newRequest.Headers.Add("Authorization", "Basic " + EncodedCredentials);

            HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }    
    }
}

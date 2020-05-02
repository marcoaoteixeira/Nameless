using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SystemUri = System.Uri;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public class HttpClient : IHttpClient {
        #region Private Constants

        private const string CONTENT_TYPE = "application/json";
        private const string HTTP_METHOD = "POST";

        #endregion

        #region Public Static Methods

        public static HttpWebRequest RequestFor (SystemUri uri) {
            Prevent.ParameterNull (uri, nameof (uri));

            var request = (HttpWebRequest)WebRequest.Create (uri);
            request.ContentType = CONTENT_TYPE;
            request.Method = HTTP_METHOD;
            if (!string.IsNullOrWhiteSpace (uri.UserInfo)) {
                request.Headers.Remove (HttpRequestHeader.Authorization);
                request.Headers.Add (HttpRequestHeader.Authorization, $"Basic {Convert.ToBase64String (Encoding.ASCII.GetBytes (uri.UserInfo))}");
            }
            return request;
        }

        #endregion

        #region Private Static Methods

        private static StreamWriter GetRequestStream (WebRequest request) {
            return new StreamWriter (request.GetRequestStream ());
        }

        private static string SerializeLogEvent (LogEvent evt) {
            return JsonConvert.SerializeObject (evt, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver (),
                Formatting = Formatting.Indented
            });
        }

        #endregion

        #region IHttpClient Members

        public void Post (Uri uri, LogEvent logEvent) {
            Prevent.ParameterNull (uri, nameof (uri));
            Prevent.ParameterNull (logEvent, nameof (logEvent));

            var request = RequestFor (uri);
            using (var writer = GetRequestStream (request)) {
                var json = SerializeLogEvent (logEvent);

                writer.Write (json);
                writer.Flush ();

                var response = (HttpWebResponse)request.GetResponse ();
                response.Close ();

                if (response.StatusCode != HttpStatusCode.Created) {
                    throw new WebException ($"Failed to post {logEvent.GetType ().Name} to {uri}.");
                }
            }
        }

        /// <summary>
        /// Post the events to the Elasticsearch _bulk API for faster inserts
        /// </summary>
        /// <param name="uri">Fully formed URI to the ES endpoint</param>
        /// <param name="logEvents">List of logEvents</param>
        public void PostBulk (Uri uri, IEnumerable<LogEvent> logEvents) {
            // For each logEvent, we build a bulk API request which consists of one line for
            // the action, one line for the document. In this case "index" (idempotent) and then the doc
            // Since we're appending _bulk to the end of the Uri, ES will default to using the
            // index and type already specified in the Uri segments
            var postBody = new StringBuilder ();
            foreach (var item in logEvents) {
                postBody.AppendLine ("{\"index\" : {} }");
                var json = SerializeLogEvent (item);
                postBody.AppendLine (json);
            }

            var request = RequestFor (uri);
            using (var writer = GetRequestStream (request)) {
                writer.Write (postBody.ToString ());
                writer.Flush ();

                var response = (HttpWebResponse)request.GetResponse ();
                response.Close ();

                if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK) {
                    throw new WebException ("Failed to post {postBody.ToString ()} to {uri}.");
                }
            }
        }

        #endregion
    }
}
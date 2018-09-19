using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using VideoIndexerExample.Models;

namespace VideoIndexerExample
{
    public class VIServiceWrapper
    {
        private string _accessToken { get; set; }
        private DateTime? _accessTokenExpires { get; set; }

        private string ApiKey { get; set; }
        public string Location { get; private set; }
        public string AccountId { get; private set; }
        private string ApiUrl { get; set; }

        private string AccessToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_accessToken) && _accessTokenExpires.HasValue
                                                        && _accessTokenExpires.Value > DateTime.UtcNow)
                    return _accessToken;
                var token = AsyncHelper.RunSync(GetAccessToken);
                if (!string.IsNullOrEmpty(token))
                {
                    _accessToken = token;
                    _accessTokenExpires = DateTime.UtcNow.AddMinutes(59);
                }

                return _accessToken;
            }
        }

        public VIServiceWrapper(string apiKey, string location, string accountId, string apiUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("ApiKey must not be empty.", nameof(apiKey));
            }

            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentException("Location must not be empty.", nameof(location));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("AccountId must not be empty.", nameof(accountId));
            }

            if (string.IsNullOrEmpty(apiUrl))
            {
                throw new ArgumentException("ApiUrl must not be empty.", nameof(apiUrl));
            }

            this.ApiKey = apiKey;
            this.Location = location;
            this.AccountId = accountId;
            this.ApiUrl = apiUrl;
        }
        
        #region Generic

        private async Task<HttpResponseMessage> ExecuteAsync(string uri, HttpMethod method,
            MultipartFormDataContent content = null)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            using (var client = new HttpClient())
            {
                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                if (content?.Headers.ContentLength.HasValue == true)
                {
                    var timeoutSecond = Convert.ToInt32(content.Headers.ContentLength.Value / 1000 / 1.25 / 1000);
                    client.Timeout = new TimeSpan(0, 0, timeoutSecond);
                }

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

                var fullUri = new Uri(new Uri(ApiUrl), uri);
                if (method == HttpMethod.Get)
                    response = await client.GetAsync(fullUri);
                else if (method == HttpMethod.Post)
                    response = await client.PostAsync(fullUri, content);
                else
                    response = await client.SendAsync(new HttpRequestMessage(method, fullUri));
            }

            return response;
        }

        #endregion

        public async Task<List<IndexerVideo>> GetAllVideos(int pageSize = 0, int skip = 0)
        {
            #region Querystring

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (pageSize > 0) queryString["pageSize"] = pageSize.ToString();

            if (skip > 0) queryString["skip"] = skip.ToString();

            #endregion

            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var content = string.Empty;
            queryString["accessToken"] = AccessToken;
            response = await ExecuteAsync($"{Location}/Accounts/{AccountId}/Videos?{queryString}", HttpMethod.Get);
            if (response?.Content != null) content = await response.Content.ReadAsStringAsync();

            var content1 = JsonConvert.DeserializeObject<dynamic>(content);
            var content2 = JsonConvert.DeserializeObject<IndexerVideo[]>(content1.results.ToString());
            return new List<IndexerVideo>(content2);
        }

        public async Task<string> GetInsightsHtml(string videoId, bool editable = false)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var content = string.Empty;
            queryString["allowEdit"] = editable ? "true" : "false";
            queryString["accessToken"] = await GetVideoAccessToken(videoId);
            response = await ExecuteAsync($"{Location}/Accounts/{AccountId}/Videos/{videoId}/InsightsWidget?{queryString}", HttpMethod.Get);
            if (response?.Content != null) content = await response.Content.ReadAsStringAsync();
            return string.Join(" ", Regex.Split(content, @"(?:\r\n|\n|\r)"));
        }

        public async Task<string> GetPlayerHtml(string videoId)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var content = string.Empty;
            queryString["accessToken"] = await GetVideoAccessToken(videoId);
            response = await ExecuteAsync($"{Location}/Accounts/{AccountId}/Videos/{videoId}/PlayerWidget?{queryString}", HttpMethod.Get);
            if (response?.Content != null) content = await response.Content.ReadAsStringAsync();
            return Regex.Replace(string.Join(" ", Regex.Split(Regex.Unescape(content), @"(?:\r\n|\n|\r)")), @"[\""]", "\"", RegexOptions.None);
        }


        #region Widgets 

        public async Task<string> GetEmbedWidgets(string videoId)
        {
            var player = await GetPlayerIFrame(videoId);
            var insights = await GetInsightsIFrame(videoId);
            return $"{player}{insights}{WidgetScript}";
        }

        public async Task<string> GetPlayerIFrame(string videoId, int width = 560, int height = 315, int border = 0,
            bool allowfullscreen = true)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["accessToken"] = await GetVideoAccessToken(videoId);
            return string.Format(PlayerIFrame, $"https://www.videoindexer.ai/embed/player/{AccountId}/{videoId}/?{queryString}", width, height, border, allowfullscreen ? "allowfullscreen" : string.Empty);
        }

        public async Task<string> GetInsightsIFrame(string videoId, int width = 680, int height = 780, int border = 0,
            bool allowfullscreen = true)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["accessToken"] = await GetVideoAccessToken(videoId);
            return string.Format(InsightsIFrame, $"https://www.videoindexer.ai/embed/insights/{AccountId}/{videoId}/?{queryString}", width, height, border, allowfullscreen ? "allowfullscreen" : string.Empty);
        }

        private const string WidgetScript = "<script src=\"https://breakdown.blob.core.windows.net/public/vb.widgets.mediator.js\"></script>";

        private const string PlayerIFrame = "<iframe width=\"{1}\" height=\"{2}\" src=\"{0}\" frameborder=\"{3}\" {4}></iframe>";

        private const string InsightsIFrame = "<iframe width=\"{1}\" height=\"{2}\" src=\"{0}\" frameborder=\"{3}\" {4}></iframe><script src=\"https://breakdown.blob.core.windows.net/public/vb.widgets.mediator.js\"></script>";

        #endregion

        #region Token Retrieval

        public async Task<string> GetAccessToken()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            using (var client = new HttpClient())
            {
                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                var fullUri = new Uri(new Uri(ApiUrl), $"auth/{Location}/Accounts/{AccountId}/AccessToken?allowEdit=true");

                response = await client.GetAsync(fullUri);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(responseContent);
        }

        public async Task<string> GetUserAccessToken()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            using (var client = new HttpClient())
            {
                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                var fullUri = new Uri(new Uri(ApiUrl), $"auth/{Location}/Users/Me/AccessToken?allowEdit=true");

                response = await client.GetAsync(fullUri);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(responseContent);
        }

        public async Task<string> GetVideoAccessToken(string videoId)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                ServicePointManager.MaxServicePointIdleTime = 5000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var fullUri = new Uri(new Uri(ApiUrl), $"auth/{Location}/Accounts/{AccountId}/Videos/{videoId}/AccessToken?allowEdit=true");
                try
                {
                    response = await client.GetAsync(fullUri);
                }
                catch (Exception err)
                {
                }
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent.Contains("BREAKDOWN_NOT_FOUND") ? "BREAKDOWN_NOT_FOUND" : JsonConvert.DeserializeObject<string>(responseContent);
        }

        #endregion
    }
}
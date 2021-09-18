using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using System.Security;

namespace ClickupInterface.Helpers
{
    internal class Empty { }

    internal class APIClient<TOut, TIn>
    {
        public string BaseURL { get; internal set; }
        public string CallURL { get; internal set; }
        public string Token { get; internal set; }
        public HttpStatusCode StatusCode { get; internal set; }
        public bool Succeded => StatusCode == HttpStatusCode.OK;
        public string ErrorString { get; internal set; }

        #region Constructors

        public APIClient(string baseURL, string callURL)
        {
            if (!Uri.IsWellFormedUriString(baseURL, UriKind.Absolute))
                throw new UriFormatException("Base URL is not formatted correctly!");
            if (!Uri.IsWellFormedUriString(callURL, UriKind.Relative))
                throw new UriFormatException("Call URL is not formatted correctly!");
            BaseURL = baseURL;
            CallURL = callURL;
        }

        public APIClient(string baseURL, string callURL, string token)
        {
            if (!Uri.IsWellFormedUriString(baseURL, UriKind.Absolute))
                throw new UriFormatException("Base URL is not formatted correctly!");
            if (!Uri.IsWellFormedUriString(callURL, UriKind.Relative))
                throw new UriFormatException("Call URL is not formatted correctly!");
            BaseURL = baseURL;
            CallURL = callURL;
            Token = token;
        }

        #endregion

        #region HTTP Get

        private async Task<TOut1> HTTPGet<TOut1,TIn1>(TIn1 inputModel)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseURL);

            SetAuthentication(httpClient);

            string querryURL = BuildBaseAndCallURL(CallURL);
            if (typeof(TIn1) != typeof(Empty))
                querryURL += QuerryfiContent(inputModel);
            HttpResponseMessage resp = await httpClient.GetAsync(querryURL);

            StatusCode = resp.StatusCode;

            return await ConvertContent<TOut1>(resp.Content);
        }

        public async Task<TOut> HTTPGet(TIn inputModel)
        {
            return await HTTPGet<TOut,TIn>(inputModel);
        }

        public async Task<TOut> HTTPGet()
        {
            return await HTTPGet<TOut, Empty>(new Empty());
        }

        #endregion

        #region HTTP Post

        private async Task<TOut1> HTTPPost<TOut1,TIn1>(TIn1 inputModel)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseURL);

            SetAuthentication(httpClient);

            HttpResponseMessage resp = await httpClient.PostAsync(BuildBaseAndCallURL(CallURL), SerializeContent(inputModel));

            StatusCode = resp.StatusCode;

            return await ConvertContent<TOut1>(resp.Content);
        }

        public async Task<TOut> HTTPPost(TIn inputModel)
        {
            return await HTTPPost<TOut, TIn>(inputModel);
        }

        public async Task<TOut> HTTPPost()
        {
            return await HTTPPost<TOut, Empty>(new Empty());
        }

        #endregion

        #region HTTP Patch

        private async Task<TOut1> HTTPPatch<TOut1, TIn1>(TIn1 inputModel)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseURL);

            SetAuthentication(httpClient);

            HttpResponseMessage resp = await httpClient.PatchAsync(BuildBaseAndCallURL(CallURL), SerializeContent(inputModel));

            StatusCode = resp.StatusCode;

            return await ConvertContent<TOut1>(resp.Content);
        }

        public async Task<TOut> HTTPPatch(TIn inputModel)
        {
            return await HTTPPatch<TOut, TIn>(inputModel);
        }

        public async Task<TOut> HTTPPatch()
        {
            return await HTTPPatch<TOut, Empty>(new Empty());
        }

        #endregion

        #region HTTP Delete

        private async Task<TOut1> HTTPDelete<TOut1, TIn1>(TIn1 inputModel)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseURL);

            SetAuthentication(httpClient);

            string querryURL = BuildBaseAndCallURL(CallURL);
            if (typeof(TIn1) != typeof(Empty))
                querryURL += QuerryfiContent(inputModel);
            HttpResponseMessage resp = await httpClient.DeleteAsync(querryURL);

            StatusCode = resp.StatusCode;

            return await ConvertContent<TOut1>(resp.Content);
        }

        public async Task<TOut> HTTPDelete(TIn inputModel)
        {
            return await HTTPDelete<TOut, TIn>(inputModel);
        }

        public async Task<TOut> HTTPDelete()
        {
            return await HTTPDelete<TOut, Empty>(new Empty());
        }

        #endregion

        #region Private Methods

        private void SetAuthentication(HttpClient httpClient)
        {
            if (Token != null)
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Token);
        }

        private string BuildBaseAndCallURL(string callName)
        {
            string url = BaseURL.ToString();
            if (callName != "")
            {
                if (url[url.Length - 1] == '/')
                {
                    if (callName[0] == '/')
                    {
                        callName = callName.Substring(1, callName.Length - 1);
                        return $"{url}{callName}";
                    }
                }
                else
                {
                    if (callName[0] != '/')
                    {
                        return $"{url}/{callName}";
                    }
                }
            }
            return $"{url}{callName}";
        }

        private string QuerryfiContent<T3>(T3 model)
        {
            var query = HttpUtility.ParseQueryString("");
            Type modelTypeInfo = model.GetType();

            if (!IsPrimitive(model))
            {
                foreach (PropertyInfo propertyInfo in modelTypeInfo.GetProperties())
                {
                    object value = propertyInfo.GetValue(model, null);
                    if (value != null)
                        query[propertyInfo.Name] = value.ToString();
                }
            }
            else
                throw new TargetParameterCountException("Please wrap in model before using parameters!");

            return $"?{query}";
        }

        private bool IsPrimitive<T>(T value)
        {
            Type modelTypeInfo = value.GetType();

            return modelTypeInfo.IsPrimitive || modelTypeInfo.IsValueType || (modelTypeInfo == typeof(string));
        }

        #endregion

        #region Convert Functions

        private async Task<T> ConvertContent<T>(HttpContent content)
        {
            if (StatusCode == HttpStatusCode.OK)
            {
                Stream contentStream = await content.ReadAsStreamAsync();
                return await ConvertContentInner<T>(content, contentStream);
            }
            else
            {
                ErrorString = await content.ReadAsStringAsync();
                throw new HttpListenerException((int)StatusCode, ErrorString);
            }
        }

        private async Task<T> ConvertContentInner<T>(HttpContent content, Stream contentStream)
        {
            if (typeof(T) == typeof(string))
            {
                string retStr = "";
                using (StreamReader strr = new StreamReader(contentStream))
                    retStr = strr.ReadToEnd();
                return (dynamic)retStr;
            }
            return await JsonSerializer.DeserializeAsync<T>(contentStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        private dynamic SerializeContent<T>(T model)
        {
            return new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
        }

        #endregion
    }
}

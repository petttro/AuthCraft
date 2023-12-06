using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using AuthCraft.Common.Extensions;

namespace AuthCraft.Data.Extensions;

public class ResponseContentModel<T>
{
    private string _errorDetails;

    public ResponseContentModel(HttpStatusCode status, string errorDetails = null)
    {
        Status = status;
        ErrorDetails = errorDetails;
    }

    [JsonIgnore]
    public T Content { get; set; }

    [JsonIgnore]
    public HttpResponseHeaders Headers { get; set; }

    [JsonIgnore]
    public HttpContentHeaders ContentHeaders { get; set; }

    [JsonIgnore]
    public HttpStatusCode Status { get; set; }

    public string ErrorDetails
    {
        get
        {
            // Nothing to return in error details if the request was successful
            if (Success)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_errorDetails))
            {
                return ErrorCode.GetEnumDescription();
            }

            return _errorDetails;
        }

        set => _errorDetails = value;
    }

    [JsonIgnore]
    public bool Success
    {
        get
        {
            return Status == HttpStatusCode.OK
                   || Status == HttpStatusCode.Created
                   || Status == HttpStatusCode.Accepted
                   || Status == HttpStatusCode.NoContent
                   || Status == HttpStatusCode.NotModified;
        }
    }

    [JsonIgnore]
    public Enum ErrorCode { get; set; }
}

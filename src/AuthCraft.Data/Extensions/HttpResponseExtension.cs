using System.Net;
using AuthCraft.Common;
using AuthCraft.Common.Extensions;

namespace AuthCraft.Data.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<ResponseContentModel<TResponse>> Deserialize<TResponse>(this HttpResponseMessage httpResponseMessage)
    {
        var result = new ResponseContentModel<TResponse>(httpResponseMessage.StatusCode)
        {
            Headers = httpResponseMessage.Headers,
            ContentHeaders = httpResponseMessage.Content?.Headers
        };

        if (httpResponseMessage.Content == null)
        {
            return result;
        }

        if (httpResponseMessage.Content.Headers.ContentLength.HasValue && httpResponseMessage.Content.Headers.ContentLength.Value == 0)
        {
            result.ErrorDetails = await httpResponseMessage.Content.ReadAsStringAsync();
            return result;
        }

        var responseContentType = httpResponseMessage.Content?.Headers?.ContentType?.MediaType;

        var receivedContent = await httpResponseMessage.Content.ReadAsStringAsync();

        try
        {
            if (!string.IsNullOrWhiteSpace(responseContentType) && responseContentType.Equals(Constants.ContentType.ApplicationJson))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    result.Content = receivedContent.JsonDeserialize<TResponse>();
                }
                else
                {
                    result.ErrorDetails = receivedContent;
                }
            }
            else
            {
                result.Status = HttpStatusCode.ExpectationFailed;
                result.ErrorDetails = "UNABLE TO DESERIALIZE: " + receivedContent;
            }
        }
        catch (Exception)
        {
            result.Status = HttpStatusCode.InternalServerError; // keeping 500
            result.ErrorDetails = receivedContent;
        }

        return result;
    }
}

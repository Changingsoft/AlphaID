using System.Net.Http.Headers;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace IntegrationTestUtilities.Helpers;

/// <summary>
/// Helpers for HTML document.
/// </summary>
public class HtmlHelpers
{
    /// <summary>
    /// Get document from Http Response Message.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        IDocument document = await BrowsingContext.New()
            .OpenAsync(ResponseFactory, CancellationToken.None);
        return (IHtmlDocument)document;

        void ResponseFactory(VirtualResponse htmlResponse)
        {
            htmlResponse
                .Address(response.RequestMessage?.RequestUri)
                .Status(response.StatusCode);

            MapHeaders(response.Headers);
            MapHeaders(response.Content.Headers);

            htmlResponse.Content(response.Content.ReadAsStream());

            void MapHeaders(HttpHeaders headers)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                foreach (string value in header.Value)
                    htmlResponse.Header(header.Key, value);
            }
        }
    }
}
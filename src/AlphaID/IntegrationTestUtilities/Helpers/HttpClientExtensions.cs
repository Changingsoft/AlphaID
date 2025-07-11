using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System.Diagnostics.CodeAnalysis;
using HttpMethod = System.Net.Http.HttpMethod;

namespace IntegrationTestUtilities.Helpers;

/// <summary>
/// Extensions for HttpClient.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Send Post form data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="form"></param>
    /// <param name="submitButton"></param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> SendAsync(
        this HttpClient client,
        IHtmlFormElement form,
        IHtmlElement submitButton)
    {
        return client.SendAsync(form, submitButton, []);
    }

    /// <summary>
    /// Send post form data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="form"></param>
    /// <param name="formValues"></param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> SendAsync(
        this HttpClient client,
        IHtmlFormElement form,
        IEnumerable<KeyValuePair<string, string>> formValues)
    {
        IElement submitElement = Assert.Single(form.QuerySelectorAll("[type=submit]"));
        var submitButton = Assert.IsAssignableFrom<IHtmlElement>(submitElement);

        return client.SendAsync(form, submitButton, formValues);
    }

    /// <summary>
    /// Send post form data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="form"></param>
    /// <param name="submitButton"></param>
    /// <param name="formValues"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static Task<HttpResponseMessage> SendAsync(
        this HttpClient client,
        IHtmlFormElement form,
        IHtmlElement submitButton,
        IEnumerable<KeyValuePair<string, string>> formValues)
    {
        foreach (KeyValuePair<string, string> kvp in formValues)
        {
            var element = Assert.IsAssignableFrom<IHtmlInputElement>(form[kvp.Key]);
            element.Value = kvp.Value;
        }

        DocumentRequest submit = form.GetSubmission(submitButton) ?? throw new InvalidOperationException("Submission is null.");
        var target = (Uri)submit.Target;
        if (submitButton.HasAttribute("formaction"))
        {
            string formAction = submitButton.GetAttribute("formaction")!;
            target = new Uri(formAction, UriKind.Relative);
        }

        var submission = new HttpRequestMessage(new HttpMethod(submit.Method.ToString()), target)
        {
            Content = new StreamContent(submit.Body)
        };

        foreach (KeyValuePair<string, string> header in submit.Headers)
        {
            submission.Headers.TryAddWithoutValidation(header.Key, header.Value);
            submission.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return client.SendAsync(submission);
    }
}
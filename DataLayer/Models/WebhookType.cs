namespace MyAppDemo.DataLayer.Models;

/// <summary>
/// Specifies the type of webhook integration supported by the system.
/// </summary>
/// <remarks>This enumeration is used to identify the source or platform of a webhook.  It can be used to
/// configure or handle webhook events based on their origin.</remarks>
 public enum WebhookType
{
    PowerAutomate,
    GitHub,
    Perplexity
}
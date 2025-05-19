using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Responses
{
    public class WebhookResponse
    {

        /// <summary>
        /// Action success.
        /// </summary>
        [Display(Name = "Success", Description = "Action success.")]
        public bool? Success { get; set; }

        /// <summary>
        /// Message response
        /// </summary>
        [Display(Name = "Message", Description = "Message response.")]
        public string? Message { get; set; }

        /// <summary>
        /// Flow identification name of your choice.
        /// </summary>
        [Display(Name = "FlowId", Description = "Unique Flow Identifier in Power Automate.")]
        public string? FlowId { get; set; }

        /// <summary>
        /// URL to delete the Webhook.
        /// </summary>
        [Display(Name = "Location", Description = "URL to delete the Webhook.")]
        public string? Location { get; set; }

    }
}

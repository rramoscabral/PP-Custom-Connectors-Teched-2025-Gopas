using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Responses
{
    public class HelloResponse
    {
        /// <summary>
        /// Message response
        /// </summary>
        [Display(Name = "Message", Description = "Message response.")]
        public string Message { get; set; }
    }
}

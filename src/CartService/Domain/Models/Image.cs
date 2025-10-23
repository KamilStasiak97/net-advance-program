namespace CartService.Domain.Models
{
    /// <summary>
    /// Represents an optional image with URL and alt text
    /// </summary>
    public class Image
    {
        public string Url { get; set; } = string.Empty;
        public string? AltText { get; set; }

        public Image()
        {
        }

        public Image(string url, string? altText = null)
        {
            Url = url;
            AltText = altText;
        }
    }
}

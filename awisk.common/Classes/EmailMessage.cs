namespace awisk.common.Classes
{
    public class EmailMessage
    {
        public List<string> To { get; set; } = [];
        public List<string> Cc { get; set; } = [];
        public List<string> Bcc { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public List<EmailAttachment> Attachments { get; set; } = [];
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public Stream Content { get; set; } = Stream.Null;
        public string ContentType { get; set; } = "application/octet-stream";
    }
}

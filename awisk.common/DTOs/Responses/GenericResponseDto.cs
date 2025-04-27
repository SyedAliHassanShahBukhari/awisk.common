using System.Net;

namespace awisk.common.DTOs.Responses
{
    public partial class GenericResponseDto<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Response { get; set; }
    }
}

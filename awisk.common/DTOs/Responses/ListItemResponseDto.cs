namespace awisk.common.DTOs.Responses
{
    public class ListItemResponseDto<T>
    {
        public T Id { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}

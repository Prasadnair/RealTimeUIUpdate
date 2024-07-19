namespace DealUpdateService.Models
{
    public record DealStatusUpdateRequest
    {
        public string DealId { get; init; }
        public string NewStatus { get; init; }
    }
}

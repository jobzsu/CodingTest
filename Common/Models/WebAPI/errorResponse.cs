namespace Common.Models.WebAPI;

public class errorResponse
{
    public string Message { get; set; }

    public List<errorDetail> Details { get; set; }
}

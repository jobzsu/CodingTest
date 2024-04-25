namespace Common.Models.WebAPI;

public class Error
{
    public string Message { get; set; }

    public List<ErrorDetail> Details { get; set; }
}

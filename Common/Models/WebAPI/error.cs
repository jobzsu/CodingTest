namespace Common.Models.WebAPI;

public class error
{
    public string Message { get; set; }

    public List<errorDetail> Details { get; set; }
}

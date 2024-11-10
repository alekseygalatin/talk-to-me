namespace TalkToMe.Handlers;

public class AgenQuestionResponseModel
{
    public string Name { get; set; }
    public Parameters Parameters { get; set; }
}

public class Parameters
{
    public string Question { get; set; }
    public string? Topic { get; set; }
}
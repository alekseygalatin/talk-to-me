namespace TalkToMe.Core.Interfaces
{
    public interface IAIProvider
    {
        IAiModelService GetModel(string modelName);
    }
}

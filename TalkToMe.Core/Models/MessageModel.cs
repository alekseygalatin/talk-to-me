using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Core.Models;

public class MessageModel
{
    public long Timestamp { get; set; }
    
    public ChatRole Role { get; set; }
    
    public string Message { get; set; }
}
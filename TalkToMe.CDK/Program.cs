using Amazon.CDK;

namespace TalkToMe.CDK;

sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
            
        new AgentsStack(app, "AgentsStack", new StackProps
        {
            Env = new Amazon.CDK.Environment
            {
                Account = "038462779690",
                Region = "us-east-1",
            }
        });
            
        app.Synth();
    }
}
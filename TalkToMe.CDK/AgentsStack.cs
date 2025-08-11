using System.Reflection;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AwsBedrock;
using Constructs;

namespace TalkToMe.CDK;

public class AgentsStack : Stack
{
    private const string FoundationModel =
        "arn:aws:bedrock:us-east-1:038462779690:inference-profile/us.anthropic.claude-3-5-sonnet-20241022-v2:0";
    
    internal AgentsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        // Create IAM role for Bedrock agents
        var agentRole = CreateBedrockAgentRole();

        // Create guardrails for content safety
        var guardrail = CreateGuardrail();

        // Create text generator agents (collaborators)
        var textGeneratorSwedish = CreateTextGeneratorAgent("Swedish", agentRole, guardrail);

        // Create comprehension analyzer agents (collaborators)
        var analyzerSwedish = CreateComprehensionAnalyzerAgent("Swedish", agentRole, guardrail);

        // Create supervisor agent with multi-agent collaboration
        var supervisor = CreateSupervisorAgent(agentRole, guardrail, new[]
        {
            (textGeneratorSwedish.alias, "TextGenerator",
                "TextGenerator skapar svenska texter för språkträning. Använd för textgenerering på svenska, anpassad efter användarens nivå och intresse."),
            // (textGeneratorEnglish.alias, "TextGeneratorEnglish",
            //     "TextGenerator creates English texts for language training. Use for English text generation, adapted to user's level and interests."),
            (analyzerSwedish.alias, "Analyzer",
                "ComprehensionAnalyzer analyserar svenska texter och återberättelser. Använd för att ställa fördjupande frågor om svensk text och återberättelse."),
            // (analyzerEnglish.alias, "AnalyzerEnglish",
            //     "ComprehensionAnalyzer analyzes English texts and retellings. Use for asking in-depth questions about English text and retelling.")
        });

        // Stack outputs
        new CfnOutput(this, "SupervisorAgentId", new CfnOutputProps
        {
            Value = supervisor.agent.AttrAgentId,
            Description = "ID of the Text Comprehension Supervisor Agent"
        });

        new CfnOutput(this, "SupervisorAgentAliasId", new CfnOutputProps
        {
            Value = supervisor.alias.AttrAgentAliasId,
            Description = "Alias ID of the Text Comprehension Supervisor Agent"
        });

        new CfnOutput(this, "TextGeneratorSwedishId", new CfnOutputProps
        {
            Value = textGeneratorSwedish.agent.AttrAgentId,
            Description = "ID of the Swedish Text Generator Agent"
        });

        new CfnOutput(this, "ComprehensionAnalyzerSwedishId", new CfnOutputProps
        {
            Value = analyzerSwedish.agent.AttrAgentId,
            Description = "ID of the Swedish Comprehension Analyzer Agent"
        });
    }

    private Role CreateBedrockAgentRole()
    {
        return new Role(this, "BedrockAgentRole", new RoleProps
        {
            RoleName = "AmazonBedrockExecutionRoleForAgents_TextComprehension",
            AssumedBy = new ServicePrincipal("bedrock.amazonaws.com"),
            ManagedPolicies = new[]
            {
                ManagedPolicy.FromAwsManagedPolicyName("AmazonBedrockFullAccess")
            },
            InlinePolicies = new Dictionary<string, PolicyDocument>
            {
                ["BedrockAgentInvokePolicy"] = new PolicyDocument(new PolicyDocumentProps
                {
                    Statements = new[]
                    {
                        new PolicyStatement(new PolicyStatementProps
                        {
                            Effect = Effect.ALLOW,
                            Actions = new[]
                            {
                                "bedrock:InvokeModel",
                                "bedrock:InvokeModelWithResponseStream"
                            },
                            Resources = new[] { "*" }
                        })
                    }
                })
            }
        });
    }

    private (CfnGuardrail guardrail, CfnGuardrailVersion version) CreateGuardrail()
    {
        var guardrail = new CfnGuardrail(this, "TextComprehensionGuardrail", new CfnGuardrailProps
        {
            Name = "TextComprehensionGuardrail",
            BlockedInputMessaging = "Detta innehåll är inte tillåtet i vår språkinlärningsmiljö.",
            BlockedOutputsMessaging = "Jag kan inte tillhandahålla den typen av innehåll för språkinlärning.",
            Description = "Content safety guardrail for text comprehension agents",
            ContentPolicyConfig = new CfnGuardrail.ContentPolicyConfigProperty
            {
                FiltersConfig = new[]
                {
                    new CfnGuardrail.ContentFilterConfigProperty
                    {
                        Type = "SEXUAL",
                        InputStrength = "HIGH",
                        OutputStrength = "HIGH"
                    },
                    new CfnGuardrail.ContentFilterConfigProperty
                    {
                        Type = "VIOLENCE",
                        InputStrength = "MEDIUM",
                        OutputStrength = "MEDIUM"
                    },
                    new CfnGuardrail.ContentFilterConfigProperty
                    {
                        Type = "HATE",
                        InputStrength = "HIGH",
                        OutputStrength = "HIGH"
                    },
                    new CfnGuardrail.ContentFilterConfigProperty
                    {
                        Type = "INSULTS",
                        InputStrength = "HIGH",
                        OutputStrength = "HIGH"
                    }
                }
            },
            WordPolicyConfig = new CfnGuardrail.WordPolicyConfigProperty
            {
                ManagedWordListsConfig = new[]
                {
                    new CfnGuardrail.ManagedWordsConfigProperty
                    {
                        Type = "PROFANITY"
                    }
                }
            }
        });

        var guardrailVersion = new CfnGuardrailVersion(this, "GuardrailVersion", new CfnGuardrailVersionProps
        {
            GuardrailIdentifier = guardrail.AttrGuardrailId
        });

        return (guardrail, guardrailVersion);
    }

    private (CfnAgent agent, CfnAgentAlias alias) CreateTextGeneratorAgent(string language, Role agentRole,
        (CfnGuardrail guardrail, CfnGuardrailVersion version) guardrail)
    {
        var instructions = GetEmbeddedResource($"text-generator-{language.ToLower()}.txt");

        var agent = new CfnAgent(this, $"TextGenerator{language}", new CfnAgentProps
        {
            AgentName = $"TextGenerator{language}",
            Instruction = instructions,
            FoundationModel = FoundationModel,
            AgentResourceRoleArn = agentRole.RoleArn,
            Description = $"Generates {language} texts for language learning and comprehension practice",
            IdleSessionTtlInSeconds = 1800, // 30 minutes
            AutoPrepare = true,
            GuardrailConfiguration = new CfnAgent.GuardrailConfigurationProperty
            {
                GuardrailIdentifier = guardrail.guardrail.AttrGuardrailId,
                GuardrailVersion = guardrail.version.AttrVersion
            },
            MemoryConfiguration = new CfnAgent.MemoryConfigurationProperty
            {
                EnabledMemoryTypes = new[] { "SESSION_SUMMARY" },
                StorageDays = 30,
                SessionSummaryConfiguration = new CfnAgent.SessionSummaryConfigurationProperty
                {
                    MaxRecentSessions = 20
                }
            }
        });

        var alias = new CfnAgentAlias(this, $"TextGenerator{language}Alias", new CfnAgentAliasProps
        {
            AgentId = agent.AttrAgentId,
            AgentAliasName = $"Generator{language}Alias",
            Description = $"Alias for {language} Text Generator Agent"
        });

        return (agent, alias);
    }

    private (CfnAgent agent, CfnAgentAlias alias) CreateComprehensionAnalyzerAgent(string language, Role agentRole,
        (CfnGuardrail guardrail, CfnGuardrailVersion version) guardrail)
    {
        var instructions = GetEmbeddedResource($"comprehension-analyzer-{language.ToLower()}.txt");

        var agent = new CfnAgent(this, $"ComprehensionAnalyzer{language}", new CfnAgentProps
        {
            AgentName = $"ComprehensionAnalyzer{language}",
            Instruction = instructions,
            FoundationModel = FoundationModel,
            AgentResourceRoleArn = agentRole.RoleArn,
            Description = $"Analyzes {language} text comprehension and asks follow-up questions",
            IdleSessionTtlInSeconds = 1800, // 30 minutes
            AutoPrepare = true,
            GuardrailConfiguration = new CfnAgent.GuardrailConfigurationProperty
            {
                GuardrailIdentifier = guardrail.guardrail.AttrGuardrailId,
                GuardrailVersion = guardrail.version.AttrVersion
            },
            MemoryConfiguration = new CfnAgent.MemoryConfigurationProperty
            {
                EnabledMemoryTypes = new[] { "SESSION_SUMMARY" },
                StorageDays = 30,
                SessionSummaryConfiguration = new CfnAgent.SessionSummaryConfigurationProperty
                {
                    MaxRecentSessions = 20
                }
            }
        });

        var alias = new CfnAgentAlias(this, $"ComprehensionAnalyzer{language}Alias", new CfnAgentAliasProps
        {
            AgentId = agent.AttrAgentId,
            AgentAliasName = $"Analyzer{language}Alias",
            Description = $"Alias for {language} Comprehension Analyzer Agent"
        });

        return (agent, alias);
    }

    private (CfnAgent agent, CfnAgentAlias alias) CreateSupervisorAgent(Role agentRole,
        (CfnGuardrail guardrail, CfnGuardrailVersion version) guardrail,
        (CfnAgentAlias alias, string name, string instruction)[] collaborators)
    {
        var instructions = GetEmbeddedResource("supervisor.txt");

        var agent = new CfnAgent(this, "TextComprehensionSupervisor", new CfnAgentProps
        {
            AgentName = "TextComprehensionSupervisor",
            Instruction = instructions,
            FoundationModel = FoundationModel,
            AgentResourceRoleArn = agentRole.RoleArn,
            Description = "Coordinates text comprehension training workflow with multi-agent collaboration",
            IdleSessionTtlInSeconds = 3600, // 60 minutes
            AutoPrepare = true,
            AgentCollaboration = "SUPERVISOR",
            GuardrailConfiguration = new CfnAgent.GuardrailConfigurationProperty
            {
                GuardrailIdentifier = guardrail.guardrail.AttrGuardrailId,
                GuardrailVersion = guardrail.version.AttrVersion
            },
            MemoryConfiguration = new CfnAgent.MemoryConfigurationProperty
            {
                EnabledMemoryTypes = new[] { "SESSION_SUMMARY" },
                StorageDays = 30,
                SessionSummaryConfiguration = new CfnAgent.SessionSummaryConfigurationProperty
                {
                    MaxRecentSessions = 20
                }
            },
            AgentCollaborators = collaborators.Select(c => new CfnAgent.AgentCollaboratorProperty
            {
                AgentDescriptor = new CfnAgent.AgentDescriptorProperty
                {
                    AliasArn = c.alias.AttrAgentAliasArn
                },
                CollaboratorName = c.name,
                CollaborationInstruction = c.instruction,
                RelayConversationHistory = "TO_COLLABORATOR"
            }).ToArray()
        });

        var alias = new CfnAgentAlias(this, "TextComprehensionSupervisorAlias", new CfnAgentAliasProps
        {
            AgentId = agent.AttrAgentId,
            AgentAliasName = "TextComprehensionSupervisorAlias",
            Description = "Alias for Text Comprehension Supervisor Agent"
        });

        return (agent, alias);
    }

    private string GetEmbeddedResource(string fileName)
    {
        var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var filePath = Path.Combine(baseDirectory, "Resources", "Instructions", fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File {filePath} not found");

        return File.ReadAllText(filePath);
    }
}
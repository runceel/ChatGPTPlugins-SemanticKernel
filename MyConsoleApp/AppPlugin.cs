using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyConsoleApp;
internal class AppPlugin
{
    public static async Task<AppPlugin> CreateAsync(IKernel pluginKernel)
    {
        await pluginKernel.ImportChatGptPluginSkillFromUrlAsync(
            "Todos",
            new Uri("https://localhost:7062/.well-known/ai-plugin.json"),
            new());
        return new AppPlugin(pluginKernel);
    }

    private ActionPlanner _planner;

    public AppPlugin(IKernel pluginsKernel)
    {
        _planner = new ActionPlanner(pluginsKernel);
    }

    [SKFunction("ChatGPT プラグインのなかから最適なものを選んで結果を返します。")]
    [SKFunctionName("Call")]
    public async Task<string> CallAsync(string userIntent, SKContext context)
    {
        try
        {
            var plan = await _planner.CreatePlanAsync(userIntent);
            if (!plan.HasNextStep)
            {
                return "";
            }

            var planResultContext = await plan.InvokeAsync();
            var planResult = JsonSerializer.Deserialize<PluginResult>(planResultContext.Result);
            var lastStep = plan.Steps.Last();
            return planResult != null ? $"""
                {lastStep.SkillName}プラグインの{lastStep.Name}関数の実行結果(書式: {planResult.ContentType})
                {planResult.Content}
                """ :
                "";
        }
        catch
        {
            return "";
        }
    }
}

file class PluginResult
{
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = "";
    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
}


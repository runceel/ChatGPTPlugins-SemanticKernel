using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using MyConsoleApp;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>()
    .Build();

var endpoint = configuration.GetValue<string>("Endpoint");
var deployName = configuration.GetValue<string>("DeployName");

if (string.IsNullOrWhiteSpace(endpoint) || !endpoint.StartsWith("https://") || string.IsNullOrWhiteSpace(deployName))
{
    Console.WriteLine("appsettings.json の Endpoint と DeployName を設定してください。");
    Console.WriteLine($"現在の値: Endpoint = {endpoint}, DeployName = {deployName}");
    return;
}

// Chat のやりとりをするためのカーネルを作成
var chatKernel = Kernel.Builder
    .WithAzureChatCompletionService(deployName,
        endpoint,
        new AzureCliCredential())
    .Build();

// 現在時刻を表示するための組み込みスキルを読み込み
chatKernel.ImportSkill(new TimeSkill(), "time");

// ユーザーの意図を汲み取るセマンティック関数を作成
var detectUserIntentFunction = chatKernel.CreateSemanticFunction("""
    あなたは、以下のチャットの履歴を読んでユーザーの最後のメッセージの時点でユーザーがやりたいと思っていることを簡潔に書いてください。
    また、チャット履歴から既に達成されている内容は含めないで最後のチャット時点でやりたいと思っていることだけを書いてください。
    その文章を読むだけで、やらなければいけないことや、必要な情報が全てわかるようにしてください。
    チャット履歴に無いことは含めないでください。
    チャットの履歴から、ユーザーが最後のメッセージ時点でやりたいことが無い場合には「無い」と書いてください。

    ### 参考情報
    - 今日の日付: {{time.Today}}

    ### チャット履歴
    {{$input}}

    ### 最後のメッセージの時点でユーザーがやりたいと思っていること
    
    """);

// チャットの返答を生成するためのセマンティック関数を作成
var generateAssistantMessageFunction = chatKernel.CreateSemanticFunction("""
    あなたは日本語でやりとりをする Assistant です。User のチャットに対して回答をしてください。
    回答の際にはチャットの履歴と、参考情報にある内容を考慮して回答してください。
    チャット履歴と参考情報に同じデータがある場合は参考情報の方を正しいデータとして参照してください。
    チャットの履歴と参考情報にないことを想像して書かないでください。
    知らない内容の場合には「わかりません」と回答してください。
    回答の最後に独立した行で使用したプラグイン名と使用した関数を "> 使用プラグイン: プラグイン名.関数名" のように記載してください。無い場合は不要です。

    ### 参考情報
    - 今日の日付: {{time.Today}}
    - ユーザーの意図: {{$userIntent}}
    - {{AppPlugin.Call $userIntent}}

    ### チャット履歴
    {{$input}}
    Assistant: 
    """,
    stopSequences: new[] { "User:", "Assistant:" },
    maxTokens: 512);

// ChatGPT Plugins を読み込むための自作プラグインを登録
var pluginKernel = Kernel.Builder
    .WithAzureChatCompletionService(deployName,
        endpoint,
        new AzureCliCredential())
    .Build();
chatKernel.ImportSkill(await AppPlugin.CreateAsync(pluginKernel), "AppPlugin");

// 延々とチャットを繰り返す
var chatHistory = new List<string>();
while (true)
{
    Console.Write("User: ");
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line)) { break; }
    if (string.Equals(line, "reset", StringComparison.InvariantCultureIgnoreCase)) 
    {
        Console.WriteLine("Assistant: 過去のチャット履歴を削除しました。");
        chatHistory.Clear();
        continue; 
    }

    chatHistory.Add($"User: {line}");

    // ユーザーの入力内容から意図を汲み取る
    var context = chatKernel.CreateNewContext();
    context.Variables.Update(CreateChatHisotryMessages(chatHistory));
    var userIntent = await detectUserIntentFunction.InvokeAsync(context);
    Console.WriteLine($"文脈を踏まえた上でのユーザーの意図: {userIntent.Result}");

    // 汲み取った意図を元に ChatGPT Plugins からの情報を埋め込んだプロンプトで回答を生成
    var chatContext = chatKernel.CreateNewContext();
    chatContext.Variables.Update(CreateChatHisotryMessages(chatHistory));
    chatContext.Variables.Set(nameof(userIntent), userIntent.Result);
    var generatedAssistantMessage = await generateAssistantMessageFunction.InvokeAsync(chatContext);
    chatHistory.Add($"Assistant: {generatedAssistantMessage.Result}");
    Console.WriteLine($"Assistant: {generatedAssistantMessage.Result}");

    if (chatHistory.Count > 21) { chatHistory.RemoveAt(0); }
}

static string CreateChatHisotryMessages(List<string> chatHistory) => string.Join("\n", chatHistory);

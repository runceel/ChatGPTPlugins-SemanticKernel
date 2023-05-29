﻿using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using MyConsoleApp;

// Chat のやりとりをするためのカーネルを作成
var chatKernel = Kernel.Builder
    .Configure(conf =>
    {
        conf.AddAzureChatCompletionService(
            "gpt-35-turbo",
            "https://ai-kazuki.openai.azure.com/",
            new AzureCliCredential());
    })
    .Build();

// 現在時刻を表示するための組み込みスキルを読み込み
chatKernel.ImportSkill(new TimeSkill(), "time");

// ユーザーの意図を汲み取るセマンティック関数を作成
var detectUserIntentFunction = chatKernel.CreateSemanticFunction("""
    あなたは、以下のチャットの履歴を読んでユーザーの最後のメッセージの時点でユーザーがお願いしたいと思っていることを書いてください。
    ユーザーがお願いしたいことはチャット履歴と参考情報から読み取って、やりたいことをするために必要な全ての情報を含めた文章にしてください。
    その文章を読むだけで、やらなければいけないことや、必要な情報が全てわかるようにしてください。
    チャット履歴に無いことは含めないでください。
    チャットの履歴から、既にお願いしたいことが達成されている場合は「無し」と回答してください。
    ユーザーがお願いしたいことが無い場合は「無し」と回答してください。

    ### 参考情報
    - 今日の日付: {{time.Today}}

    ### チャット履歴
    {{$input}}

    ### ユーザーがお願いしたいこと
    
    """);

// チャットの返答を生成するためのセマンティック関数を作成
var generateAssistantMessageFunction = chatKernel.CreateSemanticFunction("""
    あなたは Assistant です。User のチャットに対して回答をしてください。
    回答の際にはチャットの履歴と、参考情報にある内容を考慮して回答してください。
    知らない内容の場合には「わかりません」と回答してください。
    回答の最後に独立した行で、参考情報として使用したプラグインがあれば使用したプラグイン名と使用した関数を "> 使用プラグイン: プラグイン名.関数名" のように記載してください。

    ### 参考情報
    - 今日の日付: {{time.Today}}
    - {{AppPlugin.Call $userIntent}}

    ### チャット履歴
    {{$input}}
    Assistant: 
    """,
    stopSequences: new[] { "User:", "Assistant:" },
    maxTokens: 512);

// ChatGPT Plugins を読み込むための自作プラグインを登録
var pluginKernel = Kernel.Builder
    .Configure(conf =>
    {
        conf.AddAzureChatCompletionService(
            "gpt-35-turbo",
            "https://ai-kazuki.openai.azure.com/",
            new AzureCliCredential());
    })
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
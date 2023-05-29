# ChatGPT Plugins を Semantic Kernel で使うコンソールアプリ

## フォルダ構造

- TodoPlugin
  - TODO リストを管理する ChatGPT Plugins。以下のような TODO がデフォルトで登録されている。
	```json
	[
        {
            "text": "トイレットペーパーを買いに行く"
        },
        {
            "text": "牛乳を買いに行く"
        },
        {
            "text": "島根への旅行の日程を決める"
        },
        {
            "text": "2023年7月に出るスタバの新作が出たら食べる"
        }
    ]
    ```

- MyConsoleApp
  - TodoPlugin を読み込んで動くコンソールアプリのチャットプログラム。上記の TODO リストのプラグインを必要に応じて呼び出して動作する。
    
    動作例
    ```
    User: TODOの一覧を表示して。
    文脈を踏まえた上でのユーザーの意図: TODOリストの一覧を表示する。
    Assistant: 以下が現在のTODOリストです。

    - トイレットペーパーを買いに行く
    - 牛乳を買いに行く
    - 島根への旅行の日程を決める
    - 2023年7月に出るスタバの新作が出たら食べる

    以上、ご確認ください。

    > 使用プラグイン: Todos.getTodos()
    User: "納豆を買いに行く"をTODOに追加して
    文脈を踏まえた上でのユーザーの意図: ユーザーは、TODOリストに「納豆を買いに行く」を追加したいと思っています。
    Assistant: 「納豆を買いに行く」をTODOリストに追加しました。

    以下が現在のTODOリストです。

    - トイレットペーパーを買いに行く
    - 牛乳を買いに行く
    - 島根への旅行の日程を決める
    - 2023年7月に出るスタバの新作が出たら食べる
    - 納豆を買いに行く

    以上、ご確認ください。

    > 使用プラグイン: Todos.createTodo()
    User: 買わないといけない商品名だけを一覧で出して。
    文脈を踏まえた上でのユーザーの意図: 現在のTODOリストから、買わなければいけない商品名だけを一覧で表示してほしい。
    Assistant: 以下が現在のTODOリストから、買わなければいけない商品名だけの一覧です。

    - トイレットペーパーを買いに行く
    - 牛乳を買いに行く
    - 納豆を買いに行く

    以上、ご確認ください。

    > 使用プラグイン: Todos.getTodos()
    User:
    ```

## 動かし方

### 必要環境

- Visual Studio 2022 or .NET 7 SDK
- Azure OpenAI Service のリソースを作成した Azure サブスクリプションがある
- Azure CLI で Azure OpenAI Service のある Azure サブスクリプションへサインインを行っておく

### 設定

- MyConsoleApp\appsettings.json を自分の Azure OpenAI Service のリソースのエンドポイントと gpt-35-turbo のデプロイ名に更新する
- Azure OpenAI Service のあるリソースグループに自分のアカウントに対して Cognitive Service OpenAI User のロールを追加する

### 実行

- TodoPlugin プロジェクトを実行する
  - dotnet run による実行かデバッグ無しで開始して起動しっぱなしにする
- MyConsoleApp プロジェクトを実行する

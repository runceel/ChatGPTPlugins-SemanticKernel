using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddServer(new() { Url = "https://localhost:7062" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.EnableAnnotations();
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

var todos = new List<TodoItem>()
{
    new("トイレットペーパーを買いに行く"),
    new("牛乳を買いに行く"),
    new("島根への旅行の日程を決める"),
    new("2023年7月に出るスタバの新作が出たら食べる"),
};

app.MapGet("/todos", () => todos.ToArray())
    .WithName("getTodos")
    .WithDescription("全てのTODOを返します。")
    .WithOpenApi();

app.MapPost("/todos", (HttpRequest req, [FromBody, SwaggerRequestBody("TODO Item")] TodoItem body) =>
    {
        todos.Add(body);
        return Results.Accepted(value: body);
    })
    .WithName("createTodo")
    .WithDescription("TODOを作成します。")
    .WithOpenApi(op =>
    {
        op.RequestBody.Description = """
        TODO Item. { "text": "Todo item title" }
        """;
        return op;
    });

app.Run();


[SwaggerSchema("TODO Item", Required = new[] { nameof(Text) })]
class TodoItem
{
    public TodoItem(string text)
    {
        Text = text;
    }

    [Required]
    [SwaggerSchema("TODO のタイトル")]
    public string Text { get; set; }
}
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
    new("�g�C���b�g�y�[�p�[�𔃂��ɍs��"),
    new("�����𔃂��ɍs��"),
    new("�����ւ̗��s�̓��������߂�"),
    new("2023�N7���ɏo��X�^�o�̐V�삪�o����H�ׂ�"),
};

app.MapGet("/todos", () => todos.ToArray())
    .WithName("getTodos")
    .WithDescription("�S�Ă�TODO��Ԃ��܂��B")
    .WithOpenApi();

app.MapPost("/todos", (HttpRequest req, [FromBody, SwaggerRequestBody("TODO Item")] TodoItem body) =>
    {
        todos.Add(body);
        return Results.Accepted(value: body);
    })
    .WithName("createTodo")
    .WithDescription("TODO���쐬���܂��B")
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
    [SwaggerSchema("TODO �̃^�C�g��")]
    public string Text { get; set; }
}
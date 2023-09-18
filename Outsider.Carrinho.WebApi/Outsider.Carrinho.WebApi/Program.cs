var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGroup("/carrinho/v1")
    .MapCarrinhoApiV1()
    .WithTags("Carrinho Endpoints");

app.Run();

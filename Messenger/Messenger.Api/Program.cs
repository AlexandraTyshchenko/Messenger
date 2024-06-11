using System.Reflection;
using MediatR;
using Messenger.Business.Extensions;
using Messenger.Infrastructure.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddBusinessServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();//todo ????????
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();//todo ???????? ?? ??????

app.MapControllers();

app.Run();

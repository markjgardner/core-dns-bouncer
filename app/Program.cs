using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using k8s;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#if DEBUG
builder.Services.AddSingleton<IKubernetes>(new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile()));
#else
builder.Services.AddSingleton<IKubernetes>(new Kubernetes(KubernetesClientConfiguration.InClusterConfig()));
#endif

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

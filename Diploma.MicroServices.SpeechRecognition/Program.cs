using Diploma.Core.Common;
using Diploma.MicroServices.SpeechRecognition.Services.Implementations;
using Diploma.MicroServices.SpeechRecognition.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddCutomAuthentication(builder.Configuration);
builder.Services.AddScoped<ISpeechRecognitionService, SpeechRecognitionService>();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.Run();

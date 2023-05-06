using EGNLogic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/egngenerator", (DateOnly date, int gender, int region) =>
{
    var egn = new EGNGenerator(date: date, gender: (Gender)gender, region: (Region)region)
    .GenerateEGN();
    return egn;
})
    .WithName("GetEGN")
    .WithSummary("Generates EGN")
    .WithOpenApi();

app.MapGet("/egncheck", (string egn) =>
{
    return EGNGenerator.CheckValidity(egn);
})
    .WithName("CheckEGN")
    .WithSummary("Check EGN Validity")
    .WithOpenApi();

app.MapGet("/egndata", (string egn) =>
{
    EGNGenerator.TryParse(egn, out var egndata);

    return egndata;
})
    .WithName("EGNData")
    .WithSummary("Give data on EGN")
    .WithOpenApi();

app.Run();

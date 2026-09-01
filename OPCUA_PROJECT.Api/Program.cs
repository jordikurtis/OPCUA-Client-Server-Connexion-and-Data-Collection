using Microsoft.Extensions.DependencyInjection;
using OPCUA_PROJECT.Api.Repositories;
using OPCUA_PROJECT.Api.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//partie ajouter -- ici pour MachineGroupe , PlcConfig , MonitoredVariables , Measurements
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton<IMachineGroupRepository>(new MachineGroupRepository(connectionString!));

builder.Services.AddSingleton<IPlcConfigRepository>(new PlcConfigRepository(connectionString!));

builder.Services.AddSingleton<IMonitoredVariableRepository>(new MonitoredVariableRepository(connectionString!));

builder.Services.AddSingleton<IMeasurementRepository>(new MeasurementRepository(connectionString!));

builder.Services.AddScoped<IMachineGroupService,MachineGroupService>();

builder.Services.AddScoped<IMonitoredVariableService,MonitoredVariableService>();

builder.Services.AddScoped<IMeasurementService,MeasurementService>();

builder.Services.AddScoped<IPlcConfigService,PlcConfigService>();

// --- jusqu'a ici 
//---> ajouter 
// Nouveau CORS : necessaire dees que le frontend appellera cette API depuis le navigateur
// aurotorise sera le (localhist : 30000) 
// port par defaut , pour REACT - creat-react-app / Vite - restreindre en Porduction plus tard 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

});
//---jusqu'a ici 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontendDev"); // ajouter -- 
app.UseAuthorization();

app.MapControllers();

app.Run();

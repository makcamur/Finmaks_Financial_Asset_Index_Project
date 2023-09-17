
using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.Api.Services.Concrete;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFinmaksApiService, FinmaksApiService>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("https://localhost:7028").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers();
//T�m originlere, t�m headerlara ve t�m metotlara izin verdik.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var a = services.GetRequiredService<IFinmaksApiService>();
    var b = await a.GetFinmaksExchangeRates(DateTime.Now);

}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



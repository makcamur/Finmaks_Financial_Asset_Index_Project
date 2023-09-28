
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Finmaks_Financial_Asset_Index_Project.Api.Services;
using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.Api.Services.Concrete;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFinmaksApiService, FinmaksApiService>();
builder.Services.AddScoped<IUnitOfWorksRepository, UnitOfWorkRepository>();
builder.Services.AddScoped<IExchangeRepository, ExchangeRepository>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("https://localhost:7028").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers();
//T�m originlere, t�m headerlara ve t�m metotlara izin verdik.
builder.Services.AddHangfire(configuration => configuration
    .UseStorage(new MemoryStorage())); // Bellek tabanl� depolama kullanabilirsiniz, ger�ek projelerde ba�ka bir depolama se�ene�i tercih edebilirsiniz.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// db kurulmas�
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        if (!dbContext.Database.CanConnect()) // Veritaban�na ba�lan�lam�yorsa
        {
            dbContext.Database.Migrate(); // Veritaban�n� olu�tur veya g�ncelle          
            var finmaksApiService = services.GetRequiredService<IFinmaksApiService>();
            var lastdateInterval = DateTime.Now; 
            finmaksApiService.MakeExchangesUpToDate(lastdateInterval);
        }
    }
    catch (Exception ex)
    {
        // Hata y�netimi burada yap�labilir
    }
}

// Hangfire'� ba�lat�n
app.UseHangfireDashboard(); // Hangfire Dashboard'� kullanmak isterseniz
app.UseHangfireServer();
// Hangfire g�revini tan�mlay�n (�rne�in her g�n saat 00:00'da �al��acak)
RecurringJob.AddOrUpdate<MyDailyJob>("my-daily-job", x => x.Execute(), Cron.Minutely);
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



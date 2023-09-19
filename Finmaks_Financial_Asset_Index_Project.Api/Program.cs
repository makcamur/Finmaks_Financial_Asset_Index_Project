
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
//Tüm originlere, tüm headerlara ve tüm metotlara izin verdik.
builder.Services.AddHangfire(configuration => configuration
    .UseStorage(new MemoryStorage())); // Bellek tabanlý depolama kullanabilirsiniz, gerçek projelerde baþka bir depolama seçeneði tercih edebilirsiniz.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Hangfire'ý baþlatýn
app.UseHangfireDashboard(); // Hangfire Dashboard'ý kullanmak isterseniz
app.UseHangfireServer();
// Hangfire görevini tanýmlayýn (örneðin her gün saat 00:00'da çalýþacak)
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



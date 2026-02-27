using Application.Services;
using Infrastructure.Hubs;
using Infrastructure.Services;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("GeminiSettings"));

builder.Services.Configure<FileSettings>(
    builder.Configuration.GetSection("FileSettings"));

// SignalR
builder.Services.AddSignalR();

// Services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

var app = builder.Build();

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

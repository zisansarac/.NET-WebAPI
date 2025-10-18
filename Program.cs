using first_.NET_project.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// burada web uygulamasını başlat demiş olduk.

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ZisanConnection")));
// burada veritabanı bağlantısını sağladık.
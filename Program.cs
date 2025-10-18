using first_.NET_project.Data;
using first_.NET_project.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// burada web uygulamasını başlat demiş olduk.

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ZisanConnection")));
// burada veritabanı bağlantısını sağladık.
// dedik ki bizim veri tabanı bağlantımız b, sen veri tabanını oluşturacaksın adına app.db diyeceksin. Oluşturduğun veri tabanının bilgileri AppDbContext'te olacak dedik.

builder.Services.AddIdentityCore<AppUser>(
    options =>
    {
        options.User.RequireUniqueEmail = true;
        //her emailden sadece bir tane olmak zorunda.
        options.Password.RequiredLength = 6;
        //minimum şifre uzunluğu 6 karakter olacak.
        options.Password.RequireNonAlphanumeric = false;
        //şifre içerisinde özel karakterler zorunlu olmasın.
        options.Password.RequireDigit = false;
        //şifre içerisinde rakam zorunlu olmasın.
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    }
);
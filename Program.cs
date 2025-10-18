using System.Text;
using first_.NET_project.Data;
using first_.NET_project.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
).AddEntityFrameworkStores<AppDbContext>().AddSignInManager();
//giriş sistemini ayarladık ve sonra verileri sakladık, ve girişi sağlayan otomatik fonksiyonu ekledik.

//JWT ayarları burada yapılacak.
var jwt = builder.Configuration.GetSection("JWTSettings");

//key i alıp byte dizisine çeviriyoruz.
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

//giriş sistemini jwt ye göre yapıyoruz.
//varsayılan kimlik doğrulamasını jwt ile yap. Yani token le yap. Biz cookie ile değil token ile doğrulama yapacağız.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme
    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero
    };
});

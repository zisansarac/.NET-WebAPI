using System.Security.Claims;
using first_.NET_project.Data;
using first_.NET_project.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace first_.NET_project.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BlogController : ControllerBase
{
    private readonly AppDbContext _db;

    public BlogController(AppDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]  //giriş yapmadan eriş

    public async Task<ActionResult<IEnumerable<BlogResponse>>> List(
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 10,
          //endpointlerin her sayfada kaç tane olacağını belirler
          [FromQuery] string? q = null,
          // search yaparken kullanacağım(metin)
          [FromQuery] string? authorId = null,
          // search yaparken yazar araması
          [FromQuery] bool onlyPublished = true
    // sadece yayınlanmış olan veriler gösterilsin
    )
    {
        // geçersiz sayfa olmasın, olursa 1 de sabit kalsın
        if (page < 1) page = 1;

        // sayfa boyutunu güvenli bir aralıkta tutmak gerekiyor
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        //queryable yapısıyla veritabanının içerisinden ilerle
        var query = _db.BlogPosts.AsQueryable();

        // git veri tabanında published olanları getir
        if (onlyPublished)
            query = query.Where(b => b.IsPublished);

        // eğer arama metni, içeriği doluysa, title veya content ler arasında q ya yazılanı bul eşleştirmeye çalış 
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(b => b.Title.Contains(q) || b.Content.Contains(q));

        //yazar filtresi
        if (!string.IsNullOrWhiteSpace(authorId))
            query = query.Where(b => b.AuthorId == authorId);

        //artık sorguyu uygulayacağız:
        // en yeni içerik en üstte olsun
        var items = await query
        .OrderByDescending(b => b.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Include(b => b.Author)
        .ToListAsync();

        // dto dan bilgileri alıyoruz
        var result = items.Select(b => new BlogResponse
        {
            Id = b.Id,
            Title = b.Title,
            Slug = b.Slug,
            Content = b.Content,
            IsPublished = b.IsPublished,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            AuthorId = b.AuthorId,
            AuthorEmail = b.Author?.Email,
            AuthorFullName = b.Author?.FullName,
        });
        return Ok(result);

    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<BlogResponse>>GetBySlug(string slug)
    {
        var b = await _db.BlogPosts
        .Include(x => x.Author)
        .FirstOrDefaultAsync(x => x.Slug == slug);

        if (b is null) return NotFound();

        if (!b.IsPublished)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid != b.AuthorId) return Forbid();
        }

        return Ok(new BlogResponse
        {
            Id = b.Id,
            Title = b.Title,
            Slug = b.Slug,
            Content = b.Content,
            IsPublished = b.IsPublished,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            AuthorId = b.AuthorId,
            AuthorEmail = b.Author?.Email,
            AuthorFullName = b.Author?.FullName,
        });
    }
}

using System.Security.Claims;
using first_.NET_project.Data;
using first_.NET_project.Dtos;
using first_.NET_project.Models;
using first_.NET_project.Utils;
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
    public async Task<ActionResult<BlogResponse>> GetBySlug(string slug)
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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BlogResponse>> Create([FromBody] BlogCreateRequest dto)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var baseSlug = SlugHelper.ToSlug(dto.Title);
        var slug = baseSlug;
        int i = 2;
        while (await _db.BlogPosts.AnyAsync(x => x.Slug == slug))
        {
            slug = $"{baseSlug}-{i}";
            i++;

        }
        var entity = new BlogPost
        {
            Title = dto.Title,
            Slug = slug,
            Content = dto.Content,
            IsPublished = dto.IsPublished,
            AuthorId = uid,
            CreatedAt = DateTime.UtcNow
        };
        //burdan gelen veriyi veri tabanına ekliyor.
        _db.BlogPosts.Add(entity);
        await _db.SaveChangesAsync();

        var resp = new BlogResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Content = entity.Content,
            IsPublished = entity.IsPublished,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            AuthorId = entity.AuthorId
        };
        return CreatedAtAction(nameof(GetBySlug), new { slug = entity.Slug }, resp);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] BlogUpdateRequest dto)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var entity = await _db.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        if (entity.AuthorId != uid) return Forbid();

        if (!string.Equals(entity.Title, dto.Title, StringComparison.Ordinal))
        {
            var baseSlug = SlugHelper.ToSlug(dto.Title);
            var slug = baseSlug;
            int i = 2;
            while (await _db.BlogPosts.AnyAsync(x => x.Slug == slug))
            {
                slug = $"{baseSlug}-{i}";
                i++;

            }

            entity.Slug = slug;
            entity.Title = dto.Title;
        }
        else
        {
            entity.Title = dto.Title;
        }

        entity.Content = dto.Content;
        entity.IsPublished = dto.IsPublished;
        entity.UpdatedAt = DateTime.UtcNow;

        _db.BlogPosts.Update(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var entity = await _db.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        if (entity.AuthorId != uid) return Forbid();

        _db.BlogPosts.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();

    }    
}

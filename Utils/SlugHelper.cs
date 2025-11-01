using System.Text.RegularExpressions;

namespace first_.NET_project.Utils;

public static class SlugHelper
{
    public static string ToSlug(string text)
    {
        text = text.ToLowerInvariant().Trim();
        // tüm harfleri küçük harfe dönüştür ve başta sonda boşluk varsa kaldır.

        //türkçe karakterler dönüştürülecek
        text = text
        .Replace("ş", "s").Replace("Ş", "s")
        .Replace("ı", "i").Replace("İ", "i")
        .Replace("ğ", "g").Replace("Ğ", "g")
        .Replace("ü", "u").Replace("Ü", "u")
        .Replace("ö", "o").Replace("Ö", "o")
        .Replace("ç", "c").Replace("Ç", "c");

        // sadece harf rakam boşluk tre kullanılsın
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        //boşluk da olmamalı
        text = Regex.Replace(text, @"\s+", "-").Trim('-');
        text = Regex.Replace(text, @"-+", "-");

        return text;   
    }

}

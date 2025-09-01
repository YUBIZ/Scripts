using System.Text.Json.Serialization;

public readonly record struct Url([property: JsonPropertyName("loc")] string Location, [property: JsonPropertyName("lastmod")] string LastModified, [property: JsonPropertyName("changefreq")] string ChangeFrequency, [property: JsonPropertyName("priority")] string Priority);

public readonly record struct UrlSet([property: JsonPropertyName("url")] Url[] Urls);
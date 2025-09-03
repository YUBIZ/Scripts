using System.Runtime.Serialization;

[DataContract(Name = "url", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public readonly record struct Url([property: DataMember(Name = "loc", Order = 0)] string Location, [property: DataMember(Name = "lastmod", Order = 1)] string LastModified, [property: DataMember(Name = "changefreq", EmitDefaultValue = false, Order = 2)] string ChangeFrequency, [property: DataMember(Name = "priority", EmitDefaultValue = false, Order = 3)] string Priority);

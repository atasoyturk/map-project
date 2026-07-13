using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Annotation;

[Table("tbl_annotation")]
public sealed class Annotation : BaseEntity
{
    public string   NoteText { get; set; } = string.Empty;
    public Geometry Geometry { get; set; } = null!;
    public int      UserId   { get; set; }

    // Snapshot: notun yazıldığı andaki takımı sabitler.
    // Kullanıcı sonradan takım değiştirse bile not eski takımda kalır (audit/tarihsel doğruluk).
    // User.TeamId üzerinden JOIN ile TÜRETİLMİYOR — bilinçli denormalizasyon.
    public int?      TeamId   { get; set; }
}
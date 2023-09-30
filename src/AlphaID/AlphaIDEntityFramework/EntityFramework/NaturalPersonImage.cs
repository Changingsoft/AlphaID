using IDSubjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaIDEntityFramework.EntityFramework;

[Table("NaturalPersonImage")]
internal class NaturalPersonImage
{
    protected NaturalPersonImage() { }

    public NaturalPersonImage(string id, string photoMimeType, byte[] photo)
    {
        this.Id = id;
        this.PhotoMimeType = photoMimeType;
        this.Photo = photo;
    }

    [Key]
    [MaxLength(50), Unicode(false)]
    [ForeignKey(nameof(Person))]
    public string Id { get; protected set; } = default!;

    [MaxLength(50), Unicode(false)]
    public string PhotoMimeType { get; set; } = default!;

    public byte[] Photo { get; set; } = default!;

    public virtual NaturalPerson Person { get; protected set; } = default!;
}

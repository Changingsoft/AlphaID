using IDSubjects;

namespace AlphaIDEntityFramework.EntityFramework;
public class NaturalPersonImageStore : INaturalPersonImageStore
{
    private readonly IDSubjectsDbContext dbContext;

    public NaturalPersonImageStore(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task ClearPhotoAsync(NaturalPerson person)
    {
        var img = await this.dbContext.PersonImages.FindAsync(person.Id);
        if (img != null)
        {
            this.dbContext.PersonImages.Remove(img);
            _ = await this.dbContext.SaveChangesAsync();
        }
    }

    public async Task<(string MimeType, byte[] ImageContent)?> GetPhotoAsync(NaturalPerson person)
    {
        var img = await this.dbContext.PersonImages.FindAsync(person.Id);
        return img == null ? null : new(img.PhotoMimeType, img.Photo);
    }

    public async Task SetPhotoAsync(NaturalPerson person, string mimeType, byte[] imageContent)
    {
        var img = await this.dbContext.PersonImages.FindAsync(person.Id);
        if (img == null)
        {
            img = new NaturalPersonImage(person.Id, mimeType, imageContent);
            this.dbContext.PersonImages.Add(img);
        }
        else
        {
            img.PhotoMimeType = mimeType;
            img.Photo = imageContent;
            this.dbContext.PersonImages.Update(img);
        }
        await this.dbContext.SaveChangesAsync();
    }


}

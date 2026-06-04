using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

public class AttachmentMockRepository
{
    private readonly ApplicationDbContext _context;

    public AttachmentMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Attachment> GetAll() => _context.Attachments.OrderByDescending(attachment => attachment.CreatedAt).ToList();

    public Attachment? GetById(int id) => _context.Attachments.FirstOrDefault(attachment => attachment.Id == id);

    public List<Attachment> GetByEntity(string entityType, int entityId) => _context.Attachments
        .Where(attachment => attachment.EntityType == entityType && attachment.EntityId == entityId)
        .OrderByDescending(attachment => attachment.CreatedAt)
        .ToList();

    public void Add(Attachment attachment)
    {
        _context.Attachments.Add(attachment);
        _context.SaveChanges();
    }

    public void Delete(Attachment attachment)
    {
        _context.Attachments.Remove(attachment);
        _context.SaveChanges();
    }
}

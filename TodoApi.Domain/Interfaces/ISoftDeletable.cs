namespace TodoApi.Domain.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}

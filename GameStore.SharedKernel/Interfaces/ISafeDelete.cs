namespace GameStore.SharedKernel.Interfaces;

public interface ISafeDelete
{
    public bool IsDeleted { get; set; }
}
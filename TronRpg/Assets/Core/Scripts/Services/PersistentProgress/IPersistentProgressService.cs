using Core.Scripts.Data;

namespace Core.Scripts.Services.PersistentProgress
{
    public interface IPersistentProgressService
    {
        PlayerProgress Progress { get; set; }
    }
}
using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    // Interfaz del repositorio de alineaciones.
    // Hereda del repositorio genérico para reutilizar operaciones CRUD básicas.
    public interface IMatchLineupRepository : IGenericRepository<MatchLineup>
    {
        // Obtiene todos los jugadores registrados
        // en la alineación de un partido específico.
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

        // Verifica si un jugador ya fue registrado
        // en la alineación de un partido.
        Task<bool> ExistsPlayerInMatchAsync(int matchId, int playerId);

        // Cuenta cuántos jugadores titulares tiene un equipo
        // dentro de un partido.
        Task<int> CountStartersAsync(int matchId, int teamId);
    }
}
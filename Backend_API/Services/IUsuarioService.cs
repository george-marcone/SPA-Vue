using form_API.ViewModels;
using System.Security.Claims;

namespace form_API.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioSummaryViewModel[]> GetAllAsync();
        Task<UsuarioSummaryViewModel?> GetByIdAsync(int usuarioId);
        Task<UsuarioSummaryViewModel> AddAsync(UsuarioCreateViewModel viewModel, ClaimsPrincipal? usuarioAtual = null);
        Task<UsuarioSummaryViewModel?> UpdateAsync(int usuarioId, UsuarioUpdateViewModel viewModel, ClaimsPrincipal? usuarioAtual = null);
        Task<bool> DeleteAsync(int usuarioId);
        Task<PerfilViewModel[]> GetPerfisAsync(ClaimsPrincipal? usuarioAtual = null);
    }
}

using System.Security.Claims;
using form_API.Data;
using form_API.Models;
using form_API.Security;
using form_API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace form_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DataContext _context;

        public UsuarioService(DataContext context)
        {
            _context = context;
        }

        public async Task<UsuarioSummaryViewModel[]> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(usuario => usuario.Perfil)
                .AsNoTracking()
                .OrderBy(usuario => usuario.Nome)
                .Select(usuario => new UsuarioSummaryViewModel
                {
                    IdUsuario = usuario.IdUsuario,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Telefone = usuario.Telefone,
                    IdPerfil = usuario.IdPerfil,
                    DescricaoPerfil = usuario.Perfil == null ? string.Empty : usuario.Perfil.DescricaoPerfil
                })
                .ToArrayAsync();
        }

        public async Task<UsuarioSummaryViewModel?> GetByIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Perfil)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            return usuario.ToSummary();
        }

        public async Task<UsuarioSummaryViewModel> AddAsync(
            UsuarioCreateViewModel viewModel,
            ClaimsPrincipal? usuarioAtual = null)
        {
            var email = NormalizeEmail(viewModel.Email);
            var emailJaCadastrado = await _context.Usuarios
                .AnyAsync(usuario => usuario.Email.ToLower() == email);

            if (emailJaCadastrado)
            {
                throw new InvalidOperationException("Email ja cadastrado.");
            }

            await EnsurePerfilExisteAsync(viewModel.IdPerfil);
            EnsureCanCreate(viewModel.IdPerfil, usuarioAtual);

            var usuario = new Usuario
            {
                Nome = viewModel.Nome.Trim(),
                Email = email,
                Telefone = viewModel.Telefone.Trim(),
                Senha = PasswordHasher.HashPassword(DefaultPasswordPolicy.DefaultPassword),
                IdPerfil = viewModel.IdPerfil
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var created = await _context.Usuarios
                .Include(u => u.Perfil)
                .AsNoTracking()
                .FirstAsync(u => u.IdUsuario == usuario.IdUsuario);

            return created.ToSummary()!;
        }

        public async Task<UsuarioSummaryViewModel?> UpdateAsync(
            int usuarioId,
            UsuarioUpdateViewModel viewModel,
            ClaimsPrincipal? usuarioAtual = null)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            if (usuario == null)
            {
                return null;
            }

            EnsureCanUpdate(usuario, viewModel, usuarioAtual);

            var email = NormalizeEmail(viewModel.Email);
            var emailJaCadastrado = await _context.Usuarios
                .AnyAsync(u => u.IdUsuario != usuarioId && u.Email.ToLower() == email);

            if (emailJaCadastrado)
            {
                throw new InvalidOperationException("Email ja cadastrado.");
            }

            if (viewModel.IdPerfil.HasValue)
            {
                await EnsurePerfilExisteAsync(viewModel.IdPerfil.Value);
            }

            usuario.Nome = viewModel.Nome.Trim();
            usuario.Email = email;
            usuario.Telefone = viewModel.Telefone.Trim();

            if (viewModel.IdPerfil.HasValue && IsAdministrador(usuarioAtual))
            {
                usuario.IdPerfil = viewModel.IdPerfil.Value;
            }

            await _context.SaveChangesAsync();

            var updated = await _context.Usuarios
                .Include(u => u.Perfil)
                .AsNoTracking()
                .FirstAsync(u => u.IdUsuario == usuarioId);

            return updated.ToSummary();
        }

        public async Task<bool> DeleteAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            if (usuario == null)
            {
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PerfilViewModel[]> GetPerfisAsync(ClaimsPrincipal? usuarioAtual = null)
        {
            var query = _context.Perfis
                .AsNoTracking()
                .AsQueryable();

            if (IsProfessor(usuarioAtual))
            {
                query = query.Where(perfil => perfil.IdPerfil == PerfisSistema.AlunoId);
            }
            else if (!IsAdministrador(usuarioAtual))
            {
                query = query.Where(perfil => false);
            }

            return await query
                .OrderBy(perfil => perfil.IdPerfil)
                .Select(perfil => new PerfilViewModel
                {
                    IdPerfil = perfil.IdPerfil,
                    DescricaoPerfil = perfil.DescricaoPerfil
                })
                .ToArrayAsync();
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private async Task EnsurePerfilExisteAsync(int idPerfil)
        {
            var perfilExiste = await _context.Perfis
                .AnyAsync(perfil => perfil.IdPerfil == idPerfil);

            if (!perfilExiste || !PerfisSistema.IsPerfilValido(idPerfil))
            {
                throw new InvalidOperationException("Perfil informado nao existe.");
            }
        }

        private static void EnsureCanCreate(int idPerfil, ClaimsPrincipal? usuarioAtual)
        {
            if (IsAdministrador(usuarioAtual))
            {
                return;
            }

            if (IsProfessor(usuarioAtual) && idPerfil == PerfisSistema.AlunoId)
            {
                return;
            }

            if (IsProfessor(usuarioAtual))
            {
                throw new UnauthorizedAccessException("Professores podem cadastrar apenas usuarios alunos.");
            }

            throw new UnauthorizedAccessException("Usuario nao autorizado a cadastrar usuarios.");
        }

        private static void EnsureCanUpdate(
            Usuario usuario,
            UsuarioUpdateViewModel viewModel,
            ClaimsPrincipal? usuarioAtual)
        {
            if (!IsAdministrador(usuarioAtual) && viewModel.IdPerfil.HasValue)
            {
                throw new UnauthorizedAccessException("Apenas administradores podem informar o perfil do usuario.");
            }

            if (IsAdministrador(usuarioAtual))
            {
                return;
            }

            var idUsuarioAtual = GetUsuarioAtualId(usuarioAtual);

            if (IsProfessor(usuarioAtual)
                && (idUsuarioAtual == usuario.IdUsuario || usuario.IdPerfil == PerfisSistema.AlunoId))
            {
                return;
            }

            if (IsAluno(usuarioAtual) && idUsuarioAtual == usuario.IdUsuario)
            {
                return;
            }

            throw new UnauthorizedAccessException("Usuario nao autorizado a atualizar este usuario.");
        }

        private static bool IsAdministrador(ClaimsPrincipal? usuarioAtual)
        {
            return IsSystemCall(usuarioAtual) || usuarioAtual!.IsInRole(PerfisSistema.Administrador);
        }

        private static bool IsProfessor(ClaimsPrincipal? usuarioAtual)
        {
            return !IsSystemCall(usuarioAtual) && usuarioAtual!.IsInRole(PerfisSistema.Professor);
        }

        private static bool IsAluno(ClaimsPrincipal? usuarioAtual)
        {
            return !IsSystemCall(usuarioAtual) && usuarioAtual!.IsInRole(PerfisSistema.Aluno);
        }

        private static bool IsSystemCall(ClaimsPrincipal? usuarioAtual)
        {
            return usuarioAtual == null || usuarioAtual.Identity?.IsAuthenticated != true;
        }

        private static int? GetUsuarioAtualId(ClaimsPrincipal? usuarioAtual)
        {
            if (IsSystemCall(usuarioAtual))
            {
                return null;
            }

            var idClaim = usuarioAtual!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var idUsuario) ? idUsuario : null;
        }
    }
}

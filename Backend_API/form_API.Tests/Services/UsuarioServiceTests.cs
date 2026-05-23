using System;
using System.Security.Claims;
using System.Threading.Tasks;
using form_API.Data;
using form_API.Security;
using form_API.Services;
using form_API.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace form_API.Tests.Services
{
    public class UsuarioServiceTests
    {
        [Fact]
        public async Task AddAsync_WhenModelIsValid_CreatesUsuarioWithHashedPassword()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioCreateViewModel
            {
                Nome = "Usuario Novo",
                Email = "novo@escola.com",
                Telefone = "11999990000",
                IdPerfil = 2
            };

            var created = await service.AddAsync(model);
            var entity = await context.Usuarios.FirstAsync(usuario => usuario.IdUsuario == created.IdUsuario);

            Assert.Equal("Usuario Novo", created.Nome);
            Assert.Equal("novo@escola.com", created.Email);
            Assert.Equal("Professor", created.DescricaoPerfil);
            Assert.NotEqual(DefaultPasswordPolicy.DefaultPassword, entity.Senha);
            Assert.True(PasswordHasher.VerifyPassword(DefaultPasswordPolicy.DefaultPassword, entity.Senha));
        }

        [Fact]
        public async Task AddAsync_WhenProfessorCreatesAluno_CreatesUsuario()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioCreateViewModel
            {
                Nome = "Aluno Novo",
                Email = "aluno.novo@escola.com",
                Telefone = "11999990000",
                IdPerfil = PerfisSistema.AlunoId
            };

            var created = await service.AddAsync(model, CreatePrincipal(2, PerfisSistema.Professor));

            Assert.Equal(PerfisSistema.Aluno, created.DescricaoPerfil);
        }

        [Fact]
        public async Task AddAsync_WhenProfessorCreatesProfessor_ThrowsUnauthorizedAccessException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioCreateViewModel
            {
                Nome = "Professor Novo",
                Email = "professor.novo@escola.com",
                Telefone = "11999990000",
                IdPerfil = PerfisSistema.ProfessorId
            };

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.AddAsync(model, CreatePrincipal(2, PerfisSistema.Professor)));

            Assert.Equal("Professores podem cadastrar apenas usuarios alunos.", exception.Message);
        }

        [Fact]
        public async Task AddAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioCreateViewModel
            {
                Nome = "Outro Admin",
                Email = " ADMIN@ESCOLA.COM ",
                Telefone = "11999990000",
                IdPerfil = 1
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(model));

            Assert.Equal("Email ja cadastrado.", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_WhenAlunoUpdatesOwnBasicData_UpdatesUsuario()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioUpdateViewModel
            {
                Nome = "Aluno Atualizado",
                Email = "aluno01.atualizado@escola.com",
                Telefone = "11977770099"
            };

            var updated = await service.UpdateAsync(12, model, CreatePrincipal(12, PerfisSistema.Aluno));

            Assert.NotNull(updated);
            Assert.Equal("Aluno Atualizado", updated!.Nome);
            Assert.Equal(PerfisSistema.Aluno, updated.DescricaoPerfil);
        }

        [Fact]
        public async Task UpdateAsync_WhenAlunoUpdatesOtherUsuario_ThrowsUnauthorizedAccessException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioUpdateViewModel
            {
                Nome = "Outro Aluno",
                Email = "aluno02.atualizado@escola.com",
                Telefone = "11977770100"
            };

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.UpdateAsync(13, model, CreatePrincipal(12, PerfisSistema.Aluno)));

            Assert.Equal("Usuario nao autorizado a atualizar este usuario.", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_WhenEmailBelongsToAnotherUsuario_ThrowsInvalidOperationException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new UsuarioService(context);
            var model = new UsuarioUpdateViewModel
            {
                Nome = "Professor Atualizado",
                Email = " ADMIN@ESCOLA.COM ",
                Telefone = "11999991111",
                IdPerfil = 2
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(2, model));

            Assert.Equal("Email ja cadastrado.", exception.Message);
        }

        private static DataContext CreateContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(connection)
                .Options;

            return new DataContext(options);
        }

        private static ClaimsPrincipal CreatePrincipal(int usuarioId, string perfil)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Role, perfil)
            }, "Test");

            return new ClaimsPrincipal(identity);
        }
    }
}

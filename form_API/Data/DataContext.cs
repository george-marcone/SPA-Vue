using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using form_API.Models;
using Microsoft.EntityFrameworkCore;

namespace form_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Professor> Professores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Professor>()
            .HasData(
                new List<Professor>{
                    new Professor(){
                        Id = 1,
                        Nome = "Vinicius"
                    },
                    new Professor(){
                        Id = 2,
                        Nome = "Paula"
                    },
                    new Professor(){
                        Id = 3,
                        Nome = "Suzana"
                    },
                }
            );

            builder.Entity<Aluno>()
            .HasData(
                new List<Aluno>{
                    new Aluno(){
                        Id = 1,
                        Nome = "Maria",
                        Sobrenome = "Solano",
                        DataNasc = "25/02/1982",
                        ProfessorId = 1
                    },
                    new Aluno(){
                        Id = 2,
                        Nome = "Joao",
                        Sobrenome = "Gomes",
                        DataNasc = "25/01/2000",
                        ProfessorId = 2
                    },
                    new Aluno(){
                        Id = 3,
                        Nome = "Alex",
                        Sobrenome = "Alves",
                        DataNasc = "22/02/2002",
                        ProfessorId = 3
                    },
                }
            );
        }
    }
}
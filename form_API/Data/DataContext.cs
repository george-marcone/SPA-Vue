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
    }
}
namespace form_API.Security
{
    public static class PerfisSistema
    {
        public const int AdministradorId = 1;
        public const int ProfessorId = 2;
        public const int AlunoId = 3;

        public const string Administrador = "Administrador";
        public const string Professor = "Professor";
        public const string Aluno = "Aluno";

        public static bool IsPerfilValido(int idPerfil)
        {
            return idPerfil is AdministradorId or ProfessorId or AlunoId;
        }
    }
}

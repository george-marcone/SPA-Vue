<template>
  <div>
    <titulo texto="Professores" btnVoltar="true"/>
    <table>
      <thead>
        <th>Cód.</th>
        <th>Nome</th>
        <th>Alunos</th>
      </thead>
      <tbody v-if="professores.length">
        <tr v-for="(professor, index) in professores" :key="index">
          <td class="colPequeno">
            {{ professor.id }}
          </td>
          <router-link
            :to="`/alunos/${professor.id}`"
            tag="td"
            style="cursor: pointer"
          >
            {{ professor.nome }} {{ professor.sobrenome }}
          </router-link>
          <td class="colPequeno">
            {{ professor.qtdAlunos }}
          </td>
        </tr>
      </tbody>
      <tfoot v-else>
        <tr>
          <td colspan="3" style="text-align: center">
            <h5>Nenhum Professor Encontrado</h5>
          </td>
        </tr>
      </tfoot>
    </table>
  </div>
</template>

<script>
import Titulo from "../_share/Titulo";
export default {
  components: {
    Titulo,
  },
  data() {
    return {
      titulo: "Alunos",
      nome: "",
      professores: [],
      Alunos: [],
    };
  },
  created() {
    this.$http
      .get("http://localhost:5001/api/aluno")
      .then(res => res.json())
      .then(alunos => {
        this.Alunos = alunos;
        this.carregarProfessores();
      });
  },
  props: {},
  methods: {
    pegarQtdAlunosPorProfessor() {
      this.professores.forEach((professor, index) => {
        professor = {
          id: professor.id,
          nome: professor.nome,
          qtdAlunos: this.Alunos.filter(
            aluno => aluno.professor.id == professor.id
          ).length,
        };
        this.professores[index] = professor;
      });
    },

    carregarProfessores() {
      this.$http
        .get("http://localhost:5001/api/professor")
        .then(res => res.json())
        .then(professor => {
          this.professores = professor;
          this.pegarQtdAlunosPorProfessor();
        });
    },
  },
};
</script>

<style scoped>
.colPequeno {
  text-align: center;
  width: 15%;
}
</style>

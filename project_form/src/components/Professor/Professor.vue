<template>
  <div>
    <titulo texto = "Professores"/>
    <table>
      <thead>
        <th>Cód.</th>
        <th>Nome</th>
        <th>Alunos</th>
      </thead>
      <tbody v-if="professores.length">
        <tr v-for="(professor, index) in professores" :key="index">          
          <td>{{professor.id}}</td>          
            <router-link to="/alunos" tag="td" style="cursor:pointer ">
              {{professor.nome}} {{professor.sobrenome}}
            </router-link>
          <td> 
              {{professor.qtdAlunos}}     
          </td>
        </tr>
      </tbody>
      <tfoot v-else>
          Nenhum Professor Encontrado
      </tfoot>
    </table>
  </div>
</template>

<script>
import Titulo from '../_share/Titulo'
    export default {
      components:{
          Titulo
      },
      data() {
          return {
              titulo: "Alunos",
              nome: "",
              professores: [],
              Alunos: []
            };
        },
      created(){
        this.$http
        .get('http://localhost:3000/alunos/')
        .then(res => res.json())
        .then(alunos => {
          this.Alunos = alunos;
          this.carregarProfessores();
          })
      },
      props:{

      }, 
      methods: {
        pegarQtdAlunosPorProfessor(){
          this.professores.forEach((professor, index) => {
            professor = {
              id: professor.id,
              nome: professor.nome,
              qtdAlunos: this.Alunos.filter(
                aluno => aluno.professor.id == professor.id
              ).length
            }
            this.professores[index] = professor;
          });
        },

        carregarProfessores(){
          this.$http
          .get('http://localhost:3000/professores/')
          .then(res => res.json())
          .then(professor => {
            this.professores = professor;
            this.pegarQtdAlunosPorProfessor();
            });
        }
      }
    }
</script>

<style lang="scss" scoped>

</style>
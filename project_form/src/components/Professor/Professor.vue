<template>
  <div>
    <titulo texto = "Professores"/>
    <input
      type="text"
      placeholder="Nome do Professor"
      v-model="nome"
      v-on:keyup.enter="addAluno()"
    />
    <button class="btn btnInput" @click="addAluno()"> Adicionar </button>
    <br />
    <br />
    <table>
      <thead>
        <th>Cód.</th>
        <th>Nome</th>
        <th>Alunos</th>
      </thead>
      <tbody v-if="professores.length">
        <tr v-for="(professor, index) in professores" :key="index">          
          <td>{{professor.id}}</td>
          <td>{{professor.nome}} {{professor.sobrenome}}</td>
          <td> 3
            <!-- <button class="btn btn_Danger" @click="remover()">Remover</button> -->
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
                professores: [
                    {id: 1, nome: 'George', sobrenome: 'Souza'},
                    {id: 2, nome: 'James', sobrenome: 'Xavier'},
                    {id: 3, nome: 'Fernada', sobrenome: 'Cobain'}
                ]
        };
  },
    }
</script>

<style lang="scss" scoped>

</style>
<template>
  <main class="flex-1 bg-[#F5F7FB] p-8 font-inter relative min-h-screen">
    <!-- Cabeçalho -->
    <CategoryHeader @criada="carregarCategorias" />

    <!-- Filtros -->
    <CategoryFilters
      :categorias="categoriasTree"
      v-model:busca="busca"
      v-model:filtro-categorias="filtroCategorias"
      v-model:mostrar-apenas-pais="mostrarApenasPais"
      v-model:mostrar-apenas-subs="mostrarApenasSubs"
      @buscar="onBuscar"
    />

    <!-- Tabela -->
    <CategoryTable
      :categorias="categoriasTabela.itens"
      @editar="abrirModalEditar"
      @excluir="abrirModalExcluir"
      @nova-sub="abrirModalNovaSub"
    />

    <!-- Paginação com seletor embutido -->
    <PaginationBar
      :pagina-atual="paginaAtual"
      :total-paginas="categoriasTabela.totalPaginas"
      :page-size="itensPorPagina"
      :page-size-options="[10, 20, 50]"
      @mudar-pagina="mudarPagina"
      @update:pageSize="mudarItensPorPagina"
    />

    <!-- Modais -->
    <CategoryModalEditar
      v-if="modalEditarAberto"
      :categoria="categoriaSelecionada"
      :categorias-tree="categoriasTree"
      @fechar="modalEditarAberto = false"
      @salvar="onSalvarCategoria"
    />

    <CategoryModalExcluir
      v-if="modalExcluirAberto"
      :categoria="categoriaSelecionada"
      @fechar="modalExcluirAberto = false"
      @confirmar="onExcluirCategoria"
    />

    <CategoryModalNovaSub
      v-if="modalNovaSubAberto"
      :pai="categoriaSelecionada"
      @fechar="modalNovaSubAberto = false"
      @criar="onCriarSubcategoria"
    />

  </main>
</template>

<script setup>
import { onMounted } from "vue";
import CategoryHeader from "../../components/admin/components/Category/CategoryHeader.vue";
import CategoryFilters from "../../components/admin/components/Category/CategoryFilters.vue";
import CategoryTable from "../../components/admin/components/Category/CategoryTable.vue";
import PaginationBar from "../../components/admin/components/PaginationBar.vue";
import CategoryModalEditar from "../../components/admin/modals/CategoryModalEditar.vue";
import CategoryModalExcluir from "../../components/admin/modals/CategoryModalExcluir.vue";
import CategoryModalNovaSub from "../../components/admin/modals/CategoryModalNovaSub.vue";
import { useCategories } from "../../components/admin/composables/useCategories.js";

const {
  categoriasTree,
  busca,
  filtroCategorias,
  mostrarApenasPais,
  mostrarApenasSubs,
  paginaAtual,
  itensPorPagina,
  categoriasTabela,
  mudarPagina,
  mudarItensPorPagina,
  categoriaSelecionada,
  modalEditarAberto,
  modalExcluirAberto,
  modalNovaSubAberto,
  abrirModalEditar,
  abrirModalExcluir,
  abrirModalNovaSub,
  salvarCategoria,
  excluirCategoria,
  criarSubcategoria,
  carregarCategorias,
} = useCategories();

function onBuscar() {
  // Apenas recomputa; dados são reativos
}

async function onSalvarCategoria(payload) {
  await salvarCategoria(categoriaSelecionada.value?.id, payload);
}

async function onExcluirCategoria() {
  await excluirCategoria(categoriaSelecionada.value?.id);
}

async function onCriarSubcategoria(nome) {
  await criarSubcategoria(categoriaSelecionada.value?.id, nome);
}

onMounted(() => {
  carregarCategorias();
});
</script>

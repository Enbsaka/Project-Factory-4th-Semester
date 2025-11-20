<template>
  <main class="flex-1 bg-[#F5F7FB] p-8 font-inter relative min-h-screen">
    <!-- Cabeçalho -->
    <ProductHeader @novo-produto="abrirModalNovoProduto" />

    <!-- Filtros -->
    <ProductFilters
      :categorias="categorias"
      v-model:busca="busca"
      v-model:filtro-categorias="filtroCategorias"
      @buscar="carregarProdutos"
    />

    <!-- Tabela -->
    <ProductTable
      :produtos="produtos"
      :carregando="carregando"
      :erro="erro"
      @editar="abrirModalEdicao"
      @detalhes="abrirModalDetalhes"
      @variacoes="abrirModalVariacoes"
      @excluir="abrirModalExcluir"
    />

    <!-- Paginação com seletor embutido -->
    <PaginationBar
      :pagina-atual="paginaAtual"
      :total-paginas="totalPaginas"
      :page-size="itensPorPagina"
      :page-size-options="[6, 10, 20, 50]"
      @mudar-pagina="mudarPagina"
      @update:pageSize="mudarItensPorPagina"
    />

    <!-- Modais -->
    <ModalEditarProduto
      v-if="modalEditarAberto"
      :produto="produtoSelecionado"
      @fechar="modalEditarAberto = false"
      @atualizar="carregarProdutos"
    />

    <ModalExcluirProduto
      v-if="modalExcluirAberto"
      :produto="produtoSelecionado"
      @fechar="modalExcluirAberto = false"
      @confirmar="excluirProduto"
    />

    <ModalVariacoes
      v-if="modalVariacoesAberto"
      :produto="produtoSelecionado"
      @fechar="modalVariacoesAberto = false"
      @atualizar="carregarProdutos"
    />

    <ModalDetalhesProduto
      v-if="modalDetalhesAberto"
      :produto="produtoSelecionado"
      @fechar="modalDetalhesAberto = false"
    />

    <ModalNovoProduto
      v-if="modalNovoAberto"
      @fechar="modalNovoAberto = false"
      @criado="carregarProdutos"
    />
  </main>
</template>

<script setup>
import { useProducts } from "../../components/admin/composables/useProducts.js";

import ProductHeader from "../../components/admin/components/Product/ProductHeader.vue";
import ProductFilters from "../../components/admin/components/Product/ProductFilters.vue";
import ProductTable from "../../components/admin/components/Product/ProductTable.vue";
import PaginationBar from "../../components/admin/components/PaginationBar.vue";

import ModalEditarProduto from "../../components/admin/modals/ProductModalEditar.vue";
import ModalExcluirProduto from "../../components/admin/modals/ProductModalExcluir.vue";
import ModalVariacoes from "../../components/admin/modals/ProductVariation.vue";
import ModalNovoProduto from "../../components/admin/modals/ModalNovoProduto.vue";
import ModalDetalhesProduto from "../../components/admin/modals/ProductModalDetalhes.vue";

const {
  produtos,
  categorias,
  busca,
  filtroCategorias,
  paginaAtual,
  totalPaginas,
  itensPorPagina,
  carregando,
  erro,
  produtoSelecionado,
  modalEditarAberto,
  modalExcluirAberto,
  modalVariacoesAberto,
  modalNovoAberto,
  modalDetalhesAberto,
  abrirModalEdicao,
  abrirModalVariacoes,
  abrirModalDetalhes,
  abrirModalExcluir,
  abrirModalNovoProduto,
  mudarPagina,
  mudarItensPorPagina,
  carregarProdutos,
  excluirProduto,
} = useProducts();
</script>

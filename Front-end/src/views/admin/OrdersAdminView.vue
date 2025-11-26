<template>
  <main class="flex-1 bg-[#F5F7FB] p-8">
    <OrderHeader />

    <OrderFilters
      v-model:busca="busca"
      v-model:status="filtroStatus"
      :data-inicio="dataInicio"
      :data-fim="dataFim"
      @update:dataInicio="(v) => (dataInicio = v)"
      @update:dataFim="(v) => (dataFim = v)"
      @buscar="onBuscar"
    />

    <PaginationBar
      :pagina-atual="paginaAtual"
      :total-paginas="totalPaginas"
      :page-size="itensPorPagina"
      :page-size-options="[10, 20, 50]"
      @mudar-pagina="mudarPagina"
      @update:pageSize="mudarItensPorPagina"
    />

    <OrderTable
      :pedidos="pedidosFiltrados"
      @editar="abrirModalEditar"
      @excluir="abrirModalExcluir"
    />

    <!-- Modais -->
    <OrderModalEditar
      v-if="modalEditarAberto"
      :pedido="pedidoSelecionado"
      @fechar="modalEditarAberto = false"
      @salvarCupom="salvarCupom"
      @finalizar="finalizarPedido"
    />

    <OrderModalExcluir
      v-if="modalExcluirAberto"
      :pedido="pedidoSelecionado"
      @fechar="modalExcluirAberto = false"
      @confirmar="excluirPedido"
    />
  </main>
</template>

<script setup>
import { onMounted } from "vue";
import OrderHeader from "../../components/admin/components/Order/OrderHeader.vue";
import OrderFilters from "../../components/admin/components/Order/OrderFilters.vue";
import OrderTable from "../../components/admin/components/Order/OrderTable.vue";
import OrderModalEditar from "../../components/admin/orders/OrderModalEditar.vue";
import OrderModalExcluir from "../../components/admin/orders/OrderModalExcluir.vue";
import PaginationBar from "../../components/admin/components/PaginationBar.vue";
import { useOrders } from "../../components/admin/composables/useOrders.js";

const {
  pedidos,
  pedidosFiltrados,
  carregando,
  erro,
  busca,
  filtroStatus,
  dataInicio,
  dataFim,
  paginaAtual,
  totalPaginas,
  itensPorPagina,
  // modais
  modalEditarAberto,
  modalExcluirAberto,
  pedidoSelecionado,
  // métodos
  carregarPedidos,
  abrirModalEditar,
  abrirModalExcluir,
  salvarCupom,
  finalizarPedido,
  excluirPedido,
  mudarPagina,
  mudarItensPorPagina,
} = useOrders();

onMounted(() => {
  carregarPedidos();
});

const formatarData = (dataISO) => {
  const data = new Date(dataISO);
  return data.toLocaleDateString("pt-BR");
};

const formatarValor = (valor) => {
  return Number(valor).toFixed(2);
};

function onBuscar() {
  // Apenas recomputa; dados são reativos
}
</script>

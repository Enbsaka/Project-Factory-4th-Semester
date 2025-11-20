<template>
  <section class="bg-[#F5F7FB] p-6">
    <div class="overflow-x-auto bg-white shadow-md rounded-xl">
      <table class="min-w-full text-gray-700">
        <thead class="bg-gray-100 border-b">
          <tr>
            <th class="py-3 px-6 text-left text-lg font-semibold">Pedido</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Data</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Cliente</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Total</th>
            <th class="py-3 px-6 text-center text-lg font-semibold">Status</th>
            <th class="py-3 px-6 text-center text-lg font-semibold">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="pedido in pedidos" :key="pedido.id" class="hover:bg-gray-50">
            <td class="py-4 px-6 flex items-center gap-3">
              <div>
                <p class="font-medium">#{{ pedido.id }}</p>
              </div>
            </td>
            <td class="py-4 px-6">{{ formatarData(pedido.dataPedido) }}</td>
            <td class="py-4 px-6">{{ pedido.cliente.nome }}</td>
            <td class="py-4 px-6">R$ {{ formatarValor(pedido.valorTotal) }}</td>
            <td class="py-4 px-6 text-center">
              <span class="px-2 py-1 rounded-full text-xs font-medium"
                    :class="pedido.status === 1 ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'">
                {{ pedido.status === 1 ? 'Finalizado' : 'Carrinho' }}
              </span>
            </td>
            <td class="py-4 px-6 text-center text-gray-500">
              <OrderActionsMenu :pedido="pedido" @editar="$emit('editar', pedido)" @excluir="$emit('excluir', pedido)" />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup>
import OrderActionsMenu from "../../orders/OrderActionsMenu.vue";

defineProps({
  pedidos: { type: Array, default: () => [] },
});
defineEmits(["editar", "excluir"]);

const formatarData = (dataISO) => {
  const data = new Date(dataISO);
  return data.toLocaleDateString("pt-BR");
};

const formatarValor = (valor) => {
  return Number(valor).toFixed(2);
};
</script>

<style scoped>
</style>
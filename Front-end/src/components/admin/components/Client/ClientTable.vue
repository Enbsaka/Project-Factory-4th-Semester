<template>
  <section class="bg-[#F5F7FB] p-6">
    <div class="overflow-x-auto bg-white shadow-md rounded-xl">
    <table class="min-w-full text-gray-700">
      <thead class="bg-gray-100 border-b">
        <tr>
          <th class="py-3 px-6 text-left text-lg font-semibold">Clientes</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">Contato</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">CPF</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">Cadastro</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">Pedidos</th>
          <th class="py-3 px-6 text-center text-lg font-semibold">Ações</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-gray-200">
        <tr v-for="(cliente, index) in clientes" :key="index" class="hover:bg-gray-50">
          <td class="py-4 px-6">
            <p class="font-medium">{{ cliente.nome }}</p>
          </td>
          <td class="py-4 px-6">{{ cliente.email }}</td>
          <td class="py-4 px-6">{{ cliente.cpf }}</td>
          <td class="py-4 px-6">{{ formatarData(cliente.dataCadastro) }}</td>
          <td class="py-4 px-6">{{ cliente.totalPedidos }}</td>
          <td class="py-4 px-6 text-center">
            <ClientActionsMenu
              :cliente="cliente"
              @editar="$emit('editar', cliente)"
              @excluir="$emit('excluir', cliente)"
              @detalhar="$emit('detalhar', cliente)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    </div>
  </section>
</template>

<script setup>
import ClientActionsMenu from "../../clients/ClientActionsMenu.vue";
defineProps({
  clientes: { type: Array, default: () => [] },
});
defineEmits(["editar", "excluir", "detalhar"]);

function formatarData(data) {
  const d = new Date(data);
  return d.toLocaleDateString("pt-BR");
}
</script>

<style scoped>
</style>
<template>
  <section class="bg-[#F5F7FB] p-6">
    <div class="overflow-x-auto bg-white shadow-md rounded-xl">
      <table class="min-w-full text-gray-700">
        <thead class="bg-gray-100 border-b">
          <tr>
            <th class="py-3 px-6 text-left text-lg font-semibold">Código</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Desconto</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Validade</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Status</th>
            <th class="py-3 px-6 text-center text-lg font-semibold">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="cupom in cupons" :key="cupom.id" class="hover:bg-gray-50">
            <td class="py-4 px-6 font-medium">{{ cupom.codigo }}</td>
            <td class="py-4 px-6">{{ cupom.descontoPercentual }}%</td>
            <td class="py-4 px-6">{{ formatarData(cupom.dataExpiracao) }}</td>
            <td class="py-4 px-6">
              <span
                :class="[
                  'px-2 py-1 rounded-full text-xs font-medium',
                  cupom.ativo ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700',
                ]"
              >
                {{ cupom.ativo ? 'Ativo' : 'Inativo' }}
              </span>
            </td>
            <td class="py-4 px-6 text-center text-sm text-gray-600">
              <CouponActionsMenu
                :cupom="cupom"
                @editar="$emit('editar', cupom)"
                @excluir="$emit('excluir', cupom)"
              />
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="cupons.length === 0" class="text-gray-500 text-center py-6 space-y-3">
        <p>Nenhum cupom encontrado.</p>
      </div>
    </div>
  </section>
</template>

<script setup>
import CouponActionsMenu from "../../coupons/CouponActionsMenu.vue";

defineProps({
  cupons: { type: Array, default: () => [] },
});
defineEmits(["editar", "excluir"]);

function formatarData(data) {
  if (!data) return "—";
  return new Date(data).toLocaleString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}
</script>

<style scoped>
</style>
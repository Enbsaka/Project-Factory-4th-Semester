<template>
  <div class="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
      <div class="px-5 py-4 border-b">
        <h3 class="text-lg font-semibold text-gray-800">Editar Pedido</h3>
      </div>
      <div class="p-5 space-y-4">
        <div class="text-sm text-gray-700">
          <p><span class="font-medium">Pedido:</span> #{{ pedido?.id }}</p>
          <p><span class="font-medium">Cliente:</span> {{ pedido?.cliente?.nome }}</p>
          <p><span class="font-medium">Data:</span> {{ formatarData(pedido?.dataPedido) }}</p>
          <p>
            <span class="font-medium">Status:</span>
            <span class="px-2 py-1 rounded-full text-xs font-medium"
              :class="pedido?.status === 1 ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'">
              {{ pedido?.status === 1 ? 'Finalizado' : 'Carrinho' }}
            </span>
          </p>
        </div>

        <div>
          <label class="block text-sm text-gray-700 mb-1">Cupom (opcional)</label>
          <input v-model="cupom" type="text" class="w-full border rounded-md px-3 py-2" placeholder="Código do cupom" />
          <div class="flex gap-2 mt-2">
            <button type="button" class="px-3 py-2 rounded-md border" @click="aplicarCupom">Aplicar</button>
            <button type="button" class="px-3 py-2 rounded-md border" @click="removerCupom">Remover</button>
          </div>
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button type="button" @click="$emit('fechar')" class="px-3 py-2 rounded-md border">Fechar</button>
          <button
            v-if="pedido?.status === 0"
            type="button"
            @click="$emit('finalizar')"
            class="px-3 py-2 rounded-md bg-[#141A7C] text-white"
          >
            Finalizar Pedido
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
const props = defineProps({ pedido: Object });
const emit = defineEmits(["fechar", "salvarCupom", "finalizar"]);
const cupom = ref("");

watch(
  () => props.pedido,
  (val) => {
    cupom.value = val?.cupom?.codigo || "";
  },
  { immediate: true }
);

function aplicarCupom() {
  emit("salvarCupom", cupom.value);
}
function removerCupom() {
  emit("salvarCupom", "");
}
function formatarData(d) {
  if (!d) return "-";
  const data = new Date(d);
  return data.toLocaleDateString("pt-BR");
}
</script>
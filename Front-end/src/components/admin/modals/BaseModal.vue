<template>
  <div
    class="fixed inset-0 bg-black/30 backdrop-blur-sm flex items-center justify-center z-50"
    @click.self="$emit('fechar')"
  >
    <div
      class="bg-white rounded-2xl shadow-2xl border border-gray-100 w-full max-w-lg p-6 animate-fadeIn"
    >
      <!-- Cabeçalho -->
      <div class="flex items-center justify-between mb-5">
        <h2 class="text-xl font-semibold text-gray-800">{{ titulo }}</h2>
        <button
          @click="$emit('fechar')"
          class="text-gray-400 hover:text-gray-600 text-2xl leading-none"
        >
          ×
        </button>
      </div>

      <!-- Corpo -->
      <slot />

      <!-- Rodapé -->
      <div v-if="mostrarAcoes" class="flex justify-end gap-3 mt-6">
        <button
          @click="$emit('fechar')"
          class="px-4 py-2 text-sm rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-100 transition"
        >
          Cancelar
        </button>
        <button
          @click="$emit('confirmar')"
          :class="confirmClass"
        >
          {{ textoConfirmar }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  titulo: { type: String, default: "Modal" },
  textoConfirmar: { type: String, default: "Confirmar" },
  mostrarAcoes: { type: Boolean, default: true },
});
defineEmits(["fechar", "confirmar"]);

const confirmClass = computed(() => {
  const base = "px-4 py-2 text-sm rounded-lg text-white transition";
  // Se o texto do botão for "Salvar", aplicar cor #141A7C; caso contrário, manter azul padrão
  if ((props.textoConfirmar || "").toLowerCase() === "salvar") {
    return `${base} bg-[#141A7C] hover:bg-[#0f166a]`;
  }
  return `${base} bg-blue-600 hover:bg-blue-700`;
});
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
.animate-fadeIn {
  animation: fadeIn 0.25s ease;
}
</style>

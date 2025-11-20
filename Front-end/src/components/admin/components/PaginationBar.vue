<template>
  <div
    class="fixed bottom-4 left-1/2 transform -translate-x-1/2 bg-white border border-gray-200 py-2 px-4 rounded-md flex justify-center items-center space-x-4 shadow z-40"
  >
    <div class="flex items-center gap-2 text-sm">
      <label class="text-gray-700">Itens por página:</label>
      <select
        :value="pageSize"
        @change="$emit('update:pageSize', Number($event.target.value))"
        class="border rounded-md px-2 py-1 bg-white text-sm"
      >
        <option v-for="opt in pageSizeOptions" :key="opt" :value="opt">{{ opt }}</option>
      </select>
    </div>
    <button
      @click="$emit('mudar-pagina', paginaAtual - 1)"
      :disabled="paginaAtual <= 1"
      class="px-3 py-1 rounded-md border text-sm disabled:opacity-50 hover:bg-gray-100"
    >
      Anterior
    </button>
    <span class="text-sm text-gray-600">
      Página {{ Math.max(paginaAtual, 1) }} de {{ Math.max(totalPaginas, 1) }}
    </span>
    <button
      @click="$emit('mudar-pagina', paginaAtual + 1)"
      :disabled="paginaAtual >= totalPaginas"
      class="px-3 py-1 rounded-md border text-sm disabled:opacity-50 hover:bg-gray-100"
    >
      Próxima
    </button>
  </div>
</template>

<script setup>
defineProps({
  paginaAtual: Number,
  totalPaginas: Number,
  pageSize: { type: Number, default: 10 },
  pageSizeOptions: { type: Array, default: () => [10, 20, 50] },
});
defineEmits(["mudar-pagina", "update:pageSize"]);
</script>

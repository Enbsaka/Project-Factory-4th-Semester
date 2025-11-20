<template>
  <div v-if="aberto" class="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-20">
    <div class="bg-white rounded p-5 w-full max-w-md shadow">
      <h2 class="text-lg font-semibold mb-3">Excluir cupom</h2>
      <p class="mb-4">Tem certeza que deseja excluir o cupom <strong>{{ cupom?.codigo }}</strong>?</p>
      <div class="flex justify-end gap-2">
        <button class="px-3 py-1.5 bg-gray-100 hover:bg-gray-200 rounded" @click="onFechar">Cancelar</button>
        <button class="px-3 py-1.5 bg-red-600 hover:bg-red-700 text-white rounded" @click="onConfirmar">Excluir</button>
      </div>
    </div>
  </div>
  </template>

<script setup>
const props = defineProps({
  aberto: { type: Boolean, default: false },
  cupom: { type: Object, default: null },
});
const emit = defineEmits(["fechar", "confirmar"]);

const onFechar = () => emit("fechar");
const onConfirmar = () => emit("confirmar");
</script>

<style scoped></style>
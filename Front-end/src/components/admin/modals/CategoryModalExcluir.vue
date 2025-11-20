<template>
  <div class="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
      <div class="px-5 py-4 border-b">
        <h3 class="text-lg font-semibold text-gray-800">Excluir Categoria</h3>
      </div>
      <div class="p-5 space-y-4">
        <p class="text-gray-700">
          Tem certeza que deseja excluir a categoria
          <span class="font-semibold">{{ categoria?.nome }}</span>?
        </p>
        <div class="flex justify-end gap-2 pt-2">
          <button type="button" @click="$emit('fechar')" class="px-3 py-2 rounded-md border">Cancelar</button>
          <button type="button" @click="$emit('confirmar')" class="px-3 py-2 rounded-md bg-red-600 text-white">Excluir</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({ categoria: Object });
defineEmits(["fechar", "confirmar"]);
</script>
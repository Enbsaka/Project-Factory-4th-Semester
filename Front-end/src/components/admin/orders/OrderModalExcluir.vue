<template>
  <div class="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
      <div class="px-5 py-4 border-b">
        <h3 class="text-lg font-semibold text-gray-800">Excluir Pedido</h3>
      </div>
      <div class="p-5 space-y-3 text-sm text-gray-700">
        <p>Tem certeza que deseja excluir o pedido #{{ pedido?.id }}?</p>
        <p class="text-red-600">Essa ação não pode ser desfeita.</p>
        <div class="flex justify-end gap-2 pt-2">
          <button type="button" class="px-3 py-2 rounded-md border" @click="$emit('fechar')">Cancelar</button>
          <button type="button" class="px-3 py-2 rounded-md bg-red-600 text-white" @click="$emit('confirmar')">Excluir</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({ pedido: Object });
defineEmits(["fechar", "confirmar"]);
</script>
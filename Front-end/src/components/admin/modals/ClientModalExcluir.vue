<template>
  <div class="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
    <div class="bg-white w-full max-w-md rounded-lg shadow-lg">
      <header class="px-6 py-4 border-b">
        <h2 class="text-lg font-semibold text-gray-800">Excluir cliente</h2>
        <p class="text-sm text-gray-500">Esta ação é permanente.</p>
      </header>
      <div class="p-6 space-y-4">
        <p class="text-sm text-gray-700">Tem certeza que deseja excluir o cliente abaixo?</p>
        <div class="bg-gray-50 border rounded-md p-3">
          <p class="text-sm"><span class="text-gray-500">ID:</span> {{ cliente?.id }}</p>
          <p class="text-sm"><span class="text-gray-500">Nome:</span> {{ cliente?.nome }}</p>
          <p class="text-sm"><span class="text-gray-500">E-mail:</span> {{ cliente?.email }}</p>
        </div>
        <p v-if="erro" class="text-sm text-red-600">{{ erro }}</p>
        <div class="flex justify-end gap-2">
          <button class="px-4 py-2 rounded-md border" @click="close">Cancelar</button>
          <button :disabled="carregando" class="px-4 py-2 rounded-md bg-red-600 text-white" @click="confirmar">
            {{ carregando ? 'Excluindo...' : 'Excluir' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import clienteService from '../../../services/clienteService.js';
import { useNotifications } from '../composables/useNotifications.js';
const props = defineProps({ cliente: { type: Object, required: true } });
const emit = defineEmits(['fechar', 'confirmado']);
const carregando = ref(false);
const erro = ref('');
const { show } = useNotifications();
function close() { emit('fechar'); }
async function confirmar() {
  try {
    erro.value = '';
    carregando.value = true;
    const ok = await clienteService.deleteCliente(props.cliente.id);
    if (!ok) throw new Error('Falha ao excluir cliente');
    emit('confirmado');
    show('Cliente excluído com sucesso!', 'success');
    close();
  } catch (e) {
    erro.value = e?.response?.data || e?.message || 'Erro ao excluir cliente.';
    console.error(e);
    show('Erro ao excluir cliente.', 'error');
  } finally {
    carregando.value = false;
  }
}
</script>
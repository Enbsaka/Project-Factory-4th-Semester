<template>
  <div class="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
    <div class="bg-white w-full max-w-2xl rounded-lg shadow-lg">
      <header class="px-6 py-4 border-b">
        <h2 class="text-lg font-semibold text-gray-800">Editar cliente</h2>
        <p class="text-sm text-gray-500">Atualize os dados do cliente selecionado</p>
      </header>

      <form @submit.prevent="submit" class="p-6 grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="md:col-span-2">
          <label class="block text-sm font-medium text-gray-700 mb-1">Nome</label>
          <input v-model="form.nome" type="text" class="w-full border rounded-md px-3 py-2" required />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">CPF</label>
          <input v-model="form.cpf" type="text" class="w-full border rounded-md px-3 py-2" required />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">E-mail</label>
          <input v-model="form.email" type="email" class="w-full border rounded-md px-3 py-2" required />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">CEP</label>
          <input v-model="form.cep" type="text" class="w-full border rounded-md px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Número do endereço</label>
          <input v-model="form.numEndereco" type="text" class="w-full border rounded-md px-3 py-2" />
        </div>

        <div class="md:col-span-2 flex items-center gap-2">
          <input id="isAdmin" v-model="form.isAdmin" type="checkbox" class="h-4 w-4" />
          <label for="isAdmin" class="text-sm text-gray-700">Administrador</label>
        </div>

        <div class="md:col-span-2 flex gap-3 justify-end mt-2">
          <button type="button" @click="close" class="px-4 py-2 rounded-md border">Cancelar</button>
          <button :disabled="carregando" class="px-4 py-2 rounded-md bg-[#141A7C] text-white">
            {{ carregando ? 'Salvando...' : 'Salvar' }}
          </button>
        </div>
        <p v-if="erro" class="md:col-span-2 text-sm text-red-600">{{ erro }}</p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue';
import clienteService from '../../../services/clienteService.js';
import { useNotifications } from '../composables/useNotifications.js';

const props = defineProps({ cliente: { type: Object, required: true } });
const emit = defineEmits(['fechar', 'atualizado']);

const form = ref({
  nome: '',
  cpf: '',
  email: '',
  cep: '',
  numEndereco: '',
  isAdmin: false,
});
const carregando = ref(false);
const erro = ref('');
const { show } = useNotifications();

function close() { emit('fechar'); }

async function carregar() {
  try {
    const data = await clienteService.getClienteById(props.cliente.id);
    form.value.nome = data?.nome ?? data?.Nome ?? props.cliente.nome ?? '';
    form.value.email = data?.email ?? data?.Email ?? props.cliente.email ?? '';
    form.value.cpf = data?.cpf ?? data?.Cpf ?? props.cliente.cpf ?? '';
    form.value.cep = data?.cep ?? data?.Cep ?? '';
    form.value.numEndereco = data?.numEndereco ?? data?.NumEndereco ?? '';
    form.value.isAdmin = !!(data?.isAdmin ?? data?.IsAdmin ?? false);
  } catch (e) {
    console.error(e);
  }
}

onMounted(carregar);
watch(() => props.cliente?.id, carregar);

async function submit() {
  try {
    erro.value = '';
    carregando.value = true;
    const ok = await clienteService.updateCliente(props.cliente.id, form.value);
    if (!ok) throw new Error('Falha ao atualizar cliente');
    emit('atualizado');
    show('Cliente atualizado com sucesso!', 'success');
    close();
  } catch (e) {
    erro.value = e?.response?.data || e?.message || 'Erro ao salvar. Verifique os dados.';
    console.error(e);
    show('Erro ao atualizar cliente.', 'error');
  } finally {
    carregando.value = false;
  }
}
</script>
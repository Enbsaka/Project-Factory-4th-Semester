<template>
  <div class="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
    <div class="bg-white w-full max-w-2xl rounded-lg shadow-lg">
      <header class="px-6 py-4 border-b flex items-center justify-between">
        <div>
          <h2 class="text-lg font-semibold text-gray-800">Detalhes do cliente</h2>
          <p class="text-sm text-gray-500">Informações completas do cadastro</p>
        </div>
        <button class="px-3 py-1 rounded-md border" @click="close">Fechar</button>
      </header>
      <div class="p-6 grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="space-y-1">
          <p class="text-xs text-gray-500">ID</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.id }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">Nome</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.nome }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">E-mail</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.email }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">CPF</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.cpf }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">CEP</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.cep || '—' }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">Número Endereço</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.numEndereco || '—' }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">Administrador</p>
          <p class="text-sm font-medium">{{ clienteDetalhe?.isAdmin ? 'Sim' : 'Não' }}</p>
        </div>
        <div class="space-y-1">
          <p class="text-xs text-gray-500">Cadastro</p>
          <p class="text-sm font-medium">{{ formatarData(clienteDetalhe?.dataCadastro) }}</p>
        </div>

        <div class="md:col-span-2 mt-4">
          <div class="bg-gray-50 border rounded-lg p-4">
            <p class="text-sm text-gray-600">Resumo</p>
            <ul class="mt-2 text-sm text-gray-800 list-disc pl-5">
              <li>Total de pedidos: {{ clienteDetalhe?.totalPedidos ?? '—' }}</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue';
import clienteService from '../../../services/clienteService.js';
import pedidoService from '../../../services/pedidoService.js';
const props = defineProps({ cliente: { type: Object, required: true } });
const emit = defineEmits(['fechar']);
const clienteDetalhe = ref(null);
function close() { emit('fechar'); }
function formatarData(data) {
  if (!data) return '—';
  const d = new Date(data);
  return d.toLocaleDateString('pt-BR');
}
async function carregar() {
  try {
    const data = await clienteService.getClienteById(props.cliente.id);
    clienteDetalhe.value = {
      id: data?.id ?? props.cliente.id,
      nome: data?.nome ?? props.cliente.nome,
      email: data?.email ?? props.cliente.email,
      cpf: data?.cpf ?? props.cliente.cpf,
      cep: data?.cep ?? '',
      numEndereco: data?.numEndereco ?? '',
      isAdmin: !!(data?.isAdmin ?? false),
      dataCadastro: data?.dataCadastro ?? props.cliente.dataCadastro ?? null,
      totalPedidos: data?.totalPedidos ?? props.cliente.totalPedidos ?? null,
    };

    // Atualiza total de pedidos com base no serviço
    try {
      const resp = await pedidoService.getByCliente(clienteDetalhe.value.id);
      clienteDetalhe.value.totalPedidos = resp?.totalItens ?? 0;
    } catch (e) {
      console.warn('Falha ao obter total de pedidos para detalhamento', e);
    }
  } catch (e) {
    console.error(e);
    clienteDetalhe.value = props.cliente;
  }
}
onMounted(carregar);
watch(() => props.cliente?.id, carregar);
</script>
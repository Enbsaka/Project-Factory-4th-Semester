<template>
  <div class="min-h-[70vh] bg-gray-50 flex items-center justify-center">
    <div class="bg-white rounded-xl shadow-md border border-gray-200 p-6 w-full max-w-sm">
      <h1 class="text-xl font-semibold text-gray-900 mb-1">Criar conta</h1>
      <p class="text-xs text-gray-600 mb-4">Preencha seus dados básicos.</p>

      <form @submit.prevent="submit" class="space-y-3">
        <div class="space-y-1">
          <label class="block text-xs font-medium text-gray-700">Nome</label>
          <input v-model="form.nome" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600" required />
        </div>

        <div class="space-y-1">
          <label class="block text-xs font-medium text-gray-700">CPF</label>
          <input v-model="form.cpf" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600" required />
        </div>

        <div class="space-y-1">
          <label class="block text-xs font-medium text-gray-700">E-mail</label>
          <input v-model="form.email" type="email" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600" required />
        </div>

        <div class="space-y-1">
          <label class="block text-xs font-medium text-gray-700">Senha</label>
          <input v-model="form.senha" type="password" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600" required />
        </div>

        <div class="space-y-1">
          <label class="block text-xs font-medium text-gray-700">CEP</label>
          <input v-model="form.cep" @blur="onCepBlur" maxlength="9" placeholder="00000-000" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600" required />
          <p v-if="viaCepMsg" class="text-[11px]" :class="viaCepClass">{{ viaCepMsg }}</p>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div class="space-y-1 col-span-2">
            <label class="block text-xs font-medium text-gray-700">Logradouro</label>
            <input v-model="form.logradouro" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm bg-gray-50" readonly />
          </div>
          <div class="space-y-1">
            <label class="block text-xs font-medium text-gray-700">Bairro</label>
            <input v-model="form.bairro" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm bg-gray-50" readonly />
          </div>
          <div class="space-y-1">
            <label class="block text-xs font-medium text-gray-700">Cidade</label>
            <input v-model="form.localidade" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm bg-gray-50" readonly />
          </div>
          <div class="space-y-1">
            <label class="block text-xs font-medium text-gray-700">UF</label>
            <input v-model="form.uf" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm bg-gray-50" readonly />
          </div>
        </div>

        <div class="space-y-1">
          <label class="block text-xs font-medium text-gray-700">Número do endereço</label>
          <input v-model="form.numEndereco" type="text" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600" required />
        </div>

        <button :disabled="carregando" class="w-full bg-[#141A7C] text-white py-2 rounded-md hover:bg-[#0f166a]">
          {{ carregando ? 'Cadastrando...' : 'Criar conta' }}
        </button>
      </form>

      <p v-if="erro" class="text-xs text-red-600 mt-2">{{ erro }}</p>
      <p v-if="sucesso" class="text-xs text-green-600 mt-2">Cadastro realizado!</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import clienteService from '../services/clienteService.js';
import authService from '../services/authService.js';
import api from '../services/api.js';
import { parseJwt } from '../utils/jwt.js';

const router = useRouter();

const form = ref({
  nome: '',
  cpf: '',
  email: '',
  senha: '',
  cep: '',
  numEndereco: '',
  logradouro: '',
  bairro: '',
  localidade: '',
  uf: '',
});
const viaCepMsg = ref('');
const viaCepClass = ref('text-gray-500');

const sanitizeCep = (value) => (value || '').replace(/\D/g, '');
const onCepBlur = async () => {
  viaCepMsg.value = '';
  viaCepClass.value = 'text-gray-500';
  const digits = sanitizeCep(form.value.cep);
  if (digits.length !== 8) {
    viaCepMsg.value = 'CEP inválido. Use 8 dígitos (00000-000).';
    viaCepClass.value = 'text-red-600';
    form.value.logradouro = '';
    form.value.bairro = '';
    form.value.localidade = '';
    form.value.uf = '';
    return;
  }
  try {
    const { data } = await api.get(`/ViaCep/${digits}`);
    form.value.logradouro = data?.logradouro || '';
    form.value.bairro = data?.bairro || '';
    form.value.localidade = data?.localidade || '';
    form.value.uf = data?.uf || '';
    viaCepMsg.value = 'Endereço encontrado.';
    viaCepClass.value = 'text-green-600';
  } catch (e) {
    form.value.logradouro = '';
    form.value.bairro = '';
    form.value.localidade = '';
    form.value.uf = '';
    viaCepMsg.value = e?.response?.data || 'CEP não encontrado.';
    viaCepClass.value = 'text-red-600';
  }
};

const carregando = ref(false);
const erro = ref('');
const sucesso = ref(false);

const submit = async () => {
  erro.value = '';
  sucesso.value = false;
  carregando.value = true;
  try {
    const criado = await clienteService.createCliente(form.value);
    if (!criado) throw new Error('Falha ao criar cliente');
    sucesso.value = true;
    // login automático após cadastro
    const { token } = await authService.loginCliente(form.value.email, form.value.senha);
    localStorage.setItem('token', token);
    const payload = parseJwt(token);
    const roleClaim = (payload?.role || payload?.Role || payload?.roles || payload?.Roles);
    const role = Array.isArray(roleClaim) ? (roleClaim[0] || '') : (roleClaim || '');
    const normalized = (role || '').toString().toLowerCase();
    const finalRole = normalized.includes('admin') ? 'admin' : 'cliente';
    localStorage.setItem('role', finalRole);
    router.push('/');
  } catch (e) {
    // Exibe a mensagem detalhada retornada pelo backend (ex.: CPF duplicado)
    erro.value = e?.response?.data || e?.message || 'Erro no cadastro. Verifique os dados e tente novamente.';
    console.error(e);
  } finally {
    carregando.value = false;
  }
};
</script>
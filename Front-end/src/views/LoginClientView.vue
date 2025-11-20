<template>
  <div class="min-h-[70vh] flex items-center justify-center bg-gray-50">
    <div class="w-full max-w-sm bg-white rounded-xl shadow-md border border-gray-200 p-6">
      <div class="mb-4">
        <h1 class="text-xl font-semibold text-gray-900">Entrar</h1>
        <p class="text-xs text-gray-600">Use seu e-mail e senha.</p>
      </div>

      <form @submit.prevent="submit" class="space-y-3">
        <div class="space-y-1">
          <label for="email" class="block text-xs font-medium text-gray-700">E-mail</label>
          <input id="email" v-model="email" type="email" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600 focus:border-indigo-600" required />
        </div>
        <div class="space-y-1">
          <label for="senha" class="block text-xs font-medium text-gray-700">Senha</label>
          <input id="senha" v-model="senha" type="password" class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600 focus:border-indigo-600" required />
        </div>

        <button :disabled="carregando" class="w-full bg-[#141A7C] text-white py-2 rounded-md hover:bg-[#0f166a] transition">
          {{ carregando ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>

      <div class="mt-3 flex items-center justify-between">
        <router-link
          to="/cadastro"
          class="inline-flex items-center gap-1 text-xs font-medium text-white bg-[#141A7C] rounded-full px-3 py-1 hover:bg-[#0f166a] transition shadow-sm"
        >
          Criar conta
        </router-link>
        <router-link v-if="isAdmin" to="/admin/dashboard" class="text-xs text-yellow-700 hover:underline">Ir para Admin</router-link>
      </div>

      <p v-if="erro" class="text-xs text-red-600 mt-2">{{ erro }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import authService from '../services/authService.js';
import { parseJwt } from '../utils/jwt.js';

const router = useRouter();
const email = ref('');
const senha = ref('');
const carregando = ref(false);
const erro = ref('');
const isAdmin = ref(false);

const submit = async () => {
  erro.value = '';
  carregando.value = true;
  try {
    const { token } = await authService.loginCliente(email.value, senha.value);
    localStorage.setItem('token', token);
    const payload = parseJwt(token);
    const roleClaim = (payload?.role || payload?.Role || payload?.roles || payload?.Roles);
    const role = Array.isArray(roleClaim) ? (roleClaim[0] || '') : (roleClaim || '');
    const normalized = (role || '').toString().toLowerCase();
    const finalRole = normalized.includes('admin') ? 'admin' : 'cliente';
    localStorage.setItem('role', finalRole);
    isAdmin.value = finalRole === 'admin';
    router.push('/');
  } catch (e) {
    erro.value = 'Falha no login. Verifique suas credenciais.';
    console.error(e);
  } finally {
    carregando.value = false;
  }
};
</script>
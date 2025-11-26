<template>
  <header class="bg-white/70 border-b border-gray-200 backdrop-blur sticky top-0 z-50">
    <div class="max-w-7xl mx-auto flex items-center justify-between px-4 md:px-6 h-20">
      <div class="flex items-center justify-start">
        <router-link to="/" class="flex items-center">
          <img src="../../../assets/images/logo-dunderstore.png" alt="Dunder Store"
            class="h-12 md:h-14 w-auto object-contain" />
        </router-link>
      </div>
      <div class="flex-1 px-4 md:px-8">
        <div class="hidden md:flex items-center gap-6">
          <nav class="flex items-center gap-6 flex-shrink-0">
            <router-link to="/" class="text-gray-800 hover:text-indigo-700 font-medium">Início</router-link>
            <button class="text-gray-800 hover:text-indigo-700 font-medium" @mouseenter="abrirCategorias" @focus="abrirCategorias" @mouseleave="fecharCategorias">Categorias</button>
          </nav>
          <form class="flex items-center" @submit.prevent="onSearch" aria-label="Buscar produtos">
            <div class="relative w-[340px] sm:w-[400px] lg:w-[500px] xl:w-[580px]">
              <input v-model="searchQuery" type="text" placeholder="Buscar produtos, marcas e categorias..." class="w-full px-4 py-2.5 border rounded-full shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              <button type="submit" class="absolute right-1 top-1/2 -translate-y-1/2 px-4 py-2 bg-[#141A7C] text-white rounded-full hover:bg-[#0f166a]">Buscar</button>
            </div>
          </form>
        </div>
      </div>
      <div class="flex justify-end items-center gap-3 relative">
        <template v-if="!isLoggedIn">
          <router-link to="/login" class="px-4 py-2 text-sm font-medium text-gray-800 bg-gray-100 rounded-md hover:bg-gray-200 transition">Entrar</router-link>
          <router-link to="/cadastro" class="px-4 py-2 text-sm font-medium text-white bg-[#141A7C] rounded-md hover:bg-[#0f166a] transition">Cadastrar</router-link>
        </template>
        <template v-else>
          <button @click="openProfileModal" class="p-2 rounded-full border border-gray-200 hover:bg-gray-100" title="Perfil" aria-label="Perfil">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" class="w-5 h-5 text-gray-800">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.5 20.25a8.25 8.25 0 0 1 15 0v.75H4.5v-.75Z" />
            </svg>
          </button>
          <button @click="openSettings = true" class="p-2 rounded-full border border-gray-200 hover:bg-gray-100" title="Configurações" aria-label="Configurações">
            <Settings class="w-5 h-5 text-gray-800" />
          </button>
          <router-link v-if="isAdmin" to="/admin/dashboard" class="ml-1 px-2 py-1 text-xs font-semibold rounded bg-yellow-100 text-yellow-800 border border-yellow-200 hover:bg-yellow-200">Admin</router-link>
          <button @click="onLogout" class="px-3 py-2 text-sm font-medium text-white bg-red-600 rounded-md hover:bg-red-700 transition">Sair</button>
        </template>
        <router-link v-if="isLoggedIn" to="/carrinho" class="relative flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-[#141A7C] rounded-md hover:bg-indigo-700 transition">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437m0 0L6.75 14.25A2.25 2.25 0 0 0 9 16.5h9.75a2.25 2.25 0 0 0 2.212-1.791l1.263-7.043A1.125 1.125 0 0 0 21.113 6H5.106m0 0L4.5 3.75M9 20.25a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Zm10.5 0a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
          </svg>
          <span>Carrinho</span>
          <span v-if="cartCount > 0" class="absolute -top-2 -right-2 bg-red-500 text-white text-[10px] font-bold rounded-full px-1 py-0.5">{{ cartCount }}</span>
        </router-link>
        <button class="md:hidden ml-2 p-2 rounded-md border border-gray-200" @click="mobileOpen = !mobileOpen" aria-label="Abrir menu">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 5.25h16.5m-16.5 6h16.5m-16.5 6h16.5" />
          </svg>
        </button>
      </div>
    </div>
    <div class="hidden md:block relative" @mouseenter="abrirCategorias" @mouseleave="fecharCategorias">
      <div class="max-w-7xl mx-auto px-4 md:px-6">
        <div v-show="showCategorias" class="absolute left-0 right-0 mx-auto bg-white border border-gray-200 rounded-xl shadow-xl mt-1 p-4 w-full max-w-4xl">
          <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
            <div v-for="cat in categorias" :key="cat.id" class="min-w-0">
              <router-link :to="`/categoria/${cat.id}`" class="block text-gray-900 font-semibold hover:text-indigo-600 truncate">{{ cat.nome }}</router-link>
              <ul class="mt-2 space-y-1">
                <li v-for="sub in (cat.subcategorias || []).slice(0,5)" :key="sub.id">
                  <router-link :to="`/categoria/${sub.id}`" class="block text-gray-600 hover:text-indigo-600 text-xs truncate">{{ sub.nome }}</router-link>
                </li>
              </ul>
            </div>
          </div>
          <div class="text-right mt-3">
            <router-link to="/" class="text-xs text-gray-500 hover:text-indigo-600">Ver todas as categorias</router-link>
          </div>
        </div>
      </div>
    </div>
    <div v-if="mobileOpen" class="md:hidden bg-white border-t border-gray-200">
      <div class="px-4 py-3 space-y-3">
        <form @submit.prevent="onSearch">
          <div class="relative">
            <input v-model="searchQuery" type="text" placeholder="Buscar produtos..." class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            <button type="submit" class="absolute right-1 top-1/2 -translate-y-1/2 px-3 py-1.5 bg-[#141A7C] text-white rounded-md hover:bg-[#0f166a]">Buscar</button>
          </div>
        </form>
        <div class="flex items-center gap-4 text-gray-800">
          <router-link to="/" class="hover:text-indigo-700">Início</router-link>
        </div>
        <div v-for="cat in categorias" :key="cat.id" class="mb-3">
          <router-link :to="`/categoria/${cat.id}`" class="block text-gray-900 font-medium">{{ cat.nome }}</router-link>
          <div class="pl-3 mt-1">
            <router-link v-for="sub in (cat.subcategorias || []).slice(0,6)" :key="sub.id" :to="`/categoria/${sub.id}`" class="block text-gray-600 text-sm py-0.5">{{ sub.nome }}</router-link>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import api from '../../../services/api.js'
import clienteService from '../../../services/clienteService.js'
import { carrinhoService } from '../../../services/carrinhoService.js'
import { Settings } from 'lucide-vue-next'

const categorias = ref([])
const cartCount = ref(0)
const showCategorias = ref(false)
const mobileOpen = ref(false)
let hideTimer = null
const router = useRouter()
const route = useRoute()
const searchQuery = ref('')
const isLoggedIn = ref(false)
const openSettings = ref(false)
const openProfile = ref(false)
const isDark = ref(false)
const perfilForm = ref({ nome: '', email: '', senha: '' })
const savingPerfil = ref(false)
const perfilFeedback = ref('')
const perfilFeedbackClass = ref('text-gray-600')
const isAdmin = ref(false)

onMounted(async () => {
  await carregarCategorias()
  await carregarCartCount()
  searchQuery.value = route.query?.nome || ''
  isLoggedIn.value = !!localStorage.getItem('token')
  isAdmin.value = (localStorage.getItem('role') || '').toLowerCase() === 'admin'
  const saved = localStorage.getItem('theme')
  isDark.value = saved === 'dark'
  applyTheme()
})

onUnmounted(() => {
  window.removeEventListener('cart:updated', carregarCartCount)
})

watch(() => route.fullPath, () => {
  isLoggedIn.value = !!localStorage.getItem('token')
  isAdmin.value = (localStorage.getItem('role') || '').toLowerCase() === 'admin'
})

function toggleDark() {
  isDark.value = !isDark.value
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
  applyTheme()
}

function applyTheme() {
  const theme = isDark.value ? 'dark' : 'light'
  document.documentElement.setAttribute('data-theme', theme)
}

async function openProfileModal() {
  try {
    openProfile.value = true
    perfilFeedback.value = ''
    perfilFeedbackClass.value = 'text-gray-600'
    const me = await clienteService.getMeuPerfil()
    perfilForm.value.nome = me?.nome || me?.Nome || ''
    perfilForm.value.email = me?.email || me?.Email || ''
    perfilForm.value.cpf = me?.cpf || me?.Cpf || ''
    perfilForm.value.cep = me?.cep || me?.Cep || ''
    perfilForm.value.numEndereco = me?.numEndereco || me?.NumEndereco || ''
    perfilForm.value.senha = ''
  } catch (e) {
    perfilFeedback.value = 'Não foi possível carregar seu perfil.'
    perfilFeedbackClass.value = 'text-red-600'
  }
}

async function savePerfil() {
  try {
    savingPerfil.value = true
    perfilFeedback.value = ''
    const payload = {}
    if (perfilForm.value.nome) payload.nome = perfilForm.value.nome
    if (perfilForm.value.email) payload.email = perfilForm.value.email
    if (perfilForm.value.cpf) payload.cpf = perfilForm.value.cpf
    if (perfilForm.value.cep) payload.cep = perfilForm.value.cep
    if (perfilForm.value.numEndereco) payload.numEndereco = perfilForm.value.numEndereco
    if (perfilForm.value.senha) payload.senha = perfilForm.value.senha
    const params = new URLSearchParams()
    Object.keys(payload).forEach((k) => params.append(k, payload[k] ?? ''))
    await api.patch('/Cliente/me', params, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
    perfilFeedback.value = 'Perfil atualizado com sucesso!'
    perfilFeedbackClass.value = 'text-green-600'
    perfilForm.value.senha = ''
  } catch (e) {
    perfilFeedback.value = 'Falha ao atualizar perfil.'
    perfilFeedbackClass.value = 'text-red-600'
  } finally {
    savingPerfil.value = false
  }
}

async function carregarCategorias() {
  try {
    const { data } = await api.get('/categoria/hierarquia')
    const normalizar = (node) => ({
      id: node.id ?? node.Id,
      nome: node.nome ?? node.Nome,
      subcategorias: (node.subcategorias ?? node.Subcategorias ?? []).map(normalizar)
    })
    const raiz = (data || []).map(normalizar)
    categorias.value = raiz
  } catch (error) {
    console.error('Erro ao buscar categorias:', error)
  }
}

async function carregarCartCount() {
  try {
    const perfil = await clienteService.getMeuPerfil()
    const clienteId = perfil?.id || perfil?.Id
    if (!clienteId) return
    const carrinho = await carrinhoService.getCarrinho(clienteId)
    const itens = carrinho?.Produtos || carrinho?.produtos || carrinho?.PedidoProdutos || carrinho?.pedidoProdutos || []
    cartCount.value = itens.reduce((acc, item) => acc + (item.Quantidade ?? item.quantidade ?? 0), 0)
  } catch (error) {
  }
}

window.addEventListener('cart:updated', carregarCartCount)

function abrirCategorias() {
  if (hideTimer) {
    clearTimeout(hideTimer)
    hideTimer = null
  }
  showCategorias.value = true
}

function fecharCategorias() {
  hideTimer = setTimeout(() => {
    showCategorias.value = false
  }, 100)
}

function onSearch() {
  const q = (searchQuery.value || '').trim()
  const routeName = isLoggedIn.value ? 'searchLogged' : 'search'
  router.push({ name: routeName, query: { nome: q || undefined } })
  mobileOpen.value = false
}

function onLogout() {
  localStorage.removeItem('token')
  localStorage.removeItem('role')
  router.push({ name: 'home' })
}
</script>
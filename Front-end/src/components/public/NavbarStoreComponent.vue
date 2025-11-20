<template>
  <header class="bg-white/70 border-b border-gray-200 backdrop-blur sticky top-0 z-50">
    <div class="max-w-7xl mx-auto flex items-center justify-between px-4 md:px-6 h-20">

      <!-- COL 1: Logo -->
      <div class="flex items-center justify-start">
        <router-link to="/" class="flex items-center">
          <img src="../../assets/images/logo-dunderstore.png" alt="Dunder Store"
            class="h-12 md:h-14 w-auto object-contain" />
        </router-link>
      </div>

      <!-- Centro: navegação + searchbar (lado a lado) -->
      <div class="flex-1 px-4 md:px-8">
        <div class="hidden md:flex items-center gap-6">
          <!-- Navegação principal -->
          <nav class="flex items-center gap-6 flex-shrink-0">
            <router-link to="/" class="text-gray-800 hover:text-indigo-700 font-medium">Início</router-link>
            <button class="text-gray-800 hover:text-indigo-700 font-medium" @mouseenter="abrirCategorias" @focus="abrirCategorias" @mouseleave="fecharCategorias">Categorias</button>
          </nav>

          <!-- Searchbar -->
          <form class="flex items-center" @submit.prevent="onSearch" aria-label="Buscar produtos">
            <div class="relative w-[340px] sm:w-[400px] lg:w-[500px] xl:w-[580px]">
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Buscar produtos, marcas e categorias..."
                class="w-full px-4 py-2.5 border rounded-full shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
              <button
                type="submit"
                class="absolute right-1 top-1/2 -translate-y-1/2 px-4 py-2 bg-[#141A7C] text-white rounded-full hover:bg-[#0f166a]"
              >Buscar</button>
            </div>
          </form>
        </div>
      </div>

      <!-- COL 3: Ações -->
      <div class="flex justify-end items-center gap-3 relative">
        <!-- Não logado -->
        <template v-if="!isLoggedIn">
          <router-link to="/login"
            class="px-4 py-2 text-sm font-medium text-gray-800 bg-gray-100 rounded-md hover:bg-gray-200 transition">
            Entrar
          </router-link>

          <router-link to="/cadastro"
            class="px-4 py-2 text-sm font-medium text-white bg-[#141A7C] rounded-md hover:bg-[#0f166a] transition">
            Cadastrar
          </router-link>
        </template>

        <!-- Logado -->
        <template v-else>
          <!-- Ícone Perfil (abre modal) -->
          <button @click="openProfileModal" class="p-2 rounded-full border border-gray-200 hover:bg-gray-100" title="Perfil" aria-label="Perfil">
            <!-- Heroicons User -->
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" class="w-5 h-5 text-gray-800">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.5 20.25a8.25 8.25 0 0 1 15 0v.75H4.5v-.75Z" />
            </svg>
          </button>
          <!-- Ícone Configurações (abre modal) -->
          <button @click="openSettings = true" class="p-2 rounded-full border border-gray-200 hover:bg-gray-100" title="Configurações" aria-label="Configurações">
            <Settings class="w-5 h-5 text-gray-800" />
          </button>
          <router-link v-if="isAdmin" to="/admin/dashboard" class="ml-1 px-2 py-1 text-xs font-semibold rounded bg-yellow-100 text-yellow-800 border border-yellow-200 hover:bg-yellow-200">Admin</router-link>
          <!-- Sair -->
          <button @click="onLogout" class="px-3 py-2 text-sm font-medium text-white bg-red-600 rounded-md hover:bg-red-700 transition">Sair</button>
        </template>

        <router-link v-if="isLoggedIn" to="/carrinho"
          class="relative flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-[#141A7C] rounded-md hover:bg-indigo-700 transition">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
            stroke="currentColor" class="w-5 h-5">
            <path stroke-linecap="round" stroke-linejoin="round"
              d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437m0 0L6.75 14.25A2.25 2.25 0 0 0 9 16.5h9.75a2.25 2.25 0 0 0 2.212-1.791l1.263-7.043A1.125 1.125 0 0 0 21.113 6H5.106m0 0L4.5 3.75M9 20.25a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Zm10.5 0a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
          </svg>
          <span>Carrinho</span>
          <span v-if="cartCount > 0" class="absolute -top-2 -right-2 bg-red-500 text-white text-[10px] font-bold rounded-full px-1 py-0.5">{{ cartCount }}</span>
        </router-link>
        <button class="md:hidden ml-2 p-2 rounded-md border border-gray-200" @click="mobileOpen = !mobileOpen" aria-label="Abrir menu">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 5.25h16.5m-16.5 6h16.5m-16.5 6h16.5" />
          </svg>
        </button>
        
        <!-- Modal Perfil -->
        <teleport to="body">
        <div v-if="openProfile" class="fixed inset-0 z-[999] bg-black/50 flex items-center justify-center" @click.self="openProfile = false">
          <div class="w-full max-w-xl bg-white rounded-2xl shadow-xl border border-gray-200 overflow-hidden">
            <div class="p-6 bg-gray-50 flex items-center justify-between">
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Meu Perfil</h3>
                <p class="text-xs text-gray-500">Atualize seus dados pessoais</p>
              </div>
              <span v-if="isAdmin" class="px-2 py-1 text-xs font-semibold rounded bg-yellow-100 text-yellow-800 border border-yellow-200">Admin</span>
            </div>
            <div class="p-6 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div class="space-y-1">
                <label class="text-xs font-medium text-gray-600">Nome</label>
                <input v-model="perfilForm.nome" type="text" class="w-full px-3 py-2 rounded-md border border-gray-200 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              </div>
              <div class="space-y-1">
                <label class="text-xs font-medium text-gray-600">Email</label>
                <input v-model="perfilForm.email" type="email" class="w-full px-3 py-2 rounded-md border border-gray-200 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              </div>
              <div class="space-y-1">
                <label class="text-xs font-medium text-gray-600">CPF</label>
                <input v-model="perfilForm.cpf" type="text" class="w-full px-3 py-2 rounded-md border border-gray-200 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              </div>
              <div class="space-y-1">
                <label class="text-xs font-medium text-gray-600">CEP</label>
                <input v-model="perfilForm.cep" type="text" class="w-full px-3 py-2 rounded-md border border-gray-200 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              </div>
              <div class="space-y-1 md:col-span-1">
                <label class="text-xs font-medium text-gray-600">Número do Endereço</label>
                <input v-model="perfilForm.numEndereco" type="text" class="w-full px-3 py-2 rounded-md border border-gray-200 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              </div>
              <div class="space-y-1 md:col-span-1">
                <label class="text-xs font-medium text-gray-600">Nova senha</label>
                <input v-model="perfilForm.senha" type="password" class="w-full px-3 py-2 rounded-md border border-gray-200 focus:outline-none focus:ring-2 focus:ring-indigo-500" />
              </div>
              <p v-if="perfilFeedback" class="text-sm md:col-span-2" :class="perfilFeedbackClass">{{ perfilFeedback }}</p>
            </div>
            <div class="p-6 bg-gray-50 flex items-center justify-between">
              <button @click="onLogout" class="px-3 py-2 text-sm font-medium text-white bg-red-600 rounded-md hover:bg-red-700">Sair</button>
              <div class="flex items-center gap-2">
                <button @click="openProfile = false" class="px-3 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200">Cancelar</button>
                <button @click="savePerfil" class="px-3 py-2 text-sm font-medium text-white bg-[#141A7C] rounded-md hover:bg-[#0f166a]" :disabled="savingPerfil">
                  <span v-if="savingPerfil">Salvando...</span>
                  <span v-else>Salvar</span>
                </button>
              </div>
            </div>
          </div>
        </div>
        </teleport>

        <!-- Modal Configurações -->
        <teleport to="body">
        <div v-if="openSettings" class="fixed inset-0 z-[999] bg-black/50 flex items-center justify-center" @click.self="openSettings = false">
          <div class="w-full max-w-sm bg-white rounded-2xl shadow-lg border border-gray-200">
            <div class="p-5 border-b">
              <h3 class="text-lg font-semibold text-gray-900">Configurações</h3>
            </div>
            <div class="p-5 space-y-4">
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-700">Tema escuro</span>
                <label class="inline-flex items-center cursor-pointer">
                  <input type="checkbox" class="sr-only" :checked="isDark" @change="toggleDark" />
                  <span class="w-10 h-5 bg-gray-300 rounded-full relative">
                    <span class="absolute top-0.5 left-0.5 w-4 h-4 bg-white rounded-full transition" :style="{ transform: isDark ? 'translateX(20px)' : 'translateX(0)' }"></span>
                  </span>
                </label>
              </div>
            </div>
            <div class="p-5 border-t text-right">
              <button class="px-3 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200" @click="openSettings = false">Fechar</button>
            </div>
          </div>
        </div>
        </teleport>
      </div>
    </div>
    <!-- Mega menu categorias -->
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

    <!-- Mobile menu -->
    <div v-if="mobileOpen" class="md:hidden bg-white border-t border-gray-200">
      <div class="px-4 py-3 space-y-3">
        <!-- Searchbar mobile -->
        <form @submit.prevent="onSearch">
          <div class="relative">
            <input v-model="searchQuery" type="text" placeholder="Buscar produtos..." class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            <button type="submit" class="absolute right-1 top-1/2 -translate-y-1/2 px-3 py-1.5 bg-[#141A7C] text-white rounded-md hover:bg-[#0f166a]">Buscar</button>
          </div>
        </form>

        <!-- Links principais -->
        <div class="flex items-center gap-4 text-gray-800">
          <router-link to="/" class="hover:text-indigo-700">Início</router-link>
        </div>

        <!-- Categorias -->
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
import { ref, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import api from '../../services/api.js'
import clienteService from '../../services/clienteService.js'
import { carrinhoService } from '../../services/carrinhoService.js'
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
  // pré-preenche busca com query atual, se existir
  searchQuery.value = route.query?.nome || ''
  // inicializa estado de login
  isLoggedIn.value = !!localStorage.getItem('token')
  isAdmin.value = (localStorage.getItem('role') || '').toLowerCase() === 'admin'
  const saved = localStorage.getItem('theme')
  isDark.value = saved === 'dark'
  applyTheme()
})

// Atualiza estado de login quando navegação muda
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
    // Mostrar apenas categorias raiz no navbar
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
    const itens = carrinho?.pedidoProdutos || carrinho?.produtos || []
    cartCount.value = itens.reduce((acc, item) => acc + (item.quantidade ?? item.Quantidade ?? 0), 0)
  } catch (error) {
    // silêncio: se não logado ou erro, não mostra contagem
  }
}

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
  // voltar para home pública
  router.push({ name: 'home' })
}
</script>

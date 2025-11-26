<template>
  <section class="bg-[#F5F7FB] min-h-screen pb-16">
    <!-- Cabeçalho da busca -->
    <div class="text-center py-10 bg-white shadow-sm mb-6 border-b">
      <h1 class="text-3xl font-bold text-gray-900 mb-2">
        Resultados para: <span class="text-indigo-700">{{ termo }}</span>
      </h1>
      <p class="text-gray-600">{{ total }} produto(s) encontrados</p>
    </div>

    <!-- Barra de ações -->
    <div class="container mx-auto px-6 lg:px-20 mb-6 flex flex-col sm:flex-row items-center justify-between gap-4">
      <div class="flex items-center gap-3 text-sm text-gray-600">
        <span>Ordenar por</span>
        <select v-model="ordenacao" class="border rounded-md px-3 py-2 bg-white">
          <option value="relevancia">Relevância</option>
          <option value="preco_asc">Preço: menor para maior</option>
          <option value="preco_desc">Preço: maior para menor</option>
          <option value="nome_asc">Nome: A-Z</option>
          <option value="nome_desc">Nome: Z-A</option>
        </select>
      </div>

      <div class="hidden sm:flex items-center text-sm text-gray-500">
        <span>Dica: use a barra de busca da navbar para refinar.</span>
      </div>
    </div>

    <!-- Grid de resultados -->
    <div class="container mx-auto px-6 lg:px-20">
      <div v-if="carregando" class="text-center text-gray-500 py-20">Carregando...</div>

      <div v-else-if="produtosOrdenados.length" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8">
        <RouterLink
          v-for="produto in produtosOrdenados"
          :key="produto.id"
          :to="`/produto/${produto.id}`"
          class="bg-white rounded-2xl shadow-sm hover:shadow-lg transition transform hover:-translate-y-1"
        >
          <img :src="produto.imagemURL || produto.imagemUrl || placeholderImg" alt="Produto" class="rounded-t-2xl w-full h-56 object-cover" />
          <div class="p-5">
            <h3 class="font-semibold text-gray-800 text-lg mb-1 truncate">{{ produto.nome }}</h3>
            <p class="text-blue-600 font-bold text-lg">R$ {{ Number(produto.preco ?? 0).toFixed(2).replace('.', ',') }}</p>
          </div>
        </RouterLink>
      </div>

      <div v-else class="text-center text-gray-500 py-20">
        <p class="text-lg">Nenhum produto encontrado para "{{ termo }}"</p>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import productService from '../../services/productService.js'
import { RouterLink } from 'vue-router'

const route = useRoute()
const termo = computed(() => (route.query?.nome || '').toString())
const produtos = ref([])
const carregando = ref(false)
const ordenacao = ref('relevancia')
const placeholderImg = new URL('../../assets/images/imagem_sapateira.png', import.meta.url).href

const total = computed(() => produtos.value.length)

const produtosOrdenados = computed(() => {
  const lista = [...produtos.value]
  switch (ordenacao.value) {
    case 'preco_asc':
      return lista.sort((a, b) => Number(a.preco ?? 0) - Number(b.preco ?? 0))
    case 'preco_desc':
      return lista.sort((a, b) => Number(b.preco ?? 0) - Number(a.preco ?? 0))
    case 'nome_asc':
      return lista.sort((a, b) => (a.nome || '').localeCompare(b.nome || ''))
    case 'nome_desc':
      return lista.sort((a, b) => (b.nome || '').localeCompare(a.nome || ''))
    default:
      return lista
  }
})

async function carregarBusca() {
  const q = termo.value.trim()
  produtos.value = []
  if (!q) return
  try {
    carregando.value = true
    const { produtos: itens } = await productService.getAll({ nome: q, pagina: 1, itensPorPagina: 36 })
    produtos.value = itens
  } catch (err) {
    console.error('Erro ao buscar produtos:', err)
  } finally {
    carregando.value = false
  }
}

onMounted(carregarBusca)
watch(() => route.query?.nome, carregarBusca)
</script>

<style scoped>
.truncate { max-width: 100%; }
</style>
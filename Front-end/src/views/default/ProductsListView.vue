<template>
  <main class="min-h-screen py-10 px-6 md:px-20 bg-[#F5F7FB]">
    <header class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <h1 class="text-2xl font-semibold text-gray-900">Todos os Produtos</h1>
      <div class="flex items-center gap-2">
        <input
          v-model="busca"
          type="text"
          placeholder="Buscar por nome..."
          class="w-64 px-3 py-2 border rounded-md bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
        />
        <button @click="buscar" class="px-4 py-2 bg-[#141A7C] text-white rounded-md hover:bg-[#0f166a]">Buscar</button>
      </div>
    </header>

    <section>
      <div v-if="carregando" class="text-center text-gray-500 py-10">Carregando...</div>
      <div v-else>
        <div v-if="produtos.length" class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
          <RouterLink
            v-for="p in produtos"
            :key="p.id"
            :to="`/produto/${p.id}`"
            class="border rounded-xl bg-white p-3 hover:shadow-md transition flex flex-col items-center"
          >
            <img :src="p.imagemUrl || p.imagemURL || placeholderImg" class="w-[180px] h-[180px] object-cover rounded-lg mb-3" alt="Produto" />
            <h4 class="text-sm font-medium text-gray-800 w-full truncate">{{ p.nome }}</h4>
            <p class="text-gray-600 text-xs">R$ {{ Number(p.preco ?? 0).toFixed(2) }}</p>
          </RouterLink>
        </div>
        <p v-else class="text-center text-gray-500">Nenhum produto encontrado.</p>
      </div>
    </section>

    <footer v-if="totalPaginas > 1" class="mt-8 flex items-center justify-center gap-3">
      <button :disabled="paginaAtual <= 1" @click="mudarPagina(paginaAtual - 1)" class="px-3 py-1 border rounded-md bg-white disabled:opacity-50">Anterior</button>
      <span class="text-sm text-gray-700">Página {{ paginaAtual }} de {{ totalPaginas }}</span>
      <button :disabled="paginaAtual >= totalPaginas" @click="mudarPagina(paginaAtual + 1)" class="px-3 py-1 border rounded-md bg-white disabled:opacity-50">Próxima</button>
    </footer>
  </main>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { RouterLink, useRoute, useRouter } from 'vue-router';
import productService from '../../services/productService.js';

const route = useRoute();
const router = useRouter();

const busca = ref(String(route.query.nome || ''));
const produtos = ref([]);
const paginaAtual = ref(Number(route.query.pagina || 1));
const totalPaginas = ref(1);
const carregando = ref(false);
const placeholderImg = new URL('../../assets/images/imagem_sapateira.png', import.meta.url).href;

async function carregar() {
  carregando.value = true;
  try {
    const { produtos: itens, totalPaginas: tp } = await productService.getAll({ nome: busca.value || undefined, pagina: paginaAtual.value, itensPorPagina: 12 });
    produtos.value = itens;
    totalPaginas.value = tp || 1;
  } finally {
    carregando.value = false;
  }
}

function buscar() {
  paginaAtual.value = 1;
  router.replace({ path: route.path, query: { ...route.query, nome: busca.value || undefined, pagina: paginaAtual.value } });
  carregar();
}

function mudarPagina(p) {
  paginaAtual.value = p;
  router.replace({ path: route.path, query: { ...route.query, nome: busca.value || undefined, pagina: paginaAtual.value } });
  carregar();
}

onMounted(carregar);
watch(() => route.query, carregar);
</script>

<style scoped>
</style>
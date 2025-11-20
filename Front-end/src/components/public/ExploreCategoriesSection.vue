<template>
  <section class="py-10 px-6 md:px-20">
    <h2 class="text-3xl font-bold text-gray-900 mb-3">Explore Nossas Categorias</h2>
    <p class="text-gray-600 mb-10 max-w-2xl mx-auto">
      Encontre exatamente o que você procura navegando por nossas categorias.
    </p>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
      <div
        v-for="cat in categoriasRaiz"
        :key="cat.id"
        class="group relative bg-white border rounded-2xl shadow-sm hover:shadow-lg transition overflow-hidden"
      >
        <img :src="getCategoriaImage(cat.nome)" alt="Categoria" class="w-full h-44 sm:h-52 md:h-60 object-cover" />
        <RouterLink :to="`/categoria/${cat.id}`" class="block p-5">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-800 group-hover:text-blue-600">{{ cat.nome }}</h3>
            <span class="text-[12px] px-2 py-1 rounded-full bg-gray-100 text-gray-700">{{ contarProdutos(cat) }} produtos</span>
          </div>
          <p class="text-gray-500 text-sm mt-2">Categorias relacionadas</p>
          <div class="mt-3 flex flex-wrap gap-2">
            <span v-for="sub in (cat.subcategorias || []).slice(0, 6)" :key="sub.id" class="text-xs bg-gray-100 text-gray-700 rounded-full px-2 py-1">
              {{ sub.nome }}
            </span>
          </div>
        </RouterLink>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import api from '../../services/api.js';
import { RouterLink } from 'vue-router';

const categoriasRaiz = ref([]);

function normalizar(node) {
  return {
    id: node.id ?? node.Id,
    nome: node.nome ?? node.Nome,
    produtos: node.produtos ?? node.Produtos ?? [],
    subcategorias: (node.subcategorias ?? node.Subcategorias ?? []).map(normalizar),
  };
}

function contarProdutos(node) {
  let total = (node.produtos?.length ?? 0);
  (node.subcategorias || []).forEach((c) => total += contarProdutos(c));
  return total;
}

onMounted(async () => {
  const { data } = await api.get('/categoria/hierarquia');
  categoriasRaiz.value = (data || []).map(normalizar);
});

// Exporta funções para uso interno
defineExpose({ contarProdutos });

// Imagens das categorias (restauradas)
const categoriaImagens = {
  masculino: new URL('../../assets/categorias/Masculino.png', import.meta.url).href,
  feminino: new URL('../../assets/categorias/Feminino.png', import.meta.url).href,
  infantil: new URL('../../assets/categorias/Infantil.png', import.meta.url).href,
  casa: new URL('../../assets/categorias/Casa.png', import.meta.url).href,
  jeans: new URL('../../assets/categorias/Jeans.png', import.meta.url).href,
  'beleza e perfume': new URL('../../assets/categorias/Beleza e Perfume.png', import.meta.url).href,
  'eletrônicos': new URL('../../assets/categorias/Eletronicos.png', import.meta.url).href,
};

function getCategoriaImage(nome) {
  const key = (nome || '').toLowerCase();
  return categoriaImagens[key] || new URL('../../assets/images/imagem_sapateira.png', import.meta.url).href;
}
</script>

<style scoped>
.group:hover h3 { color: #2563eb; }
</style>
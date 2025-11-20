<template>
  <section class="bg-[#F5F7FB] min-h-screen pb-16">
    <!-- Cabeçalho da categoria -->
    <div class="text-center py-12 bg-white shadow-sm mb-10">
      <h1 class="text-4xl font-bold text-gray-900 mb-3">
        {{ categoriaAtual?.nome || "Carregando..." }}
      </h1>
      <p class="text-gray-600 text-lg">
        Explore os produtos disponíveis nesta categoria
      </p>
    </div>

    <div class="container mx-auto px-6 lg:px-20">
      <!-- Subcategorias (cards profissionais) -->
      <div v-if="subcategorias.length" class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-800 mb-6">Subcategorias</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          <RouterLink
            v-for="sub in subcategorias"
            :key="sub.id"
            :to="`/categoria/${sub.id}`"
            class="group relative bg-white border rounded-2xl shadow-sm hover:shadow-md transition overflow-hidden"
          >
            <img :src="getCategoriaImage(sub.nome)" alt="Imagem da categoria" class="w-full h-40 object-cover" />
            <div class="p-4">
              <h3 class="font-semibold text-gray-800 group-hover:text-blue-600">
                {{ sub.nome }}
              </h3>
              <p class="text-sm text-gray-500">{{ contarProdutos(sub) }} produtos</p>
            </div>
          </RouterLink>
        </div>
      </div>

      <!-- Produtos -->
      <div v-if="produtos.length" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8">
        <RouterLink
          v-for="produto in produtos"
          :key="produto.id"
          :to="`/produto/${produto.id}`"
          class="bg-white rounded-2xl shadow-sm hover:shadow-lg transition transform hover:-translate-y-1"
        >
          <img
            :src="produto.imagemURL || '/sem-imagem.png'"
            alt="Produto"
            class="rounded-t-2xl w-full h-56 object-cover"
          />
          <div class="p-5 text-center">
            <h3 class="font-semibold text-gray-800 text-lg mb-2">
              {{ produto.nome }}
            </h3>
            <p class="text-gray-500 text-sm line-clamp-2 mb-3">
              {{ produto.descricao }}
            </p>
            <p class="text-blue-600 font-bold text-lg mb-2">
              R$ {{ Number(produto.preco ?? 0).toFixed(2).replace('.', ',') }}
            </p>
            <span class="inline-block text-xs text-gray-400">Clique para ver detalhes</span>
          </div>
        </RouterLink>
      </div>

      <!-- Nenhum produto encontrado -->
      <div v-else class="text-center text-gray-500 mt-20">
        <p class="text-lg">Nenhum produto encontrado nesta categoria 😕</p>
      </div>
    </div>

    <!-- Sem paginação na visualização de categorias -->
  </section>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import { useRoute } from "vue-router";
import api from "../../services/api.js";
import productService from "../../services/productService.js";

const route = useRoute();

const categoriaAtual = ref(null);
const subcategorias = ref([]);
const produtos = ref([]);
// Sem paginação na CategoryView

// Mapeamento de imagens por nome (ajuste conforme seus assets)
const categoriaImagens = {
  masculino: new URL("../../assets/categorias/Masculino.png", import.meta.url).href,
  feminino: new URL("../../assets/categorias/Feminino.png", import.meta.url).href,
  infantil: new URL("../../assets/categorias/Infantil.png", import.meta.url).href,
  casa: new URL("../../assets/categorias/Casa.png", import.meta.url).href,
  jeans: new URL("../../assets/categorias/Jeans.png", import.meta.url).href,
  "beleza e perfume": new URL("../../assets/categorias/Beleza e Perfume.png", import.meta.url).href,
  "eletrônicos": new URL("../../assets/categorias/Eletronicos.png", import.meta.url).href,
};

const getCategoriaImage = (nome) => {
  const key = (nome || "").toLowerCase();
  return categoriaImagens[key] || new URL("../../assets/images/imagem_sapateira.png", import.meta.url).href;
};

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
  (node.subcategorias || []).forEach((c) => (total += contarProdutos(c)));
  return total;
}

// 🔹 Busca categoria atual e subcategorias relacionadas (hierarquia)
async function carregarCategoria() {
  const idAtual = route.params.id;
  const { data } = await api.get("/categoria/hierarquia");
  const raiz = (data || []).map(normalizar);

  // Encontrar o nó atual na hierarquia
  function findNode(nodes, id) {
    for (const n of nodes) {
      if ((n.id || n.Id) === id) return n;
      const found = findNode(n.subcategorias || [], id);
      if (found) return found;
    }
    return null;
  }

  const atual = findNode(raiz, idAtual);
  categoriaAtual.value = atual || null;
  subcategorias.value = atual?.subcategorias || [];
  await carregarProdutos(idAtual);
}

// 🔹 Busca produtos usando backend com suporte a descendentes
async function carregarProdutos(idAtual) {
  // Carrega todos os produtos da categoria sem paginação (tamanho grande)
  const { produtos: itens } = await productService.getAll({ categoriaId: idAtual, pagina: 1, itensPorPagina: 1000 });
  produtos.value = itens;
}

// 🔹 Função recursiva para pegar IDs de subcategorias
function obterSubcategorias(lista, paiId) {
  const ids = [paiId];
  lista.forEach((c) => {
    if (c.categoriaPaiId === paiId) {
      ids.push(...obterSubcategorias(lista, c.id));
    }
  });
  return ids;
}

onMounted(carregarCategoria);

// 🔹 Atualiza caso o usuário clique em outra categoria sem recarregar a página
watch(
  () => route.params.id,
  () => {
    categoriaAtual.value = null;
    produtos.value = [];
    carregarCategoria();
  }
);
</script>
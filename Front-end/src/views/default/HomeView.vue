<template>
  <section class="flex flex-col-reverse md:flex-row items-center justify-between gap-6 px-6 py-1 bg-gray-50">
    <div class="items-center w-full">
      <Swiper
        :modules="[Autoplay, Pagination]"
        :autoplay="{ delay: 3000 }"
        :loop="true"
        pagination
        class="w-full max-w-[1400px] md:w-full h-[480px] md:h-[600px] rounded-2xl shadow-lg"
      >
        <SwiperSlide v-for="(slide, i) in slides" :key="i">
          <div class="relative w-full h-full rounded-2xl overflow-hidden">
            <img :src="slide.src" :alt="`Banner ${slide.label}`" class="w-full h-full object-cover" />
            <div class="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent"></div>
            <div class="absolute left-6 bottom-6 text-white">
              <h3 class="text-2xl font-semibold capitalize">{{ slide.label }}</h3>
              <RouterLink
                :to="{ name: 'search', query: { nome: slide.label } }"
                class="mt-2 inline-block px-4 py-2 bg-white text-gray-900 rounded-md hover:bg-gray-100"
              >Ver mais</RouterLink>
            </div>
          </div>
        </SwiperSlide>
      </Swiper>
    </div>
  </section>

  <!-- Benefícios -->
  <section class="bg-white py-14 px-6 md:px-20 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8 text-center">
    <div
      v-for="(benefit, i) in benefits"
      :key="i"
      class="flex flex-col items-center space-y-3 p-4 border rounded-xl hover:shadow-md transition"
    >
      <component :is="benefit.icon" class="size-6 text-gray-700" />
      <h4 class="text-lg font-semibold text-gray-800">{{ benefit.title }}</h4>
      <p class="text-gray-500">{{ benefit.description }}</p>
    </div>
  </section>

  <!-- Categorias (refatorado) -->
  <ExploreCategoriesSection />


  <!-- Resultados da Busca removidos (usamos SearchView) -->

  <!-- Produtos em Destaque -->
  <section class="text-center py-10 px-6 md:px-20">
    <h2 class="text-3xl font-bold text-gray-900 mb-3">Produtos em Destaque</h2>
    <p class="text-gray-600 mb-10 max-w-2xl mx-auto">
      Selecionamos os melhores produtos para você
    </p>

    <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-6 justify-items-center">
      <RouterLink
        v-for="produto in produtosDestaque"
        :key="produto.id"
        :to="`/produto/${produto.id}`"
        class="flex flex-col items-center space-y-3 p-4 border rounded-xl hover:shadow-md transition w-full max-w-[240px] md:max-w-[260px]"
      >
        <img
          :src="produto.imagemUrl || produto.imagemURL || placeholderImg"
          alt="Produto"
          class="rounded-2xl shadow-lg w-full h-56 md:h-64 object-cover"
        />
        <h4 class="text-lg font-semibold text-gray-800 truncate w-full">{{ produto.nome }}</h4>
        <p class="text-gray-600">R$ {{ Number(produto.preco ?? 0).toFixed(2) }}</p>
      </RouterLink>
    </div>
  </section>

</template>

<script setup>
import { Swiper, SwiperSlide } from "swiper/vue";
import "swiper/css";
import "swiper/css/pagination";
import { Autoplay, Pagination } from "swiper/modules";
import { ref, onMounted } from "vue";
import api from "../../services/api.js";
import productService from "../../services/productService.js";
import { RouterLink } from "vue-router";
import ExploreCategoriesSection from "../../components/public/ExploreCategoriesSection.vue";

const categoriaImagens = {
  masculino: new URL("../../assets/categorias/Masculino.png", import.meta.url).href,
  feminino: new URL("../../assets/categorias/Feminino.png", import.meta.url).href,
  infantil: new URL("../../assets/categorias/Infantil.png", import.meta.url).href,
  casa: new URL("../../assets/categorias/Casa.png", import.meta.url).href,
  jeans: new URL("../../assets/categorias/Jeans.png", import.meta.url).href,
  "beleza e perfume": new URL("../../assets/categorias/Beleza e Perfume.png", import.meta.url).href,
  "eletrônicos": new URL("../../assets/categorias/Eletronicos.png", import.meta.url).href,
};

// === Dados locais ===
// Slides neutros baseados nas categorias (sem discurso de oferta/promoção)
const slides = Object.entries(categoriaImagens).map(([label, src]) => ({ label, src }));

const benefits = [
  {
    title: "Entrega Rápida",
    description: "Com as melhores transportadoras",
    icon: {
      template: `<svg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke-width='1.5' stroke='currentColor'><path stroke-linecap='round' stroke-linejoin='round' d='M8.25 18.75a1.5 1.5 0 0 1-3 0m3 0a1.5 1.5 0 0 0-3 0m3 0h6m-9 0H3.375a1.125 1.125 0 0 1-1.125-1.125V14.25m17.25 4.5a1.5 1.5 0 0 1-3 0m3 0a1.5 1.5 0 0 0-3 0m3 0h1.125c.621 0 1.129-.504 1.09-1.124a17.902 17.902 0 0 0-3.213-9.193 2.056 2.056 0 0 0-1.58-.86H14.25M16.5 18.75h-2.25m0-11.177v-.958c0-.568-.422-1.048-.987-1.106a48.554 48.554 0 0 0-10.026 0 1.106 1.106 0 0 0-.987 1.106v7.635m12-6.677v6.677m0 4.5v-4.5m0 0h-12'/></svg>`,
    },
  },
  {
    title: "Compra Segura",
    description: "100% protegida",
    icon: {
      template: `<svg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke-width='1.5' stroke='currentColor'><path stroke-linecap='round' stroke-linejoin='round' d='M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 0 0 2.25-2.25V6.75A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25v10.5A2.25 2.25 0 0 0 4.5 19.5Z'/></svg>`,
    },
  },
  {
    title: "Suporte 24/7",
    description: "Sempre disponível",
    icon: {
      template: `<svg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke-width='1.5' stroke='currentColor'><path stroke-linecap='round' stroke-linejoin='round' d='M20.25 8.511c.884.284 1.5 1.128 1.5 2.097v4.286c0 1.136-.847 2.1-1.98 2.193-.34.027-.68.052-1.02.072v3.091l-3-3c-1.354 0-2.694-.055-4.02-.163a2.115 2.115 0 0 1-.825-.242m9.345-8.334a2.126 2.126 0 0 0-.476-.095 48.64 48.64 0 0 0-8.048 0c-1.131.094-1.976 1.057-1.976 2.192v4.286c0 .837.46 1.58 1.155 1.951m9.345-8.334V6.637c0-1.621-1.152-3.026-2.76-3.235A48.455 48.455 0 0 0 11.25 3c-2.115 0-4.198.137-6.24.402-1.608.209-2.76 1.614-2.76 3.235v6.226c0 1.621 1.152 3.026 2.76 3.235.577.075 1.157.14 1.74.194V21l4.155-4.155'/></svg>`,
    },
  },
];

// === Estado ===
const categoriasPrincipais = ref([]); // não usado após refatoração, mantido se necessário
const produtosDestaque = ref([]);
const placeholderImg = new URL("../../assets/images/imagem_sapateira.png", import.meta.url).href;

// === Funções ===
const getCategoriaImage = (nome) => {
  const key = nome.toLowerCase();
  return categoriaImagens[key] || placeholderImg;
};

// === Requisições ===
const fetchCategorias = async () => {
  const { data } = await api.get("/categoria");

  // Monta mapa de categorias
  const mapa = {};
  data.forEach((cat) => (mapa[cat.id] = { ...cat, subcategorias: [] }));

  // Conecta pais e filhos
  const categoriasRaiz = [];
  data.forEach((cat) => {
    if (cat.categoriaPaiId) {
      mapa[cat.categoriaPaiId]?.subcategorias.push(mapa[cat.id]);
    } else {
      categoriasRaiz.push(mapa[cat.id]);
    }
  });

  categoriasPrincipais.value = categoriasRaiz;
};

const fetchProdutos = async () => {
  // Usa os mais vendidos como destaque para Home
  try {
    const { data } = await api.get('/Pedido/produtos-mais-vendidos');
    const basicos = Array.isArray(data) ? data.slice(0, 12) : [];
    // Enriquecer com imagem e descrição ao buscar detalhamento
    const detalhados = await Promise.all(
      basicos.map(async (p) => {
        try {
          const id = p.produtoId ?? p.id; // alguns endpoints retornam produtoId
          const full = await productService.getById(id);
          return { ...full, quantidadeVendida: p.quantidadeVendida };
        } catch {
          // Garantir forma mínima para card
          return {
            id: p.produtoId ?? p.id,
            nome: p.nome,
            preco: p.preco,
            imagemURL: p.imagemURL || p.imagemUrl,
            quantidadeVendida: p.quantidadeVendida,
          };
        }
      })
    );
    produtosDestaque.value = detalhados;
  } catch (e) {
    // Fallback para primeiros produtos se endpoint não disponível
    const { produtos } = await productService.getAll({ pagina: 1, itensPorPagina: 8 });
    produtosDestaque.value = produtos;
  }
};


onMounted(() => {
  fetchProdutos();
});
</script>

<style scoped>
.group:hover h4 {
  color: #2563eb;
}
</style>

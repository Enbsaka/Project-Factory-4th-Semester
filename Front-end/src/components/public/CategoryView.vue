<template>
  <section class="py-10 px-6 md:px-20">
    <h2 class="text-3xl font-bold text-gray-900 mb-3">
      {{ categoriaSelecionada?.nome }}
    </h2>

    <!-- Subcategorias -->
    <div v-if="subcategorias.length" class="mt-6 grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-6">
      <div
        v-for="sub in subcategorias"
        :key="sub.id"
        class="cursor-pointer border rounded-xl hover:shadow-md transition p-3"
        @click="selecionarSubcategoria(sub)"
      >
        <h4 class="font-semibold text-gray-700">{{ sub.nome }}</h4>
      </div>
    </div>

    <!-- Produtos -->
    <div v-if="produtos.length" class="mt-10">
      <h3 class="text-2xl font-semibold mb-4">Produtos</h3>
      <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-6">
        <div
          v-for="prod in produtos"
          :key="prod.id"
          class="border rounded-xl hover:shadow-lg transition p-3 flex flex-col items-center"
        >
          <img
            :src="getImagemProduto(prod)"
            alt="Produto"
            class="w-[180px] h-[180px] object-cover rounded-xl mb-3"
          />
          <h4 class="font-medium text-gray-800">{{ prod.nome }}</h4>
          <p class="text-gray-600 text-sm">R$ {{ prod.preco?.toFixed(2) }}</p>
        </div>
      </div>
    </div>

    <p v-else-if="subcategoriaSelecionada" class="text-gray-500 mt-10">
      Nenhum produto encontrado nessa subcategoria.
    </p>
  </section>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import api from "../../services/api";

const route = useRoute();
const categoriaId = route.params.id;

const categoriaSelecionada = ref(null);
const subcategorias = ref([]);
const produtos = ref([]);
const subcategoriaSelecionada = ref(null);

onMounted(async () => {
  await carregarCategoria();
});

async function carregarCategoria() {
  try {
    const { data: categorias } = await api.get("/categorias");
    categoriaSelecionada.value = categorias.find(c => c.id === categoriaId);

    subcategorias.value = categorias.filter(c => c.categoriaPaiId === categoriaId);
  } catch (err) {
    console.error("Erro ao carregar categoria:", err);
  }
}

async function selecionarSubcategoria(sub) {
  subcategoriaSelecionada.value = sub;
  try {
    const { data } = await api.get(`/produtos?categoriaId=${sub.id}`);
    produtos.value = data;
  } catch (err) {
    console.error("Erro ao carregar produtos:", err);
  }
}

function getImagemProduto(prod) {
  return prod.imagemUrl
    ? prod.imagemUrl
    : new URL("../../assets/images/default-produto.jpg", import.meta.url).href;
}
</script>

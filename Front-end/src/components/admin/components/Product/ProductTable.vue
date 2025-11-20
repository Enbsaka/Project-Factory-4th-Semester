<template>
  <section class="bg-[#F5F7FB] p-6">
    <div class="overflow-x-auto bg-white shadow-md rounded-xl">
      <table class="min-w-full text-gray-700">
        <thead class="bg-gray-100 border-b">
          <tr>
            <th class="py-3 px-6 text-left text-lg font-semibold">Produto</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Preço</th>
            <th class="py-3 px-6 text-left text-lg font-semibold">Categoria</th>
            <th class="py-3 px-6 text-center text-lg font-semibold">Variações</th>
            <th class="py-3 px-6 text-center text-lg font-semibold">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-if="carregando">
            <td :colspan="totalColspan" class="py-6 text-center text-gray-500 text-lg">Carregando produtos...</td>
          </tr>
          <tr v-else-if="erro">
            <td :colspan="totalColspan" class="py-6 text-center text-red-500 text-lg">{{ erro }}</td>
          </tr>
          <tr v-else-if="produtos.length === 0">
            <td :colspan="totalColspan" class="py-6 text-center text-gray-500 text-lg">Nenhum produto encontrado.</td>
          </tr>
          <tr v-else v-for="produto in produtos" :key="produto.id" class="hover:bg-gray-50 transition">
            <td class="py-4 px-6 flex items-center gap-4">
              <img :src="produto.imagem" alt="Produto" class="w-16 h-16 object-cover rounded-lg border"/>
              <div>
                <p class="font-medium text-gray-800 text-md">{{ produto.nome }}</p>
                <p class="text-sm text-gray-500">ID: {{ produto.id }}</p>
              </div>
            </td>
            <td class="py-4 px-6 font-semibold text-md">{{ produto.preco }}</td>
            <td class="py-4 px-6 text-md">{{ produto.categoria }}</td>
            <td class="py-4 px-6 text-center">
              <div v-if="produto.variacoes && produto.variacoes.length > 0" class="text-sm text-gray-700">
                {{ produto.variacoes.length }} variação(ões)
              </div>
              <div v-else class="text-gray-400 text-sm">Nenhuma variação</div>
            </td>
            <td class="py-4 px-6 text-center relative">
              <button
                @click.stop="alternarMenu(produto.id)"
                class="p-2 rounded hover:bg-gray-100 text-gray-600 hover:text-blue-600 transition"
                aria-label="Mais ações"
              >
                <CircleEllipsis class="w-6 h-6" />
              </button>
              <button
                @click="$emit('detalhes', produto)"
                class="p-2 ml-1 rounded hover:bg-gray-100 text-gray-600 hover:text-indigo-600 transition"
                aria-label="Detalhes do produto"
              >
                <View class="w-6 h-6" />
              </button>
              <div
                v-if="menuAberto === produto.id"
                class="absolute right-6 top-10 bg-white border border-gray-200 rounded-lg shadow-lg w-44 z-10"
              >
                <ul class="text-sm text-gray-700">
                  <li>
                    <button
                      @click="$emit('editar', produto); menuAberto = null"
                      class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100"
                    >
                      <Pencil class="w-4 h-4" /> Editar
                    </button>
                  </li>
                  <li>
                    <button
                      @click="$emit('variacoes', produto); menuAberto = null"
                      class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100"
                    >
                      <Puzzle class="w-4 h-4" /> Variações
                    </button>
                  </li>
                  <li>
                    <button
                      @click="$emit('excluir', produto); menuAberto = null"
                      class="flex items-center gap-2 w-full text-left px-4 py-2 text-red-600 hover:bg-gray-100"
                    >
                      <Trash class="w-4 h-4" /> Excluir
                    </button>
                  </li>
                </ul>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { CircleEllipsis, View, Pencil, Puzzle, Trash } from "lucide-vue-next";

const props = defineProps({
  produtos: Array,
  carregando: Boolean,
  erro: String,
});

const emit = defineEmits(["editar", "detalhes", "variacoes", "excluir"]);

const menuAberto = ref(null);

// Colspan fixo: Produto, Preço, Categoria, Variações, Ações = 5
const totalColspan = computed(() => 5);

function alternarMenu(id) {
  menuAberto.value = menuAberto.value === id ? null : id;
}

// Fecha menu ao clicar fora
onMounted(() => {
  document.addEventListener("click", (e) => {
    if (!e.target.closest("td")) menuAberto.value = null;
  });
});
</script>
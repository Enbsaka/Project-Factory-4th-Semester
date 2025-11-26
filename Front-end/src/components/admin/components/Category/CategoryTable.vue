<template>
  <div class="overflow-x-auto bg-white shadow-md rounded-xl">
    <table class="table-auto w-full text-left">
      <thead class="bg-gray-100 border-b">
        <tr>
          <th class="py-3 px-6 text-left text-lg font-semibold">Nome</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">Tipo</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">Categoria Pai</th>
          <th class="py-3 px-6 text-left text-lg font-semibold">Produtos</th>
          <th class="py-3 px-6 text-center text-lg font-semibold">Ações</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-gray-200">
        <tr v-for="cat in categorias" :key="cat.id" class="hover:bg-gray-50">
          <td class="py-4 px-6">
            <p class="font-medium text-gray-900">{{ cat.nome }}</p>
            <p v-if="cat.categoriaPaiId" class="text-xs text-gray-500">ID: {{ cat.id }}</p>
          </td>
          <td class="py-4 px-6">
            <span :class="tipoClass(cat)" class="text-xs px-2 py-1 rounded-full">{{ cat.tipo }}</span>
          </td>
          <td class="py-4 px-6">{{ cat.paiNome }}</td>
          <td class="py-4 px-6">{{ cat.totalProdutos }}</td>
          <td class="py-4 px-6 text-center relative">
            <button
              @click.stop="alternarMenu(cat.id)"
              class="p-2 rounded hover:bg-gray-100 text-gray-600 hover:text-blue-600 transition"
              aria-label="Mais ações"
            >
              <CircleEllipsis class="w-6 h-6" />
            </button>
            <div
              v-if="menuAberto === cat.id"
              class="absolute right-6 top-10 bg-white border border-gray-200 rounded-lg shadow-lg w-44 z-10"
            >
              <ul class="text-sm text-gray-700">
                <li>
                  <button
                    @click="$emit('editar', cat); menuAberto = null"
                    class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100"
                  >
                    <Pencil class="w-4 h-4" /> Editar
                  </button>
                </li>
                <li>
                  <button
                    @click="$emit('nova-sub', cat); menuAberto = null"
                    class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100"
                  >
                    <BadgePlus class="w-4 h-4" /> Subcategoria
                  </button>
                </li>
                <li>
                  <button
                    @click="$emit('excluir', cat); menuAberto = null"
                    class="flex items-center gap-2 w-full text-left px-4 py-2 text-red-600 hover:bg-gray-100"
                  >
                    <Trash class="w-4 h-4" /> Excluir
                  </button>
                </li>
              </ul>
            </div>
          </td>
        </tr>
        <tr v-if="!categorias || categorias.length === 0">
          <td colspan="5" class="py-6 text-center text-gray-500">Nenhuma categoria encontrada.</td>
        </tr>
      </tbody>
    </table>
  </div>
  
</template>

<script setup>
import { ref, onMounted } from "vue";
import { CircleEllipsis, Pencil, BadgePlus, Trash } from "lucide-vue-next";
const props = defineProps({
  categorias: { type: Array, default: () => [] },
});
defineEmits(["editar", "excluir", "nova-sub"]);

const menuAberto = ref(null);
function alternarMenu(id) {
  menuAberto.value = menuAberto.value === id ? null : id;
}
onMounted(() => {
  document.addEventListener("click", (e) => {
    if (!e.target.closest("td")) menuAberto.value = null;
  });
});

function tipoClass(cat) {
  return cat.tipo === "Categoria Pai"
    ? "bg-blue-100 text-blue-700"
    : "bg-purple-100 text-purple-700";
}
</script>
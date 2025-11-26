<template>
  <div class="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
      <div class="px-5 py-4 border-b">
        <h3 class="text-lg font-semibold text-gray-800">Editar Categoria</h3>
      </div>
      <form class="p-5 space-y-4" @submit.prevent="onSalvar">
        <div>
          <label class="block text-sm text-gray-700 mb-1">Nome</label>
          <input v-model="form.nome" type="text" class="w-full border rounded-md px-3 py-2" required />
        </div>
        <div>
          <label class="block text-sm text-gray-700 mb-1">Categoria Pai (opcional)</label>
          <select v-model="form.categoriaPaiId" class="w-full border rounded-md px-3 py-2">
            <option :value="null">— Sem categoria pai —</option>
            <option v-for="p in pais" :key="p.id" :value="p.id">{{ p.nome }}</option>
          </select>
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button type="button" @click="$emit('fechar')" class="px-3 py-2 rounded-md border">Cancelar</button>
          <button type="submit" class="px-3 py-2 rounded-md bg-[#141A7C] text-white">Salvar</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed } from "vue";

const props = defineProps({
  categoria: Object,
  categoriasTree: { type: Array, default: () => [] },
});
const emit = defineEmits(["fechar", "salvar"]);

const form = ref({ nome: "", categoriaPaiId: null });

watch(
  () => props.categoria,
  (val) => {
    form.value = {
      nome: val?.nome ?? "",
      categoriaPaiId: val?.categoriaPaiId ?? null,
    };
  },
  { immediate: true }
);

const pais = computed(() => {
  // pais = nós da árvore sem pai
  function flatten(nodes) {
    const acc = [];
    for (const n of nodes || []) {
      const id = n.id ?? n.Id;
      const nome = n.nome ?? n.Nome;
      acc.push({ id, nome, categoriaPaiId: n.CategoriaPaiId ?? n.categoriaPaiId ?? null });
      acc.push(...flatten(n.subcategorias || []));
    }
    return acc;
  }
  return flatten(props.categoriasTree).filter((c) => !c.categoriaPaiId);
});

function onSalvar() {
  emit("salvar", { ...form.value });
}
</script>
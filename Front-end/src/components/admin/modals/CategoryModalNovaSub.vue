<template>
  <div class="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
      <div class="px-5 py-4 border-b">
        <h3 class="text-lg font-semibold text-gray-800">Nova Subcategoria</h3>
      </div>
      <form class="p-5 space-y-4" @submit.prevent="onCriar">
        <p class="text-sm text-gray-600">Categoria pai: <span class="font-medium text-gray-800">{{ pai?.nome }}</span></p>
        <div>
          <label class="block text-sm text-gray-700 mb-1">Nome da subcategoria</label>
          <input v-model="nome" type="text" class="w-full border rounded-md px-3 py-2" required />
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button type="button" @click="$emit('fechar')" class="px-3 py-2 rounded-md border">Cancelar</button>
          <button type="submit" class="px-3 py-2 rounded-md bg-[#141A7C] text-white">Criar</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
const props = defineProps({ pai: Object });
const emit = defineEmits(["fechar", "criar"]);
const nome = ref("");
watch(() => props.pai, () => { nome.value = ""; });
function onCriar() { emit("criar", nome.value); }
</script>
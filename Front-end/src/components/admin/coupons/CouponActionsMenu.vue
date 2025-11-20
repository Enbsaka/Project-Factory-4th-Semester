<template>
  <div class="relative inline-block text-left">
    <button
      class="p-2 rounded hover:bg-gray-100 text-gray-600 hover:text-blue-600 transition"
      @click="open = !open"
      aria-label="Mais ações"
    >
      <CircleEllipsis class="w-6 h-6" />
    </button>
    <div
      v-if="open"
      class="absolute right-0 mt-2 w-36 bg-white border rounded shadow z-10"
    >
      <button
        class="flex items-center gap-2 w-full text-left px-3 py-2 hover:bg-gray-100"
        @click="onEditar"
      >
        <Pencil class="w-4 h-4" /> Editar
      </button>
      <button
        class="flex items-center gap-2 w-full text-left px-3 py-2 hover:bg-gray-100 text-red-600"
        @click="onExcluir"
      >
        <Trash class="w-4 h-4" /> Excluir
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { CircleEllipsis, Pencil, Trash } from "lucide-vue-next";

const props = defineProps({
  cupom: { type: Object, required: true },
});
const emit = defineEmits(["editar", "excluir"]);
const open = ref(false);

const onEditar = () => {
  open.value = false;
  emit("editar", props.cupom);
};
const onExcluir = () => {
  open.value = false;
  emit("excluir", props.cupom);
};
</script>

<style scoped>
</style>
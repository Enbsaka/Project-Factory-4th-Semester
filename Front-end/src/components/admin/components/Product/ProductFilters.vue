<template>
  <section class="bg-[#F5F7FB] rounded-lg p-4 flex flex-wrap justify-between items-center shadow-sm mb-8">
    <AdminInput
      class="w-full md:w-1/2"
      :icon="Search"
      placeholder="Buscar produto"
      :model-value="localBusca"
      @update:modelValue="onInputDebounced($event)"
    />

    <div class="flex space-x-3 mt-3 md:mt-0 items-center">
      <CategoryFilterDropdown
        :categorias="categorias"
        v-model="localFiltro"
        @change="emitBuscar"
      />
    </div>
  </section>
</template>

<script setup>
import { ref, watch } from "vue";
import CategoryFilterDropdown from "../CategoryFilterDropdown.vue";
import AdminInput from "../AdminInput.vue";
import { Search } from "lucide-vue-next";

const props = defineProps({
  categorias: Array,
  busca: String,
  filtroCategorias: { type: Array, default: () => [] },
});
const emit = defineEmits(["update:busca", "update:filtro-categorias", "buscar"]);

const localBusca = ref(props.busca);
const localFiltro = ref(Array.isArray(props.filtroCategorias) ? [...props.filtroCategorias] : []);

function emitBuscar() {
  emit("update:busca", localBusca.value);
  emit("update:filtro-categorias", localFiltro.value);
  emit("buscar");
}

// Debounce para busca (evita requisição a cada tecla)
let debounceTimer = null;
function onInputDebounced() {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    emitBuscar();
  }, 300);
}

// Mantém sincronização entre pai e componente
watch(
  () => props.busca,
  (val) => (localBusca.value = val)
);
watch(
  () => props.filtroCategorias,
  (val) => (localFiltro.value = Array.isArray(val) ? [...val] : [])
);
</script>
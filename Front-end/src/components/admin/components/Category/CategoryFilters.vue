<template>
  <section class="bg-[#F5F7FB] rounded-lg p-4 flex flex-wrap justify-between items-center shadow-sm mb-8">
    <AdminInput
      class="w-full md:w-1/2"
      :icon="Search"
      placeholder="Buscar categoria"
      :model-value="localBusca"
      @update:modelValue="onInputDebounced($event)"
    />

    <div class="flex space-x-3 mt-3 md:mt-0 items-center">
      <CategoryFilterDropdown
        :categorias="categorias"
        v-model="localFiltro"
        @change="emitBuscar"
      />
      <label class="flex items-center gap-2 text-sm text-gray-700">
        <input type="checkbox" v-model="apenasPais" @change="emitBuscar" />
        Apenas categorias pai
      </label>
      <label class="flex items-center gap-2 text-sm text-gray-700">
        <input type="checkbox" v-model="apenasSubs" @change="emitBuscar" />
        Apenas subcategorias
      </label>
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
  mostrarApenasPais: Boolean,
  mostrarApenasSubs: Boolean,
});
const emit = defineEmits([
  "update:busca",
  "update:filtro-categorias",
  "update:mostrar-apenas-pais",
  "update:mostrar-apenas-subs",
  "buscar",
]);

const localBusca = ref(props.busca);
const localFiltro = ref(Array.isArray(props.filtroCategorias) ? [...props.filtroCategorias] : []);
const apenasPais = ref(!!props.mostrarApenasPais);
const apenasSubs = ref(!!props.mostrarApenasSubs);

function emitBuscar() {
  emit("update:busca", localBusca.value);
  emit("update:filtro-categorias", localFiltro.value);
  emit("update:mostrar-apenas-pais", !!apenasPais.value);
  emit("update:mostrar-apenas-subs", !!apenasSubs.value);
  emit("buscar");
}

let debounceTimer = null;
function onInputDebounced(val) {
  localBusca.value = val;
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    emitBuscar();
  }, 300);
}

watch(() => props.busca, (val) => (localBusca.value = val));
watch(() => props.filtroCategorias, (val) => (localFiltro.value = Array.isArray(val) ? [...val] : []));
watch(() => props.mostrarApenasPais, (val) => (apenasPais.value = !!val));
watch(() => props.mostrarApenasSubs, (val) => (apenasSubs.value = !!val));
</script>
<template>
  <section class="bg-[#F5F7FB] rounded-lg p-4 flex flex-wrap justify-between items-center shadow-sm mb-8">
    <AdminInput
      class="w-full md:w-1/2"
      :icon="Search"
      placeholder="Buscar por nome, e-mail ou CPF..."
      :model-value="busca"
      @update:modelValue="$emit('update:busca', $event)"
    />
    <div class="flex items-center gap-2 w-full md:w-auto mt-3 md:mt-0">
      <label class="text-sm text-gray-700">Ordenar cadastro:</label>
      <div class="relative">
        <select
          :value="ordenacao"
          @change="$emit('update:ordenacao', $event.target.value)"
          class="border border-gray-300 rounded-md px-3 py-2 bg-white text-sm text-gray-700 appearance-none pr-8 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="desc">Mais recente</option>
          <option value="asc">Mais antigo</option>
        </select>
        <ChevronDown class="w-4 h-4 text-gray-500 absolute right-2 top-1/2 -translate-y-1/2 pointer-events-none" />
      </div>
    </div>
  </section>
</template>

<script setup>
import { Search, ChevronDown } from "lucide-vue-next";
import AdminInput from "../AdminInput.vue";
defineProps({
  busca: { type: String, default: "" },
  ordenacao: { type: String, default: "desc" },
});
defineEmits(["update:busca", "update:ordenacao"]);
</script>

<style scoped>
</style>
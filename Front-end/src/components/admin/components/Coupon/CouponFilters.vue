<template>
  <section class="bg-[#F5F7FB] rounded-lg p-4 flex flex-wrap justify-between items-center shadow-sm mb-8">
    <AdminInput
      class="w-full md:w-1/2"
      :icon="Search"
      placeholder="Buscar por código..."
      :model-value="busca"
      @update:modelValue="$emit('update:busca', $event)"
    />
    <div class="mt-3 md:mt-0 w-full md:w-1/3">
      <label class="block text-sm font-medium mb-1 text-gray-700">Status</label>
      <div class="relative">
        <select
          :value="status"
          @change="$emit('update:status', $event.target.value)"
          class="border border-gray-300 rounded-md w-full px-3 py-2 bg-white text-sm text-gray-700 appearance-none focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">Todos</option>
          <option value="ativo">Ativo</option>
          <option value="inativo">Inativo</option>
        </select>
        <ChevronDown class="w-4 h-4 text-gray-500 absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none" />
      </div>
    </div>
  </section>
</template>

<script setup>
import { Search, ChevronDown } from "lucide-vue-next";
import AdminInput from "../AdminInput.vue";
defineProps({
  busca: { type: String, default: "" },
  status: { type: String, default: "" },
});
defineEmits(["update:busca", "update:status"]);
</script>

<style scoped>
</style>
<template>
  <section class="bg-[#F5F7FB] rounded-lg p-4 flex flex-wrap justify-between items-center shadow-sm mb-8">
    <AdminInput
      class="w-full md:w-1/2"
      :icon="Search"
      placeholder="Buscar pedido/cliente"
      :model-value="busca"
      @update:modelValue="$emit('update:busca', $event)"
    />

    <div class="flex space-x-3 mt-3 md:mt-0 items-center">
      <div class="relative">
        <select
          :value="status"
          @change="$emit('update:status', $event.target.value)"
          class="border border-gray-300 rounded-md px-3 py-2 text-sm text-gray-700 bg-white appearance-none focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">Todos os status</option>
          <option value="0">Carrinho</option>
          <option value="1">Finalizado</option>
        </select>
        <ChevronDown class="w-4 h-4 text-gray-500 absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none" />
      </div>
      <OrderDateRangeFilter
        :data-inicio="dataInicio"
        :data-fim="dataFim"
        @update:data-inicio="$emit('update:dataInicio', $event)"
        @update:data-fim="$emit('update:dataFim', $event)"
        @buscar="$emit('buscar')"
      />
    </div>
  </section>
</template>

<script setup>
import { Search, ChevronDown } from "lucide-vue-next";
import AdminInput from "../AdminInput.vue";
import OrderDateRangeFilter from "../../orders/OrderDateRangeFilter.vue";

defineProps({
  busca: { type: String, default: "" },
  status: { type: [String, Number], default: "" },
  dataInicio: { type: String, default: "" },
  dataFim: { type: String, default: "" },
});

defineEmits(["update:busca", "update:status", "update:dataInicio", "update:dataFim", "buscar"]);
</script>

<style scoped>
</style>
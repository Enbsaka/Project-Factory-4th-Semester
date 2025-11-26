<template>
  <header class="flex justify-between items-center mb-8">
    <div>
      <h1 class="text-2xl font-semibold text-gray-900">Gestão de Produtos</h1>
      <p class="text-gray-600">Gerencie seu catálogo de produtos</p>
    </div>

    <div class="flex items-center gap-3">
      <ViewStoreButton />
      <AdminButton variant="primary" size="md" @click="$emit('novo-produto')">
        <template #icon>
          <BadgePlus class="w-4 h-4" />
        </template>
        Novo Produto
      </AdminButton>
    </div>
  </header>
</template>

<script setup>
import ViewStoreButton from "../../buttons/ViewStoreButton.vue";
import AdminButton from "../AdminButton.vue";
import { BadgePlus } from "lucide-vue-next";
</script>
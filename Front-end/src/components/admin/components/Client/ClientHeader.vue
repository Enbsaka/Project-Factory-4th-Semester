<template>
  <header class="flex justify-between items-center mb-8">
    <div>
      <h1 class="text-2xl font-semibold text-gray-900">Clientes</h1>
      <p class="text-gray-600">Gerencie os clientes cadastrados na sua loja</p>
    </div>
    <div class="flex items-center gap-3">
      <ViewStoreButton />
      <AdminButton variant="primary" size="md" @click="$emit('novo')">
        <template #icon>
          <BadgePlus class="w-4 h-4" />
        </template>
        Novo Cliente
      </AdminButton>
    </div>
  </header>
</template>

<script setup>
import ViewStoreButton from "../../buttons/ViewStoreButton.vue";
import { BadgePlus } from "lucide-vue-next";
import AdminButton from "../AdminButton.vue";
</script>

<style scoped>
</style>
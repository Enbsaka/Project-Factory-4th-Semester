<template>
  <header class="flex justify-between items-center mb-8">
    <div>
      <h1 class="text-2xl font-semibold text-gray-900">Pedidos</h1>
      <p class="text-gray-600">Gerencie os pedidos realizados em sua loja</p>
    </div>
    <div class="flex items-center gap-3">
      <ViewStoreButton />
    </div>
  </header>
</template>

<script setup>
import ViewStoreButton from "../../buttons/ViewStoreButton.vue";
</script>

<style scoped>
</style>
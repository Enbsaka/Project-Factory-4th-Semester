<template>
  <header class="flex justify-between items-center mb-8">
    <div>
      <h1 class="text-2xl font-semibold text-gray-900">Categorias</h1>
      <p class="text-gray-600">Organize seus produtos em categorias</p>
    </div>
    <div class="flex items-center gap-3">
      <ViewStoreButton />
      <NewCategoryButton @criada="$emit('criada')" />
    </div>
  </header>
</template>

<script setup>
import ViewStoreButton from "../../buttons/ViewStoreButton.vue";
import NewCategoryButton from "../../buttons/NewCategoryButton.vue";
</script>
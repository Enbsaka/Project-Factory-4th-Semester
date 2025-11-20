<template>
  <router-link
    to="/"
    class="flex items-center gap-2 bg-white border border-gray-300 text-gray-700 px-4 py-2 rounded-md text-sm hover:bg-gray-100 transition"
    aria-label="Ver Loja"
  >
    <Eye />
    <span>Ver Loja</span>
  </router-link>
</template>

<script setup>
import { Eye } from 'lucide-vue-next'
</script>
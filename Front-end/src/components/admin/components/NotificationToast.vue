<template>
  <div
    v-if="notificacao.aberta"
    class="fixed bottom-5 right-5 px-4 py-2 rounded-md shadow-md z-[1000]"
    :class="cls"
  >
    {{ notificacao.mensagem }}
  </div>
</template>

<script setup>
import { computed } from "vue";
import { useNotifications } from "../composables/useNotifications.js";

const { notificacao } = useNotifications();
const cls = computed(() => {
  const tipo = notificacao.value.tipo || "success";
  if (tipo === "error") return "bg-red-600 text-white";
  if (tipo === "warning") return "bg-yellow-500 text-white";
  return "bg-green-600 text-white";
});
</script>

<style scoped>
</style>
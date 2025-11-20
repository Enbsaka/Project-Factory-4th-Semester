<template>
  <component :is="layout">
    <router-view />
  </component>
</template>

<script setup>
import { computed } from "vue";
import { useRoute } from "vue-router";
import ClientLayout from "../src/components/layouts/ClientLayout.vue";
import AdminLayout from "../src/components/layouts/AdminLayout.vue";
import CheckoutLayout from "./components/layouts/CheckoutLayout.vue";
import FooterStoreComponent from "./components/public/FooterStoreComponent.vue";

const route = useRoute();

const layout = computed(() => {
  if (route.meta.layout === "admin") return AdminLayout;
  if (route.meta.layout === "checkout") return CheckoutLayout
  if (route.meta.layout === "none") return "div";
  return ClientLayout
});
</script>

<style>
/* Tema escuro global (profissional, acessível) */
:root[data-theme="dark"] body { background-color: #0b1220; color: #e5e7eb; }
:root[data-theme="dark"] header { background-color: #0f172a !important; border-color: #1f2937 !important; }
:root[data-theme="dark"] header a, :root[data-theme="dark"] header button { color: #e5e7eb; }

:root[data-theme="dark"] .client-main { background-color: #0b1220; color: #e5e7eb; }
:root[data-theme="dark"] footer { background-color: #0b1220; color: #e5e7eb; }
:root[data-theme="dark"] footer .bg-white { background-color: #111827 !important; color: #e5e7eb !important; }

/* Inputs e elementos de formulário */
:root[data-theme="dark"] input,
:root[data-theme="dark"] textarea,
:root[data-theme="dark"] select {
  background-color: #111827 !important;
  color: #e5e7eb !important;
  border-color: #374151 !important;
}

/* Bordas e sombras para melhor leitura no dark */
:root[data-theme="dark"] .border { border-color: #374151 !important; }
:root[data-theme="dark"] .shadow-lg {
  box-shadow: 0 10px 15px -3px rgba(0,0,0,0.6), 0 4px 6px -4px rgba(0,0,0,0.6) !important;
}

/* Links mais legíveis no dark */
:root[data-theme="dark"] a { color: #93c5fd; }

/* Overrides comuns do Tailwind para cobrir mais elementos em dark */
:root[data-theme="dark"] .bg-white { background-color: #111827 !important; }
:root[data-theme="dark"] .bg-gray-50 { background-color: #0b1220 !important; }
:root[data-theme="dark"] .bg-gray-100 { background-color: #0f172a !important; }
:root[data-theme="dark"] .text-gray-900 { color: #f3f4f6 !important; }
:root[data-theme="dark"] .text-gray-800 { color: #e5e7eb !important; }
:root[data-theme="dark"] .text-gray-700 { color: #e5e7eb !important; }
:root[data-theme="dark"] .text-gray-600 { color: #cbd5e1 !important; }
:root[data-theme="dark"] .border-gray-200 { border-color: #1f2937 !important; }
:root[data-theme="dark"] .border-gray-300 { border-color: #374151 !important; }
:root[data-theme="dark"] .hover\:bg-gray-100:hover { background-color: #1f2937 !important; }
:root[data-theme="dark"] .hover\:bg-gray-200:hover { background-color: #374151 !important; }
</style>
<template>
  <aside class="w-64 h-screen bg-white shadow-md flex flex-col justify-between">
    <!-- Topo -->
    <div>
      <!-- Logo -->
      <div class="flex items-center justify-center h-20 border-b">
        <img src="../../../assets/images/logo-dunderstore.png" alt="Logo Dunder Store" class="w-24" />
      </div>

      <!-- Menu principal -->
      <nav class="mt-6 px-4 space-y-2">
        <RouterLink
          v-for="item in menuItems"
          :key="item.name"
          :to="item.route"
          class="flex items-center w-full text-left px-4 py-2 rounded-lg transition-colors duration-200"
          :class="[
            isActive(item.route)
              ? 'bg-[#141A7C] text-white'
              : 'text-gray-600 hover:bg-gray-100'
          ]"
        >
          <component :is="item.icon" class="w-5 h-5 mr-3" />
          <span class="font-medium text-sm">{{ item.label }}</span>
        </RouterLink>
      </nav>
    </div>

    <!-- Parte inferior -->
    <div class="border-t px-4 py-4 space-y-2">
      <RouterLink
        v-for="item in bottomItems"
        :key="item.name"
        :to="item.route"
        class="flex items-center w-full text-left px-4 py-2 rounded-lg transition-colors duration-200"
        :class="[
          isActive(item.route)
            ? 'bg-[#141A7C] text-white'
            : 'text-gray-600 hover:bg-gray-100'
        ]"
      >
        <component :is="item.icon" class="w-5 h-5 mr-3" />
        <span class="font-medium text-sm">{{ item.label }}</span>
      </RouterLink>
    </div>
  </aside>
</template>

<script setup>
import { useRoute } from "vue-router";
import {
  Home,
  Package,
  ShoppingBag,
  Users,
  Ticket,
  User,
  Settings,
  ClipboardList
} from "lucide-vue-next";

const route = useRoute();

// Função para verificar se a rota está ativa
const isActive = (path) => route.path === path;

const menuItems = [
  { name: "Dashboard", label: "Dashboard", icon: Home, route: "/admin/dashboard" },
  { name: "Produtos", label: "Produtos", icon: Package, route: "/admin/produtos" },
  {name: "Categorias", label: "Categorias", icon: ClipboardList, route: "/admin/categorias"},
  { name: "Vendas", label: "Pedidos", icon: ShoppingBag, route: "/admin/pedidos" },
  { name: "Clientes", label: "Clientes", icon: Users, route: "/admin/clientes" },
  { name: "Cupons", label: "Cupons", icon: Ticket, route: "/admin/cupons" },
];

// Itens inferiores removidos (Perfil e Configurações)
const bottomItems = [];
</script>

<style scoped>
aside {
  font-family: "Inter", sans-serif;
}
</style>

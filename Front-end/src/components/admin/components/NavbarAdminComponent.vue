<!-- <template>
    <div class="fixed left-0 top-0 h-screen w-64 bg-white shadow-lg p-4">
        <div class="mb-8">
            <h2 class="text-2xl font-bold text-gray-600">E-commerce Admin</h2>
        </div>
        <nav class="space-y-2">
            <a href="" class="sidebar-ink flex items-center space-x-3 p-3 rounded-lg hover:bg-blue-50 text-blue-600">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                    stroke="currentColor" class="size-6">
                    <path stroke-linecap="round" stroke-linejoin="round"
                        d="M15.666 3.888A2.25 2.25 0 0 0 13.5 2.25h-3c-1.03 0-1.9.693-2.166 1.638m7.332 0c.055.194.084.4.084.612v0a.75.75 0 0 1-.75.75H9a.75.75 0 0 1-.75-.75v0c0-.212.03-.418.084-.612m7.332 0c.646.049 1.288.11 1.927.184 1.1.128 1.907 1.077 1.907 2.185V19.5a2.25 2.25 0 0 1-2.25 2.25H6.75A2.25 2.25 0 0 1 4.5 19.5V6.257c0-1.108.806-2.057 1.907-2.185a48.208 48.208 0 0 1 1.927-.184" />
                </svg>

                <span>Pedidos</span>
            </a>
            <a href="" class="sidebar-ink flex items-center space-x-3 p-3 rounded-lg hover:bg-blue-50 text-gray-600">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                    stroke="currentColor" class="size-6">
                    <path stroke-linecap="round" stroke-linejoin="round"
                        d="M15.75 10.5V6a3.75 3.75 0 1 0-7.5 0v4.5m11.356-1.993 1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 0 1-1.12-1.243l1.264-12A1.125 1.125 0 0 1 5.513 7.5h12.974c.576 0 1.059.435 1.119 1.007ZM8.625 10.5a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm7.5 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Z" />
                </svg>

                <span>Produto</span>
            </a>
            <a href="" class="sidebar-ink flex items-center space-x-3 p-3 rounded-lg hover:bg-blue-50 text-gray-600">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                    stroke="currentColor" class="size-6">
                    <path stroke-linecap="round" stroke-linejoin="round"
                        d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z" />
                </svg>

                <span>Clientes</span>
            </a>
            <a href="" class="sidebar-ink flex items-center space-x-3 p-3 rounded-lg hover:bg-blue-50 text-gray-600">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                    stroke="currentColor" class="size-6">
                    <path stroke-linecap="round" stroke-linejoin="round"
                        d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m0 12.75h7.5m-7.5 3H12M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" />
                </svg>

                <span>Relatórios</span>
            </a>
            <a href="" class="sidebar-ink flex items-center space-x-3 p-3 rounded-lg hover:bg-blue-50 text-gray-600">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                    stroke="currentColor" class="size-6">
                    <path stroke-linecap="round" stroke-linejoin="round"
                        d="M9.594 3.94c.09-.542.56-.94 1.11-.94h2.593c.55 0 1.02.398 1.11.94l.213 1.281c.063.374.313.686.645.87.074.04.147.083.22.127.325.196.72.257 1.075.124l1.217-.456a1.125 1.125 0 0 1 1.37.49l1.296 2.247a1.125 1.125 0 0 1-.26 1.431l-1.003.827c-.293.241-.438.613-.43.992a7.723 7.723 0 0 1 0 .255c-.008.378.137.75.43.991l1.004.827c.424.35.534.955.26 1.43l-1.298 2.247a1.125 1.125 0 0 1-1.369.491l-1.217-.456c-.355-.133-.75-.072-1.076.124a6.47 6.47 0 0 1-.22.128c-.331.183-.581.495-.644.869l-.213 1.281c-.09.543-.56.94-1.11.94h-2.594c-.55 0-1.019-.398-1.11-.94l-.213-1.281c-.062-.374-.312-.686-.644-.87a6.52 6.52 0 0 1-.22-.127c-.325-.196-.72-.257-1.076-.124l-1.217.456a1.125 1.125 0 0 1-1.369-.49l-1.297-2.247a1.125 1.125 0 0 1 .26-1.431l1.004-.827c.292-.24.437-.613.43-.991a6.932 6.932 0 0 1 0-.255c.007-.38-.138-.751-.43-.992l-1.004-.827a1.125 1.125 0 0 1-.26-1.43l1.297-2.247a1.125 1.125 0 0 1 1.37-.491l1.216.456c.356.133.751.072 1.076-.124.072-.044.146-.086.22-.128.332-.183.582-.495.644-.869l.214-1.28Z" />
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                </svg>

                <span>Ferramentas</span>
            </a>
        </nav>
    </div>

   
</template> -->

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

<template>
  <div class="client-layout">
    <!-- Navbar -->
    <NavbarCheckout/>

    <!-- Conteúdo principal -->
    <main class="client-main">
      <slot />
    </main>

    <!-- Rodapé -->
    <FooterComponent />
  </div>
</template>

<script setup>
import NavbarCheckout from '../public/NavbarCheckout.vue';
</script>

<style scoped>
.client-layout {
  display: flex;
  flex-direction: column;
 
}

.client-main {

  background-color: #fafafa;

}
</style>

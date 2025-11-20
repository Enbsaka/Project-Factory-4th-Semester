<template>
  <div class="max-w-2xl mx-auto text-center py-16 px-6">
    <div class="mx-auto w-20 h-20 rounded-full bg-green-100 flex items-center justify-center mb-6">
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-10 h-10 text-green-600">
        <path fill-rule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12zm13.03-1.72a.75.75 0 10-1.06-1.06L11 12.94l-1.72-1.72a.75.75 0 10-1.06 1.06l2.25 2.25a.75.75 0 001.06 0l4.25-4.25z" clip-rule="evenodd" />
      </svg>
    </div>
    <h1 class="text-2xl font-semibold text-[#0B1739]">Obrigado pela sua compra!</h1>
    <p class="mt-2 text-gray-600">Seu pedido foi finalizado com sucesso. Em breve enviaremos atualizações por e‑mail.</p>

    <div class="mt-8 flex justify-center gap-4">
      <router-link to="/" class="px-5 py-2 bg-[#0B1739] text-white rounded-md hover:opacity-90">Voltar à loja</router-link>
      <router-link to="/carrinho" class="px-5 py-2 border border-[#0B1739] text-[#0B1739] rounded-md hover:bg-[#E5EBFF]">Ver carrinho</router-link>
    </div>
  </div>
</template>

<script setup>
// Tela simples de confirmação
</script>
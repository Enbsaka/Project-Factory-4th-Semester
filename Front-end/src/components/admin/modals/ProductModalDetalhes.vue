<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
    <div class="bg-white w-full max-w-3xl rounded-xl shadow-xl overflow-hidden">
      <div class="flex items-center justify-between px-6 py-4 border-b">
        <h2 class="text-xl font-semibold text-gray-800">Detalhes do Produto</h2>
        <button @click="$emit('fechar')" class="p-2 rounded hover:bg-gray-100" aria-label="Fechar">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" class="w-6 h-6" fill="none" stroke="currentColor" stroke-width="1.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <div class="p-6 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="md:col-span-1">
          <img :src="produto?.imagem || placeholder" alt="Imagem" class="w-full h-48 object-cover rounded-lg border" />
          <div class="mt-4 text-sm text-gray-500">
            <p><span class="font-medium">ID:</span> {{ produto?.id }}</p>
            <p><span class="font-medium">Código de Barras:</span> {{ produto?.codigoDeBarra || '—' }}</p>
            <p><span class="font-medium">Categoria:</span> {{ produto?.categoria || 'Sem categoria' }}</p>
          </div>
        </div>

        <div class="md:col-span-2 space-y-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-800">{{ produto?.nome }}</h3>
            <p class="text-sm text-gray-600 mt-1">{{ produto?.descricao || 'Sem descrição' }}</p>
          </div>

          <div class="flex items-center gap-4">
            <div class="text-base font-medium text-gray-800">Preço:</div>
            <div class="text-lg font-semibold text-indigo-600">{{ produto?.preco || 'R$ 0,00' }}</div>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <div class="text-sm text-gray-500">Tamanho</div>
              <div class="text-sm text-gray-800">{{ produto?.tamanho || '—' }}</div>
            </div>
            <div>
              <div class="text-sm text-gray-500">Cor</div>
              <div class="text-sm text-gray-800">{{ produto?.cor || '—' }}</div>
            </div>
          </div>

          <div>
            <div class="text-sm font-semibold text-gray-700 mb-2">Variações</div>
            <div v-if="produto?.variacoes?.length" class="space-y-2 max-h-44 overflow-auto pr-1">
              <div v-for="v in produto.variacoes" :key="v.id" class="flex items-center gap-3 p-2 border rounded-lg">
                <img :src="v.imagem" alt="Var" class="w-10 h-10 object-cover rounded" />
                <div class="flex-1">
                  <div class="text-sm font-medium text-gray-800">{{ v.nome }}</div>
                  <div class="text-xs text-gray-600">{{ v.tamanho || '—' }} | {{ v.cor || '—' }}</div>
                </div>
                <div class="text-sm font-semibold text-gray-700">{{ v.preco ? `R$ ${Number(v.preco).toFixed(2)}` : 'R$ 0,00' }}</div>
              </div>
            </div>
            <div v-else class="text-sm text-gray-500">Sem variações cadastradas.</div>
          </div>
        </div>
      </div>

      <div class="px-6 py-4 border-t flex justify-end">
        <button @click="$emit('fechar')" class="px-4 py-2 rounded-lg bg-gray-100 hover:bg-gray-200 text-gray-800">
          Fechar
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  produto: {
    type: Object,
    required: true,
  },
});

const placeholder = "https://via.placeholder.com/200x200?text=Produto";
</script>

<style scoped>
</style>
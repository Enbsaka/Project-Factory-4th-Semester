<template>
  <main class="max-w-7xl mx-auto flex flex-col lg:flex-row gap-12 py-10 px-6">
    <!-- Galeria de imagens -->
    <div class="flex flex-col items-center lg:w-1/2">
      <div class="border rounded-lg overflow-hidden bg-white">
        <img
          :src="selectedImage || placeholderImg"
          :alt="produto?.nome || 'Produto'"
          class="w-[450px] h-[450px] object-contain"
        />
      </div>

      <div class="flex gap-4 mt-4" v-if="galeria.length">
        <img
          v-for="(image, index) in galeria"
          :key="index"
          :src="image"
          @click="selectedImage = image"
          class="w-20 h-20 object-contain border rounded-lg cursor-pointer hover:border-indigo-500 bg-white"
          :class="selectedImage === image ? 'border-indigo-500' : 'border-gray-200'"
        />
      </div>
    </div>

    <!-- Detalhes do produto -->
    <div class="lg:w-1/2 flex flex-col gap-6">
      <div>
        <h1 class="text-2xl font-semibold text-gray-900">{{ produto?.nome || 'Carregando...' }}</h1>
        <div class="flex items-center gap-2 text-sm text-gray-500 mt-1">
          <span>{{ produto?.categoria?.nome }}</span>
        </div>
      </div>

      <div>
        <span class="text-3xl font-bold text-indigo-800">R$ {{ precoFormatado }}</span>
      </div>

      <div>
        <h2 class="font-semibold text-gray-800 mb-1">Descrição</h2>
        <p class="text-gray-600 leading-relaxed text-sm">
          {{ produto?.descricao }}
        </p>
      </div>

      <!-- Variações -->
      <div v-if="variacoes.length" class="space-y-4">
        <div>
          <h3 class="text-sm font-medium text-gray-700 mb-2">Cor</h3>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="v in coresDisponiveis"
              :key="v"
              @click="selecionarCor(v)"
              class="px-3 py-1 border rounded-md text-sm"
              :class="corSelecionada === v ? 'bg-indigo-600 text-white border-indigo-600' : 'bg-white text-gray-700'"
            >{{ v }}</button>
          </div>
        </div>
        <div>
          <h3 class="text-sm font-medium text-gray-700 mb-2">Tamanho</h3>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="t in tamanhosDisponiveis"
              :key="t"
              @click="selecionarTamanho(t)"
              class="px-3 py-1 border rounded-md text-sm"
              :class="tamanhoSelecionado === t ? 'bg-indigo-600 text-white border-indigo-600' : 'bg-white text-gray-700'"
            >{{ t }}</button>
          </div>
        </div>
      </div>

      <div class="flex items-center gap-4">
        <span class="text-sm text-gray-700 font-medium">Quantidade</span>
        <div class="flex items-center border rounded-md">
          <button @click="decreaseQuantity" class="px-3 py-1 text-gray-700 hover:bg-gray-100">−</button>
          <input type="number" v-model.number="quantity" class="w-12 text-center outline-none" min="1" />
          <button @click="increaseQuantity" class="px-3 py-1 text-gray-700 hover:bg-gray-100">+</button>
        </div>
        <button v-if="corSelecionada || tamanhoSelecionado" @click="limparSelecao" class="ml-2 px-3 py-2 border rounded-md text-sm text-[#0B1739] hover:bg-[#E5EBFF]">Limpar seleção</button>
      </div>

      <div class="flex flex-col sm:flex-row gap-4 mt-6">
        <AddtToCartButton
          :produto="produto"
          :quantidade="quantity"
          :cor-selecionada="corSelecionada"
          :tamanho-selecionado="tamanhoSelecionado"
          :variacao-quantidades="variacaoQuantidades"
          :disabled="botaoDesabilitado"
        />
        <ShopNowButton
          :produto="produto"
          :quantidade="quantity"
          :cor-selecionada="corSelecionada"
          :tamanho-selecionado="tamanhoSelecionado"
          :variacao-quantidades="variacaoQuantidades"
        />
      </div>

      <!-- Quantidade por variação -->
      <div v-if="variacoes.length" class="mt-4 space-y-3">
        <h3 class="text-sm font-medium text-gray-700">Quantidades por variação</h3>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div v-for="(v, idx) in variacoes" :key="v.id || v.Id || idx" class="flex items-center justify-between border rounded-md p-3">
            <div>
              <p class="text-sm text-[#0B1739] font-medium">{{ v.nome || v.Nome || produto?.nome || 'Produto' }}</p>
              <p class="text-xs text-gray-600">Cor: {{ v.cor || v.Cor || '-' }} | Tamanho: {{ v.tamanho || v.Tamanho || '-' }}</p>
            </div>
            <div class="flex items-center border rounded-md">
              <button @click="decreaseVarQty(idx)" class="px-2 py-1 text-gray-700 hover:bg-gray-100">−</button>
              <input type="number" v-model.number="variacaoQuantidades[idx].quantidade" class="w-12 text-center outline-none" min="0" />
              <button @click="increaseVarQty(idx)" class="px-2 py-1 text-gray-700 hover:bg-gray-100">+</button>
            </div>
          </div>
        </div>
        <p class="text-xs text-gray-500">Defina quantidades por cor/tamanho para adicionar múltiplas variações de uma vez.</p>
      </div>
    </div>
  </main>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useRoute } from "vue-router";
import AddtToCartButton from "../../components/public/AddtToCartButton.vue";
import ShopNowButton from "../../components/public/ShopNowButton.vue";
import productService from "../../services/productService.js";

const route = useRoute();
const produto = ref(null);
const variacoes = ref([]);
const selectedImage = ref(null);
const galeria = ref([]);
const quantity = ref(1);
const corSelecionada = ref(null);
const tamanhoSelecionado = ref(null);
const variacaoQuantidades = ref([]);

const placeholderImg = new URL("../../assets/images/imagem_sapateira.png", import.meta.url).href;

const precoFormatado = computed(() => {
  const p = Number((produto.value?.preco) ?? 0);
  return p.toLocaleString("pt-BR", { minimumFractionDigits: 2 });
});

const coresDisponiveis = computed(() => {
  const set = new Set((variacoes.value || []).map(v => v.cor).filter(Boolean));
  if (produto.value?.cor) set.add(produto.value.cor);
  return Array.from(set);
});

const tamanhosDisponiveis = computed(() => {
  const set = new Set((variacoes.value || []).map(v => v.tamanho).filter(Boolean));
  if (produto.value?.tamanho) set.add(produto.value.tamanho);
  return Array.from(set);
});

function selecionarCor(cor) {
  // Permite desmarcar clicando na mesma opção
  if (corSelecionada.value === cor) {
    corSelecionada.value = null;
    sincronizarVariacaoSelecionada(true);
    return;
  }
  corSelecionada.value = cor;
  sincronizarVariacaoSelecionada();
}

function selecionarTamanho(tam) {
  if (tamanhoSelecionado.value === tam) {
    tamanhoSelecionado.value = null;
    sincronizarVariacaoSelecionada(true);
    return;
  }
  tamanhoSelecionado.value = tam;
  sincronizarVariacaoSelecionada();
}

function sincronizarVariacaoSelecionada(reset = false) {
  if (reset || (!corSelecionada.value && !tamanhoSelecionado.value)) { carregarProduto(); return; }
  const match = (variacoes.value || []).find(v =>
    (!corSelecionada.value || (v.cor || v.Cor) === corSelecionada.value) &&
    (!tamanhoSelecionado.value || (v.tamanho || v.Tamanho) === tamanhoSelecionado.value)
  );
  if (match) {
    produto.value = match;
    selectedImage.value = match.imagemUrl || match.imagemURL || selectedImage.value;
  }
}

function limparSelecao() {
  corSelecionada.value = null;
  tamanhoSelecionado.value = null;
  // Recarrega produto original (pai)
  carregarProduto();
}

const increaseQuantity = () => { quantity.value++; };
const decreaseQuantity = () => { if (quantity.value > 1) quantity.value--; };

async function carregarProduto() {
  const id = route.params.id;
  const data = await productService.getById(id);
  produto.value = data;
  selectedImage.value = data.imagemUrl || data.imagemURL || null;

  // Se for pai, variacoes vêm junto; se for filho, buscar o pai para obter todas
  if (Array.isArray(data.variacoes) && data.variacoes.length) {
    variacoes.value = data.variacoes;
  } else if (data.produtoPaiId) {
    try {
      const pai = await productService.getById(data.produtoPaiId);
      variacoes.value = pai.variacoes || [];
    } catch { /* silencioso */ }
  }

  // Monta galeria simples (imagem principal + imagens das variações, se houver)
  const imagens = [];
  const principal = data.imagemUrl || data.imagemURL;
  if (principal) imagens.push(principal);
  (variacoes.value || []).forEach(v => {
    const img = v.imagemUrl || v.imagemURL;
    if (img) imagens.push(img);
  });
  galeria.value = Array.from(new Set(imagens));

  // Inicializa matriz de quantidades por variação
  variacaoQuantidades.value = (variacoes.value || []).map(v => ({
    produtoId: v.id || v.Id,
    codigoDeBarra: v.codigoDeBarra || v.CodigoDeBarra,
    cor: v.cor || v.Cor,
    tamanho: v.tamanho || v.Tamanho,
    quantidade: 0
  }));
}

onMounted(carregarProduto);
watch(() => route.params.id, carregarProduto);

// Desabilita botões se variações existirem e não estiverem selecionadas
const botaoDesabilitado = computed(() => {
  const exigeCor = coresDisponiveis.value.length > 0;
  const exigeTam = tamanhosDisponiveis.value.length > 0;
  const algumaVarQtd = (variacaoQuantidades.value || []).some(v => (v.quantidade || 0) > 0);
  return ((exigeCor && !corSelecionada.value) || (exigeTam && !tamanhoSelecionado.value)) && !algumaVarQtd;
});

function increaseVarQty(idx) {
  const item = variacaoQuantidades.value[idx];
  if (!item) return;
  item.quantidade = Number(item.quantidade || 0) + 1;
}
function decreaseVarQty(idx) {
  const item = variacaoQuantidades.value[idx];
  if (!item) return;
  const q = Number(item.quantidade || 0);
  item.quantidade = q > 0 ? q - 1 : 0;
}
</script>

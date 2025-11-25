<template>
  <main class="max-w-7xl mx-auto flex flex-col lg:flex-row gap-12 py-10 px-6">
    <div class="flex flex-col items-center lg:w-1/2">
      <div class="border rounded-lg overflow-hidden bg-white">
        <img
          :src="selectedImage || placeholderImg"
          :alt="produto?.nome || 'Produto'"
          class="w-[450px] h-[450px] object-contain cursor-zoom-in"
          @click="abrirLightbox()"
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
        <p class="text-gray-600 leading-relaxed text-sm" style="white-space: pre-line;">
          {{ descricaoExibida }}
        </p>
        <button
          v-if="temDescricaoLonga"
          @click="mostrarDescricaoCompleta = !mostrarDescricaoCompleta"
          class="mt-2 text-sm text-[#141A7C] hover:underline"
        >{{ mostrarDescricaoCompleta ? 'Ver menos' : 'Ver mais' }}</button>
      </div>

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

      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 mt-6" v-if="produto">
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
        <RouterLink
          to="/app/produtos"
          class="w-full h-11 flex items-center justify-center gap-2 bg-white border border-[#141A7C] text-[#141A7C] text-sm font-medium rounded-md hover:bg-[#E5EBFF] transition"
        >Continuar Comprando</RouterLink>
      </div>

      <div v-if="variacoes.length" class="mt-4 space-y-3">
        <h3 class="text-sm font-medium text-gray-700">Quantidades por variação</h3>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div v-for="(v, idx) in variacoes" :key="v.id || v.Id || idx" class="flex items-center justify-between border rounded-md p-3">
            <div>
              <p class="text-sm text-[#0B1739] font-medium">{{ nomeVariacao(v) }}</p>
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
  <div v-if="lightboxAberto" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center" @click.self="fecharLightbox">
    <button @click="fecharLightbox" class="absolute top-4 right-4 px-3 py-2 rounded-md bg-white/10 text-white hover:bg-white/20">Fechar</button>
    <Swiper :modules="[Zoom, Navigation]" :zoom="true" :navigation="true" :initial-slide="lightboxIndex" class="w-full max-w-5xl h-[80vh]">
      <SwiperSlide v-for="(img, idx) in galeria" :key="idx">
        <div class="swiper-zoom-container w-full h-full flex items-center justify-center">
          <img :src="img" class="max-h-full max-w-full object-contain" />
        </div>
      </SwiperSlide>
    </Swiper>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { Swiper, SwiperSlide } from "swiper/vue";
import { Zoom, Navigation } from "swiper/modules";
import "swiper/css";
import "swiper/css/zoom";
import "swiper/css/navigation";
import { useRoute } from "vue-router";
import AddToCartButton from "../../components/public/AddToCartButton.vue";
const AddtToCartButton = AddToCartButton;
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
const mostrarDescricaoCompleta = ref(false);
const descricaoCompleta = computed(() => String(produto.value?.descricao || ''));
const temDescricaoLonga = computed(() => descricaoCompleta.value.length > 600);
const descricaoExibida = computed(() => {
  if (mostrarDescricaoCompleta.value) return descricaoCompleta.value;
  const max = 600;
  const base = descricaoCompleta.value.slice(0, max);
  const ajustado = base.replace(/\s+\S*$/, '');
  return temDescricaoLonga.value ? ajustado + '…' : ajustado;
});
const lightboxAberto = ref(false);
const lightboxIndex = ref(0);

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

function nomeVariacao(v) {
  const base = produto.value?.nome || produto.value?.Nome || 'Produto';
  const cor = v?.cor || v?.Cor || '';
  const tam = v?.tamanho || v?.Tamanho || '';
  const parts = [];
  if (cor) parts.push(cor);
  if (tam) parts.push(tam);
  return parts.length ? `${base} - ${parts.join(' / ')}` : base;
}

function limparSelecao() {
  corSelecionada.value = null;
  tamanhoSelecionado.value = null;
  carregarProduto();
}

function abrirLightbox() {
  lightboxAberto.value = true;
  const idx = galeria.value.findIndex((g) => g === selectedImage.value);
  lightboxIndex.value = idx >= 0 ? idx : 0;
}

function fecharLightbox() {
  lightboxAberto.value = false;
}

const increaseQuantity = () => { quantity.value++; };
const decreaseQuantity = () => { if (quantity.value > 1) quantity.value--; };

async function carregarProduto() {
  const id = route.params.id;
  const data = await productService.getById(id);
  produto.value = data;
  selectedImage.value = data.imagemUrl || data.imagemURL || null;

  if (Array.isArray(data.variacoes) && data.variacoes.length) {
    variacoes.value = data.variacoes;
  } else if (data.produtoPaiId) {
    try {
      const pai = await productService.getById(data.produtoPaiId);
      variacoes.value = pai.variacoes || [];
    } catch { /* silencioso */ }
  }

  const imagens = [];
  const principal = data.imagemUrl || data.imagemURL;
  if (principal) imagens.push(principal);
  (variacoes.value || []).forEach(v => {
    const img = v.imagemUrl || v.imagemURL;
    if (img) imagens.push(img);
  });
  galeria.value = Array.from(new Set(imagens));

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

const botaoDesabilitado = computed(() => {
  const haVariacoes = (variacoes.value || []).length > 0;
  if (!haVariacoes) return false;
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

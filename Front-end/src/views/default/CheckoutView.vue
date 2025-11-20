<template>
  <!-- Passos do Checkout -->
  <div class="flex justify-center items-center gap-8 py-6">
    <div class="flex items-center gap-2">
      <div class="w-6 h-6 rounded-full bg-[#0B1739] text-white flex items-center justify-center text-xs font-semibold">1</div>
      <span class="font-medium text-[#0B1739]">Carrinho</span>
    </div>
    <div class="w-16 h-[2px] bg-gray-300"></div>
    <div class="flex items-center gap-2">
      <div class="w-6 h-6 rounded-full border-2 border-[#0B1739] text-[#0B1739] flex items-center justify-center text-xs font-semibold bg-[#E5EBFF]">2</div>
      <span class="font-medium text-[#0B1739]">Checkout</span>
    </div>
    <div class="w-16 h-[2px] bg-gray-300"></div>
    <div class="flex items-center gap-2 opacity-50">
      <div class="w-6 h-6 rounded-full border border-gray-300 flex items-center justify-center text-xs font-semibold">3</div>
      <span class="font-medium text-gray-500">Confirmação</span>
    </div>
  </div>

  <div class="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-6 px-6 pb-16">
    <!-- Formulários -->
    <div class="lg:col-span-2 space-y-6">
      <!-- Endereço de Entrega -->
      <section class="border rounded-xl p-6 bg-white shadow-sm">
        <h2 class="flex items-center gap-2 text-lg font-semibold text-[#0B1739] mb-4">Endereço de Entrega</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div><label class="block text-sm text-gray-600 mb-1">CEP *</label>
            <input v-model="endereco.cep" type="text" placeholder="00000-000" class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
          <div><label class="block text-sm text-gray-600 mb-1">Estado *</label>
            <input v-model="endereco.estado" type="text" placeholder="SP" class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm text-gray-600 mb-1">Endereço *</label>
            <input v-model="endereco.logradouro" type="text" placeholder="Rua, Avenida, etc." class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
          <div><label class="block text-sm text-gray-600 mb-1">Número *</label>
            <input v-model="endereco.numero" type="text" placeholder="123" class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
          <div><label class="block text-sm text-gray-600 mb-1">Complemento</label>
            <input v-model="endereco.complemento" type="text" placeholder="Apto, Bloco, etc." class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
          <div><label class="block text-sm text-gray-600 mb-1">Bairro *</label>
            <input v-model="endereco.bairro" type="text" placeholder="Centro" class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
          <div><label class="block text-sm text-gray-600 mb-1">Cidade *</label>
            <input v-model="endereco.cidade" type="text" placeholder="São Paulo" class="w-full border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
          </div>
        </div>
      </section>
    </div>

    <!-- Resumo do pedido -->
    <div>
      <aside class="border rounded-xl p-6 bg-white shadow-sm">
        <h2 class="text-lg font-semibold text-[#0B1739] mb-4">Resumo do Pedido</h2>

        <div class="space-y-4">
          <div v-for="item in carrinho?.Produtos || []" :key="item.ProdutoId" class="flex items-center gap-4">
            <img :src="item.ImagemURL || 'https://via.placeholder.com/60'" alt="" class="w-16 h-16 rounded-md object-cover" />
            <div>
              <p class="text-sm font-medium text-[#0B1739]">{{ item.Nome }}</p>
              <p class="text-sm text-gray-500">Qtd: {{ item.Quantidade }}</p>
              <p class="text-[#0B1739] font-semibold">R$ {{ (item.Preco * item.Quantidade).toFixed(2) }}</p>
            </div>
          </div>
        </div>

        <hr class="my-4" />

        <div class="space-y-1 text-sm">
          <div class="flex justify-between">
            <span>Subtotal</span>
            <span>R$ {{ subtotal.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between" v-if="desconto > 0">
            <span>Desconto</span>
            <span>- R$ {{ desconto.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between">
            <span>Frete</span>
            <span>R$ {{ freteExibir.toFixed(2) }}</span>
          </div>
        </div>

        <hr class="my-4" />

        <div class="flex justify-between font-semibold text-[#0B1739] text-lg">
          <span>Total</span>
          <span>R$ {{ total.toFixed(2) }}</span>
        </div>

        <div class="mt-4">
          <label class="block text-sm text-gray-600 mb-1">Cupom de desconto</label>
          <div class="flex gap-2">
            <input v-model="cupomCodigo" type="text" placeholder="EX: PROMO10" class="flex-1 border rounded-md px-3 py-2 focus:ring-[#0B1739] focus:border-[#0B1739]" />
            <button @click="aplicarCupom" class="px-4 py-2 bg-[#0B1739] text-white rounded-md hover:opacity-90">Aplicar</button>
          </div>
          <p v-if="cupomAplicadoInfo" class="text-xs text-gray-500 mt-1">Cupom aplicado: {{ cupomAplicadoInfo }}</p>
        </div>

        <button @click="finalizarPedido" :disabled="botaoFinalizarDesabilitado" class="mt-6 w-full bg-[#0B1739] text-white font-medium py-3 rounded-md hover:opacity-90 transition disabled:opacity-50 disabled:cursor-not-allowed">
          Finalizar Pedido
        </button>

        <div class="mt-4 flex gap-3">
          <router-link to="/" class="flex-1 text-center px-4 py-2 border rounded-md text-[#0B1739] hover:bg-[#E5EBFF]">Voltar à loja</router-link>
          <router-link to="/carrinho" class="flex-1 text-center px-4 py-2 border rounded-md text-[#0B1739] hover:bg-[#E5EBFF]">Ir para o carrinho</router-link>
        </div>

        <p class="text-xs text-center text-gray-500 mt-2">
          Pagamento 100% seguro e protegido
        </p>
      </aside>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { carrinhoService } from "../../services/carrinhoService.js";
import clienteService from "../../services/clienteService.js";
import productService from "../../services/productService.js";

const route = useRoute();
const router = useRouter();
let pedidoId = route.params.id;

const carrinho = ref(null);
const endereco = ref({
  cep: "", estado: "", logradouro: "", numero: "", complemento: "", bairro: "", cidade: ""
});
const frete = ref(0);
const cupomCodigo = ref("");
const cupomAplicadoInfo = ref("");

const carregarCarrinho = async () => {
  try {
    // Obter perfil do cliente uma vez e reaproveitar
    const perfil = await clienteService.getMeuPerfil();

    // Se não houver ID na rota, buscar o carrinho atual do cliente logado
    if (!pedidoId) {
      const clienteIdTmp = perfil?.id ?? perfil?.Id;
      const carrinhoAtual = await carrinhoService.getCarrinho(clienteIdTmp);
      pedidoId = carrinhoAtual?.id ?? carrinhoAtual?.Id;
    }

    // Preencher endereço a partir do perfil
    if (perfil) {
      endereco.value.cep = perfil?.cep ?? perfil?.Cep ?? '';
      endereco.value.estado = perfil?.Uf ?? perfil?.uf ?? perfil?.Estado ?? perfil?.estado ?? '';
      endereco.value.logradouro = perfil?.Logradouro ?? perfil?.logradouro ?? perfil?.Endereco ?? perfil?.endereco ?? '';
      // Inclui fallback para camelCase e evita sobrescrever com undefined
      const numEnd = perfil?.NumEndereco ?? perfil?.numEndereco ?? '';
      endereco.value.numero = String(numEnd || '');
      endereco.value.bairro = perfil?.Bairro ?? perfil?.bairro ?? '';
      endereco.value.cidade = perfil?.Localidade ?? perfil?.cidade ?? perfil?.Cidade ?? '';
    }

    // Buscar carrinho do cliente (evita 403 do endpoint admin-only)
    const clienteId = perfil?.id ?? perfil?.Id;
    const data = await carrinhoService.getCarrinho(clienteId);
    if (data) {
      carrinho.value = {
        ...data,
        Produtos: await Promise.all((data.Produtos || data.produtos || []).map(async p => {
          const base = {
            ProdutoId: p.ProdutoId ?? p.produtoId ?? p.id,
            CodigoDeBarra: p.CodigoDeBarra ?? p.codigoDeBarra ?? null,
            Nome: p.Nome ?? p.Produto?.Nome ?? p.nome ?? "",
            Preco: p.Preco ?? p.Produto?.Preco ?? p.preco ?? 0,
            Quantidade: p.Quantidade ?? p.quantidade ?? 1,
            ImagemURL: p.ImagemURL ?? p.imagemURL ?? p?.Produto?.ImagemURL ?? ''
          };
          if (!base.ImagemURL && base.ProdutoId) {
            try {
              const detalhe = await productService.getById(base.ProdutoId);
              base.ImagemURL = detalhe?.imagemUrl || detalhe?.ImagemURL || '';
              base.CodigoDeBarra = base.CodigoDeBarra || detalhe?.CodigoDeBarra || detalhe?.codigoDeBarra || null;
            } catch {}
          }
          return base;
        })),
        Frete: data.Frete ?? data.frete ?? 0,
        ValorTotalSemDesconto: data.ValorTotalSemDesconto ?? data.valorTotalSemDesconto ?? (Array.isArray(data.Produtos || data.produtos) ? (data.Produtos || data.produtos).reduce((acc, p) => acc + Number(p.Preco ?? p.preco ?? 0) * Number(p.Quantidade ?? p.quantidade ?? 0), 0) : 0),
        ValorTotalComDesconto: data.ValorTotalComDesconto ?? data.valorTotal ?? undefined,
        Cupom: data.Cupom ?? data.cupom ?? undefined
      };
    }
    await calcularFreteApi();
  } catch (err) {
    console.error(err);
  }
};

const subtotal = computed(() => {
  const backend = carrinho.value?.ValorTotalSemDesconto;
  if (typeof backend === 'number') return backend;
  const itens = carrinho.value?.Produtos || [];
  return itens.reduce((acc, p) => acc + Number(p.Preco || 0) * Number(p.Quantidade || 0), 0);
});

const desconto = computed(() => {
  const perc = carrinho.value?.Cupom?.DescontoPercentual ?? carrinho.value?.cupom?.descontoPercentual;
  if (typeof perc === 'number' && perc > 0) {
    return subtotal.value * (perc / 100);
  }
  const back = carrinho.value?.ValorTotalComDesconto;
  if (typeof back === 'number') {
    // evita incluir frete como "desconto" se vier do backend
    const freteAtual = (carrinho.value?.Frete ?? 0) || frete.value || 0;
    return Math.max(0, subtotal.value - (back - freteAtual));
  }
  return 0;
});

const freteExibir = computed(() => (carrinho.value?.Frete ?? 0) || frete.value || 0);
const total = computed(() => subtotal.value - desconto.value + freteExibir.value);

const enderecoValido = computed(() => {
  const e = endereco.value;
  return !!(e.cep && e.estado && e.logradouro && e.numero && e.bairro && e.cidade);
});

async function calcularFreteApi() {
  try {
    const cep = (endereco.value.cep || '').replace(/\D/g, '');
    const itens = (carrinho.value?.Produtos?.reduce((acc, p) => acc + (p.Quantidade||0), 0)) || 0;
    if (!cep || cep.length !== 8 || itens === 0) { frete.value = 0; return; }
    const api = (await import('../../services/api')).default;
    const { data } = await api.get(`/ViaCep/frete/${cep}`);
    const preco = data?.preco ?? 0;
    frete.value = Number(preco) || 0;
    // Persistir frete no pedido para compor total no backend
    try {
      const perfil = await clienteService.getMeuPerfil();
      const clienteId = perfil?.id ?? perfil?.Id;
      const carrinhoAtual = await carrinhoService.getCarrinho(clienteId);
      const id = carrinhoAtual?.id ?? carrinhoAtual?.Id ?? pedidoId;
      const produtosPayload = (carrinho.value?.Produtos || []).map(p => ({
        CodigoDeBarra: p.CodigoDeBarra || '',
        Quantidade: p.Quantidade || 1
      })).filter(x => x.CodigoDeBarra);
      const cpf = perfil?.cpf ?? perfil?.Cpf ?? '';
      await carrinhoService.atualizarCarrinho(id, produtosPayload, cpf, frete.value);
      carrinho.value.Frete = frete.value;
    } catch (persistErr) {
      console.warn('Falha ao persistir frete no pedido.', persistErr);
    }
  } catch (e) {
    console.warn('Falha ao obter frete via API, fallback para 0.', e);
    frete.value = 0;
  }
}

async function preencherEnderecoPorCep() {
  try {
    const cep = (endereco.value.cep || '').replace(/\D/g, '');
    if (!cep || cep.length !== 8) return;
    const api = (await import('../../services/api')).default;
    const { data } = await api.get(`/ViaCep/${cep}`);
    endereco.value.estado = data?.uf || endereco.value.estado;
    endereco.value.logradouro = data?.logradouro || endereco.value.logradouro;
    endereco.value.bairro = data?.bairro || endereco.value.bairro;
    endereco.value.cidade = data?.localidade || endereco.value.cidade;
  } catch (e) {
    console.warn('Falha ao preencher endereço via CEP:', e);
  }
}

watch(endereco, calcularFreteApi, { deep: true });
watch(() => endereco.value.cep, preencherEnderecoPorCep);

const finalizarPedido = async () => {
  try {
    if ((carrinho.value?.Produtos?.length ?? 0) === 0) {
      alert('Seu carrinho está vazio. Adicione itens antes de finalizar.');
      return;
    }
    if (!enderecoValido.value) {
      alert('Preencha todos os dados de entrega obrigatórios (CEP, Estado, Endereço, Número, Bairro, Cidade).');
      return;
    }
    await carrinhoService.finalizarPedido(pedidoId);
    router.push(`/carrinho/checkout/confirmacao`);
  } catch (err) {
    console.error(err);
  }
};

async function aplicarCupom() {
  try {
    if (!cupomCodigo.value) return;
    await carrinhoService.aplicarCupom(pedidoId, cupomCodigo.value);
    await carregarCarrinho();
    cupomAplicadoInfo.value = carrinho.value?.Cupom?.Codigo ? `${carrinho.value.Cupom.Codigo} (${carrinho.value.Cupom.DescontoPercentual}% off)` : '';
  } catch (err) {
    console.error('Falha ao aplicar cupom:', err);
    const msg = err?.response?.data || err?.message || 'Cupom inválido ou expirado.';
    alert(String(msg));
  }
}

const botaoFinalizarDesabilitado = computed(() => {
  return ((carrinho.value?.Produtos?.length ?? 0) === 0) || !enderecoValido.value || (frete.value <= 0);
});

onMounted(carregarCarrinho);
</script>

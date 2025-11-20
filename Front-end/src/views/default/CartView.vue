<template>
  <!-- Passos do Checkout (unificado) -->
  <div class="flex justify-center items-center gap-8 py-6">
    <div class="flex items-center gap-2">
      <div class="w-6 h-6 rounded-full bg-[#0B1739] text-white flex items-center justify-center text-xs font-semibold">1</div>
      <span class="font-medium text-[#0B1739]">Carrinho</span>
    </div>
    <div class="w-16 h-[2px] bg-gray-300"></div>
    <div class="flex items-center gap-2 opacity-70">
      <div class="w-6 h-6 rounded-full border-2 border-[#0B1739] text-[#0B1739] flex items-center justify-center text-xs font-semibold bg-[#E5EBFF]">2</div>
      <span class="font-medium text-[#0B1739]">Checkout</span>
    </div>
    <div class="w-16 h-[2px] bg-gray-300"></div>
    <div class="flex items-center gap-2 opacity-50">
      <div class="w-6 h-6 rounded-full border border-gray-300 flex items-center justify-center text-xs font-semibold">3</div>
      <span class="font-medium text-gray-500">Confirmação</span>
    </div>
  </div>

  <!-- Layout unificado com Checkout -->
  <div class="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-6 px-6 pb-16">
    <!-- Itens do carrinho -->
    <section class="lg:col-span-2 space-y-6">
      <div class="flex justify-between items-center">
        <h1 class="text-xl font-semibold text-[#0B1739]">Carrinho de Compras</h1>
        <span class="text-sm text-gray-500">{{ carrinho?.Produtos.length ?? 0 }} itens</span>
      </div>

      <router-link to="/" class="flex items-center gap-2 text-sm border border-gray-300 text-gray-600 px-4 py-2 rounded-md hover:bg-gray-50">
        ← Continuar Comprando
      </router-link>

      <div v-for="(item, index) in carrinho?.Produtos ?? []" :key="item.ProdutoId" class="flex items-center justify-between border rounded-xl p-6 bg-white shadow-sm">
        <div class="flex items-center gap-4">
          <div class="w-20 h-20 bg-gray-100 rounded-md flex items-center justify-center overflow-hidden">
            <img v-if="item.ImagemURL" :src="item.ImagemURL" alt="" class="w-full h-full object-cover" />
          </div>
          <div>
            <h2 class="text-[#0B1739] font-medium">{{ item.Nome }}</h2>
            <p class="text-[#0B1739] font-semibold">R$ {{ item.Preco.toFixed(2) }}</p>
            <div class="flex items-center mt-2">
              <button @click="alterarQuantidade(item.ProdutoId, item.Quantidade - 1)" class="px-3 py-1 border border-gray-300 rounded-l hover:bg-[#E5EBFF]">−</button>
              <input type="number" v-model.number="item.Quantidade" class="w-12 text-center border-t border-b border-gray-300 outline-none" min="1" />
              <button @click="alterarQuantidade(item.ProdutoId, item.Quantidade + 1)" class="px-3 py-1 border border-gray-300 rounded-r hover:bg-[#E5EBFF]">+</button>
            </div>
          </div>
        </div>
        <button @click="removerItem(item.ProdutoId)" class="flex items-center gap-2 text-red-600 hover:text-red-700 text-sm font-medium">
          <Trash class="w-5 h-5" />
          Remover do carrinho
        </button>
      </div>
    </section>

    <!-- Resumo do pedido -->
    <aside class="border rounded-xl p-6 bg-white shadow-sm h-fit">
      <h2 class="text-lg font-semibold text-[#0B1739] mb-4">Resumo do Pedido</h2>

      <div class="text-sm text-gray-700 border-t pt-4 mt-4 space-y-2">
        <div class="flex justify-between">
          <span>Subtotal</span>
          <span>R$ {{ subtotal.toFixed(2) }}</span>
        </div>
        <div class="flex justify-between">
          <span>Economia</span>
          <span>R$ {{ desconto.toFixed(2) }}</span>
        </div>
        <div class="flex justify-between font-semibold text-[#0B1739] text-base mt-2">
          <span>Total</span>
          <span>R$ {{ total.toFixed(2) }}</span>
        </div>
      </div>

      <div class="mt-6">
        <router-link
          v-if="(carrinho?.Produtos?.length ?? 0) > 0"
          :to="`/carrinho/checkout`"
          class="w-full bg-[#0B1739] text-white font-medium py-3 rounded-md hover:opacity-90 transition text-center inline-block"
        >
          Prosseguir para o Pagamento
        </router-link>
        <button
          v-else
          disabled
          class="w-full bg-gray-300 text-gray-600 font-medium py-3 rounded-md cursor-not-allowed"
        >
          Carrinho vazio
        </button>
        <button @click="limparCarrinho" class="mt-3 w-full border border-red-200 text-red-600 font-medium py-2 rounded-md hover:bg-red-50 transition">
          Limpar carrinho
        </button>
      </div>
      <p class="text-xs text-center text-gray-500 mt-2">Pagamento 100% seguro e protegido</p>
    </aside>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { Trash } from 'lucide-vue-next'
import { carrinhoService } from "../../services/carrinhoService.js";
import clienteService from "../../services/clienteService.js";
import productService from "../../services/productService.js";

const clienteId = ref(null);
const clientecpf = ref(null);
const carrinho = ref(null);
const loading = ref(true);

const carregarCarrinho = async () => {
  try {
    loading.value = true;
    // Obter cliente logado
    const perfil = await clienteService.getMeuPerfil();
    clienteId.value = perfil?.id ?? perfil?.Id;
    clientecpf.value = perfil?.cpf ?? perfil?.Cpf ?? null;
    if (!clienteId.value) return;

    const data = await carrinhoService.getCarrinho(clienteId.value);

    console.log("Dados do carrinho:", data);

  carrinho.value = {
    Id: data.id ?? data.Id,
    ClienteId: data.clienteId ?? data.ClienteId,
    Produtos: await Promise.all((data.Produtos ?? data.produtos ?? []).map(async p => {
      const base = {
        ProdutoId: p.ProdutoId ?? p.produtoId ?? p.id,
        CodigoDeBarra: p.CodigoDeBarra ?? p.codigoDeBarra ?? null,
        Nome: p.Nome ?? p.nome ?? "Produto sem nome",
        Preco: p.Preco ?? p.preco ?? 0,
        Quantidade: p.Quantidade ?? p.quantidade ?? 1,
        ImagemURL: p.ImagemURL ?? p.imagemURL ?? p?.Produto?.ImagemURL ?? ""
      }
      if (!base.ImagemURL && base.ProdutoId) {
        try {
          const detalhe = await productService.getById(base.ProdutoId)
          base.ImagemURL = detalhe?.imagemUrl || detalhe?.ImagemURL || ""
        } catch {}
      }
      return base
    })),
      ValorTotalSemDesconto: data.ValorTotalSemDesconto ?? data.valorTotalSemDesconto ?? 0,
      ValorTotalComDesconto: data.ValorTotalComDesconto ?? data.valorTotal ?? 0
    };
  } catch (err) {
    console.error("Erro ao carregar o carrinho:", err);
  } finally {
    loading.value = false;
  }
};

const alterarQuantidade = async (produtoId, quantidade) => {
  if (quantidade < 1) return;

  // Garante código de barras para cada item
  const produtosAtualizados = [];
  for (const p of carrinho.value.Produtos) {
    let codigo = p.CodigoDeBarra;
    if (!codigo && p.ProdutoId) {
      try {
        const detalhe = await productService.getById(p.ProdutoId);
        codigo = detalhe?.CodigoDeBarra ?? detalhe?.codigoDeBarra ?? null;
      } catch {}
    }
    if (!codigo) continue;
    produtosAtualizados.push({
      CodigoDeBarra: codigo,
      Quantidade: p.ProdutoId === produtoId ? quantidade : p.Quantidade
    });
  }

  await carrinhoService.atualizarCarrinho(carrinho.value.Id, produtosAtualizados, clientecpf.value);
  await carregarCarrinho();
};

const removerItem = async (produtoId) => {
  const produtosAtualizados = [];
  for (const p of carrinho.value.Produtos) {
    if (p.ProdutoId === produtoId) continue;
    let codigo = p.CodigoDeBarra;
    if (!codigo && p.ProdutoId) {
      try {
        const detalhe = await productService.getById(p.ProdutoId);
        codigo = detalhe?.CodigoDeBarra ?? detalhe?.codigoDeBarra ?? null;
      } catch {}
    }
    if (!codigo) continue;
    produtosAtualizados.push({ CodigoDeBarra: codigo, Quantidade: p.Quantidade });
  }

  await carrinhoService.atualizarCarrinho(carrinho.value.Id, produtosAtualizados, clientecpf.value);
  await carregarCarrinho();
};

const limparCarrinho = async () => {
  try {
    await carrinhoService.limparCarrinho(carrinho.value.Id, clientecpf.value)
    await carregarCarrinho()
  } catch (err) {
    console.error('Falha ao limpar carrinho:', err)
  }
}

const desconto = computed(() => {
  if (!carrinho.value) return 0;
  return carrinho.value.ValorTotalSemDesconto - (carrinho.value.ValorTotalComDesconto ?? carrinho.value.ValorTotalSemDesconto);
});

const subtotal = computed(() => {
  if (!carrinho.value) return 0;
  return carrinho.value.Produtos.reduce((acc, p) => acc + p.Preco * p.Quantidade, 0);
});

const total = computed(() => carrinho.value?.ValorTotalComDesconto ?? subtotal.value);

onMounted(carregarCarrinho);
</script>

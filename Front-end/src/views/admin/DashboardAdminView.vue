<template>
  <main class="p-8 bg-gray-100 min-h-screen">
    <!-- Header -->
    <header class="flex justify-between items-center mb-8">
      <div class="flex items-center gap-4">
        <img src="../../assets/images/logo-dunderstore.png" alt="Dunder Store" class="h-12 w-auto object-contain" />
        <div>
          <h1 class="text-3xl font-semibold text-gray-900">Dashboard</h1>
          <p class="text-gray-600">Visão geral do seu e-commerce</p>
        </div>
      </div>
      <ViewStoreButton />
    </header>

    <!-- Cards principais -->
    <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
      <div class="bg-white rounded-2xl shadow-md p-6 flex flex-col items-center justify-center hover:shadow-lg transition">
        <span class="text-sm text-gray-500 mb-1">Total de Produtos</span>
        <p class="text-3xl font-bold text-indigo-600">{{ totalProdutos }}</p>
      </div>

      <div class="bg-white rounded-2xl shadow-md p-6 flex flex-col items-center justify-center hover:shadow-lg transition">
        <span class="text-sm text-gray-500 mb-1">Total de Pedidos</span>
        <p class="text-3xl font-bold text-blue-600">{{ totalPedidos }}</p>
      </div>

      <div class="bg-white rounded-2xl shadow-md p-6 flex flex-col items-center justify-center hover:shadow-lg transition">
        <span class="text-sm text-gray-500 mb-1">Total de Clientes</span>
        <p class="text-3xl font-bold text-green-600">{{ totalClientes }}</p>
      </div>

      <div class="bg-white rounded-2xl shadow-md p-6 flex flex-col items-center justify-center hover:shadow-lg transition">
        <span class="text-sm text-gray-500 mb-1">Receita Mensal</span>
        <p class="text-3xl font-bold text-emerald-600">{{ receitaMensal }}</p>
      </div>
    </section>

    <!-- Gráfico -->
    <section class="bg-white p-6 rounded-2xl shadow-md mb-10">
      <h2 class="text-lg font-semibold mb-4">Receita dos Últimos Meses</h2>
      <canvas id="receitaChart" height="20"></canvas>
    </section>

    <!-- Pedidos e produtos -->
    <section class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Pedidos Recentes -->
      <div class="bg-white p-6 rounded-2xl shadow-md">
        <h2 class="text-lg font-semibold mb-2">Pedidos Recentes</h2>
        <p class="text-sm text-gray-500 mb-4">Últimos pedidos realizados</p>

        <table class="w-full text-left text-sm">
          <tbody class="divide-y divide-gray-200">
            <tr v-for="pedido in pedidosRecentes" :key="pedido.id" class="hover:bg-gray-50">
              <td class="py-3">
                <p class="font-medium text-gray-900">#{{ pedido.id }}</p>
                <p class="text-gray-500 text-xs">{{ pedido.nomeCliente }}</p>
              </td>
              <td class="text-right py-3">
                <p class="font-semibold text-gray-800">{{ pedido.totalPedido }}</p>
                <p class="text-xs text-gray-500">{{ pedido.statusPedido }}</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Produtos em Destaque -->
      <div class="bg-white p-6 rounded-2xl shadow-md">
        <h2 class="text-lg font-semibold mb-2">Produtos em Destaque</h2>
        <p class="text-sm text-gray-500 mb-4">Mais vendidos recentemente</p>

        <table class="w-full text-left text-sm">
          <tbody class="divide-y divide-gray-200">
            <tr v-for="produto in produtosDestaque" :key="produto.id" class="hover:bg-gray-50">
              <td class="py-3">
                <p class="font-medium text-gray-900">{{ produto.nome }}</p>
              </td>
              <td class="text-right py-3">
                <p class="font-semibold text-gray-800">{{ produto.preco }}</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  </main>
</template>

<script setup>
import { ref, onMounted, nextTick } from "vue";
import Chart from "chart.js/auto";
import ViewStoreButton from "../../components/admin/buttons/ViewStoreButton.vue";
import productService from "../../services/productService";
import pedidoService from "../../services/pedidoService";
import api from "../../services/api";

// Totais
const totalProdutos = ref(0);
const totalPedidos = ref(0);
const totalClientes = ref(0);
const receitaMensal = ref("R$ 0,00");

// Dados do gráfico
let receitaChart = null;

// Listas
const pedidosRecentes = ref([]);
const produtosDestaque = ref([]);

function formatarBRL(valor) {
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL", maximumFractionDigits: 2 }).format(Number(valor) || 0);
}

// Função para normalizar status
function traduzirStatus(status) {
  if (status === 0 || status === "0" || status === "Carrinho") return "Carrinho 🛒";
  if (status === 1 || status === "1" || status === "Finalizado") return "Finalizado ✅";
  return "Desconhecido";
}

// Função para carregar dados
async function carregarDashboard() {
  try {
    // Produtos
    try {
      const produtosResponse = await productService.getAll({ pagina: 1, itensPorPagina: 100 });
      totalProdutos.value = produtosResponse.totalItens ?? produtosResponse.itens?.length ?? 0;
    } catch (e) {
      console.warn("Falha ao carregar produtos:", e);
      totalProdutos.value = 0;
    }

    // Pedidos
    let listaPedidos = [];
    try {
      const pedidosResponse = await pedidoService.getAll({ pagina: 1, itensPorPagina: 50 });
      listaPedidos = pedidosResponse.pedidos || pedidosResponse.itens || [];
      totalPedidos.value = pedidosResponse.totalItens ?? listaPedidos.length;
    } catch (e) {
      console.warn("Falha ao carregar pedidos:", e);
      listaPedidos = [];
      totalPedidos.value = 0;
    }

    // Pedidos recentes formatados
    pedidosRecentes.value = listaPedidos
      .filter(p => {
        const s = p?.status;
        return s === 1 || s === "1" || s === "Finalizado";
      })
      .slice(0, 5)
      .map(p => ({
        id: p.id,
        nomeCliente: p.clienteNome || p.cliente?.nome || "Cliente",
        totalPedido: formatarBRL(p.total ?? p.valorTotal ?? 0),
        statusPedido: traduzirStatus(p.status),
      }));

    // Receita mensal (apenas finalizados) via serviço/endpoint
    try {
      const receita = await pedidoService.getReceitaMensal();
      receitaMensal.value = formatarBRL(receita);
    } catch (e) {
      console.warn("Falha ao carregar receita mensal:", e);
      receitaMensal.value = "0,00";
    }

    // Total de Clientes
    try {
      const { data: clientesCount } = await api.get("/Cliente/count");
      totalClientes.value = clientesCount ?? 0;
    } catch (e) {
      console.warn("Falha ao carregar total de clientes:", e);
      totalClientes.value = 0;
    }

    // Produtos em destaque
    try {
      const { data: destaqueData } = await api.get("/Pedido/produtos-mais-vendidos");
      const lista = (destaqueData || []).map(p => ({
        id: p.produtoId,
        nome: (p.nome || "").toString(),
        preco: formatarBRL(p.preco ?? 0),
      }));
      const vistos = new Set();
      produtosDestaque.value = lista.filter(p => {
        const valido = !!p.nome.trim();
        if (!valido) return false;
        if (vistos.has(p.id)) return false;
        vistos.add(p.id);
        return true;
      });
    } catch (e) {
      console.warn("Falha ao carregar produtos em destaque:", e);
      produtosDestaque.value = [];
    }

    // Gráfico dos últimos meses (apenas pedidos finalizados)
    const { labels, valores } = calcularReceitaUltimosMeses(listaPedidos);
    await nextTick();
    renderizarGrafico(labels, valores);
  } catch (error) {
    console.error("Erro ao carregar dashboard:", error);
  }
}

// Agrupa receita por mês dos últimos N meses considerando apenas pedidos finalizados
function calcularReceitaUltimosMeses(pedidos, meses = 6) {
  const agora = new Date();
  const chaves = [];
  const labels = [];
  for (let i = meses - 1; i >= 0; i--) {
    const d = new Date(agora.getFullYear(), agora.getMonth() - i, 1);
    const key = `${d.getFullYear()}-${d.getMonth()}`;
    chaves.push(key);
    labels.push(d.toLocaleString("pt-BR", { month: "short" }).replace(".", ""));
  }
  const mapa = Object.fromEntries(chaves.map((k) => [k, 0]));
  (pedidos || []).forEach((p) => {
    const dataRaw = p.dataPedido ?? p.DataPedido;
    if (!dataRaw) return;
    const d = new Date(dataRaw);
    const key = `${d.getFullYear()}-${d.getMonth()}`;
    const status = p.status;
    const isFinalizado = status === 1 || status === "1" || status === "Finalizado";
    if (isFinalizado && mapa[key] !== undefined) {
      mapa[key] += Number(p.total ?? p.valorTotal ?? 0);
    }
  });
  const valores = chaves.map((k) => mapa[k]);
  return { labels, valores };
}

// Gráfico
function renderizarGrafico(labels, valores) {
  const ctx = document.getElementById("receitaChart");
  if (receitaChart) receitaChart.destroy();

  receitaChart = new Chart(ctx, {
    type: "line",
    data: {
      labels,
      datasets: [
        {
          label: "Receita (R$)",
          data: valores,
          borderColor: "#10B981",
          backgroundColor: "rgba(16,185,129,0.1)",
          fill: true,
          tension: 0.4,
        },
      ],
    },
    options: {
      responsive: true,
      plugins: { legend: { display: false } },
      scales: { y: { beginAtZero: true } },
    },
  });
}

onMounted(() => carregarDashboard());
</script>

<style scoped>
table {
  border-collapse: collapse;
}
</style>

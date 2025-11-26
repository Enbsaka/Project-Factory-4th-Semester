<template>
  <main class="flex-1 bg-[#F5F7FB] p-8">
    <ClientHeader @novo="abrirModalNovoCliente" />

    <ClientFilters
      v-model:busca="busca"
      v-model:ordenacao="ordenacaoCadastro"
    />

    <ClientTable
      :clientes="clientesOrdenados"
      @editar="abrirModalEditar"
      @excluir="abrirModalExcluir"
      @detalhar="abrirModalDetalhar"
    />
    <!-- Paginação com seletor embutido -->
    <PaginationBar
      :pagina-atual="paginaAtual"
      :total-paginas="totalPaginas"
      :page-size="itensPorPagina"
      :page-size-options="[10, 20, 50]"
      @mudar-pagina="mudarPagina"
      @update:pageSize="mudarItensPorPagina"
    />
    <ClientModalNovo
      v-if="modalNovoAberto"
      @fechar="fecharModalNovoCliente"
      @criado="carregarClientes"
    />

    <!-- Modais de ações -->
    <ClientModalEditar
      v-if="modalEditarAberto"
      :cliente="clienteSelecionado"
      @fechar="modalEditarAberto = false"
      @atualizado="carregarClientes"
    />
    <ClientModalExcluir
      v-if="modalExcluirAberto"
      :cliente="clienteSelecionado"
      @fechar="modalExcluirAberto = false"
      @confirmado="carregarClientes"
    />
    <ClientModalDetalhar
      v-if="modalDetalharAberto"
      :cliente="clienteSelecionado"
      @fechar="modalDetalharAberto = false"
    />
  </main>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import ClientHeader from "../../components/admin/components/Client/ClientHeader.vue";
import ClientFilters from "../../components/admin/components/Client/ClientFilters.vue";
import ClientTable from "../../components/admin/components/Client/ClientTable.vue";
import clienteService from "../../services/clienteService.js";
import pedidoService from "../../services/pedidoService.js";
import ClientModalNovo from "../../components/admin/modals/ClientModalNovo.vue";
import ClientModalEditar from "../../components/admin/modals/ClientModalEditar.vue";
import ClientModalExcluir from "../../components/admin/modals/ClientModalExcluir.vue";
import ClientModalDetalhar from "../../components/admin/modals/ClientModalDetalhar.vue";
import PaginationBar from "../../components/admin/components/PaginationBar.vue";

const clientes = ref([]);
const busca = ref("");
const ordenacaoCadastro = ref("desc");
const modalNovoAberto = ref(false);
const clienteSelecionado = ref(null);
const modalEditarAberto = ref(false);
const modalExcluirAberto = ref(false);
const modalDetalharAberto = ref(false);

// Paginação
const paginaAtual = ref(1);
const totalPaginas = ref(1);
const itensPorPagina = ref(10);

const carregarClientes = async () => {
  try {
    const { clientes: lista, paginaAtual: pag, totalPaginas: tot } = await clienteService.getAllClientes(paginaAtual.value, itensPorPagina.value);

    clientes.value = lista.map((c) => ({
      id: c.id,
      nome: c.nome,
      email: c.email,
      cpf: c.cpf,
      dataCadastro: c.dataCadastro || new Date(),
      totalPedidos: 0
    }));

    paginaAtual.value = Math.max(pag || 1, 1);
    totalPaginas.value = Math.max(tot || 1, 1);

    // Atualiza contagem de pedidos por cliente usando totalItens
    await Promise.all(
      clientes.value.map(async (c) => {
        try {
          const resp = await pedidoService.getByCliente(c.id);
          c.totalPedidos = resp?.totalItens ?? 0;
        } catch (e) {
          console.warn("Falha ao obter pedidos do cliente", c.id, e);
          c.totalPedidos = 0;
        }
      })
    );
  } catch (err) {
    console.error("Erro ao carregar clientes:", err);
  }
};


// 🔹 Normalização de CPF (apenas dígitos)
function normalizarCPF(cpf) {
  return String(cpf || "").replace(/\D/g, "");
}

// 🔹 Filtro de busca (nome, email, CPF)
const clientesFiltrados = computed(() => {
  const termo = (busca.value || "").trim().toLowerCase();
  if (!termo) return clientes.value;

  const termoDigitos = termo.replace(/\D/g, "");

  return clientes.value.filter((c) => {
    const nomeMatch = c.nome?.toLowerCase().includes(termo);
    const emailMatch = c.email?.toLowerCase().includes(termo);
    const cpfMatch = termoDigitos
      ? normalizarCPF(c.cpf).includes(termoDigitos)
      : (c.cpf?.toLowerCase().includes(termo));
    return nomeMatch || emailMatch || cpfMatch;
  });
});

// 🔹 Ordenação por data de cadastro
const clientesOrdenados = computed(() => {
  const arr = [...clientesFiltrados.value];
  arr.sort((a, b) => {
    const da = new Date(a.dataCadastro || 0).getTime();
    const db = new Date(b.dataCadastro || 0).getTime();
    return ordenacaoCadastro.value === "asc" ? da - db : db - da;
  });
  return arr;
});

// 🔹 Formatar data
const formatarData = (data) => {
  const d = new Date(data);
  return d.toLocaleDateString("pt-BR");
};

onMounted(() => {
  carregarClientes();
});

// Mudar página
function mudarPagina(novaPagina) {
  if (novaPagina < 1 || novaPagina > totalPaginas.value) return;
  paginaAtual.value = novaPagina;
  carregarClientes();
}

// Mudar itens por página
function mudarItensPorPagina(novoTamanho) {
  const n = Number(novoTamanho) || 10;
  itensPorPagina.value = n;
  paginaAtual.value = 1;
  carregarClientes();
}

const abrirModalNovoCliente = () => { modalNovoAberto.value = true; };
const fecharModalNovoCliente = () => { modalNovoAberto.value = false; };

function abrirModalEditar(cliente) {
  clienteSelecionado.value = cliente;
  modalEditarAberto.value = true;
}
function abrirModalExcluir(cliente) {
  clienteSelecionado.value = cliente;
  modalExcluirAberto.value = true;
}
function abrirModalDetalhar(cliente) {
  clienteSelecionado.value = cliente;
  modalDetalharAberto.value = true;
}
</script>

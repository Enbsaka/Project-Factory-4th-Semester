<template>
  <main class="flex-1 bg-[#F5F7FB] p-8">
    <!-- Cabeçalho padronizado -->
    <CouponHeader @novo="abrirModalNovo = true" />

    <!-- Filtros padronizados -->
    <CouponFilters v-model:busca="filtro" v-model:status="filtroStatus" />

    <!-- Tabela padronizada -->
    <CouponTable
      :cupons="cuponsPaginados"
      @editar="abrirEditar"
      @excluir="abrirExcluir"
    />

    <PaginationBar
      :paginaAtual="paginaAtual"
      :totalPaginas="totalPaginas"
      :page-size="itensPorPagina"
      :page-size-options="[10, 20, 50]"
      @mudar-pagina="mudarPagina"
      @update:pageSize="mudarItensPorPagina"
    />

    <div v-if="cupons.length === 0" class="text-gray-500 text-center py-5 space-y-3">
      <p>Nenhum cupom encontrado.</p>
      <button class="px-3 py-2 bg-blue-600 text-white rounded" @click="criarCupomTeste">Criar cupom de teste</button>
    </div>

    <!-- Modal Novo -->
    <div
      v-if="abrirModalNovo"
      class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50"
    >
      <div class="bg-white rounded-xl p-6 w-full max-w-md shadow-lg">
        <h2 class="text-lg font-semibold mb-4">Criar novo cupom</h2>

        <form @submit.prevent="criarCupom">
          <div class="mb-3">
            <label class="block text-sm font-medium mb-1">Código</label>
            <input
              v-model="novoCupom.codigo"
              class="border rounded-md w-full px-3 py-2"
            />
          </div>

          <div class="mb-3">
            <label class="block text-sm font-medium mb-1">Desconto (%)</label>
            <input
              v-model="novoCupom.descontoPercentual"
              type="number"
              class="border rounded-md w-full px-3 py-2"
            />
          </div>

          <div class="mb-3">
            <label class="block text-sm font-medium mb-1">Data e horário de expiração</label>
            <input
              v-model="novoCupom.dataExpiracao"
              type="datetime-local"
              class="border rounded-md w-full px-3 py-2"
            />
          </div>

          <div class="mb-4 flex items-center space-x-2">
            <input type="checkbox" v-model="novoCupom.ativo" id="ativo" />
            <label for="ativo" class="text-sm">Ativo</label>
          </div>

          <div class="flex justify-end space-x-2">
            <button
              type="button"
              @click="abrirModalNovo = false"
              class="px-3 py-2 text-gray-600 hover:text-gray-900"
            >
              Cancelar
            </button>
            <button
              type="submit"
              class="bg-[#141A7C] text-white px-4 py-2 rounded-md text-sm hover:bg-[#0f166a]"
            >
              Criar
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal Editar -->
    <div
      v-if="editarSelecionado"
      class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50"
    >
      <div class="bg-white rounded-xl p-6 w-full max-w-md shadow-lg">
        <h2 class="text-lg font-semibold mb-4">Editar cupom</h2>

        <form @submit.prevent="atualizarCupom">
          <div class="mb-3">
            <label class="block text-sm font-medium mb-1">Código</label>
            <input
              v-model="editarSelecionado.codigo"
              class="border rounded-md w-full px-3 py-2"
            />
          </div>

          <div class="mb-3">
            <label class="block text-sm font-medium mb-1">Desconto (%)</label>
            <input
              v-model="editarSelecionado.descontoPercentual"
              type="number"
              class="border rounded-md w-full px-3 py-2"
            />
          </div>

          <div class="mb-3">
            <label class="block text-sm font-medium mb-1">Data e horário de expiração</label>
            <input
              v-model="editarSelecionado.dataExpiracao"
              type="datetime-local"
              class="border rounded-md w-full px-3 py-2"
            />
          </div>

          <div class="mb-4 flex items-center space-x-2">
            <input
              type="checkbox"
              v-model="editarSelecionado.ativo"
              id="ativoEdit"
            />
            <label for="ativoEdit" class="text-sm">Ativo</label>
          </div>

          <div class="flex justify-end space-x-2">
            <button
              type="button"
              @click="editarSelecionado = null"
              class="px-3 py-2 text-gray-600 hover:text-gray-900"
            >
              Cancelar
            </button>
            <button
              type="submit"
              class="bg-[#141A7C] text-white px-4 py-2 rounded-md text-sm hover:bg-[#0f166a]"
            >
              Salvar
            </button>
          </div>
        </form>
      </div>
    </div>

    <CouponModalExcluir
      :aberto="modalExcluirAberto"
      :cupom="cupomParaExcluir"
      @fechar="fecharExcluir"
      @confirmar="confirmarExcluir"
    />
  </main>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { cupomService } from "../../services/cupomService.js";
import CouponHeader from "../../components/admin/components/Coupon/CouponHeader.vue";
import CouponFilters from "../../components/admin/components/Coupon/CouponFilters.vue";
import CouponTable from "../../components/admin/components/Coupon/CouponTable.vue";
import PaginationBar from "../../components/admin/components/PaginationBar.vue";
import CouponModalExcluir from "../../components/admin/modals/CouponModalExcluir.vue";
import { useNotifications } from "../../components/admin/composables/useNotifications.js";


const cupons = ref([]);
const filtro = ref("");
const filtroStatus = ref("");
const abrirModalNovo = ref(false);
const editarSelecionado = ref(null);
const modalExcluirAberto = ref(false);
const cupomParaExcluir = ref(null);
const paginaAtual = ref(1);
const itensPorPagina = ref(10);
const totalPaginas = ref(1);

const novoCupom = ref({
  codigo: "",
  descontoPercentual: "",
  dataExpiracao: "",
  ativo: true,
});
const { show } = useNotifications();

// Carregar todos
const carregarCupons = async () => {
  try {
    cupons.value = await cupomService.getAll();
    paginaAtual.value = 1;
    if (cupons.value.length === 0) {
      await criarCupomTeste();
      cupons.value = await cupomService.getAll();
    }
  } catch (error) {
    console.error("Erro ao carregar cupons:", error);
    show("Erro ao carregar cupons.", "error");
  }
};

// Criar
const criarCupom = async () => {
  try {
    await cupomService.criarCupom(novoCupom.value);
    abrirModalNovo.value = false;
    limparNovo();
    carregarCupons();
    show("Cupom criado com sucesso!", "success");
  } catch (error) {
    console.error("Erro ao criar cupom:", error);
    show("Erro ao criar cupom.", "error");
  }
};

const limparNovo = () => {
  novoCupom.value = { codigo: "", descontoPercentual: "", dataExpiracao: "", ativo: true };
};

const pad = (n) => String(n).padStart(2, "0");
const formatToDatetimeLocal = (dt) => {
  const d = new Date(dt);
  const yyyy = d.getFullYear();
  const MM = pad(d.getMonth() + 1);
  const dd = pad(d.getDate());
  const hh = pad(d.getHours());
  const mm = pad(d.getMinutes());
  return `${yyyy}-${MM}-${dd}T${hh}:${mm}`;
};

const abrirEditar = (cupom) => {
  editarSelecionado.value = { ...cupom, dataExpiracao: formatToDatetimeLocal(cupom.dataExpiracao) };
};

const abrirExcluir = (cupom) => {
  cupomParaExcluir.value = cupom;
  modalExcluirAberto.value = true;
};

const atualizarCupom = async () => {
  try {
    await cupomService.atualizarCupom(editarSelecionado.value.id, editarSelecionado.value);
    editarSelecionado.value = null;
    carregarCupons();
    show("Cupom atualizado com sucesso!", "success");
  } catch (error) {
    console.error("Erro ao atualizar cupom:", error);
    show("Erro ao atualizar cupom.", "error");
  }
};

const confirmarExcluir = async () => {
  try {
    await cupomService.deletarCupom(cupomParaExcluir.value.id);
    modalExcluirAberto.value = false;
    cupomParaExcluir.value = null;
    carregarCupons();
    show("Cupom excluído com sucesso!", "success");
  } catch (error) {
    console.error("Erro ao deletar cupom:", error);
    show("Erro ao excluir cupom.", "error");
  }
};
const fecharExcluir = () => {
  modalExcluirAberto.value = false;
  cupomParaExcluir.value = null;
};

const cuponsFiltrados = computed(() => {
  const busca = filtro.value.trim().toLowerCase();
  let lista = cupons.value;
  if (busca) {
    lista = lista.filter((c) => c.codigo?.toLowerCase().includes(busca));
  }
  if (filtroStatus.value === "ativo") {
    lista = lista.filter((c) => c.ativo === true);
  } else if (filtroStatus.value === "inativo") {
    lista = lista.filter((c) => c.ativo === false);
  }
  return lista;
});

const cuponsPaginados = computed(() => {
  const total = cuponsFiltrados.value.length;
  totalPaginas.value = Math.max(1, Math.ceil(total / itensPorPagina.value));
  const start = (paginaAtual.value - 1) * itensPorPagina.value;
  return cuponsFiltrados.value.slice(start, start + itensPorPagina.value);
});

const mudarPagina = (page) => {
  paginaAtual.value = page;
};
const mudarItensPorPagina = (size) => {
  itensPorPagina.value = size;
  paginaAtual.value = 1;
};

const formatarData = (data) => {
  if (!data) return "—";
  return new Date(data).toLocaleString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const criarCupomTeste = async () => {
  const hoje = new Date();
  const daquiUmMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, hoje.getDate(), 23, 59);
  const dtLocal = formatToDatetimeLocal(daquiUmMes);
  novoCupom.value = {
    codigo: "TESTE10",
    descontoPercentual: 10,
    dataExpiracao: dtLocal,
    ativo: true,
  };
  await criarCupom();
};

onMounted(carregarCupons);
</script>

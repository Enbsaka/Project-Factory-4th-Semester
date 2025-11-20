<template>
  <div ref="root" class="relative inline-block text-left" @keydown.stop.prevent="onKeyDown" tabindex="0" role="combobox" :aria-expanded="aberto ? 'true' : 'false'" aria-haspopup="listbox">
    <button
      type="button"
      class="inline-flex justify-between items-center w-72 px-3 py-2 border border-gray-300 bg-white text-sm rounded-md shadow-sm hover:bg-gray-50"
      @click="aberto = !aberto"
      aria-haspopup="listbox"
      :aria-expanded="aberto ? 'true' : 'false'"
    >
      <span class="truncate text-gray-700">
        {{ selectedLabel || "Todas as categorias" }}
      </span>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" class="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" stroke-width="1.5">
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 9l6 6 6-6" />
      </svg>
    </button>

    <div v-if="aberto" class="absolute z-20 mt-2 right-0 bg-white border border-gray-200 rounded-md shadow-lg sm:w-[640px] w-[90vw] max-w-[90vw]" role="listbox">
      <div class="grid grid-cols-1 md:grid-cols-2">
        <!-- Coluna esquerda: categorias pai -->
        <ul class="max-h-72 overflow-auto divide-y divide-gray-100" role="menu" aria-label="Categorias">
          <li
            v-for="cat in pais"
            :key="cat.id"
            class="px-3 py-2 cursor-pointer hover:bg-gray-50 flex justify-between items-center focus:outline-none"
            @mouseenter="hoverParent(cat)"
            @click="toggleParent(cat)"
            :aria-selected="isParentSelected(cat) ? 'true' : 'false'"
            role="menuitem"
          >
            <span class="text-gray-700 text-sm">{{ cat.nome }}</span>
            <span class="ml-2 text-[11px] px-2 py-0.5 rounded-full bg-gray-100 text-gray-600">{{ contarProdutos(cat) }}</span>
            <svg v-if="(cat.subcategorias?.length || 0) > 0" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
            </svg>
          </li>
        </ul>

        <!-- Coluna direita: subcategorias do pai em hover -->
        <div class="border-l border-gray-200 max-h-72 overflow-auto">
          <div v-if="subcatsAtual.length" class="p-2">
            <p class="px-2 py-1 text-xs font-medium text-gray-500">Subcategorias</p>
            <ul class="divide-y divide-gray-100" role="menu" aria-label="Subcategorias">
              <li
                v-for="sub in subcatsAtual"
                :key="sub.id"
                class="px-3 py-2 cursor-pointer hover:bg-gray-50 flex items-center gap-2"
                @click="toggleSub(sub)"
                role="menuitemcheckbox"
                :aria-checked="selecionadosSet.has(String(sub.id)) ? 'true' : 'false'"
              >
                <input type="checkbox" :checked="selecionadosSet.has(String(sub.id))" @change.stop="toggleSub(sub)" class="accent-blue-600" />
                <span class="text-gray-700 text-sm">{{ sub.nome }}</span>
                <span class="ml-auto text-[11px] px-2 py-0.5 rounded-full bg-gray-100 text-gray-600">{{ contarProdutos(sub) }}</span>
              </li>
            </ul>
          </div>
          <div v-else class="h-full flex items-center justify-center text-gray-400 text-sm">
            Passe o mouse em uma categoria
          </div>
        </div>
      </div>

      <!-- Barra inferior: limpar seleção -->
      <div class="border-t border-gray-200 p-2 flex justify-between items-center">
        <button class="text-xs text-gray-600 hover:text-blue-600" @click="limpar">Limpar seleção</button>
        <span class="text-xs text-gray-400">Selecione pai e/ou subcategorias</span>
      </div>
    </div>
  </div>
  
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";

const props = defineProps({
  categorias: { type: Array, default: () => [] },
  // Suporte a múltipla seleção: lista de IDs
  modelValue: { type: Array, default: () => [] },
});
const emit = defineEmits(["update:modelValue", "change"]);

const aberto = ref(false);
const hoveredParentId = ref(null);
const selecionados = ref(Array.isArray(props.modelValue) ? [...props.modelValue] : []);
const selecionadosSet = computed(() => new Set((selecionados.value || []).map((x) => String(x))));
const root = ref(null);

// Normaliza estrutura: garante 'subcategorias' em camelCase
const normalizadas = computed(() => {
  return (props.categorias || []).map((c) => ({
    id: c.id ?? c.Id,
    nome: c.nome ?? c.Nome,
    subcategorias: (c.subcategorias ?? c.Subcategorias ?? []).map((s) => ({
      id: s.id ?? s.Id,
      nome: s.nome ?? s.Nome,
      subcategorias: s.subcategorias ?? s.Subcategorias ?? [],
      produtos: s.produtos ?? s.Produtos ?? [],
    })),
    produtos: c.produtos ?? c.Produtos ?? [],
  }));
});

const pais = computed(() => normalizadas.value.filter((c) => true)); // raiz já vem na hierarquia
const selecionadaLista = computed(() => selecionados.value);

const selectedLabel = computed(() => {
  if (!selecionadaLista.value || selecionadaLista.value.length === 0) return null;
  const labels = [];
  const rec = (lista) => {
    for (const c of lista) {
      if (selecionadosSet.value.has(String(c.id))) labels.push(c.nome);
      rec(c.subcategorias || []);
    }
  };
  rec(normalizadas.value);
  return labels.join(", ");
});

const subcatsAtual = computed(() => {
  const cat = normalizadas.value.find((c) => String(c.id) === String(hoveredParentId.value));
  return cat?.subcategorias || [];
});

function hoverParent(cat) {
  hoveredParentId.value = cat.id;
}

function toggleParent(cat) {
  // Seleciona/deseleciona pai e todas subcategorias
  const ids = coletarIds(cat);
  const set = new Set(selecionados.value.map((x) => String(x)));
  const todosSelecionados = ids.every((id) => set.has(String(id)));
  if (todosSelecionados) {
    // remover todos
    ids.forEach((id) => set.delete(String(id)));
  } else {
    ids.forEach((id) => set.add(String(id)));
  }
  selecionados.value = Array.from(set);
  emitirMudanca();
}

function toggleSub(sub) {
  const idStr = String(sub.id);
  const set = new Set(selecionados.value.map((x) => String(x)));
  if (set.has(idStr)) set.delete(idStr); else set.add(idStr);
  selecionados.value = Array.from(set);
  emitirMudanca();
}

function limpar() {
  selecionados.value = [];
  emitirMudanca();
}

function fechar() {
  aberto.value = false;
}

function emitirMudanca() {
  emit("update:modelValue", selecionados.value);
  emit("change", selecionados.value);
}

function coletarIds(node) {
  const ids = [node.id];
  (node.subcategorias || []).forEach((c) => ids.push(...coletarIds(c)));
  return ids;
}

function contarProdutos(node) {
  let total = (node.produtos?.length ?? 0);
  (node.subcategorias || []).forEach((c) => {
    total += contarProdutos(c);
  });
  return total;
}

function isParentSelected(cat) {
  const ids = coletarIds(cat).map((x) => String(x));
  const set = selecionadosSet.value;
  return ids.every((id) => set.has(id));
}

function onKeyDown(e) {
  if (!aberto.value && (e.key === 'ArrowDown' || e.key === 'Enter' || e.key === ' ')) {
    aberto.value = true;
    e.preventDefault();
    return;
  }
  if (e.key === 'Escape') {
    aberto.value = false;
    e.preventDefault();
  }
}

// Fecha ao clicar fora, sem fechar quando o mouse sai da área
function onClickOutside(e) {
  if (!root.value) return;
  if (!root.value.contains(e.target)) {
    aberto.value = false;
  }
}

onMounted(() => {
  document.addEventListener('click', onClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', onClickOutside);
});
</script>

<style scoped>
</style>
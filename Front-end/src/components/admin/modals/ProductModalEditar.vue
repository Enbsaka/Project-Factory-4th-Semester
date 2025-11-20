<template>
  <BaseModal
    titulo="Editar Produto"
    textoConfirmar="Salvar"
    @confirmar="salvarAlteracoes"
    @fechar="$emit('fechar')"
  >
    <form @submit.prevent="salvarAlteracoes" class="space-y-4">
      <div>
        <label class="block text-sm font-medium text-gray-700">Nome</label>
        <input
          v-model="form.nome"
          type="text"
          class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
        />
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700">Descrição</label>
        <textarea
          v-model="form.descricao"
          rows="2"
          class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
        ></textarea>
      </div>

      <div class="flex gap-3">
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Preço</label>
          <input
            :value="form.precoTexto"
            @input="onPrecoInput"
            type="text"
            inputmode="decimal"
            placeholder="Ex.: 5,919.99 ou 5.919,99"
            class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
          />
        </div>
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Categoria</label>
          <select
            v-model="form.categoriaId"
            class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
          >
            <option disabled value="">Selecione uma categoria</option>
            <optgroup v-for="p in normalizadas" :key="p.id" :label="p.nome">
              <option :value="p.id">(Pai) {{ p.nome }}</option>
              <option v-for="s in p.subcategorias" :key="s.id" :value="s.id">{{ s.nome }}</option>
            </optgroup>
          </select>
        </div>
      </div>

      <div class="flex gap-3">
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Cor</label>
          <input
            v-model="form.cor"
            type="text"
            class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
          />
        </div>
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Tamanho</label>
          <input
            v-model="form.tamanho"
            type="text"
            class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
          />
        </div>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700">Imagem</label>
        <input
          type="file"
          multiple
          @change="handleFileChange"
          class="w-full mt-1 text-gray-600"
        />
      </div>
    </form>
  </BaseModal>
</template>

<script setup>
import { reactive, computed } from "vue";
import BaseModal from "./BaseModal.vue";
import { useProducts } from "../composables/useProducts.js";

const props = defineProps({ produto: Object });
const emit = defineEmits(["fechar", "atualizar"]);
const { categorias, salvarProduto } = useProducts();

const form = reactive({
  nome: props.produto?.nome || "",
  descricao: props.produto?.descricao || "",
  preco: props.produto?.preco || "",
  precoTexto: String(props.produto?.precoNumber ?? props.produto?.preco ?? ""),
  categoriaId: props.produto?.categoriaId || "",
  cor: props.produto?.cor || "",
  tamanho: props.produto?.tamanho || "",
  imagem: null,
  imagens: [],
});

// Flatten categorias hierárquicas para seleção de subcategoria com optgroup
const normalizadas = computed(() => {
  return (categorias.value || []).map((c) => ({
    id: c.id ?? c.Id,
    nome: c.nome ?? c.Nome,
    subcategorias: (c.subcategorias ?? c.Subcategorias ?? []).map((s) => ({
      id: s.id ?? s.Id,
      nome: s.nome ?? s.Nome,
      subcategorias: s.subcategorias ?? s.Subcategorias ?? [],
    })),
  }));
});

function parsePrecoFlex(text) {
  const s = String(text || "").replace(/\s+/g, "").replace(/[^0-9.,]/g, "");
  if (!s) return "";
  const lastComma = s.lastIndexOf(",");
  const lastDot = s.lastIndexOf(".");
  let sepIdx = Math.max(lastComma, lastDot);
  if (sepIdx < 0) {
    // Sem separador decimal
    return s.replace(/[^0-9]/g, "");
  }
  const intPart = s.slice(0, sepIdx).replace(/[^0-9]/g, "");
  const decPartRaw = s.slice(sepIdx + 1).replace(/[^0-9]/g, "");
  const decPart = decPartRaw.padEnd(2, "0").slice(0, 2);
  return `${intPart}.${decPart}`;
}

function onPrecoInput(e) {
  form.precoTexto = e.target.value;
  form.preco = parsePrecoFlex(form.precoTexto);
}

function handleFileChange(e) {
  const files = Array.from(e.target.files || []);
  form.imagens = files;
  form.imagem = files[0] || null; // compatibilidade com backend atual
}

async function salvarAlteracoes() {
  const formData = new FormData();
  // Campos básicos do patch DTO
  Object.entries({
    nome: form.nome,
    descricao: form.descricao,
    preco: String(form.preco || "").replace(".", ","),
    codigoDeBarra: form.codigoDeBarra,
    cor: form.cor,
    tamanho: form.tamanho,
    CategoriaId: form.categoriaId || undefined,
  }).forEach(([key, val]) => {
    if (val !== undefined && val !== null && val !== "") formData.append(key, val);
  });
  // Imagem nova deve ser enviada como 'novaImagem'
  if (form.imagem) {
    formData.append("novaImagem", form.imagem);
  }
  await salvarProduto(props.produto.id, formData);
  emit("fechar");
  emit("atualizar");
}
</script>
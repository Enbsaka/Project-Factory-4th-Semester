<template>
  <BaseModal
    titulo="Novo Produto"
    textoConfirmar="Criar"
    @confirmar="criarProduto"
    @fechar="$emit('fechar')"
  >
    <form @submit.prevent="criarProduto" class="space-y-4">
      <div>
        <label class="block text-sm font-medium text-gray-700">Nome</label>
        <input
          v-model="novo.nome"
          type="text"
          class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
        />
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700">Descrição</label>
        <textarea
          v-model="novo.descricao"
          rows="2"
          class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
        ></textarea>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700">Código de Barra</label>
        <input
          v-model="novo.codigoDeBarra"
          type="text"
          class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
        />
      </div>

      <div class="flex gap-3">
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Preço</label>
          <input
            :value="novo.precoTexto"
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
            v-model="novo.categoriaId"
            class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
          >
            <option disabled value="">Selecione uma subcategoria</option>
            <option v-for="opt in leafOptions" :key="opt.id" :value="opt.id">{{ opt.label }}</option>
          </select>
        </div>
      </div>

      <div class="flex gap-3">
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Cor</label>
          <input
            v-model="novo.cor"
            type="text"
            class="w-full border rounded-lg px-3 py-2 mt-1 focus:ring-2 focus:ring-blue-400 focus:outline-none"
          />
        </div>
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700">Tamanho</label>
          <input
            v-model="novo.tamanho"
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
import { useNotifications } from "../composables/useNotifications.js";

const emit = defineEmits(["fechar", "criado"]);
const { categorias, carregarProdutos } = useProducts();
const { show } = useNotifications();
import produto from "../../../services/productService.js";

const novo = reactive({
  codigoDeBarra: "",
  nome: "",
  descricao: "",
  preco: "",
  precoTexto: "",
  categoriaId: "",
  cor: "",
  tamanho: "",
  imagem: null,
  imagens: [],
});

function handleFileChange(e) {
  const files = Array.from(e.target.files || []);
  novo.imagens = files;
  novo.imagem = files[0] || null;
}

const leafOptions = computed(() => {
  const res = [];
  function walk(node, path) {
    const id = node.id ?? node.Id;
    const nome = node.nome ?? node.Nome;
    const subs = node.subcategorias ?? node.Subcategorias ?? [];
    const nextPath = path ? `${path} > ${nome}` : nome;
    if (!subs || subs.length === 0) {
      res.push({ id, label: nextPath });
      return;
    }
    subs.forEach((child) => walk(child, nextPath));
  }
  (categorias.value || []).forEach((c) => walk(c, ""));
  return res;
});

function parsePrecoFlex(text) {
  const s = String(text || "").replace(/\s+/g, "").replace(/[^0-9.,]/g, "");
  if (!s) return "";
  const lastComma = s.lastIndexOf(",");
  const lastDot = s.lastIndexOf(".");
  const sepIdx = Math.max(lastComma, lastDot);
  if (sepIdx < 0) return s.replace(/[^0-9]/g, "");
  const intPart = s.slice(0, sepIdx).replace(/[^0-9]/g, "");
  const decPartRaw = s.slice(sepIdx + 1).replace(/[^0-9]/g, "");
  const decPart = decPartRaw.padEnd(2, "0").slice(0, 2);
  return `${intPart}.${decPart}`;
}

function onPrecoInput(e) {
  novo.precoTexto = e.target.value;
  novo.preco = parsePrecoFlex(novo.precoTexto);
}

async function criarProduto() {
  const formData = new FormData();

  // ⚙️ Campos do ProdutoDTO (devem ter os mesmos nomes das propriedades do DTO em C#)
  formData.append("nome", novo.nome);
  formData.append("descricao", novo.descricao);
  formData.append("codigoDeBarra", novo.codigoDeBarra);
  const precoServidor = String(novo.preco || "").replace(".", ",");
  formData.append("preco", precoServidor);
  formData.append("categoriaId", novo.categoriaId);

  // 🖼️ Arquivo separado do DTO
  if (novo.imagem) {
    formData.append("imagem", novo.imagem);
  }
  if (Array.isArray(novo.imagens)) {
    novo.imagens.slice(1).forEach((f) => formData.append("imagens", f));
  }

  try {
    await produto.create(formData);
    await carregarProdutos();
    emit("fechar");
    emit("criado");
    show("Produto criado com sucesso!", "success");
  } catch (err) {
    console.error("Erro ao criar produto:", err);
    show("Erro ao criar produto.", "error");
  }
}

</script>



<template>
  <BaseModal
    titulo="Variações do Produto"
    textoConfirmar="Salvar"
    :mostrarAcoes="false"
    @fechar="$emit('fechar')"
  >
    <div>
      <h3 class="text-gray-700 font-medium mb-3">
        {{ produto.nome }}
      </h3>

      <ul class="space-y-2 mb-4">
        <li
          v-for="v in variacoes"
          :key="v.id"
          class="border rounded-lg px-4 py-2 text-gray-700"
        >
          <div class="flex justify-between items-center">
            <span>{{ v.nome }} — {{ v.preco }}</span>
            <div class="flex gap-2">
              <button @click="iniciarEdicao(v)" class="text-blue-600 hover:underline text-sm">Editar</button>
            </div>
          </div>
          <div v-if="editandoId === v.id" class="mt-3 grid grid-cols-1 md:grid-cols-2 gap-3">
            <input v-model="edicao.nome" placeholder="Nome" class="border rounded-lg px-3 py-2" />
            <input :value="edicao.precoTexto" @input="onEdicaoPrecoInput" type="text" inputmode="decimal" placeholder="Preço" class="border rounded-lg px-3 py-2" />
            <input v-model="edicao.cor" placeholder="Cor" class="border rounded-lg px-3 py-2" />
            <input v-model="edicao.tamanho" placeholder="Tamanho" class="border rounded-lg px-3 py-2" />
            <input type="file" @change="handleEdicaoFileChange" class="text-gray-600" />
            <div class="col-span-1 md:col-span-2 flex gap-2 justify-end">
              <button @click="salvarEdicao(v.id)" class="bg-[#141A7C] text-white px-3 py-2 rounded hover:bg-[#0f166a] text-sm">Salvar</button>
              <button @click="cancelarEdicao" class="bg-gray-200 text-gray-700 px-3 py-2 rounded hover:bg-gray-300 text-sm">Cancelar</button>
            </div>
          </div>
        </li>
      </ul>

      <form @submit.prevent="criarVariacao" class="space-y-3">
        <div class="text-sm text-gray-500">
          Nome da variação herdará do produto: <span class="font-medium text-gray-700">{{ produto.nome }}</span>
        </div>
        <input
          :value="nova.precoTexto"
          @input="onPrecoInput"
          type="text"
          inputmode="decimal"
          placeholder="Ex.: 5,919.99 ou 5.919,99 (aceita R$ 239,99)"
          class="w-full border rounded-lg px-3 py-2"
        />
        <input
          v-model="nova.cor"
          placeholder="Cor"
          class="w-full border rounded-lg px-3 py-2"
        />
        <input
          v-model="nova.tamanho"
          placeholder="Tamanho"
          class="w-full border rounded-lg px-3 py-2"
        />
        <input type="file" @change="handleFileChange" class="text-gray-600" />
        <button
          type="submit"
          class="bg-[#141A7C] text-white px-4 py-2 rounded-lg hover:bg-[#0f166a] transition w-full"
        >
          Adicionar variação
        </button>
      </form>
    </div>
  </BaseModal>
</template>

<script setup>
import { reactive, ref, onMounted } from "vue";
import BaseModal from "./BaseModal.vue";
import productService from "../../../services/productService.js";

const props = defineProps({ produto: Object });
const variacoes = ref([]);
const nova = reactive({ preco: "", precoTexto: "", cor: "", tamanho: "", imagem: null });
const editandoId = ref(null);
const edicao = reactive({ nome: "", preco: "", precoTexto: "", cor: "", tamanho: "", novaImagem: null });

function handleFileChange(e) {
  nova.imagem = e.target.files[0] || null;
}

function handleEdicaoFileChange(e) {
  edicao.novaImagem = e.target.files[0] || null;
}

function parsePrecoFlex(text) {
  const s = String(text || "").replace(/\s+/g, "").replace(/[^0-9.,]/g, "");
  if (!s) return "";
  const lastComma = s.lastIndexOf(",");
  const lastDot = s.lastIndexOf(".");
  let sepIdx = Math.max(lastComma, lastDot);
  if (sepIdx < 0) {
    return s.replace(/[^0-9]/g, "");
  }
  const intPart = s.slice(0, sepIdx).replace(/[^0-9]/g, "");
  const decPartRaw = s.slice(sepIdx + 1).replace(/[^0-9]/g, "");
  const decPart = decPartRaw.padEnd(2, "0").slice(0, 2);
  return `${intPart}.${decPart}`;
}

function onPrecoInput(e) {
  nova.precoTexto = e.target.value;
  nova.preco = parsePrecoFlex(nova.precoTexto);
}

function onEdicaoPrecoInput(e) {
  edicao.precoTexto = e.target.value;
  edicao.preco = parsePrecoFlex(edicao.precoTexto);
}

async function carregarVariacoes() {
  try {
    const data = await productService.getById(props.produto.id);
    const list = Array.isArray(data.variacoes) ? data.variacoes : [];
    variacoes.value = list.map((p) => ({
      id: p.id,
      nome: p.nome,
      preco: p.preco
        ? `R$ ${Number(p.preco).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
        : "R$ 0,00",
      cor: p.cor,
      tamanho: p.tamanho,
      imagem: p.imagemURL || "https://via.placeholder.com/50",
    }));
  } catch (e) {
    console.error(e);
  }
}

function gerarCodigoDeBarra() {
  // Gera um código numérico simples (13 dígitos) – pode ser ajustado
  const base = Date.now().toString();
  const rand = Math.floor(Math.random() * 1e6).toString().padStart(6, "0");
  return (base + rand).slice(0, 13);
}

async function criarVariacao() {
  try {
    if (!props.produto || !props.produto.id) {
      console.error("Produto não selecionado para criar variação.");
      return;
    }
    const formData = new FormData();
    // Herdar dados obrigatórios do produto pai
    formData.append("nome", String(props.produto.nome || ""));
    formData.append("descricao", String(props.produto.descricao || ""));
    if (props.produto.categoriaId) formData.append("CategoriaId", String(props.produto.categoriaId));
    // Preço da variação (se informado) — enviar com vírgula para pt-BR
    if (nova.preco) {
      const precoServidor = String(nova.preco).replace(".", ",");
      formData.append("preco", precoServidor);
    }
    if (nova.cor) formData.append("cor", String(nova.cor));
    if (nova.tamanho) formData.append("tamanho", String(nova.tamanho));
    // Código de barras obrigatório – gerado automaticamente
    formData.append("codigoDeBarra", gerarCodigoDeBarra());
    formData.append("produtoPaiId", props.produto.id);
    if (nova.imagem) formData.append("imagem", nova.imagem);
    await productService.create(formData);
    // Reset e recarrega
    nova.preco = ""; nova.precoTexto = ""; nova.cor = ""; nova.tamanho = ""; nova.imagem = null;
    await carregarVariacoes();
  } catch (e) {
    console.error(e);
  }
}

function iniciarEdicao(v) {
  editandoId.value = v.id;
  edicao.nome = v.nome || "";
  // v.preco está em formato "R$ xx,yy" na lista; preferir buscar do backend ao recarregar ou usar parse simples
  edicao.precoTexto = "";
  edicao.preco = "";
  edicao.cor = v.cor || "";
  edicao.tamanho = v.tamanho || "";
  edicao.novaImagem = null;
}

function cancelarEdicao() {
  editandoId.value = null;
  edicao.nome = "";
  edicao.preco = "";
  edicao.precoTexto = "";
  edicao.cor = "";
  edicao.tamanho = "";
  edicao.novaImagem = null;
}

async function salvarEdicao(variacaoId) {
  try {
    const formData = new FormData();
    if (edicao.nome) formData.append("nome", edicao.nome);
    if (edicao.preco) {
      const precoServidor = String(edicao.preco).replace(".", ",");
      formData.append("preco", precoServidor);
    }
    if (edicao.cor) formData.append("cor", edicao.cor);
    if (edicao.tamanho) formData.append("tamanho", edicao.tamanho);
    if (edicao.novaImagem) formData.append("novaImagem", edicao.novaImagem);
    await productService.update(variacaoId, formData);
    cancelarEdicao();
    await carregarVariacoes();
  } catch (e) {
    console.error(e);
  }
}

onMounted(() => {
  carregarVariacoes();
});
</script>

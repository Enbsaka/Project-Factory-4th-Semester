<template>
  <div class="relative">
    <button @click="onBuyNow" class="mt-4 w-full flex items-center justify-center gap-2 bg-white border border-[#141A7C] text-[#141A7C] py-2 rounded-md hover:bg-indigo-800 hover:text-white transition">
      Comprar Agora
    </button>
    <div v-if="showPrompt" class="absolute left-0 right-0 -bottom-14 mx-auto w-full max-w-sm bg-[#E5EBFF] text-[#0B1739] text-sm p-3 rounded-md shadow">
      Selecione cor e tamanho ou defina quantidades por variação.
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { computed, ref } from 'vue'
import { carrinhoService } from '../../services/carrinhoService'
import clienteService from '../../services/clienteService'
import productService from '../../services/productService'

const props = defineProps({
  produto: { type: Object, required: true },
  quantidade: { type: Number, default: 1 },
  corSelecionada: { type: String, default: null },
  tamanhoSelecionado: { type: String, default: null },
  variacaoQuantidades: { type: Array, default: () => [] }
})

const router = useRouter()
const isLoggedIn = computed(() => !!localStorage.getItem('token'))
const showPrompt = ref(false)

async function onBuyNow() {
  if (!isLoggedIn.value) {
    router.push('/login')
    return
  }
  // Exigir seleção de variação quando houver
  const exigeCor = !!props.produto?.cor || (Array.isArray(props.produto?.variacoes) && props.produto.variacoes.some(v => v.cor))
  const exigeTam = !!props.produto?.tamanho || (Array.isArray(props.produto?.variacoes) && props.produto.variacoes.some(v => v.tamanho))
  const temMatriz = Array.isArray(props.variacaoQuantidades) && props.variacaoQuantidades.some(v => Number(v.quantidade || 0) > 0)
  if (((exigeCor && !props.corSelecionada) || (exigeTam && !props.tamanhoSelecionado)) && !temMatriz) {
    showPrompt.value = true
    setTimeout(() => { showPrompt.value = false }, 2500)
    return
  }
  try {
    const perfil = await clienteService.getMeuPerfil()
    const clienteId = perfil?.id ?? perfil?.Id
    const cpf = perfil?.cpf ?? perfil?.Cpf
    if (!clienteId || !cpf) {
      router.push('/login')
      return
    }
    const carrinho = await carrinhoService.getCarrinho(clienteId)
    const pedidoId = carrinho?.Id ?? carrinho?.id
    const itens = carrinho?.Produtos ?? carrinho?.produtos ?? []

    const existentes = new Map()
    for (const item of itens) {
      const produtoId = item.ProdutoId ?? item.produtoId
      if (!produtoId) continue
      try {
        const detalhe = await productService.getById(produtoId)
        const barcode = detalhe?.codigoDeBarra ?? detalhe?.CodigoDeBarra
        if (barcode) {
          const qtd = item.Quantidade ?? item.quantidade ?? 1
          existentes.set(barcode, (existentes.get(barcode) || 0) + qtd)
        }
      } catch {}
    }
    const novosItens = []
    if (temMatriz) {
      for (const v of props.variacaoQuantidades) {
        const q = Number(v?.quantidade || 0)
        const barcode = v?.codigoDeBarra || v?.CodigoDeBarra
        if (q > 0 && barcode) novosItens.push({ CodigoDeBarra: barcode, Quantidade: q })
      }
    } else {
      const codigoNovo = props.produto.codigoDeBarra ?? props.produto.CodigoDeBarra
      if (!codigoNovo) return
      const quantidadeNum = Number(props.quantidade ?? 1)
      novosItens.push({ CodigoDeBarra: codigoNovo, Quantidade: isNaN(quantidadeNum) ? 1 : quantidadeNum })
    }

    for (const { CodigoDeBarra, Quantidade } of novosItens) {
      existentes.set(CodigoDeBarra, (existentes.get(CodigoDeBarra) || 0) + Quantidade)
    }

    const atualizados = Array.from(existentes.entries()).map(([CodigoDeBarra, Quantidade]) => ({ CodigoDeBarra, Quantidade }))
    await carrinhoService.atualizarCarrinho(pedidoId, atualizados, cpf)
    router.push('/carrinho/checkout')
  } catch (e) {
    console.error('Erro ao comprar agora:', e)
  }
}
</script>
<template>
  <button @click="onAddToCart" :disabled="disabled" class="mt-4 w-full flex items-center justify-center gap-2 bg-[#141A7C] text-white py-2 rounded-md hover:bg-indigo-800 transition disabled:opacity-50 disabled:cursor-not-allowed">
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
      <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437m0 0L6.75 14.25A2.25 2.25 0 0 0 9 16.5h9.75a2.25 2.25 0 0 0 2.212-1.791l1.263-7.043A1.125 1.125 0 0 0 21.113 6H5.106m0 0L4.5 3.75M9 20.25a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Zm10.5 0a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
    </svg>
    Adicionar ao Carrinho
  </button>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { computed } from 'vue'
import { carrinhoService } from '../../services/carrinhoService'
import clienteService from '../../services/clienteService'
import productService from '../../services/productService'

const props = defineProps({
  produto: { type: Object, required: true },
  quantidade: { type: Number, default: 1 },
  corSelecionada: { type: String, default: null },
  tamanhoSelecionado: { type: String, default: null },
  variacaoQuantidades: { type: Array, default: () => [] },
  disabled: { type: Boolean, default: false }
})

const router = useRouter()
const isLoggedIn = computed(() => !!localStorage.getItem('token'))

async function onAddToCart() {
  if (!isLoggedIn.value) {
    router.push('/login')
    return
  }

  // Exigir seleção de variação quando houver, a menos que existam quantidades por variação
  const exigeCor = !!props.produto?.cor || (Array.isArray(props.produto?.variacoes) && props.produto.variacoes.some(v => v.cor))
  const exigeTam = !!props.produto?.tamanho || (Array.isArray(props.produto?.variacoes) && props.produto.variacoes.some(v => v.tamanho))
  const temMatriz = Array.isArray(props.variacaoQuantidades) && props.variacaoQuantidades.some(v => Number(v.quantidade || 0) > 0)
  if (((exigeCor && !props.corSelecionada) || (exigeTam && !props.tamanhoSelecionado)) && !temMatriz) {
    alert('Selecione a variação ou defina quantidades por variação para adicionar ao carrinho.')
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
    // Monta itens novos: matriz de variações ou variação selecionada atual
    const novosItens = []
    if (temMatriz) {
      for (const v of props.variacaoQuantidades) {
        const q = Number(v?.quantidade || 0)
        const barcode = v?.codigoDeBarra || v?.CodigoDeBarra
        if (q > 0 && barcode) novosItens.push({ CodigoDeBarra: barcode, Quantidade: q })
      }
    } else {
      const codigoNovo = props.produto.codigoDeBarra ?? props.produto.CodigoDeBarra
      if (!codigoNovo) {
        console.error('Produto sem código de barra')
        return
      }
      const quantidadeNum = Number(props.quantidade ?? 1)
      novosItens.push({ CodigoDeBarra: codigoNovo, Quantidade: isNaN(quantidadeNum) ? 1 : quantidadeNum })
    }

    // Soma novos itens aos existentes
    for (const { CodigoDeBarra, Quantidade } of novosItens) {
      existentes.set(CodigoDeBarra, (existentes.get(CodigoDeBarra) || 0) + Quantidade)
    }

    const atualizados = Array.from(existentes.entries()).map(([CodigoDeBarra, Quantidade]) => ({ CodigoDeBarra, Quantidade }))

    await carrinhoService.atualizarCarrinho(pedidoId, atualizados, cpf)
  } catch (e) {
    console.error('Erro ao adicionar ao carrinho:', e)
  }
}
</script>
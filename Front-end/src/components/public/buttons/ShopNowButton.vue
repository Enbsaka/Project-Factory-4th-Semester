<template>
  <div class="relative">
    <button @click="onBuyNow" class="w-full h-11 flex items-center justify-center gap-2 bg-white border border-[#141A7C] text-[#141A7C] text-sm font-medium rounded-md hover:bg-[#E5EBFF] transition">
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
import { carrinhoService } from '../../../services/carrinhoService'
import clienteService from '../../../services/clienteService'
import productService from '../../../services/productService'

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
  const hasVariacoes = Array.isArray(props.produto?.variacoes) && props.produto.variacoes.length > 0
  const exigeCor = hasVariacoes && props.produto.variacoes.some(v => v.cor)
  const exigeTam = hasVariacoes && props.produto.variacoes.some(v => v.tamanho)
  const temMatriz = Array.isArray(props.variacaoQuantidades) && props.variacaoQuantidades.some(v => Number(v.quantidade || 0) > 0)
  if (hasVariacoes && ((exigeCor && !props.corSelecionada) || (exigeTam && !props.tamanhoSelecionado)) && !temMatriz) {
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
      const pid = item?.ProdutoId ?? item?.produtoId ?? null
      const barcode = item?.CodigoDeBarra ?? item?.codigoDeBarra ?? null
      const qtd = item?.Quantidade ?? item?.quantidade ?? 1
      const key = pid ? `pid:${pid}` : (barcode ? `ean:${barcode}` : null)
      if (!key) continue
      const prev = existentes.get(key)
      const merged = {
        ProdutoId: pid ?? (prev?.ProdutoId ?? null),
        CodigoDeBarra: barcode ?? (prev?.CodigoDeBarra ?? null),
        Quantidade: (prev?.Quantidade ?? 0) + qtd
      }
      existentes.set(key, merged)
    }
    const novosItens = []
    if (temMatriz) {
      for (const v of props.variacaoQuantidades) {
        const q = Number(v?.quantidade || 0)
        let barcode = v?.codigoDeBarra || v?.CodigoDeBarra
        if (!barcode && (v?.produtoId || v?.ProdutoId)) {
          try {
            const detalhe = await productService.getById(v?.produtoId || v?.ProdutoId)
            barcode = detalhe?.codigoDeBarra || detalhe?.CodigoDeBarra || null
          } catch {}
        }
        if (q > 0 && (barcode || (v?.produtoId || v?.ProdutoId))) {
          novosItens.push({ ProdutoId: v?.produtoId || v?.ProdutoId || null, CodigoDeBarra: barcode || null, Quantidade: q })
        }
      }
    } else {
      let codigoNovo = props.produto.codigoDeBarra ?? props.produto.CodigoDeBarra
      if (!codigoNovo) {
        try {
          const detalhe = await productService.getById(props.produto.id || props.produto.Id)
          codigoNovo = detalhe?.codigoDeBarra ?? detalhe?.CodigoDeBarra ?? null
        } catch {}
        if (!codigoNovo && !(props.produto.id || props.produto.Id)) return
      }
      const quantidadeNum = Number(props.quantidade ?? 1)
      novosItens.push({ ProdutoId: props.produto.id || props.produto.Id || null, CodigoDeBarra: codigoNovo || null, Quantidade: isNaN(quantidadeNum) ? 1 : quantidadeNum })
    }

    for (const novo of novosItens) {
      const key = novo.ProdutoId ? `pid:${novo.ProdutoId}` : (novo.CodigoDeBarra ? `ean:${novo.CodigoDeBarra}` : null)
      if (!key) continue
      const prev = existentes.get(key)
      const merged = {
        ProdutoId: novo.ProdutoId ?? (prev?.ProdutoId ?? null),
        CodigoDeBarra: novo.CodigoDeBarra ?? (prev?.CodigoDeBarra ?? null),
        Quantidade: (prev?.Quantidade ?? 0) + (novo.Quantidade ?? 0)
      }
      existentes.set(key, merged)
    }

    const atualizados = Array.from(existentes.values()).map(v => ({ ProdutoId: v.ProdutoId, CodigoDeBarra: v.CodigoDeBarra, Quantidade: v.Quantidade }))
    await carrinhoService.atualizarCarrinho(pedidoId, atualizados, cpf)
    const carrinhoAtual = await carrinhoService.getCarrinho(clienteId)
    const itensCarrinho = carrinhoAtual?.Produtos ?? carrinhoAtual?.produtos ?? []
    if ((itensCarrinho?.length ?? 0) === 0) {
      alert('Não foi possível adicionar o produto ao carrinho. Tente novamente.')
      return
    }
    window.dispatchEvent(new CustomEvent('cart:updated'))
    router.push('/carrinho/checkout')
  } catch (e) {
    const msg = e?.response?.data || e?.message || 'Falha ao atualizar carrinho'
    alert(String(msg))
  }
}
</script>
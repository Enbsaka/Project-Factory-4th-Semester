<template>
  <div>
    <AdminButton variant="primary" size="md" @click="abrir = true">
      <template #icon>
        <BadgePlus class="w-4 h-4" />
      </template>
      Nova Categoria
    </AdminButton>

    <div v-if="abrir" class="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-lg w-full max-w-md p-5">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Criar Categoria</h3>
        <form @submit.prevent="criar">
          <div class="mb-3">
            <label class="block text-sm text-gray-700 mb-1">Nome</label>
            <input v-model="form.nome" type="text" class="w-full border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400" required />
          </div>

          <div class="mb-4">
            <label class="block text-sm text-gray-700 mb-1">Categoria Pai (opcional)</label>
            <select v-model="form.categoriaPaiId" class="w-full border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400">
              <option :value="''">Sem categoria pai</option>
              <option v-for="c in categorias" :key="c.id" :value="c.id">{{ c.nomeCategoria }}</option>
            </select>
          </div>

          <div class="flex justify-end gap-2">
            <button type="button" class="px-3 py-2 bg-gray-100 text-gray-700 rounded-md" @click="fechar">Cancelar</button>
            <button type="submit" class="px-4 py-2 bg-[#141A7C] hover:bg-[#0f166a] text-white rounded-md disabled:opacity-60" :disabled="salvando">
              {{ salvando ? 'Salvando...' : 'Criar' }}
            </button>
          </div>
        </form>
        <p v-if="erro" class="text-red-600 text-sm mt-3">{{ erro }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import categoriaService from '../../../services/categoriaService.js'
import AdminButton from '../components/AdminButton.vue'
import { BadgePlus } from 'lucide-vue-next'

const abrir = ref(false)
const categorias = ref([])
const salvando = ref(false)
const erro = ref('')
const form = ref({ nome: '', categoriaPaiId: '' })

const emit = defineEmits(['criada'])

async function carregarCategorias() {
  categorias.value = await categoriaService.getAll()
}

function fechar() {
  abrir.value = false
  erro.value = ''
  form.value = { nome: '', categoriaPaiId: '' }
}

async function criar() {
  erro.value = ''
  salvando.value = true
  try {
    const dto = { nome: form.value.nome }
    if (form.value.categoriaPaiId) dto.categoriaPaiId = form.value.categoriaPaiId
    await categoriaService.create(dto)
    emit('criada')
    fechar()
  } catch (e) {
    erro.value = 'Falha ao criar categoria.'
    console.error('Erro ao criar categoria', e)
  } finally {
    salvando.value = false
  }
}

onMounted(carregarCategorias)
</script>

<style scoped>
</style>
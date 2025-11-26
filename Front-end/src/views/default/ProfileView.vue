<template>
  <section class="max-w-3xl mx-auto px-6 py-10">
    <h1 class="text-2xl font-bold mb-6">Meu Perfil</h1>

    <div v-if="loading" class="text-gray-600">Carregando...</div>
    <div v-else>
      <form @submit.prevent="onSave" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700">Nome</label>
          <input v-model="form.nome" type="text" class="mt-1 w-full border rounded-md px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">Email</label>
          <input v-model="form.email" type="email" class="mt-1 w-full border rounded-md px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">Nova Senha</label>
          <input v-model="form.senha" type="password" class="mt-1 w-full border rounded-md px-3 py-2" placeholder="Opcional" />
        </div>

        <div class="flex gap-3">
  <button type="submit" class="px-4 py-2 bg-[#141A7C] text-white rounded-md hover:bg-[#0f166a]">Salvar</button>
          <button type="button" @click="onLogout" class="px-4 py-2 bg-gray-200 text-gray-900 rounded-md hover:bg-gray-300">Desconectar</button>
        </div>
      </form>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import clienteService from '../../services/clienteService'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(true)
const form = ref({ id: null, nome: '', email: '', senha: '' })

onMounted(async () => {
  try {
    const me = await clienteService.getMeuPerfil()
    form.value.id = me?.id
    form.value.nome = me?.nome || ''
    form.value.email = me?.email || ''
  } catch (e) {
    // se falhar, volta para login
    router.push({ name: 'loginCliente' })
  } finally {
    loading.value = false
  }
})

async function onSave() {
  if (!form.value.id) return
  const payload = {}
  if (form.value.nome) payload.nome = form.value.nome
  if (form.value.email) payload.email = form.value.email
  if (form.value.senha) payload.senha = form.value.senha
  try {
    await clienteService.updateCliente(form.value.id, payload)
    alert('Perfil atualizado com sucesso!')
  } catch (e) {
    alert('Falha ao atualizar perfil')
  }
}

function onLogout() {
  localStorage.removeItem('token')
  router.push({ name: 'loginCliente' })
}
</script>

<style scoped>
</style>
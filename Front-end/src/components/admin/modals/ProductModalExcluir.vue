<template>
  <BaseModal
    titulo="Excluir Produto"
    textoConfirmar="Excluir"
    @confirmar="confirmar"
    @fechar="$emit('fechar')"
  >
    <p class="text-gray-600">
      Deseja realmente excluir o produto
      <b class="text-gray-800">{{ produto.nome }}</b>?
    </p>
  </BaseModal>
</template>

<script setup>
import BaseModal from "./BaseModal.vue";
const props = defineProps({ produto: Object });
const emit = defineEmits(["fechar", "confirmar"]);

function confirmar() {
  emit("confirmar", props.produto.id);
}
</script>

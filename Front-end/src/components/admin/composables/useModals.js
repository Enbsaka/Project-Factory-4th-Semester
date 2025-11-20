import { ref } from "vue";

export function useModals() {
  const modalEditarAberto = ref(false);
  const modalExcluirAberto = ref(false);
  const modalVariacoesAberto = ref(false);
  const modalNovoAberto = ref(false);
  const produtoSelecionado = ref(null);

  function abrir(tipo, produto = null) {
    produtoSelecionado.value = produto;
    if (tipo === "editar") modalEditarAberto.value = true;
    if (tipo === "excluir") modalExcluirAberto.value = true;
    if (tipo === "variacoes") modalVariacoesAberto.value = true;
    if (tipo === "novo") modalNovoAberto.value = true;
  }

  function fecharTodos() {
    modalEditarAberto.value = false;
    modalExcluirAberto.value = false;
    modalVariacoesAberto.value = false;
    modalNovoAberto.value = false;
  }

  return {
    modalEditarAberto,
    modalExcluirAberto,
    modalVariacoesAberto,
    modalNovoAberto,
    produtoSelecionado,
    abrir,
    fecharTodos,
  };
}

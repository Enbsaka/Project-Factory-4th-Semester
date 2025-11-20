import { ref } from "vue";

const notificacao = ref({ aberta: false, tipo: "", mensagem: "" });

export function useNotifications() {
  function show(mensagem, tipo = "success") {
    notificacao.value = { aberta: true, tipo, mensagem };
    setTimeout(() => (notificacao.value.aberta = false), 2500);
  }
  return { notificacao, show };
}

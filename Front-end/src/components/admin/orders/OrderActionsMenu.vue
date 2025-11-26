<template>
  <div class="relative inline-block text-left" @click.stop>
    <button
      type="button"
      class="p-2 rounded hover:bg-gray-100 text-gray-600 hover:text-blue-600 transition"
      @click="aberto = !aberto"
      aria-label="Mais ações"
    >
      <CircleEllipsis class="w-6 h-6" />
    </button>
    <div
      v-if="aberto"
      class="absolute right-6 top-10 bg-white border border-gray-200 rounded-lg shadow-lg w-44 z-10"
    >
      <ul class="text-sm text-gray-700">
        <li>
          <button
            class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100"
            @click="onEditar"
          >
            <Pencil class="w-4 h-4" /> Editar
          </button>
        </li>
        <li>
          <button
            class="flex items-center gap-2 w-full text-left px-4 py-2 text-red-600 hover:bg-gray-100"
            @click="onExcluir"
          >
            <Trash class="w-4 h-4" /> Excluir
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue";
import { CircleEllipsis, Pencil, Trash } from "lucide-vue-next";
const props = defineProps({ pedido: { type: Object, required: true } });
const emit = defineEmits(["editar", "excluir"]);
const aberto = ref(false);

function onEditar() {
  emit("editar", props.pedido);
  aberto.value = false;
}
function onExcluir() {
  emit("excluir", props.pedido);
  aberto.value = false;
}

function onClickOutside(e) {
  const el = e.target.closest(".relative.inline-block.text-left");
  if (!el) aberto.value = false;
}

onMounted(() => document.addEventListener("click", onClickOutside));
onBeforeUnmount(() => document.removeEventListener("click", onClickOutside));
</script>
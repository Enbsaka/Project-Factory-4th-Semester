<template>
  <div class="relative inline-block text-left">
    <button
      @click.stop="toggle"
      class="p-2 rounded hover:bg-gray-100 text-gray-600 hover:text-blue-600 transition"
      aria-label="Mais ações"
    >
      <CircleEllipsis class="w-6 h-6" />
    </button>
    <div v-if="aberto" class="absolute right-0 mt-2 bg-white border border-gray-200 rounded-lg shadow-lg w-44 z-10">
      <ul class="text-sm text-gray-700">
        <li>
          <button @click="onEditar" class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100">
            <Pencil class="w-4 h-4" /> Editar
          </button>
        </li>
        <li>
          <button @click="onDetalhar" class="flex items-center gap-2 w-full text-left px-4 py-2 hover:bg-gray-100">
            <View class="w-4 h-4" /> Detalhar
          </button>
        </li>
        <li>
          <button @click="onExcluir" class="flex items-center gap-2 w-full text-left px-4 py-2 text-red-600 hover:bg-gray-100">
            <Trash class="w-4 h-4" /> Excluir
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue";
import { CircleEllipsis, Pencil, View, Trash } from "lucide-vue-next";
const props = defineProps({ cliente: { type: Object, required: true } });
const emit = defineEmits(["editar", "excluir", "detalhar"]);
const aberto = ref(false);
function toggle() { aberto.value = !aberto.value; }
function close() { aberto.value = false; }
function onEditar() { emit("editar", props.cliente); close(); }
function onExcluir() { emit("excluir", props.cliente); close(); }
function onDetalhar() { emit("detalhar", props.cliente); close(); }

function handleOutside(e) {
  const el = e.target.closest(".relative.inline-block.text-left");
  if (!el) close();
}
onMounted(() => document.addEventListener("click", handleOutside));
onBeforeUnmount(() => document.removeEventListener("click", handleOutside));
</script>
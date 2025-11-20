<template>
  <div class="flex flex-col gap-2">
    <div class="flex items-center gap-3 bg-white border border-gray-200 rounded-full px-4 py-2 shadow-sm">
      <Calendar class="w-5 h-5 text-gray-600" />
      <span class="text-xs font-medium text-gray-500 select-none">Período</span>
      <input
        :value="displayInicio"
        @input="onDisplayInicio($event.target.value)"
        @keydown="maskKeydown($event)"
        inputmode="numeric"
        placeholder="dd/mm/aaaa"
        class="flex-1 bg-transparent outline-none text-sm text-gray-800 placeholder:text-gray-400"
        aria-label="Data inicial"
      />
      <span class="text-gray-300">—</span>
      <input
        :value="displayFim"
        @input="onDisplayFim($event.target.value)"
        @keydown="maskKeydown($event)"
        inputmode="numeric"
        placeholder="dd/mm/aaaa"
        class="flex-1 bg-transparent outline-none text-sm text-gray-800 placeholder:text-gray-400"
        aria-label="Data final"
      />
    </div>
    <div class="flex items-center gap-2">
      <button type="button" class="text-xs px-3 py-1.5 rounded-full border border-gray-200 hover:border-blue-500 hover:text-blue-600" @click="pickHoje">Hoje</button>
      <button type="button" class="text-xs px-3 py-1.5 rounded-full border border-gray-200 hover:border-blue-500 hover:text-blue-600" @click="pick7">7 dias</button>
      <button type="button" class="text-xs px-3 py-1.5 rounded-full border border-gray-200 hover:border-blue-500 hover:text-blue-600" @click="pick30">30 dias</button>
      <button type="button" class="text-xs px-3 py-1.5 rounded-full border border-gray-200 hover:border-blue-500 hover:text-blue-600" @click="pickMes">Este mês</button>
      <button type="button" class="ml-auto text-xs px-3 py-1.5 rounded-full border border-gray-200 hover:bg-gray-50" @click="limpar">Limpar</button>
    </div>
  </div>
  
</template>

<script setup>
import { ref, watch } from "vue";
import { Calendar } from "lucide-vue-next";
const props = defineProps({
  dataInicio: { type: String, default: "" },
  dataFim: { type: String, default: "" },
});
const emit = defineEmits(["update:dataInicio", "update:dataFim", "buscar"]);

const displayInicio = ref(isoToDisplay(props.dataInicio || ""));
const displayFim = ref(isoToDisplay(props.dataFim || ""));

function isoToDisplay(iso) {
  if (!iso) return "";
  const [y, m, d] = iso.split("-");
  if (!y || !m || !d) return "";
  return `${d.padStart(2, "0")}/${m.padStart(2, "0")}/${y}`;
}

function displayToIso(display) {
  const parts = display.split("/");
  if (parts.length !== 3) return "";
  const [dd, mm, yyyy] = parts;
  const d = Number(dd), m = Number(mm), y = Number(yyyy);
  if (!d || !m || !y) return "";
  const date = new Date(y, m - 1, d);
  if (date.getFullYear() !== y || date.getMonth() !== m - 1 || date.getDate() !== d) return "";
  return `${String(y).padStart(4, "0")}-${String(m).padStart(2, "0")}-${String(d).padStart(2, "0")}`;
}

function mask(raw) {
  const digits = raw.replace(/\D/g, "").slice(0, 8);
  const p1 = digits.slice(0, 2);
  const p2 = digits.slice(2, 4);
  const p3 = digits.slice(4, 8);
  let out = p1;
  if (p2) out += `/${p2}`;
  if (p3) out += `/${p3}`;
  return out;
}

function maskKeydown(e) {
  // Allow digits, backspace, delete, arrows, tab
  const allowed = ["Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab"]; 
  if (allowed.includes(e.key)) return; 
  if (!/^[0-9]$/.test(e.key)) {
    e.preventDefault();
  }
}

function onDisplayInicio(val) {
  displayInicio.value = mask(val);
  const iso = displayToIso(displayInicio.value);
  emit("update:dataInicio", iso);
  emit("buscar");
}
function onDisplayFim(val) {
  displayFim.value = mask(val);
  const iso = displayToIso(displayFim.value);
  emit("update:dataFim", iso);
  emit("buscar");
}

function limpar() {
  displayInicio.value = "";
  displayFim.value = "";
  emit("update:dataInicio", "");
  emit("update:dataFim", "");
  emit("buscar");
}

function fmtIso(y, m, d) {
  return `${String(y).padStart(4, "0")}-${String(m).padStart(2, "0")}-${String(d).padStart(2, "0")}`;
}

function pickHoje() {
  const now = new Date();
  const y = now.getFullYear();
  const m = now.getMonth() + 1;
  const d = now.getDate();
  const iso = fmtIso(y, m, d);
  displayInicio.value = isoToDisplay(iso);
  displayFim.value = isoToDisplay(iso);
  emit("update:dataInicio", iso);
  emit("update:dataFim", iso);
  emit("buscar");
}

function pick7() {
  const end = new Date();
  const start = new Date();
  start.setDate(end.getDate() - 6);
  const isoStart = fmtIso(start.getFullYear(), start.getMonth() + 1, start.getDate());
  const isoEnd = fmtIso(end.getFullYear(), end.getMonth() + 1, end.getDate());
  displayInicio.value = isoToDisplay(isoStart);
  displayFim.value = isoToDisplay(isoEnd);
  emit("update:dataInicio", isoStart);
  emit("update:dataFim", isoEnd);
  emit("buscar");
}

function pick30() {
  const end = new Date();
  const start = new Date();
  start.setDate(end.getDate() - 29);
  const isoStart = fmtIso(start.getFullYear(), start.getMonth() + 1, start.getDate());
  const isoEnd = fmtIso(end.getFullYear(), end.getMonth() + 1, end.getDate());
  displayInicio.value = isoToDisplay(isoStart);
  displayFim.value = isoToDisplay(isoEnd);
  emit("update:dataInicio", isoStart);
  emit("update:dataFim", isoEnd);
  emit("buscar");
}

function pickMes() {
  const now = new Date();
  const y = now.getFullYear();
  const m = now.getMonth() + 1;
  const startIso = fmtIso(y, m, 1);
  const lastDay = new Date(y, m, 0).getDate();
  const endIso = fmtIso(y, m, lastDay);
  displayInicio.value = isoToDisplay(startIso);
  displayFim.value = isoToDisplay(endIso);
  emit("update:dataInicio", startIso);
  emit("update:dataFim", endIso);
  emit("buscar");
}

watch(() => props.dataInicio, (v) => {
  displayInicio.value = isoToDisplay(v || "");
});
watch(() => props.dataFim, (v) => {
  displayFim.value = isoToDisplay(v || "");
});
</script>
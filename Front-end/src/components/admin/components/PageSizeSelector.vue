<template>
  <div class="flex items-center gap-2 text-sm">
    <label class="text-gray-700">Itens por página:</label>
    <select
      :value="modelValue"
      @change="onChange"
      class="border rounded-md px-2 py-1 bg-white text-sm"
    >
      <option v-for="opt in options" :key="opt" :value="opt">{{ opt }}</option>
    </select>
  </div>
</template>

<script setup>
const props = defineProps({
  modelValue: { type: Number, default: 10 },
  options: { type: Array, default: () => [10, 20, 50] },
});
const emit = defineEmits(["update:modelValue"]);

function onChange(e) {
  const val = Number(e.target.value);
  emit("update:modelValue", val);
}
</script>
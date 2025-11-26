<template>
  <div
    class="flex items-center border border-gray-300 rounded-md px-3 py-2 bg-white focus-within:ring-2 focus-within:ring-blue-500"
  >
    <component v-if="icon" :is="icon" class="w-4 h-4 text-gray-500 mr-2" />
    <input
      :type="type"
      :placeholder="placeholder"
      :value="modelValue"
      @input="$emit('update:modelValue', $event.target && $event.target.value)"
      class="outline-none w-full bg-white text-sm text-gray-700"
    />
  </div>
</template>

<script setup>
const props = defineProps({
  modelValue: { type: String, default: "" },
  placeholder: { type: String, default: "" },
  type: { type: String, default: "text" },
  icon: { type: [Object, Function], default: null },
});

defineEmits(["update:modelValue"]);
</script>

<style scoped>
</style>
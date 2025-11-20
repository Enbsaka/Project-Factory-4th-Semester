<template>
  <button
    :class="[
      'inline-flex items-center gap-2 rounded-md font-medium transition focus:outline-none focus:ring-2 focus:ring-offset-2',
      sizeClass,
      variantClass,
      block ? 'w-full justify-center' : 'justify-center'
    ]"
    :disabled="disabled"
    v-bind="$attrs"
  >
    <slot name="icon" />
    <slot />
  </button>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  variant: { type: String, default: 'primary' }, // primary | secondary | danger | ghost
  size: { type: String, default: 'md' }, // sm | md | lg
  block: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false }
});

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'px-3 py-1.5 text-sm';
    case 'lg': return 'px-5 py-3 text-base';
    default: return 'px-4 py-2 text-sm';
  }
});

const variantClass = computed(() => {
  switch (props.variant) {
    case 'secondary': return 'bg-white text-gray-800 border border-gray-300 hover:bg-gray-50 focus:ring-blue-500';
    case 'danger': return 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500';
    case 'ghost': return 'bg-transparent text-gray-800 hover:bg-gray-100 focus:ring-blue-500';
    default: return 'bg-[#141A7C] text-white hover:opacity-90 focus:ring-[#141A7C]';
  }
});
</script>

<style scoped>
</style>
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(router)

app.mount('#app')

// Inicializa tema a partir do storage
const savedTheme = localStorage.getItem('theme')
if (savedTheme) {
  document.documentElement.setAttribute('data-theme', savedTheme)
}

import { createRouter, createWebHistory } from 'vue-router';

// ==== PÁGINAS DO CLIENTE ====
import HomeView from "../views/default/HomeView.vue";
import CategoryView from "../views/default/CategoryView.vue";
import ProductView from "../views/default/ProductView.vue";
import CartView from "../views/default/CartView.vue";
import CheckoutView from "../views/default/CheckoutView.vue";
import SearchView from "../views/default/SearchView.vue";
// Rotas logadas
import LoginClientView from "../views/LoginClientView.vue";
import RegisterView from "../views/RegisterView.vue";

// ==== PÁGINAS DO ADMIN ====
import LoginAdminView from "../views/admin/LoginAdminView.vue";
import DashboardAdminView from '../views/admin/DashboardAdminView.vue';
import ProductsAdminView from '../views/admin/ProductsAdminView.vue';
import OrdersAdminView from '../views/admin/OrdersAdminView.vue';
import ClientsAdminView from '../views/admin/ClientsAdminView.vue';
import CouponAdminView from '../views/admin/CouponAdminView.vue';
import CategoriesAdminView from '../views/admin/CategoriesAdminView.vue';

// ==== ROTAS ====
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // ==== SITE PÚBLICO ====
    {
      path: "/",
      name: "home",
      component: HomeView,
      meta: { layout: "client" },
    },
    {
      path: "/categoria/:id",
      name: "category",
      component: CategoryView,
      meta: { layout: "client" },
    },
    {
      path: "/produto/:id",
      name: "product",
      component: ProductView,
      meta: { layout: "client" },
    },
    {
      path: "/carrinho",
      name: "cart",
      component: CartView,
      meta: { layout: "client" },
    },
    {
      path: "/checkout/:id",
      name: "checkout",
      component: CheckoutView,
      meta: { layout: "checkout", requiresAuth: true },
    },
    {
      path: "/carrinho/checkout",
      name: "checkoutCart",
      component: CheckoutView,
      meta: { layout: "checkout", requiresAuth: true },
    },
    {
      path: "/checkout/:id/confirmacao",
      name: "checkoutConfirm",
      component: () => import("../views/default/CheckoutSuccessView.vue"),
      meta: { layout: "checkout", requiresAuth: true },
    },
    {
      path: "/carrinho/checkout/confirmacao",
      name: "checkoutCartConfirm",
      component: () => import("../views/default/CheckoutSuccessView.vue"),
      meta: { layout: "checkout", requiresAuth: true },
    },

    {
      path: "/buscar",
      name: "search",
      component: SearchView,
      meta: { layout: "client" },
    },

    // ==== ÁREA LOGADA ====
    {
      path: "/app",
      name: "homeLogged",
      component: HomeView,
      meta: { layout: "client", requiresAuth: true },
    },
    {
      path: "/app/buscar",
      name: "searchLogged",
      component: SearchView,
      meta: { layout: "client", requiresAuth: true },
    },
    // Rota de perfil removida conforme solicitação

    {
      path: "/login",
      name: "loginCliente",
      component: LoginClientView,
      meta: { layout: "client" },
    },
    {
      path: "/cadastro",
      name: "cadastroCliente",
      component: RegisterView,
      meta: { layout: "client" },
    },

    // ==== ADMIN ====
    {
      path: "/admin/login",
      name: "login",
      component: LoginAdminView,
      meta: { layout: "none" },
    },
    {
      path: "/admin/dashboard",
      name: "dashboard",
      component: DashboardAdminView,
      meta: { layout: "admin" },
    },
    {
      path: "/admin/produtos",
      name: "products",
      component: ProductsAdminView,
      meta: { layout: "admin" },
    },
    {
      path: "/admin/categorias",
      name: "categories",
      component: CategoriesAdminView,
      meta: { layout: "admin" },
    },
    {
      path: "/admin/pedidos",
      name: "orders",
      component: OrdersAdminView,
      meta: { layout: "admin" },
    },
    {
      path: "/admin/clientes",
      name: "clients",
      component: ClientsAdminView,
      meta: { layout: "admin" },
    },
    {
      path: "/admin/cupons",
      name: "coupons",
      component: CouponAdminView,
      meta: { layout: "admin" },
    },
  ],
});

// Guarda de navegação
router.beforeEach((to, from, next) => {
  const isLogged = !!localStorage.getItem('token');
  const role = localStorage.getItem('role') || '';

  // Protege rotas do admin: exige login com papel 'admin'
  if (to.meta?.layout === 'admin') {
    if (!isLogged || role !== 'admin') {
      return next({ name: 'login' });
    }
  }

  // Protege rotas que exigem login
  if (to.meta?.requiresAuth && !isLogged) {
    return next({ name: 'loginCliente' });
  }

  // Usuario logado indo para home pública => redireciona para /app
  if (to.name === 'home' && isLogged) {
    return next({ name: 'homeLogged' });
  }

  // Evita carrinho para não logados
  if (to.name === 'cart' && !isLogged) {
    return next({ name: 'loginCliente' });
  }
  // Evita checkout sem login
  if ((to.name === 'checkout' || to.name === 'checkoutCart' || to.name === 'checkoutConfirm' || to.name === 'checkoutCartConfirm') && !isLogged) {
    return next({ name: 'loginCliente' });
  }

  next();
});

export default router;

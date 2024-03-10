import { RouteRecordRaw } from "vue-router";
import { HOME_URL, LOGIN_URL } from "@/config";

/**
 * staticRouter (静态路由)
 */
export const staticRouter: RouteRecordRaw[] = [
  {
    path: "/",
    name: "Layout",
    redirect: HOME_URL,
    component: () => import("@/layouts/index.vue"),
    // component: () => import("@/layouts/indexAsync.vue")
    meta: {
      title: "",
      isKeepAlive: false
    },
    children: []
  },
  {
    path: LOGIN_URL,
    name: "Login",
    component: () => import("@/views/login/index.vue"),
    meta: {
      title: "登录",
      isKeepAlive: false
    }
  },
  {
    path: "/403",
    name: "403",
    component: () => import("@/components/ErrorMessage/403.vue"),
    meta: {
      title: "403页面"
    }
  },
  {
    path: "/404",
    name: "404",
    component: () => import("@/components/ErrorMessage/404.vue"),
    meta: {
      title: "404页面"
    }
  },
  {
    path: "/500",
    name: "500",
    component: () => import("@/components/ErrorMessage/500.vue"),
    meta: {
      title: "500页面"
    }
  },
  // Resolve refresh page, route warnings
  {
    path: "/:pathMatch(.*)*",
    component: () => import("@/components/ErrorMessage/404.vue")
  }
];

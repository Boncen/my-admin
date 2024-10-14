import { DEFAULT_LAYOUT } from "../base";
import { AppRouteRecordRaw } from "../types";

const HOME: AppRouteRecordRaw = {
  path: "/sys",
  name: "sys",
  component: DEFAULT_LAYOUT,
  meta: {
    requiresAuth: true,
    icon: "icon-dashboard",
    order: 0,
  },
  children: [
    {
      path: "/users",
      name: "users",
      component: () => import("@/views/sys/users/index.vue"),
      meta: {
        requiresAuth: true,
        locale: "menu.server.users",
      },
    },
    {
      path: "/tenants",
      name: "tenants",
      component: () => import("@/views/sys/tenants/index.vue"),
      meta: {
        requiresAuth: true,
        locale: "menu.server.tenants",
      },
    },
    {
      path: "/roles",
      name: "roles",
      component: () => import("@/views/sys/roles/index.vue"),
      meta: {
        requiresAuth: true,
        locale: "menu.server.roles",
      },
    },
    {
      path: "/menus",
      name: "menus",
      component: () => import("@/views/sys/menus/index.vue"),
      meta: {
        requiresAuth: true,
        locale: "menu.server.menus",
      },
    },
    {
      path: "/logs",
      name: "sys_log",
      component: () => import("@/views/sys/log/index.vue"),
      meta: {
        requiresAuth: true,
        locale: "menu.server.syslog",
      },
    },
    {
      path: "/test",
      name: "test",
      component: () => import("@/views/sys/log/index.vue"),
      meta: {
        requiresAuth: true,
        locale: "test",
      },
    },
  ],
};

export default HOME;

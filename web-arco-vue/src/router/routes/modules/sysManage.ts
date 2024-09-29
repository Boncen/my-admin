import { DEFAULT_LAYOUT } from '../base';
import { AppRouteRecordRaw } from '../types';

const HOME: AppRouteRecordRaw = {
  path: '/sys',
  name: 'sys',
  component: DEFAULT_LAYOUT,
  meta: {
    requiresAuth: true,
    icon: 'icon-dashboard',
    order: 0,
  },
  children:[
    {
        path: '/users',
        name: 'users',
        component: () => import('@/views/sys/users/index.vue'),
        meta:{
             requiresAuth: true,
             title:"用户管理"
        }
    },
    {
        path: '/tenants',
        name: 'tenants',
        component: () => import('@/views/sys/tenants/index.vue'),
        meta:{
             requiresAuth: true,
        }
    },
    {
        path: '/roles',
        name: 'roles',
        component: () => import('@/views/sys/roles/index.vue'),
        meta:{
             requiresAuth: true,
        }
    },
    {
        path: '/menus',
        name: 'menus',
        component: () => import('@/views/sys/menus/index.vue'),
        meta:{
             requiresAuth: true,
        }
    },
  ]
};

export default HOME;

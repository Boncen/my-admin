import { DEFAULT_LAYOUT } from '../base';
import { AppRouteRecordRaw } from '../types';

const HOME: AppRouteRecordRaw = {
  path: '/home',
  name: 'home',
  component: DEFAULT_LAYOUT,
  meta: {
    locale: 'menu.server.home',
    requiresAuth: false,
    icon: 'icon-dashboard',
    order: 0,
  },
  
};

export default HOME;

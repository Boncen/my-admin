import {
    createBrowserRouter,
} from "react-router-dom";
import RootLayout from '@/layout'
import { AppRouteObject } from '#/router';

// get menus from api

const mods = getMenuModules();
console.log(mods);


const router = createBrowserRouter([
    {
        path: "/",
        element: <RootLayout />,
        children: [
            {
                index: true,
                element: <div>a</div>
            },
            {
                path: "aa",
                element: <div>aa</div>
            },
            {
                path: "*",
                element: <div>err</div>
            }
        ]
    },
    
  ]);

/**
 * 基于 src/router/modules 文件结构动态生成路由
 */
function getMenuModules() {
    const menuModules: AppRouteObject[] = [];
  
    const modules = import.meta.glob('./modules/**/*.tsx', { eager: true });
    Object.keys(modules).forEach((key) => {
      const mod = (modules as any)[key].default || {};
      const modList = Array.isArray(mod) ? [...mod] : [mod];
      menuModules.push(...modList);
    });
    return menuModules;
  }

export default router;
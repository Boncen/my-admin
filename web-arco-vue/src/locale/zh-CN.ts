import localeMessageBox from '@/components/message-box/locale/zh-CN';
import localeLogin from '@/views/login/locale/zh-CN';

import localeWorkplace from '@/views/dashboard/workplace/locale/zh-CN';
/** simple */

import localeSearchTable from '@/views/list/search-table/locale/zh-CN';


import locale403 from '@/views/exception/403/locale/zh-CN';
import locale404 from '@/views/exception/404/locale/zh-CN';
import locale500 from '@/views/exception/500/locale/zh-CN';

/** simple end */
import localeSettings from './zh-CN/settings';
import localeMenus from './zh-CN/menus';

export default {
 
  '': '',
  'navbar.action.locale': '切换为中文',
  ...localeSettings,
  ...localeMessageBox,
  ...localeLogin,
  ...localeWorkplace,
  /** simple */
  ...localeSearchTable,
  ...locale403,
  ...locale404,
  ...locale500,
  /** simple end */
  ...localeMenus
};

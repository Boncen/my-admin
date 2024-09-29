import localeMessageBox from '@/components/message-box/locale/en-US';
import localeLogin from '@/views/login/locale/en-US';

import localeWorkplace from '@/views/dashboard/workplace/locale/en-US';
/** simple */

import localeSearchTable from '@/views/list/search-table/locale/en-US';





import locale403 from '@/views/exception/403/locale/en-US';
import locale404 from '@/views/exception/404/locale/en-US';
import locale500 from '@/views/exception/500/locale/en-US';

/** simple end */
import localeSettings from './en-US/settings';
import localeMenus from './en-US/menus';

export default {

  'navbar.docs': 'Docs',
  'navbar.action.locale': 'Switch to English',
  ...localeSettings,
  ...localeMessageBox,
  ...localeLogin,
  ...localeWorkplace,
  /** simple */
  ...localeSearchTable,
  ...locale403,
  ...locale404,
  ...locale500,
  ...localeMenus,
  /** simple end */
};

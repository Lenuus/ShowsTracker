import { MenuItem } from './MenuItem'
import { MenuInnerWithSub } from './MenuInnerWithSub'
import { useIntl } from 'react-intl'
import { Category } from '../../../../models/enums/category-enum'
import { useAuth } from '../../../../app/modules/auth'

export function MenuInner() {
  const intl = useIntl()
  const auth = useAuth();
  return (
    <>
      <MenuItem title={intl.formatMessage({ id: 'MENU.DASHBOARD' })} icon='home' to='/dashboard' />
      <MenuInnerWithSub
        title='Series'
        to='/series'
        icon='book'
        menuPlacement='bottom-start'
        menuTrigger={`{default:'click', lg: 'hover'}`}
      >
        {/* Anime */}
        <MenuItem icon='right' to={'/shows/' + Category.Anime} title='Anime' />

        {/* Manga */}
        <MenuItem icon='right' to={'/shows/' + Category.Manga} title='Manga' />

        {/* Manhwa */}
        <MenuItem icon='right' to={'/shows/' + Category.Manhwa} title='Manhwa' />

        {/* Manhua */}
        <MenuItem icon='right' to={'/shows/' + Category.Manhua} title='Manhua' />

        {/* Manhua */}
        <MenuItem icon='right' to={'/shows/' + Category.Novel} title='Novel' />
      </MenuInnerWithSub>

      {/* Popular */}
      <MenuItem icon='graph-up' to='/shows/popular' title='Popular' />

      {/* Popular */}
      <MenuItem icon='heart' to='/find-your-taste' title='Find Your Taste' />

      {/* My Series */}
      <MenuItem icon='bookmark-2' to='/my-shows' title='My Series' />

      {
        auth.currentUser?.role == "Admin" &&
        <><MenuInnerWithSub
          title='Administrative'
          to='/admin'
          icon='setting-2'
          menuPlacement='bottom-start'
          menuTrigger={`{default:'click', lg: 'hover'}`}
        >
          <MenuItem icon='design' to='/admin/votings' title='Votings' />
          <MenuItem icon='chart-line-up-2' to='/admin/report' title='Report' />
          <MenuItem icon='book' to='/admin/shows' title='Shows' />
        </MenuInnerWithSub>
        </>
      }
    </>
  )
}
